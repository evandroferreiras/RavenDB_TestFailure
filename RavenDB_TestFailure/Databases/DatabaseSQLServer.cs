using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Extensions;
using ReportsEverywhereClass.Database_structure;

namespace ReportsEverywhereClass.Databases
{

    public class DatabaseSQLServer : DataBaseBase
    {

        public DatabaseSQLServer()
        {
            
        }

        public DatabaseSQLServer(string connectionStringRDB, string connectionStringODDB, string databaseODDB)
            : base(connectionStringRDB, connectionStringODDB, databaseODDB)
        {
            this.ConnectionStringRdb = connectionStringRDB;
        }

        private DSSchema CreateSchema(int schemaId, string schemaName)
        {
            DSSchema schema = new DSSchema { Id = schemaId, SchemaName = schemaName };

            #region Tables

            using (SqlConnection connectionTables = new SqlConnection(ConnectionStringRdb))
            {
                connectionTables.Open();
                SqlCommand commandTables = new SqlCommand("Select object_id from sys.tables Where schema_id = @schema_id",
                                                          connectionTables);
                commandTables.Parameters.AddWithValue("schema_id", schema.Id);
                SqlDataReader readerTables = commandTables.ExecuteReader();
                if (readerTables.HasRows)
                {
                    schema.Tables = new List<DSTable>();
                    while (readerTables.Read())
                    {
                        DSTable table = CreateTable(schema.Id, readerTables.GetInt32(0));

                        schema.Tables.Add(table);
                    }
                }
            }

            #endregion

            return schema;
        }

        private DSTable CreateTable(int schemaId, int objectId)
        {

            DSSchema schemaFinded = null;
            DSTable tableFinded = null;


            schemaFinded = this.Schemas.Find(x => (x.Id == schemaId));
            if (schemaFinded != null)
            {
                tableFinded = schemaFinded.Tables.Find(x => (x.Id == objectId));
            }

            if (tableFinded != null)
            {
                return tableFinded;
            }
            else
            {
                using (SqlConnection connectionCreateTable = new SqlConnection(ConnectionStringRdb))
                {
                    connectionCreateTable.Open();

                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(" Select s.name as SchemaName, ");
                    stringBuilder.Append("        t.name as TableName ");
                    stringBuilder.Append(" from ");
                    stringBuilder.Append(" 	sys.tables t");
                    stringBuilder.Append(" inner join ");
                    stringBuilder.Append(" 	sys.schemas s ");
                    stringBuilder.Append(" on ");
                    stringBuilder.Append(" 	s.schema_id = t.schema_id");
                    stringBuilder.Append(" Where");
                    stringBuilder.Append(" 	t.schema_id = @schema_id");
                    stringBuilder.Append(" and t.object_id = @object_id");

                    SqlCommand commandCreateTable = new SqlCommand(stringBuilder.ToString(), connectionCreateTable);
                    commandCreateTable.Parameters.AddWithValue("schema_id", schemaId);
                    commandCreateTable.Parameters.AddWithValue("object_id", objectId);
                    SqlDataReader readerCreateTable = commandCreateTable.ExecuteReader();
                    if (readerCreateTable.HasRows)
                    {
                        readerCreateTable.Read();
                        DSTable table = new DSTable(objectId, readerCreateTable["TableName"].ToString(), readerCreateTable["SchemaName"].ToString());
                        
                        #region Columns
                        using (SqlConnection connectionColumns = new SqlConnection(ConnectionStringRdb))
                        {
                            connectionColumns.Open();

                            stringBuilder = new StringBuilder();

                            stringBuilder.Append(" Select c.column_id,");
                            stringBuilder.Append("        c.object_id");
                            stringBuilder.Append(" from sys.columns c ");
                            stringBuilder.Append(" Where c.object_id = @object_id");

                            SqlCommand commandColumns = new SqlCommand(stringBuilder.ToString(), connectionColumns);
                            commandColumns.Parameters.AddWithValue("object_id", table.Id);
                            SqlDataReader readerColumns = commandColumns.ExecuteReader();
                            if (readerColumns.HasRows)
                            {
                                table.Columns = new List<DSColumn>();
                                while (readerColumns.Read())
                                {
                                    table.Columns.Add(CreateColumn(columnId: readerColumns.GetInt32(0), objectId: readerColumns.GetInt32(1), schemaId: schemaId));
                                }
                            }
                        }
                        #endregion

                        return table;
                    }
                    else
                    {
                        return null;
                    }
                }
            }


        }

        private DSColumn CreateColumn(int objectId, int columnId, int schemaId)
        {
            DSSchema schemaFinded = null;
            DSTable tableFinded = null;
            DSColumn columnFinded = null;

            schemaFinded = this.Schemas.Find(x => (x.Id == schemaId));
            if (schemaFinded != null)
            {
                tableFinded = schemaFinded.Tables.Find(x => (x.Id == objectId));
                if (tableFinded != null)
                {
                    columnFinded = tableFinded.Columns.Find(x => (x.Id == columnId));
                }
            }

            if (columnFinded != null)
            {
                return columnFinded;
            }
            else
            {
                using (SqlConnection connectionCreateColumn = new SqlConnection(ConnectionStringRdb))
                {
                    connectionCreateColumn.Open();
                    StringBuilder stringBuilder = new StringBuilder();

                    stringBuilder.Append(" Select c.name,");
                    stringBuilder.Append("        c.column_id,");
                    stringBuilder.Append("        tp1.name as typename");
                    stringBuilder.Append(" from sys.tables t");
                    stringBuilder.Append(" left join sys.columns c on c.object_id = t.object_id");
                    stringBuilder.Append(" left join sys.types AS tp ON c.user_type_id= tp.user_type_id");
                    stringBuilder.Append(" left join sys.types AS tp1 ON tp1.user_type_id= tp.system_type_id");
                    stringBuilder.Append(" Where c.object_id = @object_id and c.column_id = @column_id");
                    SqlCommand commandCreateColumn = new SqlCommand(stringBuilder.ToString(), connectionCreateColumn);
                    commandCreateColumn.Parameters.AddWithValue("object_id", objectId);
                    commandCreateColumn.Parameters.AddWithValue("column_id", columnId);
                    SqlDataReader readerCreateColumn = commandCreateColumn.ExecuteReader();
                    if (readerCreateColumn.HasRows)
                    {
                        readerCreateColumn.Read();
                        DSColumn column = new DSColumn
                        {
                            ColumnName = readerCreateColumn["name"].ToString(),
                            Id = Int32.Parse(readerCreateColumn["column_id"].ToString()),
                            ParentTableId = objectId,
                            ParentSchemaId = schemaId,
                            TypeName = readerCreateColumn["typename"].ToString()
                        };
                        return column;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        private DSForeignKey CreateForeignKey(int foreignKeyId, int parentSchemaId, int parentTableId, int parentColumnId, int referencedSchemaId, int referencedTableId, int referencedColumnId)
        {
            return new DSForeignKey(foreignKeyId, parentSchemaId, parentTableId, parentColumnId, referencedSchemaId, referencedTableId, referencedColumnId);
        }

        public override void SaveToODDB()
        {

            using (var documentStore = new DocumentStore())
            {
                documentStore.Url = ConnectionStringODDB;
                documentStore.Initialize();
                documentStore.DatabaseCommands.EnsureDatabaseExists(DatabaseODDB);
                using (IDocumentSession session = documentStore.OpenSession(DatabaseODDB))
                {
                    var existingDatabase = session.Query<DatabaseSQLServer>();

                    if (existingDatabase != null)
                    {
                        foreach (var dataBaseBase in existingDatabase)
                        {
                            session.Delete(dataBaseBase);
                            session.SaveChanges();
                        }
                    }
                }
            }

            base.SaveToRavenDB();
        }

        public override void LoadSchema()
        {
            try
            {
                using (SqlConnection connectionSchemas = new SqlConnection(ConnectionStringRdb))
                {
                    connectionSchemas.Open();
                    SqlCommand commandSchemas = new SqlCommand("Select SCHEMA_NAME(schema_id),schema_id from sys.tables group by schema_id", connectionSchemas);
                    SqlDataReader readerSchemas = commandSchemas.ExecuteReader();
                    if (readerSchemas.HasRows)
                    {
                        this.Schemas = new List<DSSchema>();

                        while (readerSchemas.Read())
                        {
                            var schema = CreateSchema(schemaId: readerSchemas.GetInt32(1), schemaName: readerSchemas.GetString(0));
                            this.Schemas.Add(schema);
                        }
                        readerSchemas.Close();
                    }


                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(" select ");
                    stringBuilder.Append("     fkcol.constraint_object_id as fkId,");
                    stringBuilder.Append("     parent.schema_id as ParentSchemaId,");
                    stringBuilder.Append("     parent.object_id as ParentTableId,");
                    stringBuilder.Append("     parentcol.column_id as ParentColumnId,");
                    stringBuilder.Append("     referenced.schema_id as ReferencedSchemaId,");
                    stringBuilder.Append("     referenced.object_id as ReferencedTableId,");
                    stringBuilder.Append("     referencedcol.column_id as ReferencedColumnId");
                    stringBuilder.Append(" from ");
                    stringBuilder.Append("     sys.foreign_key_columns fkcol");
                    stringBuilder.Append(" left join sys.foreign_keys fk on fk.object_id = fkcol.constraint_object_id");
                    stringBuilder.Append(" left join sys.all_objects parent on parent.object_id = fk.parent_object_id");
                    stringBuilder.Append(" left join sys.all_objects referenced on referenced.object_id = fk.referenced_object_id");
                    stringBuilder.Append(" left join sys.all_columns parentcol on parentcol.object_id = fkcol.parent_object_id and parentcol.column_id = fkcol.parent_column_id");
                    stringBuilder.Append(" left join sys.all_columns referencedcol on referencedcol.object_id = fkcol.referenced_object_id and referencedcol.column_id = fkcol.referenced_column_id");
                    stringBuilder.Append(" order by fkcol.constraint_object_id");                    
                    SqlCommand commandForeignKeys = new SqlCommand(stringBuilder.ToString(),connectionSchemas);
                    SqlDataReader readerForeignKeys = commandForeignKeys.ExecuteReader();
                    if (readerForeignKeys.HasRows)
                    {
                        this.ForeignKeys = new List<DSForeignKey>();
                        while (readerForeignKeys.Read())
                        {
                            var foreignkey = CreateForeignKey(
                                foreignKeyId: Int32.Parse(readerForeignKeys["fkId"].ToString()),
                                parentSchemaId: Int32.Parse(readerForeignKeys["ParentSchemaId"].ToString()),
                                parentTableId: Int32.Parse(readerForeignKeys["ParentTableId"].ToString()),
                                parentColumnId: Int32.Parse(readerForeignKeys["ParentColumnId"].ToString()),
                                referencedSchemaId: Int32.Parse(readerForeignKeys["ReferencedSchemaId"].ToString()),
                                referencedTableId: Int32.Parse(readerForeignKeys["ReferencedTableId"].ToString()),
                                referencedColumnId: Int32.Parse(readerForeignKeys["ReferencedColumnId"].ToString())
                                );
                            ForeignKeys.Add(foreignkey);
                        }
                        readerForeignKeys.Close();
                        
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}

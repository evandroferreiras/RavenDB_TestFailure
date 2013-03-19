using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ReportsEverywhereClass.Database_structure;

namespace ReportsEverywhereClass.Reports
{
    public class ReportDatasetGeneric : ReportDatasetBase
    {
        public ReportDatasetGeneric(string connectionStringRDB, ReportConfiguration config, DataBaseBase dataBase)
            : base(connectionStringRDB, config, dataBase)
        {
        }

        public override string LoadDataset()
        {
            #region Definindo a tabela principal(MAIN TABLE)
            String mainSchemaAndTable = String.Empty;
            if (!String.IsNullOrEmpty(Config.MainSchemaName))
            {
                mainSchemaAndTable = string.Format("{0}.", Config.MainSchemaName);
            }

            mainSchemaAndTable = string.Format("{0}{1}", mainSchemaAndTable, Config.MainTable.TableName);
            #endregion

            #region Montando colunas
            

            String strColumnsSet;
            if (Config.Columns != null)
            {
                List<String> columnsList = new List<String>();
                foreach (DSColumn column in Config.Columns)
                {
                    String tableName = String.Empty;
                    if (column.ParentSchemaId > 0)
                    {
                        DSSchema schema;
                        try
                        {
                            schema = Database.Schemas.Find(x => (x.Id == column.ParentSchemaId));
                        }
                        catch (Exception)
                        {
                            schema = null;
                        }

                        if (schema != null)
                        {
                            String schemaName = schema.SchemaName;
                            if (column.ParentTableId > 0)
                            {
                                DSTable table;
                                try
                                {
                                    table = schema.Tables.Find(x => (x.Id == column.ParentTableId));                                   
                                }
                                catch (Exception)
                                {
                                    table = null;
                                }

                                if (table != null)
                                {
                                    tableName = string.Format("{0}.{1}", schemaName, table.TableName);
                                }
                            }
                        }
                    }
                    columnsList.Add(String.Format("{0}.{1}", tableName, column.ColumnName));
                }

                strColumnsSet = String.Join(",", columnsList);
            }
            else
            {
                strColumnsSet = "*";
            }
            #endregion

            #region Montando filtros(WHERE)

            String strConditionsSet = String.Empty;
            if (Config.Filters != null)
            {
                List<String> conditionsList = new List<string>();
                
                foreach (FilterConfiguration filterConfiguration in Config.Filters)
                {
                    List<String> valuesWithCast = filterConfiguration.Values.Select(value => String.Format("CAST('{0}' as {1})", value, filterConfiguration.Column.TypeName)).ToList();

                    String strCondition;
                    String strTableName = "";
                    if (filterConfiguration.Column.ParentSchemaId > 0)
                    {
                        DSSchema schema;
                        try
                        {
                            schema = Database.Schemas.Find(x => (x.Id == filterConfiguration.Column.ParentSchemaId));
                        }
                        catch (Exception)
                        {
                            schema = null;
                        }

                        if (schema != null)
                        {
                            String schemaName = schema.SchemaName;
                            if (filterConfiguration.Column.ParentTableId > 0)
                            {
                                DSTable table;
                                try
                                {
                                    table = schema.Tables.Find(x => (x.Id == filterConfiguration.Column.ParentTableId));
                                }
                                catch (Exception)
                                {
                                    table = null;
                                }

                                if (table != null)
                                {
                                    strTableName = string.Format("{0}.{1}", schemaName, table.TableName);
                                }
                            }
                        }
                    }


                    String strTableColumn = String.Format("{0}.{1}", strTableName, filterConfiguration.Column.ColumnName);
                    var comparatorsOneToOne = new[]
                                                  {
                                                      FilterConfiguration.ComparatorType.Equals,
                                                      FilterConfiguration.ComparatorType.NotEqualTo,
                                                      FilterConfiguration.ComparatorType.GreaterThan,
                                                      FilterConfiguration.ComparatorType.GreaterThanOrEqualTo,
                                                      FilterConfiguration.ComparatorType.LessThan,
                                                      FilterConfiguration.ComparatorType.LessThanOrEqualThan,
                                                      FilterConfiguration.ComparatorType.Like,
                                                      FilterConfiguration.ComparatorType.NotLike
                                                  };
                    List<FilterConfiguration.ComparatorType> listType = new List<FilterConfiguration.ComparatorType>(comparatorsOneToOne);

                    

                    //Se é um comparador um para um...
                    if (listType.Contains(filterConfiguration.Comparator))
                    {
                        strCondition = String.Format("{0} {1} {2} ", strTableColumn,
                                                     FilterConfiguration.ComparatorTypeToString(filterConfiguration.Comparator),
                                                     valuesWithCast.First());
                    }
                    else
                    {
                        //Se for BETWEEN(ou NOT BETWEEN)
                        if (filterConfiguration.Comparator.Equals(FilterConfiguration.ComparatorType.Between) ||
                            filterConfiguration.Comparator.Equals(FilterConfiguration.ComparatorType.NotBetween))
                        {
                            strCondition = String.Format("{0} {1} {2} and {3} ",
                                                         strTableColumn,
                                                         FilterConfiguration.ComparatorTypeToString(filterConfiguration.Comparator),
                                                         valuesWithCast.ElementAt(0),
                                                         valuesWithCast.ElementAt(1));
                        }
                        //Se for IN(ou NOT INT)
                        else
                        {
                            strCondition = String.Format("{0} {1} ({2}) ", strTableColumn,
                                                         FilterConfiguration.ComparatorTypeToString(
                                                             filterConfiguration.Comparator),
                                                         String.Join(",", valuesWithCast));
                        }
                    }
                    //Adicionando Operador(and, or)
                    if (conditionsList.Count+1 != Config.Filters.Count)
                        strCondition = String.Format("{0} {1} ", strCondition, FilterConfiguration.OperatorTypeToString(filterConfiguration.Operator));
                    conditionsList.Add(strCondition);
                    
                }
                strConditionsSet = "WHERE " + String.Join(" ", conditionsList);
            }

            #endregion

            #region Montando "Joins"
            //Buscar todos as FKs que a tabela principal(MainTable) possui.
            String strJoinsSet = String.Empty;
            if (Database.ForeignKeys != null)
            {
                List<DSForeignKey> listOfFkFromMainTable = Database.ForeignKeys.Where(x => (x.ParentTableId == Config.MainTable.Id)).ToList();
                listOfFkFromMainTable = listOfFkFromMainTable.Where(x => (Config.Columns.Select(value => value.ParentTableId).Contains(x.ReferencedTableId))).ToList();
                var groupedbyReferencedTableId = (from foreignKey in listOfFkFromMainTable
                                                  group foreignKey by foreignKey.ReferencedTableId
                                                      into g
                                                      select new
                                                                 {
                                                                     ReferencedSchemaId =
                                                          g.Select(x => (x.ReferencedSchemaId)),
                                                                     ReferencedTableId =
                                                          g.Select(x => (x.ReferencedTableId))
                                                                 });
                List<String> listOfJoins = new List<string>();
                foreach (var group in groupedbyReferencedTableId)
                {
                    List<String> currentJoin = new List<string>();
                    int referencedSchemaId = group.ReferencedSchemaId.First();
                    int referencedTableId = group.ReferencedTableId.First();

                    DSSchema referencedSchema = Database.Schemas.Find(x => (x.Id == referencedSchemaId));
                    if (referencedSchema != null)
                    {
                        DSTable referencedTable = referencedSchema.Tables.Find(x => (x.Id == referencedTableId));
                        String referencedSchemaAndTable = String.Format("{0}.{1}", referencedTable.SchemaName,
                                                                        referencedTable.TableName);

                        foreach (
                            var foreignKey in
                                listOfFkFromMainTable.Where(x => (x.ReferencedTableId == referencedTable.Id)))
                        {
                            DSColumn refencedColumn =
                                referencedTable.Columns.Find(x => (x.Id == foreignKey.ReferencedColumnId));
                            DSColumn parentColumn = Config.Columns.Find(x => (x.Id == foreignKey.ParentColumnId));
                            currentJoin.Add(String.Format("{0}.{1} = {2}.{3}", referencedSchemaAndTable,
                                                          refencedColumn.ColumnName, mainSchemaAndTable,
                                                          parentColumn.ColumnName));
                        }

                        String join = String.Format("LEFT JOIN {0} On {1}", referencedSchemaAndTable,
                                                    String.Join(" AND ", currentJoin));
                        listOfJoins.Add(join);
                    }
                }
                strJoinsSet = String.Join(Environment.NewLine, listOfJoins);
            }

            #endregion
            
            String finalSQL = String.Format("SELECT {0} FROM {1} {2} {3}", strColumnsSet, mainSchemaAndTable, strJoinsSet, strConditionsSet);
            this.DatasetLoaded = new DataSet();
            SqlConnection sqlConnection = new SqlConnection(this.ConnectionStringRDB);
            SqlCommand sqlCommand = new SqlCommand(finalSQL,sqlConnection);
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            sqlDataAdapter.Fill(this.DatasetLoaded);

            return finalSQL;
        }
    }
}


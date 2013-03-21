using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Extensions;

namespace ReportsEverywhereClass.Database_structure
{
    public abstract class DataBaseBase : RavenDBBase
    {

        protected DataBaseBase()
        {
            
        }

        public enum BancoDados
        {
            MSSQLSERVER
        }

        protected String ConnectionStringRdb;
        public List<DSSchema> Schemas { get; set; }

        public List<DSForeignKey> ForeignKeys { get; set; }

        protected DataBaseBase(string connectionStringRDB,string connectionStringODDB, string databaseODDB) :base(connectionStringODDB,  databaseODDB)
        {
            ConnectionStringRdb = connectionStringRDB;
        }

        public abstract void LoadSchema();

        public abstract void SaveToODDB();

        public override bool Equals(object obj)
        {

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            DataBaseBase database = (DataBaseBase)obj;

            bool isEqual = false;

            foreach (var schema in this.Schemas)
            {
                isEqual = database.Schemas.Contains(schema);
                if (!isEqual)
                    break;
            }

            if (isEqual)
            {
                isEqual = (database.Schemas.Count == this.Schemas.Count);
            }

            if (isEqual)
            {
                foreach (var foreignKey in ForeignKeys)
                {
                    isEqual = database.ForeignKeys.Contains(foreignKey);    
                    if (!isEqual)
                        break;                    
                }
                if (isEqual)
                {
                    isEqual = (database.ForeignKeys.Count == this.ForeignKeys.Count);
                }
                
            }
            return isEqual;
        }

        public override int GetHashCode()
        {
            return ((Schemas == null) ? 0 : Schemas.GetHashCode());
        }
    }
}
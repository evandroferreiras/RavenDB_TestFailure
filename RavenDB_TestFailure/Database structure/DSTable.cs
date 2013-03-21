using System;
using System.Collections.Generic;
using Raven.Imports.Newtonsoft.Json;

namespace ReportsEverywhereClass.Database_structure
{
    [JsonObject(IsReference = true)]
    public class DSTable
    {
        public int Id { get; set; }
        public String TableName { get; set; }
        public List<DSColumn> Columns { get; set; }
        public String SchemaName { get; set; }

        public DSTable()
        {
        }

        public DSTable(int id, string tableName, string schemaName)
        {
            Id = id;
            TableName = tableName;
            SchemaName = schemaName;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            DSTable table = (DSTable) obj;

            bool isEqual = false;

            if (table.TableName != null)
            {
                isEqual = (this.TableName.Equals(table.TableName));
            }else
            {
                isEqual = table.TableName == this.TableName;
            }

            if (isEqual){
                foreach (var column in this.Columns)
                {
                    if (column != null)
                    {
                        isEqual = this.Columns.Contains(column);
                    }
                    if (!isEqual)
                        break;
                }
                if (isEqual)
                {
                    isEqual = (this.Columns.Count == table.Columns.Count);
                }
            }

            return isEqual;
        }

        public override int GetHashCode()
        {
            return string.IsNullOrEmpty(TableName) ? 0 : TableName.GetHashCode() + ((Columns == null) ? 0 : Columns.GetHashCode()) + SchemaName.GetHashCode();
        }        
    }
}

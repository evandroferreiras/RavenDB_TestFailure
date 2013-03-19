using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ReportsEverywhereClass.Database_structure
{
    public class DSSchema
    {
        public int Id { get; set; }
        public String SchemaName { get; set; }
        public List<DSTable> Tables { get; set; }


        public override bool Equals(object obj)
        {

            if (obj == null || GetType() != obj.GetType())
                return false;

            var schema = (DSSchema)obj;

            bool isEqual;
            if (schema.SchemaName != null)
            {
                isEqual = (this.SchemaName.Equals(schema.SchemaName));
            }
            else
            {
                isEqual = (schema.SchemaName == this.SchemaName);
            }

            if (isEqual)
            {
                foreach (var table in this.Tables)
                {
                    if (table != null)
                    {
                        isEqual = schema.Tables.Contains(table);
                    }
                    if (!isEqual)
                        break;
                }
                if (isEqual)
                {
                    isEqual = (schema.Tables.Count == this.Tables.Count);
                }

            }

            return isEqual;
        }

        public override int GetHashCode()
        {
            return string.IsNullOrEmpty(SchemaName) ? 0 : SchemaName.GetHashCode() +
                   ((Tables == null) ? 0 : Tables.GetHashCode());
        }
    }
}

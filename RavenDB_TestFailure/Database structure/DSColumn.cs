using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportsEverywhereClass.Database_structure
{
    public class DSColumn
    {
        
        public int Id { get; set; }
        public String ColumnName { get; set; }
        public int ParentSchemaId { get; set; }
        public int ParentTableId { get; set; }        
        public String TypeName { get; set; }

        public override bool Equals(object obj)
        {

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            DSColumn column = (DSColumn)obj;

            bool isEqual = false;
            if (column.ColumnName != null)
            {
                isEqual = (this.ColumnName.Equals(column.ColumnName));
            }
            else
            {
                isEqual = column.ColumnName == this.ColumnName;
            }


            if (column.TypeName != null) 
            {
                isEqual = isEqual && column.TypeName.Equals(this.TypeName);
            }
            else
            {
                isEqual = isEqual && (column.TypeName == this.TypeName);
            }


            return isEqual;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() + (string.IsNullOrEmpty(ColumnName) ? 0 : ColumnName.GetHashCode()) + ParentTableId.GetHashCode() + ParentSchemaId.GetHashCode();
        }
    }
}

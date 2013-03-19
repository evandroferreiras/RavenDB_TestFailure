using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportsEverywhereClass.Database_structure
{
    public class DSForeignKey
    {

        public DSForeignKey(int id, int parentSchemaId, int parentTableId, int parentColumnId, int referencedSchemaId, int referencedTableId, int referencedColumnId)
        {
            Id = id;
            ParentSchemaId = parentSchemaId;
            ParentTableId = parentTableId;
            ParentColumnId = parentColumnId;
            ReferencedSchemaId = referencedSchemaId;
            ReferencedTableId = referencedTableId;
            ReferencedColumnId = referencedColumnId;
        }

        public int Id { get; set; }
        public int ParentSchemaId { get; set; }
        public int ParentTableId { get; set; }
        public int ParentColumnId { get; set; }
        public int ReferencedSchemaId { get; set; }
        public int ReferencedTableId { get; set; }
        public int ReferencedColumnId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var foreignkey = (DSForeignKey)obj;
            bool isEqual = this.ParentColumnId.Equals(foreignkey.ParentColumnId);
            isEqual = isEqual && this.ParentSchemaId.Equals(foreignkey.ParentSchemaId);
            isEqual = isEqual && this.ParentTableId.Equals(foreignkey.ParentTableId);
            isEqual = isEqual && this.ReferencedColumnId.Equals(foreignkey.ReferencedColumnId);
            isEqual = isEqual && this.ReferencedSchemaId.Equals(foreignkey.ReferencedSchemaId);
            isEqual = isEqual && this.ReferencedTableId.Equals(foreignkey.ReferencedTableId);            
            return isEqual;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() + ParentTableId.GetHashCode() + ParentColumnId.GetHashCode() +
                   ReferencedTableId.GetHashCode() + ReferencedColumnId.GetHashCode();
        }
    }
}

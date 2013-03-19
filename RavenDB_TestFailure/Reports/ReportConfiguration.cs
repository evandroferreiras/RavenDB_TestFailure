using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;
using ReportsEverywhereClass.Database_structure;


namespace ReportsEverywhereClass.Reports
{
    [JsonObject(IsReference = true)]
    public class ReportConfiguration
    {
        public int Id;
        public String ReportName;
        public String MainSchemaName;
        public DSTable MainTable;
        public List<DSColumn> Columns;
        public List<FilterConfiguration> Filters;
        public DataTable DataTableResult;
        private StatusType _status;
        public StatusType Status
        {
            get { return _status; }
            set
            {
                switch (value)
                {
                    case StatusType.Pending:
                        break;
                    case StatusType.Processing:
                        break;
                    case StatusType.Completed:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("value");
                }
                _status = value;
            }
        }

        public enum StatusType
        {
            Pending,
            Processing,
            Completed
        }

        public ReportConfiguration()
        {
            _status = StatusType.Pending;
            MainTable = new DSTable(0,"","");
        }

        public override bool Equals(object obj)
        {
            ReportConfiguration aux = (ReportConfiguration) obj;
            return (this.Id == aux.Id);
        }

        public override int GetHashCode()
        {
            //Get hash code for the Name field if it is not null. 
            int hashID = Id.GetHashCode();
            
            //Calculate the hash code for the product. 
            return hashID;
        }
    }
}

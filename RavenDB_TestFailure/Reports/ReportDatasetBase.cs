using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Raven.Imports.Newtonsoft.Json;
using ReportsEverywhereClass.Database_structure;

namespace ReportsEverywhereClass.Reports
{
    public abstract class ReportDatasetBase
    {
        public DataSet DatasetLoaded;
        [JsonIgnore]
        protected ReportConfiguration Config;
        protected String ConnectionStringRDB;
        protected DataBaseBase Database;

        protected ReportDatasetBase(String connectionStringRDB, ReportConfiguration config, DataBaseBase database)
        {
            this.Config = config;
            ConnectionStringRDB = connectionStringRDB;
            this.Database = database;
        }

        public abstract String LoadDataset();

    }
}

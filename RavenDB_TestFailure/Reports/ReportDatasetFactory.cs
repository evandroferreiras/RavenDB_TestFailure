using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReportsEverywhereClass.Database_structure;

namespace ReportsEverywhereClass.Reports
{
    public static class ReportDatasetFactory
    {
        public static ReportDatasetBase GetReportDataset(DataBaseBase.BancoDados tipoBancoDados, string connectionStringRDB, ReportConfiguration config, DataBaseBase dataBase )
        {
            switch (tipoBancoDados)
            {
                case DataBaseBase.BancoDados.MSSQLSERVER:
                    return new ReportDatasetGeneric(connectionStringRDB, config, dataBase);
                default:
                    return new ReportDatasetGeneric(connectionStringRDB, config, dataBase);
            }
        }
    }
}

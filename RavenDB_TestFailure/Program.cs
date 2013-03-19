using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReportsEverywhereClass.Database_structure;
using ReportsEverywhereClass.Databases;
using ReportsEverywhereClass.Reports;

namespace RavenDB_TestFailure
{
    class Program
    {
        static void Main(string[] args)
        {
            DataBaseBase dataBaseBase =
                new DatabaseSQLServer("Server=.\\SQLEXPRESS;Database=AdventureWorks2008;Trusted_Connection=True;","http://localhost:8080", "Test");
            dataBaseBase.LoadSchema();
            ProcessingQueueReports reports = new ProcessingQueueReports("http://localhost:8080",
                                                                        "Server=.\\SQLEXPRESS;Database=AdventureWorks2008;Trusted_Connection=True;",
                                                                        "Teste",
                                                                        DataBaseBase.BancoDados.MSSQLSERVER,
                                                                        dataBaseBase);
            ReportConfiguration rc = new ReportConfiguration();
            rc.Id = 1;
            rc.ReportName = "Relatório de teste";
            rc.MainTable = dataBaseBase.Schemas.Find(x => (x.SchemaName.ToUpper() == "PERSON")).Tables.Find(x => (x.TableName.ToUpper() == "PERSON"));
            rc.MainSchemaName = rc.MainTable.SchemaName;
            rc.Columns = rc.MainTable.Columns;

            rc.Filters = new List<FilterConfiguration>
                             {
                                 new FilterConfiguration()
                                     {
                                         Column = rc.Columns.Find(c => (c.ColumnName.ToUpper() == "businessentityid".ToUpper())),
                                         Comparator = FilterConfiguration.ComparatorType.Between,
                                         Operator = FilterConfiguration.OperatorType.And,
                                         Values = new List<String> {"1","203"}
                                     }
                             };


            ReportDatasetBase rd = new ReportDatasetGeneric(reports.ConnectionStringRDB, rc, reports.DataBaseBase);
            rd.LoadDataset();
            rc.DataTableResult = rd.DatasetLoaded.Tables[0];
            reports.AddProcessReport(rc);
            reports.SendQueue();


        }
    }
}

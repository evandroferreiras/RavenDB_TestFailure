using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Raven.Tests.Helpers;
using ReportsEverywhereClass.Database_structure;
using ReportsEverywhereClass.Databases;
using ReportsEverywhereClass.Reports;
using Xunit;
namespace RavenDB_TestFailure
{


    public class MyTest : RavenTestBase
    {

        [Fact]
        public void Execute()
        {
            
            #region "Creating and preparing the object to send to RavenDB"

            DataBaseBase dataBaseBase = new DatabaseSQLServer("Server=.\\SQLEXPRESS;Database=AdventureWorks2008;Trusted_Connection=True;",
                      "http://localhost:8080", "Test");
            dataBaseBase.LoadSchema();
            ProcessingQueueReports reports = new ProcessingQueueReports("http://localhost:8080",                                                                        
                                                                        "",
                                                                        "Teste",
                                                                        DataBaseBase.BancoDados.MSSQLSERVER,
                                                                        dataBaseBase);
            ReportConfiguration rc = new ReportConfiguration();
            rc.Id = 1;
            rc.ReportName = "ReportTest";
            rc.MainTable = new DSTable(1,"Test","schema");           
            rc.MainSchemaName = "schema";
            rc.Columns = new List<DSColumn>{new DSColumn(),new DSColumn()};
            reports.AddProcessReport(rc);
            #endregion

                       
            using (var store = NewDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(reports);
                    session.SaveChanges();
                }

            }

        }
    }

    
}

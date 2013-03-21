using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Extensions;
using Raven.Imports.Newtonsoft.Json;
using ReportsEverywhereClass.Database_structure;


namespace ReportsEverywhereClass.Reports
{
    public class ProcessingQueueReports : RavenDBBase
    {
        [JsonIgnore]
        public string ConnectionStringRDB;
        [JsonIgnore]
        public DataBaseBase DataBaseBase;
        [JsonIgnore]
        private DataBaseBase.BancoDados _tipoBancoDados;

        public List<ReportConfiguration> ReportConfigurations;

        public ProcessingQueueReports() 
        {
        }

        public ProcessingQueueReports(string connectionStringODDB, string connectionStringRDB, string databaseODDB, DataBaseBase.BancoDados bancoDados, DataBaseBase dataBaseBase) : base(connectionStringODDB, databaseODDB)
        {
            ReportConfigurations = new List<ReportConfiguration>();
            ConnectionStringRDB = connectionStringRDB;
            _tipoBancoDados = bancoDados;
            DataBaseBase = dataBaseBase;
        }

        public int LoadQueue()
        {
            
            using (var documentStore = new DocumentStore())
            {
                documentStore.Url = ConnectionStringODDB;
                documentStore.Initialize();
                documentStore.DatabaseCommands.EnsureDatabaseExists(DatabaseODDB);
                using (IDocumentSession session = documentStore.OpenSession(DatabaseODDB))
                {
                    var existingInDatabase = session.Query<ProcessingQueueReports>();

                    if (existingInDatabase != null)
                    {
                        foreach (var p in existingInDatabase.ToList())
                        {
                            this.ReportConfigurations = p.ReportConfigurations;
                        }
                    }
                }
            }
            

            return this.ReportConfigurations.Count(x => (x.Status == ReportConfiguration.StatusType.Pending) );
        }

        public void SendQueue()
        {
            ProcessingQueueReports processingQueueReports = new ProcessingQueueReports(
                base.ConnectionStringODDB,
                this.ConnectionStringRDB, 
                base.DatabaseODDB, this._tipoBancoDados, this.DataBaseBase);
            processingQueueReports.LoadQueue();
            var exceptDiference = processingQueueReports.ReportConfigurations.Except(this.ReportConfigurations);
            int count = exceptDiference.Count();
            if (count > 0)
            {
                foreach (var reportConfiguration in exceptDiference)
                {
                    this.ReportConfigurations.Add(reportConfiguration);
                }
            }

            using (var documentStore = new DocumentStore())
            {
                documentStore.Url = ConnectionStringODDB;
                documentStore.Initialize();
                documentStore.DatabaseCommands.EnsureDatabaseExists(DatabaseODDB);
                using (IDocumentSession session = documentStore.OpenSession(DatabaseODDB))
                {
                    var existingInDatabase = session.Query<ProcessingQueueReports>();

                    if (existingInDatabase != null)
                    {
                        foreach (var dataBaseBase in existingInDatabase)
                        {
                            session.Delete(dataBaseBase);
                            session.SaveChanges();
                        }
                    }
                }
            }

            base.SaveToRavenDB();
        }

        public void AddProcessReport(ReportConfiguration reportConfiguration)
        {
            LoadQueue();
            reportConfiguration.Id =  ReportConfigurations.Count + 1;
            ReportConfigurations.Add(reportConfiguration);
        }

        public void DeleteProcessReport(int idRc)
        {
            ReportConfigurations.RemoveAt(idRc);
        }

        public ReportConfiguration ProcessNextReport()
        {
            ReportConfiguration rc = null;
            if (ReportConfigurations.Exists(x => (x.Status == ReportConfiguration.StatusType.Pending )))
            {
                rc = ReportConfigurations.First(x => (x.Status == ReportConfiguration.StatusType.Pending ));    
            }
            

            if (rc != null){

                ReportConfigurations.Remove(rc);
                rc.Status = ReportConfiguration.StatusType.Processing;
                ReportConfigurations.Add(rc);
                SendQueue();

                ReportDatasetBase rd = ReportDatasetFactory.GetReportDataset(_tipoBancoDados, ConnectionStringRDB, rc, DataBaseBase);
                rd.LoadDataset();

                rc.DataTableResult = rd.DatasetLoaded.Tables[0];

                if (ReportConfigurations.Exists(x => (x.Id == rc.Id)))
                {
                    int idx = ReportConfigurations.FindIndex(x => (x.Id == rc.Id));
                    rc.Status = ReportConfiguration.StatusType.Completed;                    
                    ReportConfigurations[idx] = rc;
                    SendQueue();    
                }
                
                return rc;
            }else
            {
                return null;
            }
        }
    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Extensions;
using Raven.Client.Linq;

namespace ReportsEverywhereClass
{
    public abstract class RavenDBBase
    {

        public string ConnectionStringODDB;
        public string DatabaseODDB;

        protected RavenDBBase()
        {
        }

        protected RavenDBBase(string connectionStringODDB, string databaseODDB)
        {
            this.ConnectionStringODDB = connectionStringODDB;
            this.DatabaseODDB = databaseODDB;
        }


        protected void SaveToRavenDB()
        {
            using(var documentStore = new DocumentStore())
            {
                documentStore.Url = ConnectionStringODDB;
                documentStore.Initialize();
                documentStore.DatabaseCommands.EnsureDatabaseExists(DatabaseODDB);
                using (IDocumentSession session = documentStore.OpenSession(DatabaseODDB))
                {                    
                    session.Store(this);
                    session.SaveChanges();
                }                
            }
        }

        public virtual T LoadFromRavenDB<T>()
        {
            throw new NotImplementedException();
        }
    }
}

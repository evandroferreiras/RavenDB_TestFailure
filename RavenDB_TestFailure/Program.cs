using System;
using System.Collections.Generic;
using Raven.Tests.Helpers;
using ReportsEverywhereClass.Database_structure;
using ReportsEverywhereClass.Databases;
using ReportsEverywhereClass.Reports;
using Xunit;

namespace RavenDB_TestFailure
{
    internal class Program
    {        
        static void Main(string[] args)
        {
            new MyTest().Execute();
        }

    }
}

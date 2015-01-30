using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Linq;
using NHibernate.Tool.hbm2ddl;
using NHibernateThenFetchManyTest.Entities;

namespace NHibernateThenFetchManyTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();

            var sessionFactory = CreateSessionFactory();

            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    // create a couple of Stores each with some Components and Employees
                    var bigMachine = new MaintenanceObject {Name = "Great big Machine"};
                    var smallMachine = new MaintenanceObject {Name = "Smaller Machine"};

                    var cog = new Component {Name = "cog", Price = 3.60};
                    var screw = new Component {Name = "screw", Price = 4.49};
                    var torque = new Component {Name = "torque", Price = 0.79};
                    var couplings = new Component {Name = "couplings", Price = 1.29};
                    var hinge = new Component {Name = "hinge", Price = 2.10};
                    var toggle = new Component {Name = "toggle", Price = 2.41};

                    var job1 = new MaintenanceJob {JobNumber = "#1", JobInfo = "Harrison"};
                    var job2 = new MaintenanceJob {JobNumber = "#2", JobInfo = "Torrance"};
                    var job3 = new MaintenanceJob {JobNumber = "#3", JobInfo = "Walkters"};
                    var job4 = new MaintenanceJob {JobNumber = "#4", JobInfo = "Taft"};
                    var job5 = new MaintenanceJob {JobNumber = "#5", JobInfo = "Pope"};

                    // add componentses to the stores, there's some crossover in the componentses in each
                    // MaintenanceObject, because the MaintenanceObject-Component relationship is many-to-many
                    AddComponentsToMaintenanceObject(bigMachine, cog, screw, torque, couplings, hinge);
                    AddComponentsToMaintenanceObject(smallMachine, couplings, hinge, toggle);

                    // add maintenanceJobs to the stores, this relationship is a one-to-many, so one
                    // MaintenanceJob can only work at one MaintenanceObject at a time
                    AddMaintenanceJobsToMaintenanceObject(bigMachine, job1, job2, job3);
                    AddMaintenanceJobsToMaintenanceObject(smallMachine, job4, job5);

                    // save both stores, this saves everything else via cascading
                    session.SaveOrUpdate(bigMachine);
                    session.SaveOrUpdate(smallMachine);

                    transaction.Commit();
                }

                // retreive all stores and display them
                using (session.BeginTransaction())
                {
                    //var stores = session.CreateCriteria(typeof (MaintenanceObject))
                    //    .List<MaintenanceObject>();

                    //foreach (var MaintenanceObject in stores)
                    //{
                    //    WriteStorePretty(MaintenanceObject);
                    //}

                    var jobs = session.Query<MaintenanceJob>().Fetch(x => x.MaintenanceObject).ThenFetchMany(x => x.Components).ToList();

                }

                Console.ReadKey();
            }
        }

        private static void WriteStorePretty(MaintenanceObject maintenanceObject)
        {
            Console.WriteLine(maintenanceObject.Name);
            Console.WriteLine("  Components:");

            foreach (var product in maintenanceObject.Components)
            {
                Console.WriteLine("    " + product.Name);
            }

            Console.WriteLine("  Jobs:");

            foreach (var employee in maintenanceObject.Jobs)
            {
                Console.WriteLine("    " + employee.JobNumber + " " + employee.JobInfo);
            }

            Console.WriteLine();
        }

        public static void AddComponentsToMaintenanceObject(MaintenanceObject maintenanceObject, params Component[] componentses)
        {
            foreach (var product in componentses)
            {
                maintenanceObject.AddProduct(product);
            }
        }

        public static void AddMaintenanceJobsToMaintenanceObject(MaintenanceObject maintenanceObject, params MaintenanceJob[] maintenanceJobs)
        {
            foreach (var employee in maintenanceJobs)
            {
                maintenanceObject.AddEmployee(employee);
            }
        }

        private static ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
                .Database(
                    SQLiteConfiguration.Standard
                        .UsingFile("firstProject.db")
                )
                .Mappings(m =>
                    m.FluentMappings.AddFromAssemblyOf<Program>())
                .ExposeConfiguration(BuildSchema)
                .BuildSessionFactory();
        }

        private static void BuildSchema(Configuration config)
        {
            // delete the existing db on each run
            if (File.Exists("firstProject.db"))
                File.Delete("firstProject.db");

            // this NHibernate tool takes a configuration (with mapping info in)
            // and exports a database schema from it
            new SchemaExport(config)
              .Create(false, true);
        }
    }
}

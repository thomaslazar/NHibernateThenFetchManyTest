﻿using System;
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
                    // create a couple of Stores each with some Products and Employees
                    var barginBasin = new Store {Name = "Bargin Basin"};
                    var superMart = new Store {Name = "SuperMart"};

                    var potatoes = new Product {Name = "Potatoes", Price = 3.60};
                    var fish = new Product {Name = "Fish", Price = 4.49};
                    var milk = new Product {Name = "Milk", Price = 0.79};
                    var bread = new Product {Name = "Bread", Price = 1.29};
                    var cheese = new Product {Name = "Cheese", Price = 2.10};
                    var waffles = new Product {Name = "Waffles", Price = 2.41};

                    var daisy = new Employee {FirstName = "Daisy", LastName = "Harrison"};
                    var jack = new Employee {FirstName = "Jack", LastName = "Torrance"};
                    var sue = new Employee {FirstName = "Sue", LastName = "Walkters"};
                    var bill = new Employee {FirstName = "Bill", LastName = "Taft"};
                    var joan = new Employee {FirstName = "Joan", LastName = "Pope"};

                    // add products to the stores, there's some crossover in the products in each
                    // store, because the store-product relationship is many-to-many
                    AddProductsToStore(barginBasin, potatoes, fish, milk, bread, cheese);
                    AddProductsToStore(superMart, bread, cheese, waffles);

                    // add employees to the stores, this relationship is a one-to-many, so one
                    // employee can only work at one store at a time
                    AddEmployeesToStore(barginBasin, daisy, jack, sue);
                    AddEmployeesToStore(superMart, bill, joan);

                    // save both stores, this saves everything else via cascading
                    session.SaveOrUpdate(barginBasin);
                    session.SaveOrUpdate(superMart);

                    transaction.Commit();
                }

                // retreive all stores and display them
                using (session.BeginTransaction())
                {
                    //var stores = session.CreateCriteria(typeof (Store))
                    //    .List<Store>();

                    //foreach (var store in stores)
                    //{
                    //    WriteStorePretty(store);
                    //}

                    var employees = session.Query<Employee>().Fetch(x => x.Store).ThenFetchMany(x => x.Products).ToList();

                }

                Console.ReadKey();
            }
        }

        private static void WriteStorePretty(Store store)
        {
            Console.WriteLine(store.Name);
            Console.WriteLine("  Products:");

            foreach (var product in store.Products)
            {
                Console.WriteLine("    " + product.Name);
            }

            Console.WriteLine("  Staff:");

            foreach (var employee in store.Staff)
            {
                Console.WriteLine("    " + employee.FirstName + " " + employee.LastName);
            }

            Console.WriteLine();
        }

        public static void AddProductsToStore(Store store, params Product[] products)
        {
            foreach (var product in products)
            {
                store.AddProduct(product);
            }
        }

        public static void AddEmployeesToStore(Store store, params Employee[] employees)
        {
            foreach (var employee in employees)
            {
                store.AddEmployee(employee);
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

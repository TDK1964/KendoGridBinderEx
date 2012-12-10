using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Transactions;
using KendoGridBinder.Examples.MVC.Data.Entities;

namespace KendoGridBinder.Examples.MVC.Data
{
    public class MyDataContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<MainCompany> MainCompanies { get; set; }

        public DbSet<Product> Products { get; set; }

        static MyDataContext()
        {
            if (ApplicationConfig.DatabaseInit)
            {
                Database.SetInitializer(new InitDatabase());
            }
            else
            {
                Database.SetInitializer<MyDataContext>(null); // must be turned off before mini profiler runs
            }
        }
    }

    public class InitDatabase : IDatabaseInitializer<MyDataContext>
    {
        public void InitializeDatabase(MyDataContext context)
        {
            bool dbExists;
            using (new TransactionScope(TransactionScopeOption.Suppress))
            {
                dbExists = context.Database.Exists();
            }

            if (!dbExists)
            {
                throw new Exception();
            }

            var objectContext = (context as IObjectContextAdapter).ObjectContext;

            // remove all tables if present
            var tables = new[] { "KendoGrid_Product", "KendoGrid_Employee", "KendoGrid_Company", "KendoGrid_MainCompany" };

            foreach (var table in tables)
            {
                var results = objectContext.ExecuteStoreQuery<string>(string.Format("SELECT name FROM dbo.sysobjects WHERE xtype = 'U' AND name = '{0}'", table));
                if (results.Any())
                {
                    context.Database.ExecuteSqlCommand(string.Format("DROP TABLE [{0}]", table));
                }
            }

            // recreate all tables
            var dbCreationScript = objectContext.CreateDatabaseScript();
            context.Database.ExecuteSqlCommand(dbCreationScript);

            Seed(context);
            context.SaveChanges();
        }

        private void Seed(MyDataContext context)
        {
            var mainCompany1 = new MainCompany { Id = 10, Name = "M - 1" };
            var mainCompany2 = new MainCompany { Id = 20, Name = "M - 2" };
            var mainCompanies = new List<MainCompany> { mainCompany1, mainCompany2 };
            mainCompanies.ForEach(x => context.MainCompanies.Add(x));

            var companyA = new Company { Id = 1, Name = "A", MainCompany = mainCompany1 };
            var companyB = new Company { Id = 2, Name = "B", MainCompany = mainCompany1 };
            var companyC = new Company { Id = 3, Name = "C", MainCompany = mainCompany2 };
            var companies = new List<Company> { companyA, companyB, companyC };
            companies.ForEach(x => context.Companies.Add(x));

            var employees = new List<Employee>
            {
                new Employee { Id = 1, Company = companyA, FirstName = "Bill", LastName = "Smith", Email = "bsmith@email.com", EmployeeNumber = 1001, HireDate = Convert.ToDateTime("01/12/1990")},
                new Employee { Id = 2, Company = companyB, FirstName = "Jack", LastName = "Smith", Email = "jsmith@email.com", EmployeeNumber = 1002, HireDate = Convert.ToDateTime("12/12/1997")},
                new Employee { Id = 3, Company = companyC, FirstName = "Mary", LastName = "Gates", Email = "mgates@email.com", EmployeeNumber = 1003, HireDate = Convert.ToDateTime("03/03/2000")},
                new Employee { Id = 4, Company = companyA, FirstName = "John", LastName = "Doe", Email = "jd@email.com", EmployeeNumber = 1004, HireDate = Convert.ToDateTime("11/11/2011")},
                new Employee { Id = 5, Company = companyB, FirstName = "Chris", LastName = "Cross", Email = "cc@email.com", EmployeeNumber = 1005, HireDate = Convert.ToDateTime("05/05/1995")},
                new Employee { Id = 6, Company = companyC, FirstName = "Niki", LastName = "Myers", Email = "nm@email.com", EmployeeNumber = 1006, HireDate = Convert.ToDateTime("06/05/1995")},
                new Employee { Id = 7, Company = companyA, FirstName = "Joseph", LastName = "Hall", Email = "jh@email.com", EmployeeNumber = 1007, HireDate = Convert.ToDateTime("07/05/1995")},
                new Employee { Id = 8, Company = companyB, FirstName = "Daniel", LastName = "Wells", Email = "cc@email.com", EmployeeNumber = 1008, HireDate = Convert.ToDateTime("08/05/1995")},
                new Employee { Id = 9, Company = companyC, FirstName = "Robert", LastName = "Lawrence", Email = "cc@email.com", EmployeeNumber = 1009, HireDate = Convert.ToDateTime("09/05/1995")},
                new Employee { Id = 10, Company = companyA, FirstName = "Reginald", LastName = "Quinn", Email = "cc@email.com", EmployeeNumber = 1010, HireDate = Convert.ToDateTime("10/05/1995")},
                new Employee { Id = 11, Company = companyB, FirstName = "Quinn", LastName = "Quick", Email = "cc@email.com", EmployeeNumber = 1011, HireDate = Convert.ToDateTime("11/05/1995")},
                new Employee { Id = 12, Company = companyC, FirstName = "Test", LastName = "User", Email = "tu@email.com", EmployeeNumber = 1012, HireDate = Convert.ToDateTime("11/05/2012")},
            };
            employees.ForEach(x => context.Employees.Add(x));

            var products = new List<Product>
            {
                new Product { Id = 1, Code = "AR-5381", Name = "Adjustable Race"},
                new Product { Id = 2, Code = "BA-8327", Name = "Bearing Ball"},
                new Product { Id = 3, Code = "BE-2349", Name = "BB Ball Bearing"},
                new Product { Id = 4, Code = "BE-2908", Name = "Headset Ball Bearings"},
                new Product { Id = 316, Code = "BL-2036", Name = "Blade"},
                new Product { Id = 317, Code = "CA-5965", Name = "LL Crankarm"},
                new Product { Id = 318, Code = "CA-6738", Name = "ML Crankarm"},
                new Product { Id = 319, Code = "CA-7457", Name = "HL Crankarm"},
                new Product { Id = 320, Code = "CB-2903", Name = "Chainring Bolts"},
                new Product { Id = 321, Code = "CN-6137", Name = "Chainring Nut"},
                new Product { Id = 322, Code = "CR-7833", Name = "Chainring"}
            };
            products.ForEach(x => context.Products.Add(x));
        }
    }
}
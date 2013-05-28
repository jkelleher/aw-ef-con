using System;
using System.Linq;
using System.Collections.Generic;
using AWModel;
using System.Data.Entity;

namespace AWConEF
{
    internal class Program
    {
        private static AWEntities context = new AWEntities();

        private static void Main(string[] args)
        {
            //init_database();
            var query = (from c in context.Customers
                        where (c.CompanyName.StartsWith("S"))
                        orderby c.CompanyName descending
                        select c).Count();

            var prdInCat = from p in context.ProductCategories.Include("Product")
                           where (p.ProductCategoryID == 6)
                           select p.Products;

            var prodRoadBikes = from p in context.Products
                                where p.ProductCategoryID == 6
                                select p;
            //foreach (var roadbike in prodRoadBikes)
            //{
            //    Console.WriteLine(roadbike.Name);
            //}
            foreach (Product prd in prdInCat.First())
            {
                Console.WriteLine(prd.Name);
            }
            
            //var query = context.Customers
            //    .Where(c => c.CompanyName.StartsWith("S"))
            //    .OrderByDescending(c => c.CompanyName)
            //    .Skip(30);

            //foreach (var cust in query)
            //{
            //    Console.WriteLine(cust.CompanyName);
            //}
            //Console.WriteLine("Count of Customers is: " + query);
            //dumpQuery(firstCustomer(),100);
            //dumpQuery(pullMr(),10);
            addCustomer();
            deleteCustomer();
            salesByCustomer(29741);
            Console.ReadLine();
        }

        private static void addCustomer()
        {
            Customer c = new Customer { FirstName = "John", LastName = "Kelleher" };
            //context.Customers.AddObject(c);       // only for ObjectContext
            context.Customers.Attach(c);
            context.Customers.Add(c);
            context.SaveChanges();
        }

        private static void deleteCustomer()
        {
            var fndCust = (from c in context.Customers
                                    where c.LastName=="Kelleher"
                                    select c).Single();
            context.Customers.Remove(fndCust);
        }

        private static void init_database()
        {
            var ctx = new AWEntities();
            //Database.SetInitializer<AWEntities>(new DropCreateDatabaseIfModelChanges<AWEntities>());
        }

        private static void salesByCustomer(int cID)
        {
            // invoke eager loading
            //var query = (from c in context.Customers.Include("SalesOrderHeader")
            // using lazy loading
            var query = (from c in context.Customers
                        where c.CustomerID == cID
                        select new { CID = c.CustomerID, CSales = c.SalesOrderHeaders });
                        
            var C = query.First();
            Console.WriteLine(String.Format("***** {0} *****",C.CID));
                foreach (SalesOrderHeader item in C.CSales)
                {
                    Console.WriteLine(String.Format("     {0:D}: {1:C2}", item.DueDate, item.TotalDue));
                }
        }

        private static void dumpQuery(IQueryable<Customer> qry, int topN)
        {
            foreach (Customer c in qry.Take(topN))
            {
                Console.WriteLine(String.Format("{0} {1} {2}",c.Title, c.FirstName, c.LastName));
            }
        }

        private static IQueryable<AWModel.Customer> firstCustomer()
        {
            var query = (from c in context.Customers
                         select c);
            return query;
        }
        private static IQueryable<Customer> pullMr()
        {
            //IQueryable<Customer> query;
            return from c in context.Customers
                            where (c.Title == "Mr.")
                            select c;
            //return query;
        }
    }   // end Main()
}   // end Program
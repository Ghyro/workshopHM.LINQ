// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using SampleSupport;
using Task.Data;

// Version Mad01

namespace SampleQueries
{
    [Title("LINQ Module")]
    [Prefix("Linq")]
    public class LinqSamples : SampleHarness
    {

        private DataSource dataSource = new DataSource();

        [Category("LINQ Workshop")]
        [Title("Task 1")]
        [Description("This sample returns a list of customers whose the sum of all orders total greater than X")]
        public void Linq1()
        {
            const int x = 1000;

            var customers =
                from c in dataSource.Customers
                let sum = c.Orders.Sum(o => o.Total)
                where sum > x
                orderby sum
                select new
                {
                    ID = c.CustomerID,
                    CompanyName = c.CompanyName,
                    TotalSum = sum
                };

            Console.WriteLine("The list of all customers: ");

            foreach (var i in customers)
            {
                ObjectDumper.Write(i);
            }
        }

        [Category("LINQ Workshop")]
        [Title("Task 2")]
        [Description("This sample returns a list of suppliers for each customer, located in the same country and in the same city")]
        public void Linq2()
        {
            var customerSuppliers =
                from customer in dataSource.Customers
                from supplier in dataSource.Suppliers
                where supplier.Country == customer.Country && supplier.City == customer.City
                select new
                {
                    Customer = customer.CompanyName,
                    Country = customer.Country,
                    City = customer.City,
                    Supplier = supplier.SupplierName,
                    SupCountry = supplier.Country,
                    SupCity = supplier.City
                };

            Console.WriteLine("Customer country and city equals Supplier country and city");

            foreach (var i in customerSuppliers)
            {
                ObjectDumper.Write(i);
            }
        }

        [Category("LINQ Workshop")]
        [Title("Task 3")]
        [Description("This sample returns a list of customers who had orders that the sum greater than X")]
        public void Linq3()
        {
            const int x = 1000;

            var customers =
                from customer in dataSource.Customers
                from order in customer.Orders
                where order.Total > x
                orderby order.Total
                select new
                {
                    Customer = customer.CompanyName,
                    OrderTotal = order.Total
                };

            Console.WriteLine("The list of customers where their order total sum > 1000");

            foreach (var i in customers)
            {
                ObjectDumper.Write(i);
            }
        }

        [Category("LINQ Workshop")]
        [Title("Task 4")]
        [Description("This sample returns a list of customers that include the month of year when they became customers")]
        public void Linq4()
        {
            var customers =
                from customer in dataSource.Customers
                where customer.Orders.Count() > 0
                let date = customer.Orders.Min(o => o.OrderDate)
                select new
                {
                    Customer = customer.CompanyName,
                    From = date
                };

            Console.WriteLine("The list of customers which shows when they became customers");

            foreach (var i in customers)
            {
                ObjectDumper.Write(i);
            }
        }

        [Category("LINQ Workshop")]
        [Title("Task 5")]
        [Description("This sample returns a list of customers that include the month of year when they became customers and sorted by month, year, customer's turnover (from max to min), name")]
        public void Linq5()
        {
            var customers =
                from customer in dataSource.Customers
                where customer.Orders.Count() > 0
                let date = customer.Orders.Min(o => o.OrderDate)
                let turnover = customer.Orders.Sum(o => o.Total)
                orderby date.Year, date.Month, turnover descending, customer.CompanyName
                select new
                {
                    Customer = customer.CompanyName,
                    From = date,
                    Total = turnover
                };

            Console.WriteLine("The list of customers which include the month of year when they became customers that sorted by month, year, customer's turnover (from max to min), name");

            foreach (var i in customers)
            {
                ObjectDumper.Write(i);
            }
        }

        [Category("LINQ Workshop")]
        [Title("Task 6")]
        [Description("This sample returns a list of customers who have a non-digit postal code or the region isn't filled or phone number doens't have the operator code")]
        public void Linq6()
        {
            const string validPattern = @"^\d{10}$";

            var regex = new Regex(validPattern);

            var customers =
                from customer in dataSource.Customers
                let validPostalCode = customer.PostalCode != null && regex.IsMatch(customer.PostalCode)
                where validPostalCode == false || customer.Region is null || customer.Phone.StartsWith("(")
                select new
                {
                    Customer = customer.CompanyName,
                    PostalCode = customer.PostalCode,
                    Region = customer.Region,
                    PhoneNumber = customer.Phone
                };

            Console.WriteLine("Non-digit postal code or the region isn't filled or phone number doesn't have the operator code");

            foreach (var i in customers)
            {
                ObjectDumper.Write(i);
            }
        }

        [Category("LINQ Workshop")]
        [Title("Task 7")]
        [Description("This sample returns group of products category which inside - by available in stock, inside group need to sort by cost")]
        public void Linq7()
        {
            var products =
                from product in dataSource.Products
                group product by product.Category into categorGroup
                select new
                {
                    Category = categorGroup.Key,
                    Products =
                    from product in categorGroup
                    group product by product.UnitsInStock > 0 into available
                    select new
                    {
                        inStock = available.Key,
                        Product =
                        from product in available
                        orderby product.UnitPrice
                        select new
                        {
                            Product = product.ProductName,
                            Count = product.UnitsInStock,
                            Price = product.UnitPrice
                        }
                    }
                };

            Console.WriteLine("Group of products category which inside - by available in stock, inside group need to sort by cost");

            foreach (var i in products)
            {
                Console.WriteLine("--------------");

                ObjectDumper.Write($"{i.Category}");

                foreach (var j in i.Products)
                {
                    string status = j.inStock ? "Available" : "Not available";

                    ObjectDumper.Write(status);

                    Console.WriteLine("--------------");

                    foreach (var product in j.Product)
                    {
                        ObjectDumper.Write(product);
                    }
                }
            }
        }

        [Category("LINQ Workshop")]
        [Title("Task 8")]
        [Description("This sample returns products into groups: cheap, average, expensive")]
        public void Linq8()
        {
            const int cheap = 10;
            const int expensive = 50;

            var products =
                from product in dataSource.Products
                group product by product.UnitPrice <= 10 ? "Cheap" : product.UnitPrice > cheap && product.UnitPrice < expensive ?
                "Average" : product.UnitPrice > expensive ? "Expensive" : "Unknown" into g
                orderby g.Key
                select g;

            Console.WriteLine("Product groups: cheap, average, expensive");

            foreach (var i in products)
            {
                ObjectDumper.Write(i.Key);

                Console.WriteLine("--------");

                foreach (var j in i)
                {
                    ObjectDumper.Write($"{j.ProductName} - {j.UnitPrice}");
                }

                Console.WriteLine();
            }

            /*Without groupBy*/

            //var products =
            //    from product in dataSource.Products
            //    let groupOf = (
            //        product.UnitPrice <= 10 ? "Cheap" : product.UnitPrice > cheap && product.UnitPrice < expensive ? "Average" : product.UnitPrice > expensive ? "Expensive" : "Unknown"
            //    )
            //    select new
            //    {
            //        Product = product.ProductName,
            //        Price = product.UnitPrice,
            //        GroupOf = groupOf
            //    };

            //Console.WriteLine("Product groups: cheap, average, expensive");

            //foreach (var i in products)
            //{
            //    ObjectDumper.Write(i);
            //}
        }

        [Category("LINQ Workshop")]
        [Title("Task 9")]
        [Description("This sample returns average profitability of each city and the average intensity")]
        public void Linq9()
        {
            var cities =
                from customer in dataSource.Customers
                group customer by customer.City into g
                select new
                {
                    City = g.Key,
                    AverageProfit = g.Average(p => p.Orders.Sum(s => s.Total)),
                    AverageIntensity = g.Average(p => p.Orders.Length)
                };

            Console.WriteLine("AverageProfit and AverageIntensity each city");

            foreach (var i in cities)
            {
                ObjectDumper.Write($"City: {i.City}");
                ObjectDumper.Write($"AverageProfit : {i.AverageProfit}");
                ObjectDumper.Write($"AverageIntensity: {i.AverageIntensity}");

                Console.WriteLine("-----------");
            }
        }

        [Category("LINQ Workshop")]
        [Title("Task 10")]
        [Description("This sample returns monthly, annual and for the year - month statistics")]
        public void Linq10()
        {
            var customers =
                from customer in dataSource.Customers
                select new
                {
                    Customer = customer.CompanyName,
                    Month =
                        from i in customer.Orders
                        group i by i.OrderDate.Month into g
                        select new
                        {
                            Month = g.Key,
                            CountOfOrder = g.Count()
                        },
                    Year =
                        from i in customer.Orders
                        group i by i.OrderDate.Year into g
                        select new
                        {
                            Year = g.Key,
                            CountOfOrder = g.Count()
                        },
                    YearMonth = 
                        from i in customer.Orders
                        group i by new
                        {
                            i.OrderDate.Month,
                            i.OrderDate.Year
                        }
                        into g
                        select new
                        {
                            Year = g.Key,
                            Month = g.Key,
                            CountOfOrder = g.Count()
                        }
                };

            Console.WriteLine("Monthly, annual, year-month statistics");

            foreach(var i in customers)
            {
                ObjectDumper.Write(i.Customer);

                Console.WriteLine("------");

                ObjectDumper.Write("Monthly statistics:");

                foreach(var m in i.Month)
                {
                    ObjectDumper.Write($"{m.Month} - {m.CountOfOrder}");
                }

                ObjectDumper.Write("Annual statistics:");

                foreach(var y in i.Year)
                {
                    ObjectDumper.Write($"{y.Year} - {y.CountOfOrder}");
                }

                ObjectDumper.Write("Year-month statistics:");

                foreach(var ym in i.YearMonth)
                {
                    ObjectDumper.Write($"{ym.Year} - {ym.Month} - {ym.CountOfOrder}");
                }

                Console.WriteLine();
            }
        }
    }
}
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

            foreach(var i in customerSuppliers)
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

            foreach(var i in customers)
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

            foreach(var i in customers)
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

            foreach(var i in customers)
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
                where validPostalCode == false || customer.Region is null || customer.Phone[0] != '('
                select new
                {
                    Customer = customer.CompanyName,
                    PostalCode = customer.PostalCode,
                    Region = customer.Region,
                    PhoneNumber = customer.Phone
                };

            Console.WriteLine("Non-digit postal code or the region isn't filled or phone number doesn't have the operator code");

            foreach(var i in customers)
            {
                ObjectDumper.Write(i);
            }
        }

    }
}
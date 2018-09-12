using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace DapperTestManyToMany
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();

            watch.Start();

            var gr = Person.QueryAll();
            foreach (Person item in gr)
            {
                foreach (Person elternteil in item.Eltern)
                {
                    Console.WriteLine($"Person {item.Name} mit Elternteil {elternteil.Name}");

                }
            }

            Console.ReadLine();
        }

        
       
    }
}

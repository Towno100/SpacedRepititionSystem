using Microsoft.EntityFrameworkCore;
using ReviewEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpacedReptititonSystem
{
    public class Program
    {
        static void Main(string[] args)
        {
            var dbContext = new CapitalCityDbContext();
            const int numberToStudy = 10;
            IReviewManager<CapitalCity> reviewManager = new ReviewManager<CapitalCity>(numberToStudy, 20);
            ConsoleKeyInfo nextAction = new ConsoleKeyInfo();
            while (nextAction.Key != ConsoleKey.X)
            {
                Console.Write("Press a key to [S]tudy, [T]est, or E[x]it");
                var capitalCities = dbContext.CapitalCities;
                var studyCities = reviewManager.GetCurrent(capitalCities);
                nextAction = Console.ReadKey();
                Console.WriteLine();

                if (nextAction.Key == ConsoleKey.S)
                {
                    foreach (var c in studyCities)
                    {
                        Console.WriteLine($"{c.Country}:\t{c.Capital}");
                    }
                }

                if (nextAction.Key == ConsoleKey.T)
                {
                    var answers = new string[numberToStudy];
                    var index = 0;
                    foreach (var c in studyCities)
                    {
                        Console.WriteLine($"{c.Country}");
                        answers[index] = Console.ReadLine();
                        index++;
                    }
                    var correctCount = reviewManager.ParseTestResults<string>(studyCities, answers, (a, c) => c.Capital.Trim() == a);
                    
                    Console.WriteLine($"Well done, you got {correctCount} correct");
                }
                dbContext.SaveChanges();
            }
        }
    }

    public class CapitalCity : IReviewable
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public int StudyOrder { get; set; }
        public bool IsCurrent { get; set; }
        public DateTime LastStudied { get; set; }
        public string Country { get; set; }
        public string Capital { get; set; }
    }

    public class CapitalCityDbContext : DbContext
    {
        private const string _connectionString = "server=.; database=CapitalCities;Trusted_Connection=True;";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        public DbSet<CapitalCity> CapitalCities { get; set; }
    }
}

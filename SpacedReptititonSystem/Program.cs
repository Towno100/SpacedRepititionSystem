using Microsoft.EntityFrameworkCore;
using ReviewEngine;
using System;

namespace SpacedReptititonSystem
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
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
        private const string _connectionString = "server=LAPTOP-S30LJ72T; database=CapitalCities;Trusted_Connection=True;";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        public DbSet<CapitalCity> CapitalCities { get; set; }
    }
}

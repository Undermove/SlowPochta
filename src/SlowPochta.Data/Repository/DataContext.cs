using System;
using Microsoft.EntityFrameworkCore;
using SlowPochta.Data.Model;

namespace SlowPochta.Data.Repository
{
	public class DataContext : DbContext
	{
		public string ConnectionString { get; set; }

		public DbSet<Person> Persons { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseNpgsql(ConnectionString);
		}
	}
}

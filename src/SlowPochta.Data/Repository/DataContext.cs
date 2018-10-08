using System;
using Microsoft.EntityFrameworkCore;
using SlowPochta.Data.Model;

namespace SlowPochta.Data.Repository
{
	public class DataContext : DbContext
	{
		public DbSet<Person> Persons { get; set; }

		public DataContext(DbContextOptions<DataContext> options)
			: base(options)
		{

		}
	}
}

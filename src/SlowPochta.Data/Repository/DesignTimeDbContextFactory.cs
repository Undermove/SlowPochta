using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SlowPochta.Data.Repository
{
	public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DataContext>
	{
		public DataContext CreateDbContext(string[] args)
		{
			var dbContext = new DataContext
			{
				ConnectionString = args[0]
			};

			return dbContext;
		}
	}
}

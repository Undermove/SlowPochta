using Microsoft.EntityFrameworkCore;
using SlowPochta.Data.Model;

namespace SlowPochta.Data.Repository
{
	public class DataContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<MessageToUser> MessagesToUsers { get; set; }
		public DbSet<MessageFromUser> MessagesFromUsers { get; set; }
		public DbSet<Message> Messages { get; set; }
        public DbSet<MessagePassedDeliveryStatus> MessagePassedDeliveryStatuses { get; set; }
        public DbSet<MessageDeliveryStatusVariant> MessageDeliveryStatusVariants { get; set; }

		public DataContext(DbContextOptions<DataContext> options)
			: base(options)
		{

		}
	}
}

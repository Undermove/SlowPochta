namespace SlowPochta.Data.Model
{
	public class User
	{
		public long Id { get; set; }
		public string Login { get; set; }
		public string Password { get; set; }
		public RoleTypes Role { get; set; }
	}

	public enum RoleTypes
	{
		User,
		Administrator
	}
}

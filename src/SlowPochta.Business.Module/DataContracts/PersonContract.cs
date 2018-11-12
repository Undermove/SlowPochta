using System.ComponentModel.DataAnnotations;

namespace SlowPochta.Business.Module.DataContracts
{
	public class PersonContract
	{
		[Required]
		public string Login { get; set; }

		[Required]
		public string Password { get; set; }
	}
}

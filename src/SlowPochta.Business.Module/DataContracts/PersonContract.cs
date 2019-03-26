using System.ComponentModel.DataAnnotations;

namespace SlowPochta.Business.Module.DataContracts
{
	public class PersonContract
	{
		[Required]
		[MinLength(2)]
		public string Login { get; set; }

		[Required]
		[MinLength(6)]		
		public string Password { get; set; }
	}
}

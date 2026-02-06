using Microsoft.EntityFrameworkCore;
using ProjCars.Data;

namespace ProjCars.Tests
{
	public static class DbFactory
	{
		public static ProjCarsContext Create()
		{
			var options = new DbContextOptionsBuilder<ProjCarsContext>()
				.UseInMemoryDatabase($"projcars_tests_{System.Guid.NewGuid()}")
				.Options;

			return new ProjCarsContext(options);
		}
	}
}

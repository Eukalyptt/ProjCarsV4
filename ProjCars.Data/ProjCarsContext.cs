using Microsoft.EntityFrameworkCore;
using ProjCars.Core.Domain;

namespace ProjCars.Data
{
	public class ProjCarsContext : DbContext
	{
		public ProjCarsContext(DbContextOptions<ProjCarsContext> options) : base(options) { }

		public DbSet<Car> Cars { get; set; }
	}
}

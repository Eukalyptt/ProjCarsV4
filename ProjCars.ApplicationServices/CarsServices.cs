using ProjCars.Data;
using ProjCars.Core.Domain;
using Microsoft.EntityFrameworkCore;
using ProjCars.Core.Services;
using ProjCars.Core.Dto;

namespace ProjCars.ApplicationServices
{
	public class CarsServices : ICarsServices
	{
		private readonly ProjCarsContext _context;

		public CarsServices(ProjCarsContext context)
		{
			_context = context;
		}

		public async Task<Car> Create(CarDto dto)
		{
			Car car = new();

			car.Id = dto.Id;
			car.Brand = dto.Brand;
			car.Model = dto.Model;
			car.Year = dto.Year;
			car.Engine = dto.Engine;
			car.Price = dto.Price;
			car.CreatedAt = DateTime.UtcNow;
			car.ModifiedAt = DateTime.UtcNow;

			await _context.Cars.AddAsync(car);
			await _context.SaveChangesAsync();

			return car;
		}

		public async Task<Car?> GetAsync(int id)
		{
			var result = await _context.Cars.FirstOrDefaultAsync(x => x.Id == id);
			return result;
		}
		public async Task<Car?> UpdateAsync(int id, CarDto dto)
		{
			var car = await _context.Cars.FirstOrDefaultAsync(x => x.Id == id);
			if (car == null) return null;

			car.Brand = dto.Brand;
			car.Model = dto.Model;
			car.Year = dto.Year;
			car.Engine = dto.Engine;
			car.Price = dto.Price;
			car.ModifiedAt = DateTime.UtcNow;

			await _context.SaveChangesAsync();
			return car;
		}
		public async Task<bool> DeleteAsync(int id)
		{
			var car = await _context.Cars.FirstOrDefaultAsync(x => x.Id == id);
			if (car == null) return false;

			_context.Cars.Remove(car);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}

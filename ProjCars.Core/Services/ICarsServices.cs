using ProjCars.Core.Domain;
using ProjCars.Core.Dto;

namespace ProjCars.Core.Services
{
	public interface ICarsServices
	{
		Task<Car> Create(CarDto dto);
		Task<Car> GetAsync(int id);
		Task<Car> UpdateAsync(int id, CarDto dto);
		Task<bool> DeleteAsync(int id);
	}
}

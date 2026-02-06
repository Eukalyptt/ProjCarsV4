using Microsoft.AspNetCore.Mvc;
using ProjCars.Models.Cars;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjCars.Core.Domain;
using ProjCars.Data;
using ProjCars.Core.Services;
using ProjCars.Core.Dto;

namespace ProjCars.Controllers
{
	public class CarsController : Controller
	{
		private readonly ProjCarsContext _context;
		private readonly ICarsServices _carsServices;

		public CarsController 
			(
				ProjCarsContext context,
				ICarsServices carsServices
			)
		{
			_context = context;
			_carsServices = carsServices;
		}

		public IActionResult Index()
		{
			var result = _context.Cars
				.Select(x => new CarsIndexViewModel
				{
					Id = x.Id,
					Brand = x.Brand,
					Model = x.Model,
					Year = x.Year,
					Engine = x.Engine,
					Price = x.Price,
				});

			return View(result);
		}

		[HttpGet]
		public IActionResult Create()
		{
			CarsCreateUpdateViewModel cars = new();

			return View("CreateUpdate", cars);
		}

		[HttpPost]
		public async Task<IActionResult> Create(CarsCreateUpdateViewModel vm)
		{
			var dto = new CarDto()
			{
				Id = vm.Id,
				Brand = vm.Brand,
				Model = vm.Model,
				Year = vm.Year,
				Engine = vm.Engine,
				Price = vm.Price,
				CreatedAt = vm.CreatedAt,
				ModifiedAt = vm.ModifiedAt
			};

			var result = await _carsServices.Create(dto);

			if (result == null)
			{
				return RedirectToAction(nameof(Index));
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			var car = await _carsServices.GetAsync(id);
			if (car == null) return NotFound();

			var vm = new CarsCreateUpdateViewModel
			{
				Id = car.Id,
				Brand = car.Brand,
				Model = car.Model,
				Year = car.Year,
				Engine = car.Engine,
				Price = car.Price,
				CreatedAt = car.CreatedAt,
				ModifiedAt = car.ModifiedAt
			};

			return View("CreateUpdate", vm);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(int id, CarsCreateUpdateViewModel vm)
		{
			if (!ModelState.IsValid)
				return View("CreateUpdate", vm);

			var dto = new CarDto
			{
				Brand = vm.Brand,
				Model = vm.Model,
				Year = vm.Year,
				Engine = vm.Engine,
				Price = vm.Price
			};

			var updated = await _carsServices.UpdateAsync(id, dto);
			if (updated == null) return NotFound();

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> Details(int id)
		{
			var car = await _carsServices.GetAsync(id);
			if (car == null) return NotFound();

			return View(car); // kasutame domain modelit view's
		}

		[HttpGet]
		public async Task<IActionResult> Delete(int id)
		{
			var car = await _carsServices.GetAsync(id);
			if (car == null) return NotFound();

			return View(car);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			await _carsServices.DeleteAsync(id);
			return RedirectToAction(nameof(Index));
		}
	}
}

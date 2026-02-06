using System;
using System.Threading.Tasks;
using Xunit;
using ProjCars.ApplicationServices;
using ProjCars.Core.Dto;

namespace ProjCars.Tests
{
	public class CarsCRUDTests
	{
		[Fact]
		public async Task Create_saves_brand_model_year_engine_price()
		{
			using var ctx = DbFactory.Create();
			var service = new CarsServices(ctx);

			var dto = new CarDto
			{
				Id = 101,
				Brand = "Toyota",
				Model = "Supra",
				Year = 1998,
				Engine = "2JZ-GTE",
				Price = 45000m
			};

			var created = await service.Create(dto);

			Assert.NotNull(created);
			Assert.Equal(101, created.Id);
			Assert.Equal("Toyota", created.Brand);
			Assert.Equal("Supra", created.Model);
			Assert.Equal(1998, created.Year);
			Assert.Equal("2JZ-GTE", created.Engine);
			Assert.Equal(45000m, created.Price);
		}

		[Fact]
		public async Task Create_sets_created_and_modified_to_utc_nowish()
		{
			using var ctx = DbFactory.Create();
			var service = new CarsServices(ctx);

			var before = DateTime.UtcNow;

			var created = await service.Create(new CarDto
			{
				Id = 102,
				Brand = "Honda",
				Model = "S2000",
				Year = 2004,
				Engine = "F20C",
				Price = 28000m
			});

			var after = DateTime.UtcNow;

			Assert.True(created.CreatedAt >= before && created.CreatedAt <= after);
			Assert.True(created.ModifiedAt >= before && created.ModifiedAt <= after);
		}

		[Fact]
		public async Task Create_overwrites_dto_timestamps()
		{
			using var ctx = DbFactory.Create();
			var service = new CarsServices(ctx);

			var dto = new CarDto
			{
				Id = 103,
				Brand = "Mazda",
				Model = "MX-5",
				Year = 2016,
				Engine = "2.0 SkyActiv",
				Price = 17000m,
				CreatedAt = new DateTime(2000, 1, 1),
				ModifiedAt = new DateTime(2000, 1, 2)
			};

			var created = await service.Create(dto);

			Assert.NotEqual(dto.CreatedAt, created.CreatedAt);
			Assert.NotEqual(dto.ModifiedAt, created.ModifiedAt);
		}

		[Fact]
		public async Task GetAsync_returns_created_car()
		{
			using var ctx = DbFactory.Create();
			var service = new CarsServices(ctx);

			var created = await service.Create(new CarDto
			{
				Id = 104,
				Brand = "BMW",
				Model = "M3",
				Year = 2011,
				Engine = "S65 V8",
				Price = 39000m
			});

			var fetched = await service.GetAsync(104);

			Assert.NotNull(fetched);
			Assert.Equal(created.Id, fetched!.Id);
			Assert.Equal("BMW", fetched.Brand);
			Assert.Equal("M3", fetched.Model);
		}

		[Fact]
		public async Task GetAsync_unknown_id_returns_null()
		{
			using var ctx = DbFactory.Create();
			var service = new CarsServices(ctx);

			var fetched = await service.GetAsync(999999);

			Assert.Null(fetched);
		}

		[Fact]
		public async Task UpdateAsync_updates_fields()
		{
			using var ctx = DbFactory.Create();
			var service = new CarsServices(ctx);

			await service.Create(new CarDto
			{
				Id = 105,
				Brand = "Audi",
				Model = "A4",
				Year = 2012,
				Engine = "2.0 TFSI",
				Price = 12000m
			});

			var updated = await service.UpdateAsync(105, new CarDto
			{
				Brand = "Audi",
				Model = "S4",
				Year = 2012,
				Engine = "3.0 V6 Supercharged",
				Price = 18500m
			});

			Assert.NotNull(updated);
			Assert.Equal(105, updated!.Id);
			Assert.Equal("S4", updated.Model);
			Assert.Equal("3.0 V6 Supercharged", updated.Engine);
			Assert.Equal(18500m, updated.Price);
		}

		[Fact]
		public async Task UpdateAsync_keeps_createdat_but_changes_modifiedat()
		{
			using var ctx = DbFactory.Create();
			var service = new CarsServices(ctx);

			var created = await service.Create(new CarDto
			{
				Id = 106,
				Brand = "Volvo",
				Model = "V70",
				Year = 2008,
				Engine = "2.4D",
				Price = 6500m
			});

			var oldCreatedAt = created.CreatedAt;
			var oldModifiedAt = created.ModifiedAt;

			await Task.Delay(5);

			var updated = await service.UpdateAsync(106, new CarDto
			{
				Brand = "Volvo",
				Model = "V70 R-Design",
				Year = 2008,
				Engine = "2.4D",
				Price = 7200m
			});

			Assert.NotNull(updated);
			Assert.Equal(oldCreatedAt, updated!.CreatedAt);
			Assert.True(updated.ModifiedAt > oldModifiedAt);
		}

		[Fact]
		public async Task UpdateAsync_unknown_id_returns_null()
		{
			using var ctx = DbFactory.Create();
			var service = new CarsServices(ctx);

			var updated = await service.UpdateAsync(404, new CarDto
			{
				Brand = "Ford",
				Model = "Focus",
				Year = 2010,
				Engine = "1.6",
				Price = 3000m
			});

			Assert.Null(updated);
		}

		[Fact]
		public async Task DeleteAsync_existing_returns_true()
		{
			using var ctx = DbFactory.Create();
			var service = new CarsServices(ctx);

			await service.Create(new CarDto
			{
				Id = 107,
				Brand = "Mercedes",
				Model = "C220",
				Year = 2015,
				Engine = "2.1 Diesel",
				Price = 15000m
			});

			var ok = await service.DeleteAsync(107);

			Assert.True(ok);
		}

		[Fact]
		public async Task DeleteAsync_removes_car_from_getasync()
		{
			using var ctx = DbFactory.Create();
			var service = new CarsServices(ctx);

			await service.Create(new CarDto
			{
				Id = 108,
				Brand = "Skoda",
				Model = "Octavia",
				Year = 2019,
				Engine = "1.5 TSI",
				Price = 13500m
			});

			await service.DeleteAsync(108);

			var fetched = await service.GetAsync(108);
			Assert.Null(fetched);
		}
	}
}

using ECommerceAPI.Application.Repositories;
using ECommerceAPI.Application.ViewModels.Products;
using ECommerceAPI.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ECommerceAPI.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		readonly private IProductWriteRepository productWriteRepository;
		readonly private IProductReadRepository productReadRepository;

		public ProductsController(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository)
		{
			this.productWriteRepository = productWriteRepository;
			this.productReadRepository = productReadRepository;
		}

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			return Ok(productReadRepository.GetAll(false));
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> Get(string id)
		{
			return Ok(await productReadRepository.GetByIdAsync(id, false));
		}

		[HttpPost]
		public async Task<IActionResult> Post(ProductCreateVM model)
		{
			if(ModelState.IsValid)
			{

			}
			await productWriteRepository.AddAsync(new()
			{
				Name = model.Name,
				Price = model.Price,
				Stock = model.Stock }
			);
			await productWriteRepository.SaveAsync();
			return Ok((int)HttpStatusCode.Created);
		}

		[HttpPut]
		public async Task<IActionResult> Put(ProductUpdateVM model)
		{
			Product product = await productReadRepository.GetByIdAsync(model.Id);
			product.Name = model.Name;
			product.Price = model.Price;
			product.Stock = model.Stock;
			await productWriteRepository.SaveAsync();
			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(string id)
		{
			await productWriteRepository.RemoveAsync(id);
			await productWriteRepository.SaveAsync();
			return Ok();
		}
	}
}

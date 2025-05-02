using ECommerceAPI.Application.Abstractions.Storage;
using ECommerceAPI.Application.Repositories;
using ECommerceAPI.Application.RequestParameters;
using ECommerceAPI.Application.ViewModels.Products;
using ECommerceAPI.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ECommerceAPI.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		readonly private IProductWriteRepository productWriteRepository;
		readonly private IProductReadRepository productReadRepository;
		readonly private IWebHostEnvironment webHostEnvironment;
		readonly IFileReadRepository fileReadRepository;
		readonly IFileWriteRepository fileWriteRepository;
		readonly IProductImageFileReadRepository productImageFileReadRepository;
		readonly IProductImageFileWriteRepository productImageFileWriteRepository;
		readonly IInvoiceFileReadRepository invoiceFileReadRepository;
		readonly IInvoiceFileWriteRepository invoiceFileWriteRepository;
		readonly IStorageService storageService;
		readonly IConfiguration configuration;

		public ProductsController(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository, IWebHostEnvironment webHostEnvironment, IFileReadRepository fileReadRepository, IFileWriteRepository fileWriteRepository, IProductImageFileReadRepository productImageFileReadRepository, IProductImageFileWriteRepository productImageFileWriteRepository, IInvoiceFileReadRepository invoiceFileReadRepository, IInvoiceFileWriteRepository invoiceFileWriteRepository, IStorageService storageService, IConfiguration configuration)
		{
			this.productWriteRepository = productWriteRepository;
			this.productReadRepository = productReadRepository;
			this.webHostEnvironment = webHostEnvironment;
			this.fileReadRepository = fileReadRepository;
			this.fileWriteRepository = fileWriteRepository;
			this.productImageFileReadRepository = productImageFileReadRepository;
			this.productImageFileWriteRepository = productImageFileWriteRepository;
			this.invoiceFileReadRepository = invoiceFileReadRepository;
			this.invoiceFileWriteRepository = invoiceFileWriteRepository;
			this.storageService = storageService;
			this.configuration = configuration;
		}

		[HttpGet]
		public async Task<IActionResult> Get([FromQuery] Pagination pagination)
		{
			var totalCount = productReadRepository.GetAll(false).Count();
			var products = productReadRepository.GetAll(false).Select(p => new
			{
				p.Id,
				p.Name,
				p.Stock,
				p.Price,
				p.CreatedDate,
				p.UpdatedDate
			}).Skip(pagination.Page * pagination.Size).Take(pagination.Size);

			return Ok(new
			{
				totalCount,
				products
			});
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> Get(string id)
		{
			return Ok(await productReadRepository.GetByIdAsync(id, false));
		}

		[HttpPost]
		public async Task<IActionResult> Post(ProductCreateVM model)
		{
			if (ModelState.IsValid)
			{

			}
			await productWriteRepository.AddAsync(new()
			{
				Name = model.Name,
				Price = model.Price,
				Stock = model.Stock
			}
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

		[HttpPost("[action]")]
		public async Task<IActionResult> Upload(string id)
		{
			List<(string fileName, string pathOrContainerName)> result = await storageService.UploadAsync("photo-images", Request.Form.Files);

			Product product = await productReadRepository.GetByIdAsync(id);

			await productImageFileWriteRepository.AddRangeAsync(result.Select(r => new ProductImageFile
			{
				FileName = r.fileName,
				Path = r.pathOrContainerName,
				Storage = storageService.StorageName,
				Products = new List<Product>() { product }
			}).ToList());

			await productImageFileWriteRepository.SaveAsync();

			return Ok();
		}

		[HttpGet("[action]/{id}")]
		public async Task<IActionResult> GetProductImages(string id)
		{
			Product? product = await productReadRepository.Table.Include(p=>p.ProductImageFiles).FirstOrDefaultAsync(p => p.Id == Guid.Parse(id));
			
			return Ok(product.ProductImageFiles.Select(p => new
			{
				Path = $"{configuration["BaseStorageUrl"]}/{p.Path}",
				p.FileName,
				p.Id
			}));
		}

		[HttpDelete("[action]/{id}")]
		public async Task<IActionResult> DeleteProductImage(string id, string imageId)
		{
			Product? product = await productReadRepository.Table.Include(p => p.ProductImageFiles).FirstOrDefaultAsync(p => p.Id == Guid.Parse(id));

			ProductImageFile productImageFile = product.ProductImageFiles.FirstOrDefault(p => p.Id == Guid.Parse(imageId));
			product.ProductImageFiles.Remove(productImageFile);
			await productWriteRepository.SaveAsync();
			return Ok();

		}
	}
}

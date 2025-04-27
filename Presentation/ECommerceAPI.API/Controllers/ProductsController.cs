using ECommerceAPI.Application.Abstractions.Storage;
using ECommerceAPI.Application.Repositories;
using ECommerceAPI.Application.RequestParameters;
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
		readonly private IWebHostEnvironment webHostEnvironment;
		readonly IFileReadRepository fileReadRepository;
		readonly IFileWriteRepository fileWriteRepository;
		readonly IProductImageFileReadRepository productImageFileReadRepository;
		readonly IProductImageFileWriteRepository productImageFileWriteRepository;
		readonly IInvoiceFileReadRepository invoiceFileReadRepository;
		readonly IInvoiceFileWriteRepository invoiceFileWriteRepository;
		readonly IStorageService storageService;

		public ProductsController(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository, IWebHostEnvironment webHostEnvironment, IFileReadRepository fileReadRepository, IFileWriteRepository fileWriteRepository, IProductImageFileReadRepository productImageFileReadRepository, IProductImageFileWriteRepository productImageFileWriteRepository, IInvoiceFileReadRepository invoiceFileReadRepository, IInvoiceFileWriteRepository invoiceFileWriteRepository, IStorageService storageService)
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
		public async Task<IActionResult> Upload()
		{
			var datas = await storageService.UploadAsync("resource\\files", Request.Form.Files);
			//var datas = await fileService.UploadAsync("resource/files", Request.Form.Files);
			await productImageFileWriteRepository.AddRangeAsync(datas.Select(d => new ProductImageFile()
			{
				FileName = d.fileName,
				Path = d.pathOrContainerName,
				Storage = storageService.StorageName
			}).ToList());
			//await productImageFileWriteRepository.SaveAsync();

			//await invoiceFileWriteRepository.AddRangeAsync(datas.Select(d => new InvoiceFile()
			//{
			//	FileName = d.fileName,
			//	Path = d.path,
			//	Price = new Random().Next()
			//}).ToList());
			//await invoiceFileWriteRepository.SaveAsync();

			//await fileWriteRepository.AddRangeAsync(datas.Select(d => new Domain.Entities.File()
			//{
			//	FileName = d.fileName,
			//	Path = d.path,
			//}).ToList());
			//await fileWriteRepository.SaveAsync();

			//var d1 = fileReadRepository.GetAll();
			//var d2 = invoiceFileReadRepository.GetAll();
			//var d3 = productImageFileReadRepository.GetAll();

			return Ok();
		}
	}
}

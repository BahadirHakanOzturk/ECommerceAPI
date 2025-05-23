﻿using ECommerceAPI.Application.Features.Commands.Product.CreateProduct;
using ECommerceAPI.Application.Features.Commands.Product.RemoveProduct;
using ECommerceAPI.Application.Features.Commands.Product.UpdateProduct;
using ECommerceAPI.Application.Features.Commands.ProductImageFile.RemoveProductImage;
using ECommerceAPI.Application.Features.Commands.ProductImageFile.UploadProductImage;
using ECommerceAPI.Application.Features.Queries.Product.GetAllProducts;
using ECommerceAPI.Application.Features.Queries.Product.GetByIdProduct;
using ECommerceAPI.Application.Features.Queries.ProductImageFile.GetProductImages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ECommerceAPI.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(AuthenticationSchemes = "Admin")]
	public class ProductsController : ControllerBase
	{
		readonly IMediator mediator;

		public ProductsController(IMediator mediator)
		{
			this.mediator = mediator;
		}

		[HttpGet]
		public async Task<IActionResult> Get([FromQuery] GetAllProductsQueryRequest getAllProductsQueryRequest)
		{
			GetAllProductsQueryResponse response = await mediator.Send(getAllProductsQueryRequest);
			return Ok(response);
		}

		[HttpGet("{Id}")]
		public async Task<IActionResult> Get([FromRoute] GetByIdProductQueryRequest getByIdProductQueryRequest)
		{
			GetByIdProductQueryResponse response = await mediator.Send(getByIdProductQueryRequest);
			return Ok(response);
		}

		[HttpPost]
		public async Task<IActionResult> Post(CreateProductCommandRequest createProductCommandRequest)
		{
			CreateProductCommandResponse response = await mediator.Send(createProductCommandRequest);
			return Ok((int)HttpStatusCode.Created);
		}

		[HttpPut]
		public async Task<IActionResult> Put([FromBody] UpdateProductCommandRequest updateProductCommandRequest)
		{
			UpdateProductCommandResponse response = await mediator.Send(updateProductCommandRequest);
			return Ok();
		}

		[HttpDelete("{Id}")]
		public async Task<IActionResult> Delete([FromRoute] RemoveProductCommandRequest removeProductCommandRequest)
		{
			RemoveProductCommandResponse response = await mediator.Send(removeProductCommandRequest);
			return Ok();
		}

		[HttpPost("[action]")]
		public async Task<IActionResult> Upload([FromQuery] UploadProductImageCommandRequest uploadProductImageCommandRequest)
		{
			uploadProductImageCommandRequest.Files = Request.Form.Files;
			UploadProductImageCommandResponse response = await mediator.Send(uploadProductImageCommandRequest);
			return Ok();
		}

		[HttpGet("[action]/{id}")]
		public async Task<IActionResult> GetProductImages([FromRoute] GetProductImagesQueryRequest getProductImagesQueryRequest)
		{
			List<GetProductImagesQueryResponse> response = await mediator.Send(getProductImagesQueryRequest);
			return Ok(response);
		}

		[HttpDelete("[action]/{Id}")]
		public async Task<IActionResult> DeleteProductImage([FromRoute] RemoveProductImageCommandRequest removeProductImageCommandRequest, [FromQuery] string imageId)
		{
			removeProductImageCommandRequest.ImageId = imageId;
			RemoveProductImageCommandResponse response = await mediator.Send(removeProductImageCommandRequest);
			return Ok();
		}
	}
}

using ECommerceAPI.Application.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace ECommerceAPI.Application.Features.Commands.ProductImageFile.RemoveProductImage;

public class RemoveProductImageCommandHandler : IRequestHandler<RemoveProductImageCommandRequest, RemoveProductImageCommandResponse>
{
	readonly IProductReadRepository productReadRepository;
	readonly IProductWriteRepository productWriteRepository;

	public RemoveProductImageCommandHandler(IProductReadRepository productReadRepository, IProductWriteRepository productWriteRepository)
	{
		this.productReadRepository = productReadRepository;
		this.productWriteRepository = productWriteRepository;
	}

	public async Task<RemoveProductImageCommandResponse> Handle(RemoveProductImageCommandRequest request, CancellationToken cancellationToken)
	{
		Domain.Entities.Product? product = await productReadRepository.Table.Include(p => p.ProductImageFiles).FirstOrDefaultAsync(p => p.Id == Guid.Parse(request.Id));

		Domain.Entities.ProductImageFile? productImageFile = product?.ProductImageFiles.FirstOrDefault(p => p.Id == Guid.Parse(request.ImageId));

		if (productImageFile != null) 
			product?.ProductImageFiles.Remove(productImageFile);

		await productWriteRepository.SaveAsync();

		return new();
	}
}

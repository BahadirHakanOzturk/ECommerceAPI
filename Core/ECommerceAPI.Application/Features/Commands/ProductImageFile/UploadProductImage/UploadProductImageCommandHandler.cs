using ECommerceAPI.Application.Abstractions.Storage;
using ECommerceAPI.Application.Repositories;
using MediatR;

namespace ECommerceAPI.Application.Features.Commands.ProductImageFile.UploadProductImage;

public class UploadProductImageCommandHandler : IRequestHandler<UploadProductImageCommandRequest, UploadProductImageCommandResponse>
{
	readonly IStorageService storageService;
	readonly IProductReadRepository productReadRepository;
	readonly IProductImageFileWriteRepository productImageFileWriteRepository;

	public UploadProductImageCommandHandler(IStorageService storageService, IProductReadRepository productReadRepository, IProductImageFileWriteRepository productImageFileWriteRepository)
	{
		this.storageService = storageService;
		this.productReadRepository = productReadRepository;
		this.productImageFileWriteRepository = productImageFileWriteRepository;
	}

	public async Task<UploadProductImageCommandResponse> Handle(UploadProductImageCommandRequest request, CancellationToken cancellationToken)
	{
		List<(string fileName, string pathOrContainerName)> result = await storageService.UploadAsync("photo-images", request.Files);

		Domain.Entities.Product product = await productReadRepository.GetByIdAsync(request.Id);

		await productImageFileWriteRepository.AddRangeAsync(result.Select(r => new Domain.Entities.ProductImageFile
		{
			FileName = r.fileName,
			Path = r.pathOrContainerName,
			Storage = storageService.StorageName,
			Products = new List<Domain.Entities.Product>() { product }
		}).ToList());

		await productImageFileWriteRepository.SaveAsync();
		return new();
	}
}

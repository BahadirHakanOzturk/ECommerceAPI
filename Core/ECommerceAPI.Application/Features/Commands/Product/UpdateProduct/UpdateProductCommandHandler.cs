using ECommerceAPI.Application.Repositories;
using MediatR;

namespace ECommerceAPI.Application.Features.Commands.Product.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommandRequest, UpdateProductCommandResponse>
{
	readonly IProductReadRepository productReadRepository;
	readonly IProductWriteRepository productWriteRepository;

	public UpdateProductCommandHandler(IProductReadRepository productReadRepository, IProductWriteRepository productWriteRepository)
	{
		this.productReadRepository = productReadRepository;
		this.productWriteRepository = productWriteRepository;
	}

	public async Task<UpdateProductCommandResponse> Handle(UpdateProductCommandRequest request, CancellationToken cancellationToken)
	{
		Domain.Entities.Product product = await productReadRepository.GetByIdAsync(request.Id);
		product.Name = request.Name;
		product.Price = request.Price;
		product.Stock = request.Stock;
		await productWriteRepository.SaveAsync();
		return new();
	}
}

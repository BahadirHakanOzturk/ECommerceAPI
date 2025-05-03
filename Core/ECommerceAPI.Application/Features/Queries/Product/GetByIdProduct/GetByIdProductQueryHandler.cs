using ECommerceAPI.Application.Repositories;
using MediatR;

namespace ECommerceAPI.Application.Features.Queries.Product.GetByIdProduct;

public class GetByIdProductQueryHandler : IRequestHandler<GetByIdProductQueryRequest, GetByIdProductQueryResponse>
{
	readonly IProductReadRepository productReadRepository;

	public GetByIdProductQueryHandler(IProductReadRepository productReadRepository)
	{
		this.productReadRepository = productReadRepository;

	}

	public async Task<GetByIdProductQueryResponse> Handle(GetByIdProductQueryRequest request, CancellationToken cancellationToken)
	{
		Domain.Entities.Product product = await productReadRepository.GetByIdAsync(request.Id, false);
		return new()
		{
			Name = product.Name,
			Price = product.Price,
			Stock = product.Stock
		};
	}
}

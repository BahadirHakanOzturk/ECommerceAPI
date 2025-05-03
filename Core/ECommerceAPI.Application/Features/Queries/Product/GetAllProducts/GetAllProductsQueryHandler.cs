using ECommerceAPI.Application.Repositories;
using MediatR;

namespace ECommerceAPI.Application.Features.Queries.Product.GetAllProducts;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQueryRequest, GetAllProductsQueryResponse>
{
    readonly IProductReadRepository productReadRepository;

    public GetAllProductsQueryHandler(IProductReadRepository productReadRepository)
    {
        this.productReadRepository = productReadRepository;
    }

    public async Task<GetAllProductsQueryResponse> Handle(GetAllProductsQueryRequest request, CancellationToken cancellationToken)
    {
        var totalCount = productReadRepository.GetAll(false).Count();
        var products = productReadRepository.GetAll(false).Skip(request.Page * request.Size).Take(request.Size).Select(p => new
        {
            p.Id,
            p.Name,
            p.Stock,
            p.Price,
            p.CreatedDate,
            p.UpdatedDate
        }).ToList();

        return new()
        {
            Products = products,
            TotalCount = totalCount
        };
    }
}

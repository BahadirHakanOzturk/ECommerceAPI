﻿using ECommerceAPI.Application.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.Features.Commands.Product.RemoveProduct;

public class RemoveProductCommandHandler : IRequestHandler<RemoveProductCommandRequest, RemoveProductCommandResponse>
{
	readonly IProductWriteRepository productWriteRepository;

	public RemoveProductCommandHandler(IProductWriteRepository productWriteRepository)
	{
		this.productWriteRepository = productWriteRepository;
	}

	public async Task<RemoveProductCommandResponse> Handle(RemoveProductCommandRequest request, CancellationToken cancellationToken)
	{
		await productWriteRepository.RemoveAsync(request.Id);
		await productWriteRepository.SaveAsync();
		return new();
	}
}

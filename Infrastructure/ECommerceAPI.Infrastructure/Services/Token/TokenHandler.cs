﻿using ECommerceAPI.Application.Abstractions.Token;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace ECommerceAPI.Infrastructure.Services.Token;

public class TokenHandler : ITokenHandler
{
	readonly IConfiguration configuration;

	public TokenHandler(IConfiguration configuration)
	{
		this.configuration = configuration;
	}

	public Application.DTOs.Token CreateAccessToken(int second)
	{
		Application.DTOs.Token token = new();

		SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(configuration["Token:SecurityKey"]));

		SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

		token.Expiration = DateTime.UtcNow.AddSeconds(second);

		JwtSecurityToken securityToken = new(
			audience: configuration["Token:Audience"],
			issuer: configuration["Token:Issuer"],
			expires: token.Expiration,
			notBefore: DateTime.UtcNow,
			signingCredentials: signingCredentials
			);

		JwtSecurityTokenHandler tokenHandler = new();
		token.AccessToken = tokenHandler.WriteToken(securityToken);

		token.RefreshToken = CreateRefreshToken();

		return token;
	}

	public string CreateRefreshToken()
	{
		byte[] number = new byte[32];
		using RandomNumberGenerator random = RandomNumberGenerator.Create();
		random.GetBytes(number);
		return Convert.ToBase64String(number);
	}
}

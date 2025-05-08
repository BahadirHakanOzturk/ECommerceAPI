using ECommerceAPI.Application.Validators.Products;
using ECommerceAPI.Persistence;
using ECommerceAPI.Infrastructure;
using FluentValidation.AspNetCore;
using ECommerceAPI.Infrastructure.Services.Storage.Local;
using ECommerceAPI.Infrastructure.Services.Storage.Azure;
using ECommerceAPI.Application;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddPersistenceServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();

builder.Services.AddStorage<AzureStorage>();

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
	policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
	//policy.WithOrigins("http://localhost:4200/", "https://localhost:4200/").AllowAnyHeader().AllowAnyMethod()
));

builder.Services.AddControllers()
	.AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<ProductCreateValidator>());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer("Admin", options =>
	{
		options.TokenValidationParameters = new()
		{
			ValidateAudience = true,
			ValidateIssuer = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,

			ValidAudience = builder.Configuration["Token:Audience"],
			ValidIssuer = builder.Configuration["Token:Issuer"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"]))
		};
	});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

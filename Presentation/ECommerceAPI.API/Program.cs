using ECommerceAPI.Application.Validators.Products;
using ECommerceAPI.Persistence;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddPersistenceServices();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
	policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
	//policy.WithOrigins("http://localhost:4200/", "https://localhost:4200/").AllowAnyHeader().AllowAnyMethod()
));

builder.Services.AddControllers()
	.AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<ProductCreateValidator>());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

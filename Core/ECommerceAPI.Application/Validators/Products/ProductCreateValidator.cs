using ECommerceAPI.Application.ViewModels.Products;
using FluentValidation;

namespace ECommerceAPI.Application.Validators.Products;

public class ProductCreateValidator : AbstractValidator<ProductCreateVM>
{
    public ProductCreateValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .NotNull()
                .WithMessage("Lütfen ürün adını boş geçmeyiniz.")
            .MaximumLength(100)
            .MinimumLength(2)
                .WithMessage("Lütfen ürün adını 2 ile 100 karakter arasında giriniz.");

        RuleFor(p => p.Stock)
            .NotEmpty()
            .NotNull()
                .WithMessage("Lütfen stok bilgisini boş geçmeyiniz.")
            .Must(s => s >= 0)
                .WithMessage("Stok bilgisi negatif olamaz!");

		RuleFor(p => p.Price)
			.NotEmpty()
			.NotNull()
				.WithMessage("Lütfen fiyat bilgisini boş geçmeyiniz.")
			.Must(p => p >= 0)
				.WithMessage("Fiyat bilgisi negatif olamaz!");
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using TheArtMarketplacePlatform.Core.DTOs;

namespace TheArtMarketplacePlatform.BusinessLayer.Validators
{
    public class ArtisanInsertProductValidator : AbstractValidator<ArtisanInsertProductRequest>
    {
        public ArtisanInsertProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Product name is required.");

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage("Price must be greater than zero.");

            RuleFor(x => x.QuantityLeft)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Quantity left must be zero or greater.");

            RuleFor(x => x.Category)
                .NotEmpty()
                .WithMessage("Category is required.");

            RuleFor(x => x.Availability)
                .NotNull()
                .WithMessage("Availability status is required.");
        }
    }

    public class ArtisanUpdateProductValidator : AbstractValidator<ArtisanUpdateProductRequest>
    {
        public ArtisanUpdateProductValidator()
        {

            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Product ID is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Product name is required.");

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage("Price must be greater than zero.");

            RuleFor(x => x.QuantityLeft)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Quantity left must be zero or greater.");

            RuleFor(x => x.Category)
                .NotEmpty()
                .WithMessage("Category is required.");

            RuleFor(x => x.Availability)
                .NotNull()
                .WithMessage("Availability status is required.");
        }
    }
}
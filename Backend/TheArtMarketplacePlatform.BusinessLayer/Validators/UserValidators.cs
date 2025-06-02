using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using TheArtMarketplacePlatform.Core.DTOs;

namespace TheArtMarketplacePlatform.BusinessLayer.Validators
{

    public class CustomerUpdateProfileRequestValidator : AbstractValidator<CustomerUpdateProfileRequest>
    {
        public CustomerUpdateProfileRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username is required.");

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("A valid email address is required.");

            RuleFor(x => x.ShippingAddress)
                .MaximumLength(200)
                .WithMessage("Shipping address cannot exceed 200 characters.");
        }
    }

    public class ArtisanUpdateProfileRequestValidator : AbstractValidator<ArtisanUpdateProfileRequest>
    {
        public ArtisanUpdateProfileRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username is required.");

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("A valid email address is required.");

            RuleFor(x => x.Bio)
                .MaximumLength(500)
                .WithMessage("Bio cannot exceed 500 characters.");

            RuleFor(x => x.City)
                .MaximumLength(100)
                .WithMessage("City cannot exceed 100 characters.");
        }
    }
}
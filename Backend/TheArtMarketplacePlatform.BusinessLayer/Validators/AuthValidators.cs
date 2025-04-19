using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using TheArtMarketplacePlatform.Core.DTOs;

namespace TheArtMarketplacePlatform.BusinessLayer.Validators
{
    public class RegisterArtisanValidator : AbstractValidator<RegisterArtisanRequest>
    {
        public RegisterArtisanValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username is required.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("A valid email is required.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$")
                .WithMessage("Password must be at least 6 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password)
                .WithMessage("Passwords do not match.");

            RuleFor(x => x.Bio)
                .NotEmpty()
                .WithMessage("Bio is required.");

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("City is required.");
        }
    }

    public class RegisterCustomerValidator : AbstractValidator<RegisterCustomerRequest>
    {
        public RegisterCustomerValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username is required.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
                .EmailAddress()
                .WithMessage("A valid email is required.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$")
                .WithMessage("Password must be at least 6 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password)
                .WithMessage("Passwords do not match.");

            RuleFor(x => x.ShippingAddress)
                .NotEmpty()
                .WithMessage("Shipping address is required.");
        }
    }
    public class RegisterDeliveryPartnerValidator : AbstractValidator<RegisterDeliveryPartnerRequest>
    {
        public RegisterDeliveryPartnerValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username is required.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("A valid email is required.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$")
                .WithMessage("Password must be at least 6 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password)
                .WithMessage("Passwords do not match.");
        }
    }
}
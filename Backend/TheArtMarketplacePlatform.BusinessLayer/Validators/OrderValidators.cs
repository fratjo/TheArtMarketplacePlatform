using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using TheArtMarketplacePlatform.Core.DTOs;
using TheArtMarketplacePlatform.Core.Entities;

namespace TheArtMarketplacePlatform.BusinessLayer.Validators
{
    public class CustomerInsertOrderValidators : AbstractValidator<CustomerInsertOrderRequest>
    {
        public CustomerInsertOrderValidators()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .WithMessage("Customer ID is required.");

            RuleFor(x => x.OrderProducts)
                .NotEmpty()
                .WithMessage("Order products are required.")
                .Must(x => x.Count > 0)
                .WithMessage("At least one order product is required.");

            RuleForEach(x => x.OrderProducts)
                .ChildRules(orderProduct =>
                {
                    orderProduct.RuleFor(x => x.ProductId)
                        .NotEmpty()
                        .WithMessage("Product ID is required.");

                    orderProduct.RuleFor(x => x.Quantity)
                        .GreaterThan(0)
                        .WithMessage("Quantity must be greater than zero.");
                });
        }
    }

    public class ArtisanUpdateOrderStatusValidators : AbstractValidator<ArtisanUpdateOrderStatusRequest>
    {
        public ArtisanUpdateOrderStatusValidators()
        {
            RuleFor(x => x.Status)
                .NotEmpty()
                .WithMessage("Status is required.")
                .Must(status => Enum.TryParse(typeof(OrderStatus), status, true, out _))
                .WithMessage("Invalid order status.");
        }
    }
}
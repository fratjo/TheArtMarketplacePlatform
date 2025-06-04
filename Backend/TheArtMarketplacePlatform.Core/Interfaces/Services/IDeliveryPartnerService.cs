using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheArtMarketplacePlatform.Core.DTOs;
using TheArtMarketplacePlatform.Core.Entities;

namespace TheArtMarketplacePlatform.Core.Interfaces.Services
{
    public interface IDeliveryPartnerService
    {
        Task<ICollection<DeliveryPartnerDeliveryOrderResponse>> GetDeliveryOrdersByDeliveryPartnerAsync(Guid deliveryPartnerId, string? status = null, string? sortBy = null, string? sortOrder = null);
        Task<bool> SetDeliveryAsDeliveredAsync(Guid deliveryPartnerId, Guid deliveryId);
        Task<DeliveryPartnerProfileResponse?> GetDeliveryPartnerAsync(Guid deliveryPartnerId);
        Task<bool> UpdateDeliveryPartnerAsync(Guid deliveryPartnerId, DeliveryPartnerUpdateProfileRequest request);
    }
}
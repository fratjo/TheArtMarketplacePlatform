import { DeliveryPartnerProfile } from './order.interface';

export interface DeliveryPartner {
  id: string;
  username: string;
  deliveryPartnerProfile: DeliveryPartnerProfile;
}

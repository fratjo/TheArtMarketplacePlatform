export interface CreateOrder {
  customerId: string;
  deliveryPartnerId: string;
  orderProducts: CreateOrderProduct[];
}

export interface CreateOrderProduct {
  productId: string;
  quantity: number;
}

export interface Order {
  id: string;
  deliveryPartnerId?: string | null;
  customerId?: string | null;
  deliveryPartnerName: string;
  shippingAddress: string;
  status: number; // ou OrderStatus si tu as un enum côté TS
  createdAt: Date;
  updatedAt: Date;
  deliveryPartner?: DeliveryPartnerProfile | null;
  customer?: CustomerProfile | null;
  orderProducts: OrderProduct[];
  deliveryStatusUpdates?: DeliveryStatusUpdate[];
}

export type Orders = Order[];

export interface OrderProduct {
  id: string;
  orderId?: string; // optionnel si présent dans la réponse
  productId: string;
  productName: string;
  productDescription?: string;
  productPrice: number;
  quantity: number;
  artisanName?: string;
  imageUrl?: string;
}

export interface DeliveryPartnerProfile {}

export interface CustomerProfile {}

export interface DeliveryStatusUpdate {}

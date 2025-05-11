export interface CreateOrder {
  customerId: string;
  orderProducts: CreateOrderProduct[];
}

export interface CreateOrderProduct {
  productId: string;
  quantity: number;
}

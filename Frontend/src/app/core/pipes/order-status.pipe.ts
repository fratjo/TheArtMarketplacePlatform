import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'orderStatus',
  standalone: true,
})
export class OrderStatusPipe implements PipeTransform {
  private statusMap: Record<number, string> = {
    0: 'Pending',
    1: 'Processing',
    2: 'Shipped',
    3: 'Delivered',
    4: 'Cancelled',
  };

  transform(value: unknown): string {
    if (typeof value === 'number' && this.statusMap[value]) {
      return this.statusMap[value];
    }

    return value as string;
  }
}

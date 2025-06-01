import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'deliveryStatus',
})
export class DeliveryStatusPipe implements PipeTransform {
  private statusMap: Record<number, string> = {
    0: 'Pending',
    1: 'In Transit',
    2: 'Delivered',
  };

  transform(value: unknown): string {
    if (typeof value === 'number' && this.statusMap[value]) {
      return this.statusMap[value];
    }

    return value as string;
  }
}

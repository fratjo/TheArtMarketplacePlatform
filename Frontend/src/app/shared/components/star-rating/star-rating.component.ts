import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-star-rating',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="product-rating" *ngIf="rating !== undefined && rating !== null">
      <ng-container *ngFor="let star of stars; let i = index">
        <i
          class="bi"
          [ngClass]="{
            'bi-star-fill': rating >= i + 1,
            'bi-star-half': rating >= i + 0.5 && rating < i + 1,
            'bi-star': rating < i + 0.5
          }"
        ></i>
      </ng-container>
    </div>
  `,
  styleUrl: './star-rating.component.css',
})
export class StarRatingComponent {
  @Input() rating: number = 0;

  stars = [1, 2, 3, 4, 5];
}

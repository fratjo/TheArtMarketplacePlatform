import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  ViewEncapsulation,
} from '@angular/core';
import { NgxSliderModule, Options } from '@angular-slider/ngx-slider';

@Component({
  selector: 'app-single-slider',
  imports: [NgxSliderModule],
  template:
    '<ngx-slider [(value)]="value" [options]="options" (valueChange)="onValueChange()"></ngx-slider>',
  styleUrl: './single-slider.component.css',
})
export class SingleSliderComponent implements OnChanges {
  // Input for floor and ceiling values
  @Input() floor: number = 0;
  @Input() ceil: number = 5;
  @Input() step: number = 0.1;
  @Output() valueChange = new EventEmitter<number>();
  value: number = this.floor;

  ngOnChanges() {
    this.options = {
      ...this.options,
      floor: this.floor,
      ceil: this.ceil,
      step: this.step,
    };
  }

  onValueChange() {
    this.valueChange.emit(this.value);
  }

  options: Options = {
    floor: this.floor,
    ceil: this.ceil,
    step: this.step,
  };
}

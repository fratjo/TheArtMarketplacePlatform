import { Component, EventEmitter, Input, Output } from '@angular/core';
import {
  FormGroup,
  FormBuilder,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { Product } from '../../../../core/models/product.interface';

@Component({
  selector: 'app-product-form',
  imports: [ReactiveFormsModule],
  templateUrl: './product-form.component.html',
  styleUrl: './product-form.component.css',
})
export class ProductFormComponent {
  @Input() product: Product | null = null;
  @Output() save = new EventEmitter<Product>();

  productForm!: FormGroup;

  constructor(private fb: FormBuilder) {}

  ngOnInit() {
    console.log('Product:', this.product);

    this.productForm = this.fb.group({
      name: [this.product?.name || '', Validators.required],
      description: [this.product?.description || ''],
      price: [
        this.product?.price || 0,
        [Validators.required, Validators.min(0.01)],
      ],
      category: [this.product?.category || '', Validators.required],
      imageUrl: [this.product?.image || ''],
      quantityLeft: [
        this.product?.quantityLeft || 0,
        [Validators.required, Validators.min(0)],
      ],
      availability: [this.product?.availability || true, Validators.required],
    });
  }

  onSubmit() {
    if (this.productForm.valid) {
      const formValue = this.productForm.value;
      const finalProduct: Product = { ...this.product, ...formValue };
      console.log('Final Product:', finalProduct);
      this.save.emit(finalProduct);
    }
  }
}

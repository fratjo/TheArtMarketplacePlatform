import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
} from '@angular/core';
import {
  FormGroup,
  FormBuilder,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import {
  Categories,
  Product,
  ProductForm,
} from '../../../../core/models/product.interface';
import { ArtisanService } from '../../../../core/services/artisan.service';
import { BehaviorSubject, Observable } from 'rxjs';
import { ToastService } from '../../../../core/services/toast.service';
import { AsyncPipe, CommonModule } from '@angular/common';

@Component({
  selector: 'app-product-form',
  imports: [ReactiveFormsModule, AsyncPipe, CommonModule],
  templateUrl: './product-form.component.html',
  styleUrl: './product-form.component.css',
})
export class ProductFormComponent implements OnInit, OnChanges {
  @Input() product: Product | null = null;
  @Output() save = new EventEmitter<ProductForm>();

  categories$!: Observable<Categories>;

  productForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private artisanService: ArtisanService,
    private toastService: ToastService
  ) {}

  ngOnInit() {
    this.artisanService.getCategories().subscribe({
      next: (categories) => {
        this.categories$ = new BehaviorSubject<Categories>(categories);
      },
      error: (error) => {
        console.error('Error fetching categories:', error);
        this.toastService.show({
          text: `Error fetching categories: ${error.error.title}`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
    });

    this.initializeForm();
  }

  ngOnChanges() {
    if (this.product) {
      console.log('Product changed:', this.product);

      this.productForm.patchValue({
        name: this.product.name,
        description: this.product.description,
        price: this.product.price,
        category: this.product.category.name.toLowerCase(),
        imageUrl: this.product.image,
        quantityLeft: this.product.quantityLeft,
        availability: this.product.availability ? 'not available' : 'available',
      });
    }

    console.log('Product form value:', this.productForm);
  }

  initializeForm() {
    this.productForm = this.fb.group({
      name: [this.product?.name || '', Validators.required],
      description: [this.product?.description || ''],
      price: [
        this.product?.price || 0,
        [Validators.required, Validators.min(0.01)],
      ],
      category: [this.product?.category.name || '', Validators.required],
      imageUrl: [this.product?.image || ''],
      quantityLeft: [
        this.product?.quantityLeft || 0,
        [Validators.required, Validators.min(0)],
      ],
      availability: [
        this.product?.availability ? 'not available' : 'available',
        Validators.required,
      ],
    });
  }

  onSubmit() {
    if (this.productForm.valid) {
      const formValue = this.productForm.value;
      const finalProduct: ProductForm = { ...this.product, ...formValue };
      console.log('Final Product:', finalProduct);
      this.save.emit(finalProduct);
    }
  }
}

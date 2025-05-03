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
  @Output() save = new EventEmitter<FormData>();

  categories$!: Observable<Categories>;
  imagePreview: string | null = null;
  selectedFile: File | null = null;
  productForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private artisanService: ArtisanService,
    private toastService: ToastService
  ) {}

  ngOnInit() {
    this.initializeForm();

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

    this.updateImagePreview();
  }

  ngOnChanges() {
    if (this.product && this.productForm) {
      this.productForm.patchValue({
        name: this.product.name,
        description: this.product.description,
        price: this.product.price,
        category: this.product.category.name.toLowerCase(),
        imageUrl: this.product.imageUrl,
        quantityLeft: this.product.quantityLeft,
        availability: this.product.availability ? 'not available' : 'available',
      });
      this.updateImagePreview();

      this.imagePreview = this.product.imageUrl
        ? `http://localhost:5140/${this.product.imageUrl}`
        : null;
    }
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

  updateImagePreview() {
    if (this.productForm) {
      const imageUrl = this.productForm.get('imageUrl')?.value;
      this.imagePreview = imageUrl ? imageUrl : null;
    }

    if (this.product && this.product.imageUrl) {
      this.imagePreview = `http://localhost:5140/${this.product.imageUrl}`;
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length > 0) {
      const file = input.files[0];

      // Vérifier le type de fichier
      if (!file.type.startsWith('image/')) {
        alert('Please select a valid image file');
        return;
      }

      // Vérifier la taille du fichier (par exemple, 2 Mo max)
      // if (file.size > 2 * 1024 * 1024) {
      //   alert('File size must be less than 2 MB');
      //   return;
      // }

      this.selectedFile = file;

      // Lire le fichier et mettre à jour la prévisualisation
      const reader = new FileReader();
      reader.onload = () => {
        this.imagePreview = reader.result as string;
      };
      reader.readAsDataURL(this.selectedFile);
    }
  }

  async onSubmit() {
    if (this.productForm.valid) {
      const formValue = this.productForm.value;

      // Créer un objet FormData pour inclure le fichier
      const formData = new FormData();
      formData.append('name', formValue.name);
      formData.append('description', formValue.description);
      formData.append('price', formValue.price.toString());
      formData.append('category', formValue.category);
      formData.append('quantityLeft', formValue.quantityLeft.toString());
      formData.append('availability', formValue.availability);

      if (this.selectedFile) {
        // Si un fichier est sélectionné, l'ajouter au FormData

        formData.append('image', await this.fileToBase64(this.selectedFile));
        formData.append('extension', this.selectedFile.name.split('.').pop()!);
      }

      // Envoyer les données au serveur
      this.save.emit(formData);
    }
  }

  fileToBase64(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => {
        resolve((reader.result as string).split(',')[1]); // on enlève le prefix data:image/...
      };
      reader.onerror = reject;
      reader.readAsDataURL(file);
    });
  }
}

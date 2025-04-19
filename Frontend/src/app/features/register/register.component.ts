import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import {
  hasLowercase,
  hasMinimumLength,
  hasNumber,
  hasSpecialCharacter,
  hasUppercase,
} from './password.validators';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  public registerForm!: FormGroup;
  public passwordFocus: boolean = false;
  public showPassword: boolean = false;
  public showConfirmPassword: boolean = false;
  public selectedRole: string = 'none';
  public specialCharacters: string = '@$!%*?&';

  constructor(
    private authService: AuthService,
    private toastService: ToastService
  ) {
    this.registerForm = new FormGroup(
      {
        username: new FormControl('', {
          validators: [Validators.required],
          updateOn: 'blur',
        }),
        email: new FormControl('', {
          validators: [Validators.required, Validators.email],
          updateOn: 'blur',
        }),
        password: new FormControl('', {
          validators: [
            Validators.required,
            hasLowercase,
            hasUppercase,
            hasNumber,
            hasSpecialCharacter,
            hasMinimumLength,
          ],
          updateOn: 'change',
        }),
        confirmPassword: new FormControl('', {
          validators: [Validators.required],
          updateOn: 'change',
        }),
        bio: new FormControl('', {
          updateOn: 'blur',
        }),
        city: new FormControl('', {
          updateOn: 'blur',
        }),
        shippingAddress: new FormControl('', {
          updateOn: 'blur',
        }),
      },
      { validators: this.passwordsMatchValidator }
    );
  }

  updateValidators() {
    const bioControl = this.registerForm.get('bio');
    const cityControl = this.registerForm.get('city');

    if (this.selectedRole === 'artisan') {
      // Rendre le champ "bio" requis pour les artisans
      bioControl?.setValidators([Validators.required]);
      // Rendre le champ "city" requis pour les artisans
      cityControl?.setValidators([Validators.required]);
    } else if (this.selectedRole !== 'artisan') {
      // Rendre le champ "bio" optionnel pour les autres rôles
      bioControl?.clearValidators();
      // Rendre le champ "city" optionnel pour les autres rôles
      cityControl?.clearValidators();
    }

    if (this.selectedRole === 'customer') {
      // Rendre le champ "address" requis pour les partenaires de livraison
      this.registerForm
        .get('shippingaddress')
        ?.setValidators([Validators.required]);
    } else {
      // Rendre le champ "address" optionnel pour les autres rôles
      this.registerForm.get('shippingaddress')?.clearValidators();
    }

    // Revalider le champ après avoir modifié les validateurs
    bioControl?.updateValueAndValidity();
  }

  onSubmit(event: any) {
    event.preventDefault(); // Empêche le rechargement de la page
    if (this.registerForm.valid) {
      switch (this.selectedRole) {
        case 'artisan':
          this.registerArtisan();
          break;
        case 'customer':
          this.registerClient();
          break;
        case 'delivery_partner':
          this.registerDeliveryPartner();
          break;
        default:
          console.log('Please select a role');
          return;
      }
    } else {
      console.log('Form is invalid');
    }
  }

  registerArtisan() {
    const artisanData = this.registerForm.value;
    this.authService.registerArtisan(artisanData).subscribe({
      next: (response) => {
        console.log('Registration successful', response);
        // save token
        this.authService.saveToken(response.token);
      },
      error: (error) => {
        console.error('Registration failed', error);
        this.toastService.show({
          text: `Register failed:  ${error.error.title}`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
      complete: () => {
        console.log('Registration request completed');
        // redirtect to dashboard
        window.location.href = '/dashboard';
      },
    });
  }
  registerClient() {
    const clientData = this.registerForm.value;
    this.authService.registerCustomer(clientData).subscribe({
      next: (response) => {
        console.log('Registration successful', response);
        // save token
        this.authService.saveToken(response.token);
      },
      error: (error) => {
        console.error('Registration failed', error);
        // Handle registration error, e.g., show error message
        this.toastService.show({
          text: `Register failed:  ${error.error.title}`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
      complete: () => {
        console.log('Registration request completed');
        // redirtect to dashboard
        window.location.href = '/dashboard';
      },
    });
  }

  registerDeliveryPartner() {
    const deliveryPartnerData = this.registerForm.value;
    this.authService.registerDeliveryPartner(deliveryPartnerData).subscribe({
      next: (response) => {
        console.log('Registration successful', response);
        // save token
        this.authService.saveToken(response.token);
      },
      error: (error) => {
        console.error('Registration failed', error);
        // Handle registration error, e.g., show error message
        this.toastService.show({
          text: `Register failed:  ${error.error.title}`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
      complete: () => {
        console.log('Registration request completed');
        // redirtect to dashboard
        window.location.href = '/dashboard';
      },
    });
  }

  passwordsMatchValidator(
    control: AbstractControl
  ): { [key: string]: boolean } | null {
    const password = control.get('password')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;
    if (password && confirmPassword && password !== confirmPassword) {
      return { notMatch: true };
    }
    return null;
  }

  checkIfEmailExist() {
    this.authService
      .checkIfEmailExist(this.registerForm.get('email')?.value)
      .subscribe({
        next: (response) => {
          console.log('Email exists:', response);
          if (response.exists) {
            this.registerForm.get('email')?.setErrors({ emailExists: true });
          } else {
            this.registerForm.get('email')?.setErrors(null);
          }
        },
      });
  }

  async checkIfUsernameExist() {
    this.authService
      .checkIfUsernameExist(this.registerForm.get('username')?.value)
      .subscribe({
        next: (response) => {
          console.log('Username exists:', response);
          if (response.exists) {
            this.registerForm
              .get('username')
              ?.setErrors({ usernameExists: true });
          } else {
            this.registerForm.get('username')?.setErrors(null);
          }
        },
      });
  }

  onRoleChange(event: any) {
    this.selectedRole = event.target.value;
    this.updateValidators();
  }
}

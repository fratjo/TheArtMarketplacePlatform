import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import {
  AbstractControl,
  AsyncValidatorFn,
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { RouterLink } from '@angular/router';
import { catchError, debounceTime, map, of, switchMap } from 'rxjs';
import { AuthService } from '../../core/services/auth.service';
import { NgModule } from '@angular/core';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  public registerForm!: FormGroup;
  public showPassword: boolean = false;
  public showConfirmPassword: boolean = false;
  public selectedRole: string = 'none';

  constructor(private authService: AuthService) {
    this.registerForm = new FormGroup(
      {
        username: new FormControl('', {
          validators: [Validators.required],
          asyncValidators: [this.checkIfUsernameExist()],
          updateOn: 'blur',
        }),
        email: new FormControl('', {
          validators: [Validators.required, Validators.email],
          asyncValidators: [this.checkIfEmailExist()],
          updateOn: 'blur',
        }),
        password: new FormControl('', {
          validators: [
            Validators.required,
            Validators.minLength(6),
            Validators.pattern(/(?=.*[0-9])(?=.*[a-zA-Z])/),
          ],
          updateOn: 'blur',
        }),
        confirmPassword: new FormControl('', {
          validators: [Validators.required],
          updateOn: 'blur',
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

  onSubmit() {
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
      },
      complete: () => {
        console.log('Registration request completed');
        // redirect to user dashboard
        // this.router.navigate(['/user-dashboard']);
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
      },
      complete: () => {
        console.log('Registration request completed');
        // Optionally, you can reset the form or perform other actions
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
      },
      complete: () => {
        console.log('Registration request completed');
        // Optionally, you can reset the form or perform other actions
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

  checkIfEmailExist(): AsyncValidatorFn {
    return (control: AbstractControl) => {
      const email = control.value;

      if (!email) {
        return of(null); // No validation if email is empty
      }

      // Retourne un Observable après un délai pour éviter des appels excessifs
      return of(email).pipe(
        debounceTime(300), // Ajoute un délai pour limiter les appels
        switchMap((emailValue) =>
          this.authService.checkIfEmailExist(emailValue).pipe(
            map((emailExists: boolean) => {
              return emailExists ? { emailExists: true } : null;
            }),
            catchError((error) => {
              console.error("Erreur lors de la vérification de l'email", error);
              return of(null); // Retourne null en cas d'erreur
            })
          )
        )
      );
    };
  }

  checkIfUsernameExist(): AsyncValidatorFn {
    return (control: AbstractControl) => {
      const username = control.value;

      if (!username) {
        return of(null); // No validation if username is empty
      }

      // Retourne un Observable après un délai pour éviter des appels excessifs
      return of(username).pipe(
        debounceTime(300), // Ajoute un délai pour limiter les appels
        switchMap((usernameValue) =>
          this.authService.checkIfUsernameExist(usernameValue).pipe(
            map((usernameExists: boolean) => {
              return usernameExists ? { usernameExists: true } : null;
            }),
            catchError((error) => {
              console.error(
                'Erreur lors de la vérification du username',
                error
              );
              return of(null); // Retourne null en cas d'erreur
            })
          )
        )
      );
    };
  }

  onRoleChange(event: any) {
    this.selectedRole = event.target.value;
    this.updateValidators();
  }
}

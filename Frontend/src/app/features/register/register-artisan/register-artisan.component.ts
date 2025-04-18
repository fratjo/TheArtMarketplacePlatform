import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import {
  AbstractControl,
  AsyncValidatorFn,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { RouterLink } from '@angular/router';
import { catchError, debounceTime, map, of, switchMap } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register-artisan',
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  templateUrl: './register-artisan.component.html',
  styleUrl: './register-artisan.component.css',
})
export class RegisterArtisanComponent {
  public registerForm!: FormGroup;
  public showPassword: boolean = false;
  public showConfirmPassword: boolean = false;

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
        bio: new FormControl('', {
          validators: [Validators.required],
          updateOn: 'blur',
        }),
        city: new FormControl('', {
          validators: [Validators.required],
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
      },
      { validators: this.passwordsMatchValidator }
    );
  }

  onSubmit() {
    if (this.registerForm.valid) {
      const artisanData = this.registerForm.value;
      this.authService.registerArtisan(artisanData).subscribe({
        next: (response) => {
          console.log('Registration successful', response);
          // Handle successful registration, e.g., redirect to login
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
    } else {
      console.log('Form is invalid');
    }
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
}

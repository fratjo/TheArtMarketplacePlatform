import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import {
  FormGroup,
  FormControl,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  public loginForm!: FormGroup;
  public showPassword: boolean = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private toastService: ToastService
  ) {
    this.loginForm = new FormGroup({
      email: new FormControl('', {
        validators: [Validators.required, Validators.email],
        updateOn: 'change',
      }),
      password: new FormControl('', {
        validators: [Validators.required],
        updateOn: 'change',
      }),
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      const loginData = this.loginForm.value;
      this.authService.login(loginData).subscribe({
        next: (response) => {
          console.log('Login successful', response);
          // save token
          this.authService.saveToken(response.token);
        },
        error: (error) => {
          console.error('Login failed', error);
          // Handle login error, e.g., show error message
          this.toastService.show({
            text: `Login failed:  ${error.error.title}`,
            classname: 'bg-danger text-light',
            delay: 5000,
          });
        },
        complete: () => {
          console.log('Login request completed');
          // redirtect to dashboard
          window.location.href = '/dashboard';
        },
      });
    } else {
      console.log('Form is invalid');
    }
  }
}

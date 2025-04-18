import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import {
  FormGroup,
  FormControl,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  public loginForm!: FormGroup;

  constructor(private authService: AuthService) {
    this.loginForm = new FormGroup({
      email: new FormControl('', {
        validators: [Validators.required, Validators.email],
        updateOn: 'blur',
      }),
      password: new FormControl('', {
        validators: [Validators.required],
        updateOn: 'blur',
      }),
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      const loginData = this.loginForm.value;
      this.authService.login(loginData).subscribe({
        next: (response) => {
          console.log('Login successful', response);
          // Handle successful login, e.g., redirect to dashboard
        },
        error: (error) => {
          console.error('Login failed', error);
          // Handle login error, e.g., show error message
        },
        complete: () => {
          console.log('Login request completed');
          // Optionally, you can reset the form or perform other actions
        },
      });
    } else {
      console.log('Form is invalid');
    }
  }
}

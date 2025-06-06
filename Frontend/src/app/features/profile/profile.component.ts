import { Component, OnInit } from '@angular/core';
import { ProfileService } from '../../core/services/profile.service';
import { ToastService } from '../../core/services/toast.service';
import {
  AnyUserProfile,
  ArtisanProfile,
  CustomerProfile,
  DeliveryPartnerProfile,
} from '../../core/models/user.interface';
import { AuthService } from '../../core/services/auth.service';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import {
  hasLowercase,
  hasUppercase,
  hasNumber,
  hasSpecialCharacter,
  hasMinimumLength,
} from '../register/password.validators';

@Component({
  selector: 'app-profile',
  imports: [ReactiveFormsModule, FormsModule, CommonModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css',
})
export class ProfileComponent implements OnInit {
  editMode = false;
  role!: 'customer' | 'artisan' | 'deliverypartner';
  profile: AnyUserProfile | null = null;
  originalProfile = {} as AnyUserProfile;
  passwordData = {
    currentPassword: '',
    newPassword: '',
    confirmPassword: '',
  };
  profileForm!: FormGroup;
  passwordForm!: FormGroup;
  public passwordFocus: boolean = false;
  public newPasswordFocus: boolean = false;
  public confirmPasswordFocus: boolean = false;
  public showPassword: boolean = false;
  public showNewPassword: boolean = false;
  public showConfirmPassword: boolean = false;
  public specialCharacters: string = '@$!%*?&';

  constructor(
    private profileService: ProfileService,
    private toastService: ToastService,
    private authService: AuthService,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.role = this.authService.getUserRole() as
      | 'customer'
      | 'artisan'
      | 'deliverypartner';
    this.loadProfile();
  }

  loadProfile(): void {
    this.profileService.getProfile().subscribe({
      next: (profile) => {
        console.log('Profile loaded:', profile);
        // Création dynamique du formulaire selon le rôle
        if (this.role === 'customer') {
          this.profile = profile as CustomerProfile;
          this.profileForm = this.fb.group({
            username: [this.profile.username, Validators.required],
            email: [
              this.profile.email,
              [Validators.required, Validators.email],
            ],
            shippingAddress: [
              this.profile.shippingAddress,
              Validators.required,
            ],
          });
        } else if (this.role === 'artisan') {
          this.profile = profile as ArtisanProfile;
          this.profileForm = this.fb.group({
            username: [this.profile.username, Validators.required],
            email: [
              this.profile.email,
              [Validators.required, Validators.email],
            ],
            bio: [this.profile.bio],
            city: [this.profile.city],
          });
        } else if (this.role === 'deliverypartner') {
          this.profile = profile as DeliveryPartnerProfile;
          this.profileForm = this.fb.group({
            username: [this.profile.username, Validators.required],
            email: [
              this.profile.email,
              [Validators.required, Validators.email],
            ],
          });
        }
        this.originalProfile = { ...this.profile } as AnyUserProfile;
        this.passwordForm = this.fb.group(
          {
            currentPassword: ['', Validators.required],
            newPassword: [
              '',
              {
                validators: [
                  Validators.required,
                  hasLowercase,
                  hasUppercase,
                  hasNumber,
                  hasSpecialCharacter,
                  hasMinimumLength,
                ],
                updateOn: 'change',
              },
            ],
            confirmPassword: [
              '',
              {
                validators: [Validators.required],
                updateOn: 'change',
              },
            ],
          },
          { validators: this.passwordsMatchValidator }
        );
      },
      error: (error) => {
        console.error('Error fetching profile:', error);
        this.toastService.show({
          text: `Error fetching profile: ${error.error.detail}`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
    });
  }

  saveProfile() {
    if (this.profileForm.valid) {
      const updatedProfile = {
        ...this.profile,
        ...this.profileForm.value,
      };
      this.profileService.updateProfile(updatedProfile).subscribe({
        next: (response) => {
          console.log('Profile updated successfully:', response);
          this.toastService.show({
            text: 'Profile updated successfully',
            classname: 'bg-success text-light',
            delay: 5000,
          });
          this.loadProfile();
          this.originalProfile = { ...updatedProfile } as AnyUserProfile;
          this.profileForm.markAsPristine(); // Mark the form as pristine after saving
        },
        error: (error) => {
          console.error('Error updating profile:', error);
          this.toastService.show({
            text: `Error updating profile: ${error.error.detail}`,
            classname: 'bg-danger text-light',
            delay: 5000,
          });
        },
      });
      this.editMode = false;
    }
  }

  cancelEdit() {
    if (this.profile) {
      this.loadProfile();
      this.editMode = false;
    }
  }

  isCustomerProfile(profile: any): profile is CustomerProfile {
    return profile && profile.profileType === 'customer';
  }

  changePassword() {
    this.authService
      .changePassword(
        this.passwordForm.get('currentPassword')?.value,
        this.passwordForm.get('newPassword')?.value
      )
      .subscribe({
        next: (response) => {
          console.log('Password changed successfully:', response);
          this.toastService.show({
            text: 'Password changed successfully',
            classname: 'bg-success text-light',
            delay: 5000,
          });
          this.passwordForm.reset();
        },
        error: (error) => {
          console.error('Error changing password:', error);
          this.toastService.show({
            text: `Error changing password: ${error.error.detail}`,
            classname: 'bg-danger text-light',
            delay: 5000,
          });
        },
      });
  }

  async checkIfEmailExist() {
    if (this.profileForm.get('email')?.value === this.originalProfile.email)
      return;
    this.authService
      .checkIfEmailExist(this.profileForm.get('email')?.value)
      .subscribe({
        next: (response) => {
          console.log('Email exists:', response);
          if (response.exists) {
            this.profileForm.get('email')?.setErrors({ emailExists: true });
          } else {
            this.profileForm.get('email')?.setErrors(null);
          }
        },
      });
  }

  async checkIfUsernameExist() {
    if (
      this.profileForm.get('username')?.value === this.originalProfile.username
    )
      return;
    this.authService
      .checkIfUsernameExist(this.profileForm.get('username')?.value)
      .subscribe({
        next: (response) => {
          console.log('Username exists:', response);
          if (response.exists) {
            this.profileForm
              .get('username')
              ?.setErrors({ usernameExists: true });
          } else {
            this.profileForm.get('username')?.setErrors(null);
          }
        },
      });
  }

  passwordsMatchValidator(group: FormGroup) {
    const newPassword = group.get('newPassword')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;
    return newPassword === confirmPassword ? null : { notMatch: true };
  }
}

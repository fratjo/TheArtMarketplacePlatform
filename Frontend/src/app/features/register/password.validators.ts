import { AbstractControl, ValidationErrors } from '@angular/forms';

export function hasUppercase(
  control: AbstractControl
): ValidationErrors | null {
  const value = control.value || '';
  return /[A-Z]/.test(value) ? null : { missingUppercase: true };
}

export function hasLowercase(
  control: AbstractControl
): ValidationErrors | null {
  const value = control.value || '';
  return /[a-z]/.test(value) ? null : { missingLowercase: true };
}

export function hasNumber(control: AbstractControl): ValidationErrors | null {
  const value = control.value || '';
  return /\d/.test(value) ? null : { missingNumber: true };
}

export function hasSpecialCharacter(
  control: AbstractControl
): ValidationErrors | null {
  const value = control.value || '';
  return /[@$!%*?&]/.test(value) ? null : { missingSpecialCharacter: true };
}

export function hasMinimumLength(
  control: AbstractControl
): ValidationErrors | null {
  const value = control.value || '';
  return value.length >= 6 ? null : { tooShort: true };
}

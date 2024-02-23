import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

// funkcja walidująca wzorzec
export class CustomValidators {
  static patternValidator(regex: RegExp, error: ValidationErrors): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      if (!control.value) {
        // jeśli kontrolka jest pusta, zwróć brak błędu
        return null;
      }

      // testuj wartość kontrolki względem podanego wzorca
      const valid = regex.test(control.value);

      // jeśli wartość jest poprawna, zwróć brak błędu, w przeciwnym razie zwróć błąd przekazany jako drugi parametr
      return valid ? null : error;
    };
  }

  // funkcja walidująca hasła
  static passwordMatchValidator(control: AbstractControl) {
    const passwordControl = control.get('password');
    const confirmPasswordControl = control.get('confirmPassword');
    if (!passwordControl || !confirmPasswordControl) {
      return null; // jeśli nie ma kontrolek, zwróć brak błędu
    }
    const password: string = passwordControl.value; // pobierz hasło z kontrolki formularza hasła
    const confirmPassword: string = confirmPasswordControl.value; // pobierz hasło z kontrolki formularza potwierdzenia hasła
    // porównaj, czy hasła są takie same
    if (password !== confirmPassword) {
      // jeśli nie są takie same, ustaw błąd w kontrolce formularza potwierdzenia hasła
      confirmPasswordControl.setErrors({ NoPasswordMatch: true });
    }
    return null;
  }
}

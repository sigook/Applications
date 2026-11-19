import { configure } from 'vee-validate';

export function configureValidation(): void {
  configure({
    validateOnInput: true,
  });
}

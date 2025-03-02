import { Component } from '@angular/core';
import { FormGroup, AbstractControl } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';

@Component({
  template: ''
})
export abstract class BaseFormComponent {
  // the form model
  form!: FormGroup;

  getErrors(
    control: AbstractControl,
    displayName: string,
    customMessages: { [key: string]: string } | null = null
  ): string[] {
    var errors: string[] = [];
    Object.keys(control.errors || {}).forEach((key) => {

      switch (key) {
        case 'required':
          errors.push(`${displayName} ${customMessages?.[key] ?? this.translateService.instant('common.Error.FieldIsRequired')}`);
          break;
        case 'pattern':
          errors.push(`${displayName} ${customMessages?.[key] ?? this.translateService.instant('common.Error.ContainsInvalidCharaters')}`);
          break;
        case 'isDupeField':
          errors.push(`${displayName} ${customMessages?.[key] ?? this.translateService.instant('common.Error.InvalidExistsChooseAnother' )}`);
          break;
        default:
          errors.push(`${displayName} {this.translate.instant('common.Error.IsInvalid')}.`);
          break;
      }
    });
    return errors;
  }
  constructor(private translateService: TranslateService) {
  }
}

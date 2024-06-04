import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';

@Component({
  selector: 'app-text-input-f2',
  templateUrl: './text-input-f2.component.html',
  styleUrls: ['./text-input-f2.component.scss']
})
export class TextInputF2Component implements ControlValueAccessor  {
  @Input() label = '';
  @Input() type = 'text';
  @Input() isRequired = false;
  @Input() submitted = false;
  @Input() labelOverride: string | undefined;
  @Input() invalidPatternMessage: string | undefined;

  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this;
  }

  get control(): FormControl {
    return this.ngControl.control as FormControl;
  }

  writeValue(obj: any): void { }
  registerOnChange(fn: any): void { }
  registerOnTouched(fn: any): void { }
}

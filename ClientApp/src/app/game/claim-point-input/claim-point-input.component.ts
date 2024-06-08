import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';

@Component({
  selector: 'app-claim-point-input',
  templateUrl: './claim-point-input.component.html',
  styleUrls: ['./claim-point-input.component.scss']
})
export class ClaimPointInputComponent implements OnInit {
  @Input() initialValue: number | undefined;
  @Output() pointToClaim = new EventEmitter();
  claimPointForm: FormGroup = new FormGroup({});
  submitted = false;
  
  constructor(private formBuilder: FormBuilder) {

  }
  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.claimPointForm = this.formBuilder.group({
      point: [this.initialValue, [Validators.required, Validators.pattern('(105|110|115|120|125|130|135|140|145|150|155|160|165)'), 
        this.biggerOrEqualsToInitialValue()]],
    });
  }

  claimPoint() {
    this.submitted = true;

    if (this.claimPointForm.valid) {
      this.pointToClaim.emit(this.claimPointForm.get('point')?.value);
    }
  }

  pass() {
    this.pointToClaim.emit(-1);
  }

  private  biggerOrEqualsToInitialValue(): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value >= this.initialValue! ? null : { lessThan: true };
    }
  }
}

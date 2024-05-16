import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { DropDownFilteSelection } from 'src/app/shared/models/pagination/dropDownFilterSelection';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-drop-down-filter-entries',
  templateUrl: './drop-down-filter-entries.component.html',
  styleUrls: ['./drop-down-filter-entries.component.scss']
})
export class DropDownFilterEntriesComponent implements OnInit {
  @Input() entries: string[] | undefined;
  @Input() label: string | undefined;
  @Input() defaultValue: string | undefined;
  @Input() action: string | undefined;
  @Output() selectedValue = new EventEmitter<{}>();
  dropdownForm: FormGroup = new FormGroup({});

  constructor(private sharedService: SharedService,
    private formBuilder: FormBuilder
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.sharedService.resetFilter$.subscribe(_ => {
      this.dropdownForm.controls['mydropdown'].setValue(this.defaultValue);
    })
  }

  initializeForm() {
    this.dropdownForm = this.formBuilder.group({
      mydropdown: ['']
    });
  }

  onEntriesSelection(event: any) {
    if (this.action) {
      this.selectedValue.emit(new DropDownFilteSelection(event.target.value, this.action));
    }
  }
}

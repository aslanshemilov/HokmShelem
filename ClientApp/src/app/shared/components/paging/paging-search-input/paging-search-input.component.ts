import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { debounceTime } from 'rxjs';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-paging-input-search',
  templateUrl: './paging-search-input.component.html',
  styleUrls: ['./paging-search-input.component.scss']
})
export class PagingSearchInputComponent implements OnInit {
  searchForm: FormGroup = new FormGroup({});
  @Output() searchOutput  = new EventEmitter<string>();

  constructor(private sharedService: SharedService,
    private formBuilder: FormBuilder) {}

  ngOnInit(): void {
    this.initializeForm();
    this.sharedService.resetFilter$.subscribe(_ => {
      this.searchForm.controls['search'].setValue('');
    })
  }

  initializeForm() {
    this.searchForm = this.formBuilder.group({
      search: ['']
    });

    this.searchForm.get('search')?.valueChanges
      .pipe(debounceTime(500))
      .subscribe(value => {
        this.searchOutput.emit(value);
      });
  }
}

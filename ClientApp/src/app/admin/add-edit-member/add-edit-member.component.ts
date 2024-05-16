import { Component, OnInit } from '@angular/core';
import { AdminService } from '../admin.service';
import { ActivatedRoute, Router } from '@angular/router';
import { MemberAddEdit } from 'src/app/shared/models/admin/MemberAddEdit';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-add-edit-member',
  templateUrl: './add-edit-member.component.html',
  styleUrls: ['./add-edit-member.component.scss']
})
export class AddEditMemberComponent implements OnInit {
  addNew = true;
  formInitialized = false;
  submitted = false;
  memberForm: FormGroup = new FormGroup({});
  errorMessages: string[] = [];

  constructor(private adminService : AdminService,
    private formBuilder: FormBuilder,
    private sharedService: SharedService,
    private router: Router,
    private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    const id = this .activatedRoute.snapshot.paramMap.get('id');
    if(id)
      {
        this.addNew = false; // this means we are editing a member
      this.getMember(id);
    } else {
      this.initializeForm(undefined);
    }
  }
  getMember(id: string) {
    this.adminService.getMember(id).subscribe({
      next: member => {
        console.log(member);
       this.initializeForm(member);
      }
    })
  }

  initializeForm(member: MemberAddEdit | undefined) {
    if (member) {
      // form for editing an existing member
      this.memberForm = this.formBuilder.group({
        id: [member.id],
        userName: [member.userName, Validators.required],
        playerName: [member.playerName, Validators.required],
        provider: [member.provider, Validators.required],
        password: [''],
        email: [member.email, Validators.required]
      });

      //this.existingMemberRoles = member.roles.split(',');
    } else {
      // form for creating a member
      this.memberForm = this.formBuilder.group({
        id: [''],
        userName: ['', Validators.required],
        playerName: ['', Validators.required],
        provider: ['', Validators.required],
        password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(15)]]
      });
    }

    this.formInitialized = true;
  }

  submit() {
    this.submitted = true;
    this.errorMessages = [];


    if (this.memberForm.valid) {
      console.log('here');
      console.log(this.memberForm.value);
      this.adminService.addEditMember(this.memberForm.value).subscribe({
        next: (response: any) => {
          this.sharedService.showNotification(true, response.title, response.message);
          this.router.navigateByUrl('/admin');
        },
        error: error => {
          if (error.error.errors) {
            this.errorMessages = error.error.errors;
          } else {
            this.errorMessages.push(error.error);
          }
        }
      })
    }
  }

}

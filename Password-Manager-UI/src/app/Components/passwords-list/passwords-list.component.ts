import { FormGroup, FormControl, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { User } from './../../Models/User';
import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/Services/user.service';

@Component({
  selector: 'app-passwords-list',
  templateUrl: './passwords-list.component.html',
  styleUrls: ['./passwords-list.component.css']
})
export class PasswordsListComponent implements OnInit {

  newPasswordForm!: FormGroup;
  passwordsList: any;
  newPasswordModel: any;
  showPassword: boolean = false;

  constructor(
    private userservice: UserService
  ) { }

  ngOnInit(): void {
    this.getCurrentUserData();
    this.newPasswordForm = new FormGroup({
      email: new FormControl(null, [Validators.required]),
      encryptedPassword: new FormControl(null, [Validators.required]),
      name: new FormControl(null, [Validators.required])
    });
  }

  showHidePassword() {
    this.showPassword = !this.showPassword;
  }

  // get all user passwords
  getCurrentUserData(){
    this.userservice.getCurrrentUserData().subscribe({
      next: (res: any) => {
        this.passwordsList = res;
        console.log(this.passwordsList);
      },
      error: (err: HttpErrorResponse) => {
        console.log(err);
      }

    })
  }

  // add new password to user data
  newPassword(){
    this.newPasswordModel = this.newPasswordForm.value;
    this.userservice.newPassword(this.newPasswordModel).subscribe({
      next: (res:any) => {
        console.log(res);
      },
      error: (err: HttpErrorResponse) => {
        console.log(err);
      }
    })
    
  }

}

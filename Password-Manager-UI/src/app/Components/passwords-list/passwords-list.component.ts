import { FormGroup, FormControl, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { User } from './../../Models/User';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { UserService } from 'src/app/Services/user.service';
import Logos from 'src/assets/logos.json';
declare var window: any;
interface LOGOS {
  name: string,
  value: string
}
@Component({
  selector: 'app-passwords-list',
  templateUrl: './passwords-list.component.html',
  styleUrls: ['./passwords-list.component.css']
})
export class PasswordsListComponent implements OnInit {
  @ViewChild('closeModal') private closeModal!: ElementRef;
  formModal: any;
  newPasswordForm!: FormGroup;
  passwordsList: any;
  newPasswordModel: any;
  showPassword: boolean = false;
  logos: LOGOS[] = Logos;

  constructor(
    private userservice: UserService,
  ) { 
    console.log(this.logos);
    
  }

  ngOnInit(): void {
    this.formModal = new window.bootstrap.Modal(
      document.getElementById('modalAddPassword')
    );

    this.getCurrentUserData();
    this.newPasswordForm = new FormGroup({
      email: new FormControl(null, [Validators.required]),
      encryptedPassword: new FormControl(null, [Validators.required]),
      name: new FormControl(null, [Validators.required]),
      imageUrl: new FormControl(null)
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
    //this.newPasswordModel = this.newPasswordForm.value;
    this.userservice.newPassword(this.newPasswordForm.value).subscribe({
      next: (res:any) => {
        console.log(res);
        window.location.reload();
        //this.formModal.hide();
 
      },
      error: (err: HttpErrorResponse) => {
        console.log(err);
      }
    })
  }

  // close add password modal
  // public hideModel() {
  //   this.closeModal.nativeElement.click();
  // }

  chooseImageUrl(url:string){
    this.logos.map( val => {
      if(url == val.name){
        this.newPasswordForm.value.imageUrl = val.value;
      }
    });
    console.log(this.newPasswordForm.value.imageUrl);
  }

  // delete password
  deletepassword(id: any){
    this.userservice.deletePassword(id).subscribe({
      next: (res: any) => {
        console.log(res);
        console.log("password deleted successfully!");
        window.location.reload();
      },
      error: (err: HttpErrorResponse) => {
        console.log(err);
        
      }
    })
  }

}

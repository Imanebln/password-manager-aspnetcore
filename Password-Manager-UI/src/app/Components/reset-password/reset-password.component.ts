import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/Services/auth.service';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { RequestResetPassword } from 'src/app/Models/RequestResetPassword';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {

  recoverPassForm! : FormGroup;
  passwordModel : RequestResetPassword = <RequestResetPassword>{};

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.recoverPassForm = new FormGroup({
      newPassword: new FormControl(null,[Validators.required]),
      confirmNewPassword: new FormControl(null, [Validators.required]),
      token: new FormControl(null,Validators.required)
    })
  }

  onSubmit(){
    // retreive email from url params
    const queryString = window.location.search;
    const urlParams = new URLSearchParams(queryString);

    this.passwordModel.email = urlParams.get('email') + '';
    this.passwordModel.newPassword = this.recoverPassForm.value.newPassword;
    this.passwordModel.confirmNewPassword = this.recoverPassForm.value.confirmNewPassword;
    this.passwordModel.token = this.recoverPassForm.value.token;

    this.authService.resetPassword(this.passwordModel).subscribe({
      next: (res: any) => {
        console.log(this.passwordModel.email);
        
        console.log(res);
        this.router.navigate((['login']));
      },
      error: (err: HttpErrorResponse) => {
        console.log(this.passwordModel.email);
        console.log(err);
        if(err.status == 200){
          this.router.navigate((['login']));
        }
      }
    })
  }

}
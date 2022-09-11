import { HttpErrorResponse } from '@angular/common/http';
import { TwoFAModel } from './../../Models/TwoFAModel';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/Services/auth.service';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-two-fa-login',
  templateUrl: './two-fa-login.component.html',
  styleUrls: ['./two-fa-login.component.css']
})
export class TwoFaLoginComponent implements OnInit {

  twoFAForm!: FormGroup;
  twoFAModel!: TwoFAModel;
  
  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.twoFAForm = new FormGroup({
      provider: new FormControl(null, [Validators.required]),
    })
  }

  onSubmit(){
    // retreive email and token from url params
    const queryString = window.location.search;
    const urlParams = new URLSearchParams(queryString);

    this.twoFAModel.email = urlParams.get('email') + '';
    this.twoFAModel.token = urlParams.get('token') + '';
    this.twoFAModel.provider = this.twoFAForm.value.provider;

    this.authService.login_2fa(this.twoFAModel).subscribe({
      next: (res:any) => {
        console.log(this.twoFAModel.email + " " + this.twoFAModel.provider);
        console.log(res);
        this.router.navigate(['dashboard']);
      },
      error: (err: HttpErrorResponse) => {
        console.log(this.twoFAModel.email + " " + this.twoFAModel.provider);
        console.log(err);
        if(err.status == 200){
          this.router.navigate((['dashboard']));
        }
      }
    })
  }

}

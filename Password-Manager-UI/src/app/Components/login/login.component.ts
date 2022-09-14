import { HttpErrorResponse } from '@angular/common/http';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TwoFAModel } from 'src/app/Models/TwoFAModel';
import { AuthService } from 'src/app/Services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  @Output() redirect:EventEmitter<any> = new EventEmitter();

  signInForm!: FormGroup;
  res: any;
  
  constructor(
    private router: Router,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    // Sign in form
    console.log("this is register page!");

    this.signInForm = new FormGroup({
      username: new FormControl(null, [Validators.required]),
      password: new FormControl(null, [Validators.required])
    })
  }

  onSubmit() {
    // calling login method in auth service
    this.authService.signIn(this.signInForm.value).subscribe({
      next: (response: any) => {
        // show success alert here
        
        console.log(response.username);
        if(response.is2FARequired == true){
          this.res = {... response};
          this.redirect.emit(this.res);
          this.router.navigate(['two-fa-login',this.res]);
        }
        else{
          const token = response.accessToken.accessToken;
          const data = response.accessToken.username;
          localStorage.setItem('jwt', token);
          localStorage.setItem('user-data',data);
          this.router.navigate(['dashboard']);
        }
        
        
      },
      error: (err: HttpErrorResponse) => {
        // show error alert here
        console.log(err);

      }
    })
  }

  recoverPassword() {
    this.authService.requestPassReset(this.signInForm.value.email).subscribe({
      next: (res: any) => {
        console.log(res);
        // this.router.navigate(['reset-password']);
      },
      error: (err: HttpErrorResponse) => {
        console.log(err);
      }
    })
  }

}

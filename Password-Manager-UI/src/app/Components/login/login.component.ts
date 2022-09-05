import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/Services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  signInForm!: FormGroup;
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
        const token = response.accessToken.accessToken;
        console.log(response);
        localStorage.setItem('jwt', token);
        this.router.navigate(['dashboard']);
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

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
    username: new FormControl(null,[Validators.required]),
    password: new FormControl(null, [Validators.required])
  })
  }

  onSubmit(){
    // calling login method in auth service
    this.authService.signIn(this.signInForm.value).subscribe({
      next: () => {
        // show success alert here
        this.router.navigate(['dashboard']);
      },
      error: (err: HttpErrorResponse) => {
        // show error alert here
        console.log(err);
        
      }
    })
  }

}

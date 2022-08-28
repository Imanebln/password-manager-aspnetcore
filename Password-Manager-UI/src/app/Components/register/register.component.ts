import { AuthService } from './../../Services/auth.service';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { faUser } from '@fortawesome/free-solid-svg-icons';
import { HttpErrorResponse } from '@angular/common/http';
@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  signUpForm!: FormGroup;

  constructor(
    private router: Router,
    private authService: AuthService
  ) { }

  

  ngOnInit(): void {
    // Sign up form
  this.signUpForm = new FormGroup({
    username: new FormControl(null,[Validators.required]),
    email: new FormControl(
      null,
      [Validators.required, Validators.email]
    ),
    password: new FormControl(null, [Validators.required])
  })
  }

  onSubmit(){
    // call register method in auth service 
    this.authService.signUp(this.signUpForm.value).subscribe({
      next: (response: any) => {
        // show success alert here
        this.router.navigate(['dashboard']);
      },
      error: (err: HttpErrorResponse) => {
        // show error alert here
      }
    })
  }

}

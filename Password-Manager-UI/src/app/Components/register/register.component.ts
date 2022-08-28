import { AuthService } from './../../Services/auth.service';
import { Component, OnInit } from '@angular/core';
import { AsyncValidatorFn, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
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
    console.log("this is register page!");
    
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
    // calling register method in auth service 
    this.authService.signUp(this.signUpForm.value).subscribe({
      next: () => {
        // show success alert here
        this.router.navigate(['login']);
      },
      error: (err: HttpErrorResponse) => {
        // show error alert here
        if(err.status == 200){
          this.router.navigate(['login']);
        }
        console.log(err);
        
      }
    })
  }

 

}

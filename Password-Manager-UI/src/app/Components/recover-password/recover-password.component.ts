import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/Services/auth.service';

@Component({
  selector: 'app-recover-password',
  templateUrl: './recover-password.component.html',
  styleUrls: ['./recover-password.component.css']
})
export class RecoverPasswordComponent implements OnInit {

  recoverPassForm! : FormGroup;
  constructor(
    private router : Router,
    private authservice: AuthService
  ) { }

  ngOnInit(): void {
    this.recoverPassForm = new FormGroup({
      email: new FormControl(null,[Validators.required])
    });
  }

  onSubmit(){
    this.authservice.requestPassReset(this.recoverPassForm.value).subscribe({
      next: (res: any) => {
        console.log(res);
        this.router.navigate(['verification']);
        // success alert goes here 
      },
      error: (err: HttpErrorResponse) => {
        console.log(err);
        if(err.status == 200){
          this.router.navigate(['verification']);
        }
        // error alert goes here
      }
    })
  }


}

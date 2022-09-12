import { HttpErrorResponse } from '@angular/common/http';
import { TwoFAModel } from './../../Models/TwoFAModel';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/Services/auth.service';
import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-two-fa-login',
  templateUrl: './two-fa-login.component.html',
  styleUrls: ['./two-fa-login.component.css']
})
export class TwoFaLoginComponent implements OnInit {

  // @Input() twoFAModel!: TwoFAModel;
  
  twoFAForm!: FormGroup;
  res: any;
  twoFAModel: TwoFAModel = <TwoFAModel>{};
  
  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
  ) { }

  ngOnInit(): void {
    this.res = this.route.snapshot.params;

    console.log(this.res.username + " " + this.res.provider);
    this.twoFAForm = new FormGroup({
      token: new FormControl(null, [Validators.required]),
    })
  }

  onSubmit(){
    this.twoFAModel.username = this.res.username;
    this.twoFAModel.provider = this.res.provider;
    this.twoFAModel.token = this.twoFAForm.value.token;

    this.authService.login_2fa(this.twoFAModel).subscribe({
      next: (resp:any) => {
        console.log(resp);
        const token = resp.accessToken.accessToken;
        localStorage.setItem('jwt', token);
        this.router.navigate(['dashboard']);
      },
      error: (err: HttpErrorResponse) => {
        console.log(err);
        // if(err.status == 200){
        //   this.router.navigate((['dashboard']));
        // }
      }
    })
  }

}

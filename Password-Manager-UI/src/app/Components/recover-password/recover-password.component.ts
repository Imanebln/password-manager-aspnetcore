import { HttpErrorResponse } from '@angular/common/http';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from 'src/app/Services/auth.service';

@Component({
  selector: 'app-recover-password',
  templateUrl: './recover-password.component.html',
  styleUrls: ['./recover-password.component.css']
})
export class RecoverPasswordComponent implements OnInit {
  @Output() redirect:EventEmitter<any> = new EventEmitter();

  recoverPassForm! : FormGroup;
  data!: any;
  constructor(
    private router : Router,
    private authservice: AuthService,
    private route: ActivatedRoute
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
        this.data = {... this.recoverPassForm};
        this.redirect.emit(this.data);
        this.router.navigate(['verification',this.data]);
        // success alert goes here 
      },
      error: (err: HttpErrorResponse) => {
        console.log(err);
        if(err.status == 200){
          this.data = {... this.recoverPassForm.value};
          this.router.navigate(['verification',this.data]);
        }
        // error alert goes here
      }
    })
  }


}

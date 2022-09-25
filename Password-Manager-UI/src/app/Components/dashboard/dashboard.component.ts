import { UserService } from './../../Services/user.service';
import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  user: any;
  passwordsList: any;

  constructor(
    private router: Router,
    private userService: UserService
  ) { }

  ngOnInit(): void {
    this.getCurrentUserData();
  }

  //generate pdf summary
  generatePdfSummary(){
    this.userService.generatePdfSummary().subscribe({
      next: (res:any) => {
        console.log(res);
        
      },
      error: (err: HttpErrorResponse) => {
        console.log(err);
        
      }
    })
  }

  // get all user passwords
  getCurrentUserData(){
    this.userService.getCurrrentUserData().subscribe({
      next: (res: any) => {
        this.passwordsList = res;
        console.log(this.passwordsList);
      },
      error: (err: HttpErrorResponse) => {
        console.log(err);
      }

    })
  }

  

}

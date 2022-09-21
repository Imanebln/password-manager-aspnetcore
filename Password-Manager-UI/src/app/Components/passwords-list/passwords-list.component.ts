import { HttpErrorResponse } from '@angular/common/http';
import { User } from './../../Models/User';
import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/Services/user.service';

@Component({
  selector: 'app-passwords-list',
  templateUrl: './passwords-list.component.html',
  styleUrls: ['./passwords-list.component.css']
})
export class PasswordsListComponent implements OnInit {

  passwordsList: any;
  constructor(
    private userservice: UserService
  ) { }

  ngOnInit(): void {
    this.getCurrentUserData();
  }

  add(){
    
  }

  getCurrentUserData(){
    this.userservice.getCurrrentUserData().subscribe({
      next: (res: any) => {
        this.passwordsList = res.accountInfos;
        console.log(res.accountInfos);
        console.log(this.passwordsList);
      },
      error: (err: HttpErrorResponse) => {
        console.log(err);
      }

    })
  }

}

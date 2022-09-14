import { UserService } from './../../Services/user.service';
import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  user: any;

  constructor(
    private router: Router,
    private userService: UserService
  ) { }

  ngOnInit(): void {
    const data = localStorage.getItem('user-data');
    this.user = data;
  }

  logout(){
    localStorage.removeItem('jwt');
    this.router.navigate(['/login']);
  }

}

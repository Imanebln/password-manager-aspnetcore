import { CookieService } from './../../../Services/cookie.service';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from 'src/app/Services/user.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {

  user: any;
  constructor(
    private router: Router,
    private userService: UserService,
    private cookieService: CookieService
  ) { }

  ngOnInit(): void {
    const data = localStorage.getItem('user-data');
    this.user = data;
  }

  logout(){
    localStorage.removeItem('jwt');
    localStorage.removeItem('user-data');
    this.cookieService.deleteCookie('decryptedKey');
    this.cookieService.deleteCookie('refreshToken');
    this.router.navigate(['/login']);
  }

}

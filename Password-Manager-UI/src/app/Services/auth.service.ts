import { Login } from './../Models/Login';
import { Register } from './../Models/Register';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  apiUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) { }

  // Sign Up 
  signUp(user : any){
    return this.http.post(this.apiUrl + 'Accounts/signup', user);
  }
  signIn(user : Login){
    return this.http.post(this.apiUrl + 'Accounts/Login', user);
  }
}

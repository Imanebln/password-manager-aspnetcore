import { Login } from './../Models/Login';
import { Register } from './../Models/Register';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { RequestResetPassword } from '../Models/RequestResetPassword';
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
  // Login
  signIn(user : Login){
    return this.http.post(this.apiUrl + 'Accounts/Login', user);
  }
  // request password reset
  requestPassReset(email: string){
    return this.http.post(this.apiUrl + 'Accounts/request-password-reset', email);
  }
  // reset password
  resetPassword(model: any){
    return this.http.post(this.apiUrl + 'Accounts/reset-password', model);
  }

  // 2FA Config
  set_2fa(model: boolean)
  {
    return this.http.get(this.apiUrl + 'Accounts/Set-2fa?enabled=' + model);
  }
  send_2fa_code(email: string){
    return this.http.get(this.apiUrl + 'Accounts/send-2fa-code?email=' + email);
  }
  login_2fa(model: any){
    return this.http.post(this.apiUrl + 'Accounts/2fa-login', model);
  }
}

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  apiUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) { }

  // get current user data
  getCurrrentUser(){
    return this.http.get(this.apiUrl + 'Data/get-current-user-data');
  }

}

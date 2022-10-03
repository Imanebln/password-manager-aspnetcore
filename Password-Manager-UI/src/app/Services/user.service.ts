import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  apiUrl: string = environment.apiUrl;
  
  httpOptions = {
    withCredentials: true
  };

  constructor(private http: HttpClient
    ) { }
    

  // get current user data
  getCurrrentUserData(){
    return this.http.get<[]>(this.apiUrl + 'Data/passwords',this.httpOptions);
  }

  // add new password to user data
  newPassword(model: any){
    return this.http.post(this.apiUrl + 'Data/passwords',model,this.httpOptions);
  }

  // delete password
  deletePassword(id: any){
    return this.http.delete(this.apiUrl + 'Data/passwords/' + id, this.httpOptions);
  }

  // generate pdf summary
  generatePdfSummary(){
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/pdf');
    return this.http.get(this.apiUrl + 'Data/generate-pdf-summary',{ headers: headers, responseType: 'blob',withCredentials: true });
  }  

}

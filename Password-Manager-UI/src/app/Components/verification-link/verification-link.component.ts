import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from 'src/app/Services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-verification-link',
  templateUrl: './verification-link.component.html',
  styleUrls: ['./verification-link.component.css']
})
export class VerificationLinkComponent implements OnInit {

  data!: any;
  email!: string;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService

  ) { }

  ngOnInit(): void {
    
    this.data = this.route.snapshot.params;
    
    this.router.navigate(['verification'], {
      queryParams: {
        'yourParamName': null,
        'youCanRemoveMultiple': null,
      },
      queryParamsHandling: 'merge'
    })

    this.email = this.data.email;
    console.log(this.email);
    
  }

  sendAgain(){
    this.authService.send_email_confirmation(this.email).subscribe({
      next: (res: any) => {
        console.log(res);
        // success alert goes here
      },
      error: (err: HttpErrorResponse) => {
        console.log(err);
        // error alert goes here
        
      }
    })
  }

}

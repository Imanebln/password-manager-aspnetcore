import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-verification-link',
  templateUrl: './verification-link.component.html',
  styleUrls: ['./verification-link.component.css']
})
export class VerificationLinkComponent implements OnInit {

  data!: any;

  constructor(
    private route: ActivatedRoute,
    private router: Router

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

    console.log(this.data);
    
  }

}

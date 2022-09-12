import { ActivatedRoute } from '@angular/router';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-verification-link',
  templateUrl: './verification-link.component.html',
  styleUrls: ['./verification-link.component.css']
})
export class VerificationLinkComponent implements OnInit {

  data!: any;

  constructor(
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    this.data = this.route.snapshot.params;
    console.log(this.data);
    
  }

}

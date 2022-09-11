import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VerificationLinkComponent } from './verification-link.component';

describe('VerificationLinkComponent', () => {
  let component: VerificationLinkComponent;
  let fixture: ComponentFixture<VerificationLinkComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ VerificationLinkComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(VerificationLinkComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

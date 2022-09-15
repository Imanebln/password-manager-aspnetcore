import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserPasswordsComponent } from './user-passwords.component';

describe('UserPasswordsComponent', () => {
  let component: UserPasswordsComponent;
  let fixture: ComponentFixture<UserPasswordsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserPasswordsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UserPasswordsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

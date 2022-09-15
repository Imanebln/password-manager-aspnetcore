import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { RegisterComponent } from './Components/register/register.component';
import { LoginComponent } from './Components/login/login.component';
import { DashboardComponent } from './Components/dashboard/dashboard.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { JwtModule } from '@auth0/angular-jwt';
import { AuthGuard } from './Guards/auth.guard';
import { ResetPasswordComponent } from './Components/reset-password/reset-password.component';
import { RecoverPasswordComponent } from './Components/recover-password/recover-password.component';
import { AlertComponent } from './alert/alert/alert.component';
import { VerificationLinkComponent } from './Components/verification-link/verification-link.component';
import { TwoFaLoginComponent } from './Components/two-fa-login/two-fa-login.component';
import { DoughnutChartComponent } from './Components/Charts/doughnut-chart/doughnut-chart.component';
import { PasswordsListComponent } from './Components/passwords-list/passwords-list.component';
import { NavbarComponent } from './Components/Shared/navbar/navbar.component';
import { SidebarComponent } from './Components/Shared/sidebar/sidebar.component';
import { UserPasswordsComponent } from './Components/user-passwords/user-passwords.component';
import { AddPasswordComponent } from './Components/Forms/add-password/add-password.component';

export function tokenGetter() {
  return localStorage.getItem('jwt');
}
@NgModule({
  declarations: [
    AppComponent,
    RegisterComponent,
    LoginComponent,
    DashboardComponent,
    ResetPasswordComponent,
    RecoverPasswordComponent,
    AlertComponent,
    VerificationLinkComponent,
    TwoFaLoginComponent,
    DoughnutChartComponent,
    PasswordsListComponent,
    NavbarComponent,
    SidebarComponent,
    UserPasswordsComponent,
    AddPasswordComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FontAwesomeModule,
    ReactiveFormsModule,
    HttpClientModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        allowedDomains: ['localhost:7077'],
        disallowedRoutes: [],
      },
    }),
  ],
  providers: [AuthGuard],
  bootstrap: [AppComponent]
})
export class AppModule { }

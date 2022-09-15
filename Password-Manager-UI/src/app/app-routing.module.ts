import { UserPasswordsComponent } from './Components/user-passwords/user-passwords.component';
import { VerificationLinkComponent } from './Components/verification-link/verification-link.component';
import { ResetPasswordComponent } from './Components/reset-password/reset-password.component';
import { DashboardComponent } from './Components/dashboard/dashboard.component';
import { LoginComponent } from './Components/login/login.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegisterComponent } from './Components/register/register.component';
import { AuthGuard } from './Guards/auth.guard';
import { RecoverPasswordComponent } from './Components/recover-password/recover-password.component';
import { TwoFaLoginComponent } from './Components/two-fa-login/two-fa-login.component';

const routes: Routes = [
  { path: 'register', component: RegisterComponent},
  { path: 'login', component: LoginComponent},
  { path: 'two-fa-login', component: TwoFaLoginComponent},
  { path: 'reset-password', component: ResetPasswordComponent},
  { path: 'recover-password', component: RecoverPasswordComponent},
  { path: 'verification', component: VerificationLinkComponent},
  { path: 'dashboard', component: DashboardComponent, canActivate:[AuthGuard]},
  { path: 'user', component: UserPasswordsComponent, canActivate:[AuthGuard]},
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

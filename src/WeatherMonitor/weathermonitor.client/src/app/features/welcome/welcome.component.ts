import { Component, inject } from '@angular/core';
import { AuthorizationService } from '../authorization/services/authorization-service';
import { RouterModule, RouterOutlet } from '@angular/router';
import { MenuComponent } from '../menu/menu.component';
import { MaterialModule } from '../../shared/material.module';
import { LogoComponent } from '../../shared/components/logo/logo.component';
import { LoginComponent } from '../../shared/components/login/login.component';
import { TranslateModule } from '@ngx-translate/core';
import { SampleComponent } from '../../sample/app.component';
import { ThemeSwitchComponent } from '../menu/components/theme-switch/theme-switch.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-welcome',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterModule,
    MenuComponent,
    MaterialModule,
    LogoComponent,
    LoginComponent,
    TranslateModule,
    ThemeSwitchComponent,
    SampleComponent,
  ],
  templateUrl: './welcome.component.html',
})
export class WelcomeComponent {
  private authService = inject(AuthorizationService);
  isLoggedIn = this.authService.isAuthorized;
}

import { Component, inject } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { AuthorizationService } from './features/authorization/services/authorization-service';
import { MenuComponent } from './features/menu/menu.component';
import { MaterialModule } from './shared/material.module';
import { LoginComponent } from './shared/components/login/login.component';
import { TranslateModule } from '@ngx-translate/core';
import { ThemeSwitchComponent } from './features/menu/components/theme-switch/theme-switch.component';
import { SampleComponent } from './sample/app.component';
import { LogoComponent } from './shared/components/logo/logo.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
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
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  title = 'Weather Monitor';

  private authService = inject(AuthorizationService);
  isAuthenticated = this.authService.isAuthorized;
  isAdminRole = this.authService.isAdminRole;
}

import { Component, inject, OnInit } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { AuthorizationService } from './features/authorization/services/authorization-service';
import { MaterialModule } from './shared/material.module';
import { LoginComponent } from './shared/components/login/login.component';
import { TranslateModule } from '@ngx-translate/core';
import { ThemeSwitchComponent } from './features/menu/components/theme-switch/theme-switch.component';
import { LogoComponent } from './shared/components/logo/logo.component';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { CookiePolicyDialogComponent } from './shared/components/cookie-policy-dialog/cookie-policy-dialog.component';
import { CookieConsentService } from './shared/services/cookie-consent.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterModule,
    MaterialModule,
    LogoComponent,
    LoginComponent,
    TranslateModule,
    ThemeSwitchComponent,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit {
  title = 'Weather Monitor';

  private authService = inject(AuthorizationService);
  isAuthenticated = this.authService.isAuthorized;
  isAdminRole = this.authService.isAdminRole;

  private readonly cookiesConsentService = inject(CookieConsentService);
  #dialog = inject(MatDialog);

  ngOnInit(): void {
    if (!this.cookiesConsentService.checkCookieConsent()) {
      this.openCookieConsentDialog();
    }
  }

  private openCookieConsentDialog() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;

    this.#dialog.open<CookiePolicyDialogComponent, unknown, boolean>(
      CookiePolicyDialogComponent,
      {
        data: {},
        panelClass: 'smallPopup',
        maxWidth: '90dvw',
        maxHeight: '120dvw',
        disableClose: true,
      }
    );
  }
}

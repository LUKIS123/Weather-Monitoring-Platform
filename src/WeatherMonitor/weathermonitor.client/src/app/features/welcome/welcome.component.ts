import { Component, inject } from '@angular/core';
import { AuthorizationService } from '../authorization/services/authorization-service';
import { RouterModule } from '@angular/router';
import { MaterialModule } from '../../shared/material.module';
import { TranslateModule } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';
import { ImageLoaderDirective } from '../../shared/directives/image-loader-directive';
import { GoogleSigninButtonModule } from '@abacritt/angularx-social-login';

@Component({
  selector: 'app-welcome',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MaterialModule,
    TranslateModule,
    ImageLoaderDirective,
    GoogleSigninButtonModule,
  ],
  templateUrl: './welcome.component.html',
})
export class WelcomeComponent {
  private authService = inject(AuthorizationService);
  isLoggedIn = this.authService.isAuthorized;
}

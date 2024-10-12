import { Component, inject } from '@angular/core';
import { AuthorizationService } from '../../features/authorization/services/authorization-service';
import { CommonModule } from '@angular/common';
import { GoogleSigninButtonModule } from '@abacritt/angularx-social-login';
import { TranslateModule } from '@ngx-translate/core';
import { RouterLink } from '@angular/router';
import { MaterialModule } from '../material.module';
import { ImageLoaderDirective } from '../../directives/image-loader-directive';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    GoogleSigninButtonModule,
    MaterialModule,
    ImageLoaderDirective,
    RouterLink,
    TranslateModule,
  ],
  templateUrl: './login.component.html',
})
export class LoginComponent {
  private authService = inject(AuthorizationService);

  userData = this.authService.userData;

  logout() {
    this.authService.logout();
  }
}

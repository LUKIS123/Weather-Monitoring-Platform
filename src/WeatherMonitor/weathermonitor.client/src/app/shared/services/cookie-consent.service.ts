import { inject, Injectable, signal } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root',
})
export class CookieConsentService {
  private readonly cookieService = inject(CookieService);

  #cookieAccepted = signal<boolean>(false);
  public cookieAccepted = this.#cookieAccepted.asReadonly();

  constructor() {
    this.#cookieAccepted.set(this.checkCookieConsent());
  }

  public checkCookieConsent() {
    const consent = this.cookieService.get('CookiePolicyAccepted');
    return consent === 'true';
  }

  public acceptCookiePolicy() {
    const expireDate = new Date();
    expireDate.setFullYear(expireDate.getFullYear() + 10);
    this.cookieService.set('CookiePolicyAccepted', 'true', expireDate);
    this.#cookieAccepted.set(true);
  }
}

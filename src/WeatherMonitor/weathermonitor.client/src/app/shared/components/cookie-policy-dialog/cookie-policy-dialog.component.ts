import { CommonModule } from '@angular/common';
import { Component, Inject, inject } from '@angular/core';
import { MaterialModule } from '../../material.module';
import { TranslateModule } from '@ngx-translate/core';
import { CookieConsentService } from '../../services/cookie-consent.service';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-cookie-policy-dialog',
  standalone: true,
  imports: [CommonModule, MaterialModule, TranslateModule],
  templateUrl: './cookie-policy-dialog.component.html',
})
export class CookiePolicyDialogComponent {
  private readonly cookiesConsentService = inject(CookieConsentService);

  constructor(
    @Inject(MAT_DIALOG_DATA) private data: object,
    private dialogRef: MatDialogRef<CookiePolicyDialogComponent>
  ) {}

  public acceptCookiePolicy() {
    this.cookiesConsentService.acceptCookiePolicy();
    if (this.cookiesConsentService.checkCookieConsent()) {
      this.dialogRef.close(true);
    }
  }
}

import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { UserSettingService } from './user-settings.service';
import { UserSettings } from './models/user-settings';
import { finalize } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ToastService } from '../../shared/services/toast.service';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../shared/material.module';
import { ImageLoaderDirective } from '../../shared/directives/image-loader-directive';
import { UserRole } from '../../shared/models/user-role';

@Component({
  selector: 'app-user-settings',
  standalone: true,
  imports: [
    CommonModule,
    MaterialModule,
    ImageLoaderDirective,
    TranslateModule,
  ],
  templateUrl: './user-settings.component.html',
})
export class UserSettingsComponent implements OnInit {
  private readonly userSettingsDataService = inject(UserSettingService);
  private readonly toastService = inject(ToastService);
  private readonly translateService = inject(TranslateService);

  #settings = signal<UserSettings | null>(null);
  public settings = this.#settings.asReadonly();

  roleName = computed(() =>
    this.getStatusLabel(this.#settings()?.role ?? null)
  );

  #isLoading = signal<boolean>(true);
  public readonly isLoading = this.#isLoading.asReadonly();

  public ngOnInit(): void {
    this.userSettingsDataService
      .getUserSettings()
      .pipe(finalize(() => this.#isLoading.set(false)))
      .subscribe({
        next: (settings) => {
          this.#settings.set(settings);
        },
        error: () => {
          this.toastService.openError(
            this.translateService.instant('UserAccount.Error')
          );
        },
      });
  }

  private getStatusLabel(status: UserRole | null): string {
    if (status == null) {
      return this.translateService.instant('UserAccount.Unknown');
    }

    switch (status) {
      case UserRole.User:
        return this.translateService.instant('UserAccount.User');
      case UserRole.Admin:
        return this.translateService.instant('UserAccount.Admin');
      default:
        return this.translateService.instant('UserAccount.Unknown');
    }
  }
}

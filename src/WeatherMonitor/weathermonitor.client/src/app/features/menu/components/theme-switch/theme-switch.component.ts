import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { MaterialModule } from '../../../../shared/material.module';
import { TranslateModule } from '@ngx-translate/core';
import { HttpClient } from '@angular/common/http';
import { ThemeService } from '../../services/theme.service';

@Component({
  selector: 'app-theme-switch',
  standalone: true,
  imports: [CommonModule, MaterialModule, TranslateModule],
  templateUrl: './theme-switch.component.html',
})
export class ThemeSwitchComponent implements OnInit {
  private readonly themeService = inject(ThemeService);
  private httpClient: HttpClient = inject(HttpClient);

  public readonly darkTheme = this.themeService.darkTheme;

  ngOnInit(): void {
    this.httpClient.get<boolean>('/api/user/dark-theme-status').subscribe({
      next: (darkTheme: boolean) => {
        if (darkTheme) {
          this.themeService.setDarkTheme();
        } else {
          this.themeService.setLightTheme();
        }
      },
    });
  }

  changeTheme() {
    this.httpClient
      .post('/api/user/set-dark-theme', !this.themeService.darkTheme())
      .subscribe({
        next: () => {
          if (this.themeService.darkTheme()) {
            this.themeService.setLightTheme();
          } else {
            this.themeService.setDarkTheme();
          }
        },
      });
  }
}

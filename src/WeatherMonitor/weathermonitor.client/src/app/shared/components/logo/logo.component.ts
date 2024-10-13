import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

import { RouterLink } from '@angular/router';
import { ImageLoaderDirective } from '../../directives/image-loader-directive';

@Component({
  selector: 'app-logo',
  standalone: true,
  imports: [CommonModule, ImageLoaderDirective, RouterLink],
  templateUrl: './logo.component.html',
})
export class LogoComponent {}

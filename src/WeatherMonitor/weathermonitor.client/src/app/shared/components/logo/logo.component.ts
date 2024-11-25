import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

import { ImageLoaderDirective } from '../../directives/image-loader-directive';

@Component({
  selector: 'app-logo',
  standalone: true,
  imports: [CommonModule, ImageLoaderDirective],
  templateUrl: './logo.component.html',
})
export class LogoComponent {}

import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { ImageLoaderDirective } from '../../directives/image-loader-directive';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-logo',
  standalone: true,
  imports: [CommonModule, ImageLoaderDirective, RouterLink],
  templateUrl: './logo.component.html',
})
export class LogoComponent {}

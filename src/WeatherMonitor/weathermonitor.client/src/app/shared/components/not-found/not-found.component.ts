import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './not-found.component.html',
})
export class NotFoundComponent {
  public readonly imageUrl =
    'https://i.kym-cdn.com/photos/images/original/002/738/959/060.gif';
}

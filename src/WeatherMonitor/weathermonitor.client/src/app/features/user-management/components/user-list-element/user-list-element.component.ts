import { Component, input } from '@angular/core';
import { User } from '../../models/user';

@Component({
  selector: 'app-user-list-element',
  standalone: true,
  imports: [],
  templateUrl: './user-list-element.component.html',
})
export class UserListElementComponent {
  user = input.required<User>();
}

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserPermissionsManagementComponent } from './user-permissions-management.component';

describe('UserPermissionsManagementComponent', () => {
  let component: UserPermissionsManagementComponent;
  let fixture: ComponentFixture<UserPermissionsManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserPermissionsManagementComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserPermissionsManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

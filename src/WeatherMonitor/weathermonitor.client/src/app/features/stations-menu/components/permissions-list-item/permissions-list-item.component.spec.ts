import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PermissionsListItemComponent } from './permissions-list-item.component';

describe('PermissionsListItemComponent', () => {
  let component: PermissionsListItemComponent;
  let fixture: ComponentFixture<PermissionsListItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PermissionsListItemComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PermissionsListItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CookiePolicyInfoComponent } from './cookie-policy-info.component';

describe('CookiePolicyInfoComponent', () => {
  let component: CookiePolicyInfoComponent;
  let fixture: ComponentFixture<CookiePolicyInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CookiePolicyInfoComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CookiePolicyInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LastWeekHourlyWeatherDataComponent } from './last-week-hourly-weather-data.component';

describe('LastWeekHourlyWeatherDataComponent', () => {
  let component: LastWeekHourlyWeatherDataComponent;
  let fixture: ComponentFixture<LastWeekHourlyWeatherDataComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LastWeekHourlyWeatherDataComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LastWeekHourlyWeatherDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

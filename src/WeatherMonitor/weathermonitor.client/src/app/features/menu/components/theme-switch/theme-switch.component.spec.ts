/* eslint-disable @typescript-eslint/no-unused-vars */
import { ComponentFixture, TestBed } from '@angular/core/testing';
import {
  HttpClientTestingModule,
  HttpTestingController,
} from '@angular/common/http/testing';
import { ThemeSwitchComponent } from './theme-switch.component';
import { ThemeService } from '../../services/theme.service';
import { TranslateService } from '@ngx-translate/core';

describe('ThemeSwitchComponent', () => {
  let component: ThemeSwitchComponent;
  let fixture: ComponentFixture<ThemeSwitchComponent>;
  let themeService: jasmine.SpyObj<ThemeService>;
  let translateService: jasmine.SpyObj<TranslateService>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    const themeServiceSpy = jasmine.createSpyObj('ThemeService', [
      'setDarkTheme',
      'setLightTheme',
      'darkTheme',
    ]);
    const translateServiceSpy = jasmine.createSpyObj('TranslateService', [
      'get',
      'setDefaultLang',
      'use',
    ]);

    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, ThemeSwitchComponent], // Importing the component directly
      providers: [
        { provide: ThemeService, useValue: themeServiceSpy },
        { provide: TranslateService, useValue: translateServiceSpy }, // Mocking TranslateService
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ThemeSwitchComponent);
    component = fixture.componentInstance;
    themeService = TestBed.inject(ThemeService) as jasmine.SpyObj<ThemeService>;
    translateService = TestBed.inject(
      TranslateService
    ) as jasmine.SpyObj<TranslateService>;
    httpMock = TestBed.inject(HttpTestingController);
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should call setDarkTheme if darkTheme status is true', () => {
    themeService.darkTheme.and.returnValue(false);

    // Simulate the API response
    component.ngOnInit();
    const req = httpMock.expectOne('/api/user/dark-theme-status');
    req.flush(true); // Respond with true for dark theme status

    expect(themeService.setDarkTheme).toHaveBeenCalled();
  });

  it('should call setLightTheme if darkTheme status is false', () => {
    themeService.darkTheme.and.returnValue(false);

    // Simulate the API response
    component.ngOnInit();
    const req = httpMock.expectOne('/api/user/dark-theme-status');
    req.flush(false); // Respond with false for dark theme status

    expect(themeService.setLightTheme).toHaveBeenCalled();
  });

  afterEach(() => {
    httpMock.verify();
  });
});

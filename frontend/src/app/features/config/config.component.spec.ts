import { TestBed, ComponentFixture, fakeAsync, tick } from '@angular/core/testing';
import { of } from 'rxjs';
import { ConfigComponent } from './config.component';
import { ApiConfigService } from '../../core/api-config.service';
import { AuthService } from '../../application/auth.service';

describe('ConfigComponent', () => {
  let fixture: ComponentFixture<ConfigComponent>;
  let component: ConfigComponent;
  let apiConfigSpy: jasmine.SpyObj<ApiConfigService>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    apiConfigSpy = jasmine.createSpyObj<ApiConfigService>('ApiConfigService', [
      'getApiUrl',
      'setApiUrl',
      'getToken',
      'setToken',
    ]);
    apiConfigSpy.getApiUrl.and.returnValue('https://api.initial.com');
    apiConfigSpy.getToken.and.returnValue('');

    authServiceSpy = jasmine.createSpyObj<AuthService>('AuthService', ['login']);
    authServiceSpy.login.and.returnValue(
      of({ sucesso: false, token: null, expiresAt: null, requer2FA: false, erro: null }),
    );

    await TestBed.configureTestingModule({
      imports: [ConfigComponent],
      providers: [
        { provide: ApiConfigService, useValue: apiConfigSpy },
        { provide: AuthService, useValue: authServiceSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize apiUrl from ApiConfigService', () => {
    expect(apiConfigSpy.getApiUrl).toHaveBeenCalled();
    expect(component.apiUrl).toBe('https://api.initial.com');
  });

  describe('save()', () => {
    it('should call ApiConfigService.setApiUrl with the current apiUrl', () => {
      component.apiUrl = 'https://api.new.com';
      component.save();
      expect(apiConfigSpy.setApiUrl).toHaveBeenCalledWith('https://api.new.com');
    });

    it('should set the confirmation message after save', () => {
      component.save();
      expect(component.message).toBe('Configurações salvas.');
    });

    it('should clear the message after 3 seconds', fakeAsync(() => {
      component.save();
      expect(component.message).toBe('Configurações salvas.');
      tick(3001);
      expect(component.message).toBe('');
    }));
  });
});

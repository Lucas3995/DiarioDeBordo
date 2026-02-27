import { TestBed } from '@angular/core/testing';
import { ComponentFixture } from '@angular/core/testing';
import { ConfigComponent } from './config.component';
import { ApiConfigService } from '../../core/api-config.service';

describe('ConfigComponent', () => {
  let fixture: ComponentFixture<ConfigComponent>;
  let component: ConfigComponent;
  let apiConfigSpy: jasmine.SpyObj<ApiConfigService>;

  beforeEach(async () => {
    apiConfigSpy = jasmine.createSpyObj<ApiConfigService>('ApiConfigService', [
      'getApiUrl',
      'setApiUrl',
    ]);
    apiConfigSpy.getApiUrl.and.returnValue('https://api.initial.com');

    await TestBed.configureTestingModule({
      imports: [ConfigComponent],
      providers: [{ provide: ApiConfigService, useValue: apiConfigSpy }],
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
      expect(component.message).toBe('URL salva.');
    });

    it('should clear the message after 3 seconds', (done) => {
      jasmine.clock().install();
      component.save();
      expect(component.message).toBe('URL salva.');
      jasmine.clock().tick(3001);
      expect(component.message).toBe('');
      jasmine.clock().uninstall();
      done();
    });
  });
});

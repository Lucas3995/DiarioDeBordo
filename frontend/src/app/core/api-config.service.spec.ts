import { TestBed } from '@angular/core/testing';
import { ApiConfigService } from './api-config.service';

const STORAGE_KEY = 'diariodebordo_api_url';
const STORAGE_KEY_TOKEN = 'diariodebordo_api_token';

describe('ApiConfigService', () => {
  let service: ApiConfigService;
  let storageSpy: { getItem: jasmine.Spy; setItem: jasmine.Spy; removeItem: jasmine.Spy };

  beforeEach(() => {
    storageSpy = {
      getItem: jasmine.createSpy('getItem').and.returnValue(null),
      setItem: jasmine.createSpy('setItem'),
      removeItem: jasmine.createSpy('removeItem'),
    };
    spyOnProperty(window, 'localStorage', 'get').and.returnValue(storageSpy as unknown as Storage);

    TestBed.configureTestingModule({});
    service = TestBed.inject(ApiConfigService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getApiUrl', () => {
    it('should return empty string when localStorage has no value', () => {
      storageSpy.getItem.and.returnValue(null);
      expect(service.getApiUrl()).toBe('');
    });

    it('should return the stored URL when localStorage has a value', () => {
      storageSpy.getItem.and.returnValue('https://api.example.com');
      expect(service.getApiUrl()).toBe('https://api.example.com');
      expect(storageSpy.getItem).toHaveBeenCalledWith(STORAGE_KEY);
    });
  });

  describe('setApiUrl', () => {
    it('should store the URL when a non-empty value is provided', () => {
      service.setApiUrl('https://api.example.com');
      expect(storageSpy.setItem).toHaveBeenCalledWith(STORAGE_KEY, 'https://api.example.com');
    });

    it('should trim whitespace before storing', () => {
      service.setApiUrl('  https://api.example.com  ');
      expect(storageSpy.setItem).toHaveBeenCalledWith(STORAGE_KEY, 'https://api.example.com');
    });

    it('should remove the key when an empty string is provided', () => {
      service.setApiUrl('');
      expect(storageSpy.removeItem).toHaveBeenCalledWith(STORAGE_KEY);
      expect(storageSpy.setItem).not.toHaveBeenCalled();
    });

    it('should remove the key when a whitespace-only string is provided', () => {
      service.setApiUrl('   ');
      expect(storageSpy.removeItem).toHaveBeenCalledWith(STORAGE_KEY);
      expect(storageSpy.setItem).not.toHaveBeenCalled();
    });
  });

  describe('getToken', () => {
    it('should return empty string when localStorage has no token', () => {
      storageSpy.getItem.and.returnValue(null);
      expect(service.getToken()).toBe('');
    });

    it('should return the stored token when localStorage has a value', () => {
      storageSpy.getItem.and.callFake((key: string) =>
        key === STORAGE_KEY_TOKEN ? 'meu-jwt-token' : null,
      );
      expect(service.getToken()).toBe('meu-jwt-token');
      expect(storageSpy.getItem).toHaveBeenCalledWith(STORAGE_KEY_TOKEN);
    });
  });

  describe('setToken', () => {
    it('should store the token when a non-empty value is provided', () => {
      service.setToken('meu-jwt-token');
      expect(storageSpy.setItem).toHaveBeenCalledWith(STORAGE_KEY_TOKEN, 'meu-jwt-token');
    });

    it('should remove the key when an empty string is provided', () => {
      service.setToken('');
      expect(storageSpy.removeItem).toHaveBeenCalledWith(STORAGE_KEY_TOKEN);
      expect(storageSpy.setItem).not.toHaveBeenCalled();
    });
  });
});

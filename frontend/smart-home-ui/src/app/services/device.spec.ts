import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

import { DeviceApiService } from './device.api.service';

describe('DeviceApiService (smoke)', () => {
  it('should be created', () => {
    TestBed.configureTestingModule({
      providers: [DeviceApiService, provideHttpClient(), provideHttpClientTesting()],
    });
    const service = TestBed.inject(DeviceApiService);
    expect(service).toBeTruthy();
  });
});

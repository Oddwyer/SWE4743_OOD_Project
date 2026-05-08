import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';

import { DeviceApiService } from './device.api.service';
import { RegisterDeviceRequest } from '../devicemodels/registerdevicerequest';
import { ControlDeviceRequest } from '../devicemodels/controldevicerequest';
import { DeviceResponse } from '../devicemodels/deviceresponse';
import { CommandHistoryResponse } from '../historymodels/commandhistoryresponse';

// ── Tests ──────────────────────────────────────────────────────────────────────

describe('DeviceApiService', () => {
  let service: DeviceApiService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        DeviceApiService,
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    });

    service = TestBed.inject(DeviceApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  // ── getAllDevices ──────────────────────────────────────────────────────────

  describe('getAllDevices', () => {
    it('sends a GET request to the devices endpoint', () => {
      service.getAllDevices().subscribe();
      const req = httpMock.expectOne((r) => r.url.endsWith('/devices') && r.method === 'GET');
      req.flush([]);
    });

    it('returns the device array from the response body', () => {
      const devices: Partial<DeviceResponse>[] = [
        { id: 'a', deviceName: 'Lamp', type: 'Light' },
        { id: 'b', deviceName: 'Fan', type: 'Fan' },
      ];
      let result: DeviceResponse[] = [];

      service.getAllDevices().subscribe((data) => (result = data));
      httpMock.expectOne((r) => r.url.endsWith('/devices')).flush(devices);

      expect(result).toEqual(devices);
    });

    it('propagates a 500 error to the subscriber', () => {
      let thrownError: { status: number } | null = null;

      service.getAllDevices().subscribe({ error: (err) => (thrownError = err) });
      httpMock
        .expectOne((r) => r.url.endsWith('/devices'))
        .flush('Server error', { status: 500, statusText: 'Internal Server Error' });

      expect(thrownError).not.toBeNull();
      expect(thrownError!.status).toBe(500);
    });

    it('propagates a 404 error to the subscriber', () => {
      let thrownError: { status: number } | null = null;

      service.getAllDevices().subscribe({ error: (err) => (thrownError = err) });
      httpMock
        .expectOne((r) => r.url.endsWith('/devices'))
        .flush('Not found', { status: 404, statusText: 'Not Found' });

      expect(thrownError!.status).toBe(404);
    });
  });

  // ── getDeviceById ──────────────────────────────────────────────────────────

  describe('getDeviceById', () => {
    it('sends a GET request to /devices/:id', () => {
      service.getDeviceById('abc-123').subscribe();
      const req = httpMock.expectOne(
        (r) => r.url.endsWith('/devices/abc-123') && r.method === 'GET',
      );
      req.flush({});
    });

    it('returns the device from the response body', () => {
      const device: Partial<DeviceResponse> = { id: 'abc-123', deviceName: 'Desk Lamp' };
      let result: DeviceResponse | null = null;

      service.getDeviceById('abc-123').subscribe((data) => (result = data));
      httpMock.expectOne((r) => r.url.endsWith('/devices/abc-123')).flush(device);

      expect(result).toEqual(device);
    });
  });

  // ── registerDevice ─────────────────────────────────────────────────────────

  describe('registerDevice', () => {
    it('sends a POST request to the devices endpoint', () => {
      const request: RegisterDeviceRequest = {
        deviceName: 'Desk Lamp',
        deviceLocation: 'Office',
        type: 'Light',
      };

      service.registerDevice(request).subscribe();
      const req = httpMock.expectOne((r) => r.url.endsWith('/devices') && r.method === 'POST');
      req.flush({});
    });

    it('sends the full request body', () => {
      const request: RegisterDeviceRequest = {
        deviceName: 'Ceiling Fan',
        deviceLocation: 'Bedroom',
        type: 'Fan',
      };

      service.registerDevice(request).subscribe();
      const req = httpMock.expectOne((r) => r.url.endsWith('/devices') && r.method === 'POST');

      expect(req.request.body).toEqual(request);
      req.flush({});
    });

    it('returns the created device from the response body', () => {
      const created: Partial<DeviceResponse> = { id: 'new-1', deviceName: 'Ceiling Fan' };
      let result: DeviceResponse | null = null;

      service
        .registerDevice({ deviceName: 'Ceiling Fan', deviceLocation: 'Bedroom', type: 'Fan' })
        .subscribe((data) => (result = data));
      httpMock.expectOne((r) => r.url.endsWith('/devices') && r.method === 'POST').flush(created);

      expect(result).toEqual(created);
    });

    it('propagates an error to the subscriber', () => {
      let thrownError: { status: number } | null = null;

      service
        .registerDevice({ deviceName: 'X', deviceLocation: 'Y', type: 'Light' })
        .subscribe({ error: (err) => (thrownError = err) });
      httpMock
        .expectOne((r) => r.url.endsWith('/devices') && r.method === 'POST')
        .flush('Bad Request', { status: 400, statusText: 'Bad Request' });

      expect(thrownError!.status).toBe(400);
    });
  });

  // ── removeDevice ───────────────────────────────────────────────────────────

  describe('removeDevice', () => {
    it('sends a DELETE request to /devices/:id', () => {
      service.removeDevice('dev-42').subscribe();
      const req = httpMock.expectOne(
        (r) => r.url.endsWith('/devices/dev-42') && r.method === 'DELETE',
      );
      req.flush(null);
    });

    it('completes without error on 204 No Content', () => {
      let completed = false;

      service.removeDevice('dev-42').subscribe({ complete: () => (completed = true) });
      httpMock
        .expectOne((r) => r.url.endsWith('/devices/dev-42') && r.method === 'DELETE')
        .flush(null, { status: 204, statusText: 'No Content' });

      expect(completed).toBe(true);
    });

    it('propagates a 404 error when the device is not found', () => {
      let thrownError: { status: number } | null = null;

      service.removeDevice('missing').subscribe({ error: (err) => (thrownError = err) });
      httpMock
        .expectOne((r) => r.url.endsWith('/devices/missing') && r.method === 'DELETE')
        .flush('Not Found', { status: 404, statusText: 'Not Found' });

      expect(thrownError!.status).toBe(404);
    });
  });

  // ── controlDevice ──────────────────────────────────────────────────────────

  describe('controlDevice', () => {
    it('sends a POST request to /devices/:id/commands', () => {
      const request: ControlDeviceRequest = { command: 'TogglePower' };

      service.controlDevice('dev-1', request).subscribe();
      const req = httpMock.expectOne(
        (r) => r.url.endsWith('/devices/dev-1/commands') && r.method === 'POST',
      );
      req.flush({});
    });

    it('sends the control request body', () => {
      const request: ControlDeviceRequest = { command: 'SetBrightness', brightness: 80 };

      service.controlDevice('dev-1', request).subscribe();
      const req = httpMock.expectOne(
        (r) => r.url.endsWith('/devices/dev-1/commands') && r.method === 'POST',
      );

      expect(req.request.body).toEqual(request);
      req.flush({});
    });

    it('sends SetFanSpeed with a fanSpeed body', () => {
      const request: ControlDeviceRequest = { command: 'SetFanSpeed', fanSpeed: 'High' };

      service.controlDevice('fan-1', request).subscribe();
      const req = httpMock.expectOne(
        (r) => r.url.endsWith('/devices/fan-1/commands') && r.method === 'POST',
      );

      expect(req.request.body).toEqual(request);
      req.flush({});
    });

    it('sends SetThermostatMode with a thermostatMode body', () => {
      const request: ControlDeviceRequest = { command: 'SetThermostatMode', thermostatMode: 'Heat' };

      service.controlDevice('thermo-1', request).subscribe();
      const req = httpMock.expectOne(
        (r) => r.url.endsWith('/devices/thermo-1/commands') && r.method === 'POST',
      );

      expect(req.request.body).toEqual(request);
      req.flush({});
    });

    it('sends SetTargetTemperature with a targetTemperature body', () => {
      const request: ControlDeviceRequest = { command: 'SetTargetTemperature', targetTemperature: 72 };

      service.controlDevice('thermo-1', request).subscribe();
      const req = httpMock.expectOne(
        (r) => r.url.endsWith('/devices/thermo-1/commands') && r.method === 'POST',
      );

      expect(req.request.body).toEqual(request);
      req.flush({});
    });

    it('sends ToggleLock with an isLocked body', () => {
      const request: ControlDeviceRequest = { command: 'ToggleLock', isLocked: true };

      service.controlDevice('door-1', request).subscribe();
      const req = httpMock.expectOne(
        (r) => r.url.endsWith('/devices/door-1/commands') && r.method === 'POST',
      );

      expect(req.request.body).toEqual(request);
      req.flush({});
    });

    it('returns the updated device from the response body', () => {
      const updated: Partial<DeviceResponse> = { id: 'dev-1', isPoweredOn: true };
      let result: DeviceResponse | null = null;

      service
        .controlDevice('dev-1', { command: 'TogglePower' })
        .subscribe((data) => (result = data));
      httpMock
        .expectOne((r) => r.url.endsWith('/devices/dev-1/commands') && r.method === 'POST')
        .flush(updated);

      expect(result).toEqual(updated);
    });

    it('propagates a 422 error to the subscriber', () => {
      let thrownError: { status: number } | null = null;

      service
        .controlDevice('dev-1', { command: 'TogglePower' })
        .subscribe({ error: (err) => (thrownError = err) });
      httpMock
        .expectOne((r) => r.url.endsWith('/devices/dev-1/commands') && r.method === 'POST')
        .flush('Unprocessable', { status: 422, statusText: 'Unprocessable Entity' });

      expect(thrownError!.status).toBe(422);
    });
  });

  // ── getCommandHistory ──────────────────────────────────────────────────────

  describe('getCommandHistory', () => {
    it('sends a GET request to /devices/:id/history', () => {
      service.getCommandHistory('dev-5').subscribe();
      const req = httpMock.expectOne(
        (r) => r.url.endsWith('/devices/dev-5/history') && r.method === 'GET',
      );
      req.flush([]);
    });

    it('returns command history entries from the response body', () => {
      const history: CommandHistoryResponse[] = [
        { id: 'h1', commandName: 'PowerOn', timestamp: '2024-01-01T10:00:00Z' },
        { id: 'h2', commandName: 'SetBrightness', timestamp: '2024-01-01T10:05:00Z' },
      ];
      let result: CommandHistoryResponse[] = [];

      service.getCommandHistory('dev-5').subscribe((data) => (result = data));
      httpMock
        .expectOne((r) => r.url.endsWith('/devices/dev-5/history') && r.method === 'GET')
        .flush(history);

      expect(result).toEqual(history);
    });

    it('returns an empty array when there is no history', () => {
      let result: CommandHistoryResponse[] | null = null;

      service.getCommandHistory('new-dev').subscribe((data) => (result = data));
      httpMock
        .expectOne((r) => r.url.endsWith('/devices/new-dev/history') && r.method === 'GET')
        .flush([]);

      expect(result).toEqual([]);
    });

    it('propagates a 404 error when the device is not found', () => {
      let thrownError: { status: number } | null = null;

      service.getCommandHistory('ghost').subscribe({ error: (err) => (thrownError = err) });
      httpMock
        .expectOne((r) => r.url.endsWith('/devices/ghost/history') && r.method === 'GET')
        .flush('Not Found', { status: 404, statusText: 'Not Found' });

      expect(thrownError!.status).toBe(404);
    });
  });
});

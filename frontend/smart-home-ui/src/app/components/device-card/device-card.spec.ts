import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { Observable, Subject, of, throwError } from 'rxjs';

import { DeviceCardComponent } from './device-card';
import { DeviceApiService } from '../../services/device.api.service';
import { DeviceResponse } from '../../devicemodels/deviceresponse';

// ── Test data factories ────────────────────────────────────────────────────────

function makeLightOn(overrides: Partial<DeviceResponse> = {}): DeviceResponse {
  return {
    id: 'light-1', deviceName: 'Desk Lamp', deviceLocation: 'Office',
    type: 'Light', isDeviceOn: true, isPoweredOn: true,
    createdAt: '', updatedAt: '',
    lightBrightness: 75, minBrightness: 10, maxBrightness: 100,
    lightColor: '#FF0000',
    ...overrides,
  };
}

function makeLightOff(): DeviceResponse {
  return makeLightOn({ isDeviceOn: false, isPoweredOn: false });
}

function makeFanOn(overrides: Partial<DeviceResponse> = {}): DeviceResponse {
  return {
    id: 'fan-1', deviceName: 'Ceiling Fan', deviceLocation: 'Bedroom',
    type: 'Fan', isDeviceOn: true, isPoweredOn: true,
    createdAt: '', updatedAt: '', fanSpeed: 'Medium',
    ...overrides,
  };
}

function makeFanOff(): DeviceResponse {
  return makeFanOn({ isDeviceOn: false, isPoweredOn: false });
}

function makeThermostat(
  state: 'Off' | 'Idle' | 'Heating' | 'Cooling',
  overrides: Partial<DeviceResponse> = {},
): DeviceResponse {
  return {
    id: 'thermo-1', deviceName: 'Nest', deviceLocation: 'Bedroom',
    type: 'Thermostat',
    isDeviceOn: state === 'Heating' || state === 'Cooling',
    isPoweredOn: state !== 'Off',
    createdAt: '', updatedAt: '',
    thermostatMode: 'Auto', thermostatState: state,
    targetTemperature: 72, defaultTemperature: 72,
    minTemperature: 60, maxTemperature: 80,
    ambientTemperature: 68,
    ...overrides,
  };
}

function makeDoorLock(locked = true): DeviceResponse {
  return {
    id: 'door-1', deviceName: 'Front Door', deviceLocation: 'Entrance',
    type: 'DoorLock', isDeviceOn: true, isPoweredOn: null,
    createdAt: '', updatedAt: '', isLocked: locked,
  };
}

// ── Shared setup ───────────────────────────────────────────────────────────────

type ApiSpy = {
  controlDevice: ReturnType<typeof vi.fn>;
  getAllDevices: ReturnType<typeof vi.fn>;
  registerDevice: ReturnType<typeof vi.fn>;
  removeDevice: ReturnType<typeof vi.fn>;
  getCommandHistory: ReturnType<typeof vi.fn>;
};

describe('DeviceCardComponent', () => {
  let fixture: ComponentFixture<DeviceCardComponent>;
  let component: DeviceCardComponent;
  let apiSpy: ApiSpy;

  beforeEach(async () => {
    apiSpy = {
      controlDevice: vi.fn(),
      getAllDevices: vi.fn(),
      registerDevice: vi.fn(),
      removeDevice: vi.fn(),
      getCommandHistory: vi.fn(),
    };

    await TestBed.configureTestingModule({
      imports: [DeviceCardComponent],
      providers: [
        { provide: DeviceApiService, useValue: apiSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(DeviceCardComponent);
    component = fixture.componentInstance;
  });

  afterEach(() => fixture.destroy());

  /** Initialise the component with a device; wire a default API return value. */
  function init(
    device: DeviceResponse,
    controlReturn: Observable<DeviceResponse> = of(device),
  ): void {
    apiSpy.controlDevice.mockReturnValue(controlReturn);
    fixture.componentRef.setInput('device', device);
    fixture.detectChanges();
  }

  // ── getDeviceIcon() ──────────────────────────────────────────────────────────

  describe('getDeviceIcon()', () => {
    beforeEach(() => init(makeLightOff()));

    it('returns emoji_objects for Light',    () => expect(component.getDeviceIcon('Light')).toBe('emoji_objects'));
    it('returns mode_fan for Fan',           () => expect(component.getDeviceIcon('Fan')).toBe('mode_fan'));
    it('returns thermostat for Thermostat',  () => expect(component.getDeviceIcon('Thermostat')).toBe('thermostat'));
    it('returns sensor_door for DoorLock',   () => expect(component.getDeviceIcon('DoorLock')).toBe('sensor_door'));
    it('returns devices for unknown type',   () => expect(component.getDeviceIcon('Unknown')).toBe('devices'));
  });

  // ── canTogglePower() / canToggleLatch() ──────────────────────────────────────

  describe('canTogglePower()', () => {
    it('is true for Light',      () => { init(makeLightOff());      expect(component.canTogglePower()).toBe(true); });
    it('is true for Fan',        () => { init(makeFanOff());        expect(component.canTogglePower()).toBe(true); });
    it('is true for Thermostat', () => { init(makeThermostat('Off')); expect(component.canTogglePower()).toBe(true); });
    it('is false for DoorLock',  () => { init(makeDoorLock());      expect(component.canTogglePower()).toBe(false); });
  });

  describe('canToggleLatch()', () => {
    it('is true for DoorLock', () => { init(makeDoorLock()); expect(component.canToggleLatch()).toBe(true); });
    it('is false for Light',   () => { init(makeLightOn()); expect(component.canToggleLatch()).toBe(false); });
    it('is false for Fan',     () => { init(makeFanOn());   expect(component.canToggleLatch()).toBe(false); });
  });

  // ── getDeviceStatus() ────────────────────────────────────────────────────────

  describe('getDeviceStatus()', () => {
    it('returns ON when DoorLock is locked',       () => { init(makeDoorLock(true));     expect(component.getDeviceStatus()).toBe('ON'); });
    it('returns OFF when DoorLock is unlocked',    () => { init(makeDoorLock(false));    expect(component.getDeviceStatus()).toBe('OFF'); });
    it('returns ON when Thermostat isPoweredOn',   () => { init(makeThermostat('Idle')); expect(component.getDeviceStatus()).toBe('ON'); });
    it('returns OFF when Thermostat is off',       () => { init(makeThermostat('Off'));  expect(component.getDeviceStatus()).toBe('OFF'); });
    it('returns ON when Light isDeviceOn',         () => { init(makeLightOn());          expect(component.getDeviceStatus()).toBe('ON'); });
    it('returns OFF when Light isDeviceOn is false', () => { init(makeLightOff());       expect(component.getDeviceStatus()).toBe('OFF'); });
  });

  // ── Light card — ON ──────────────────────────────────────────────────────────

  describe('Light card — ON', () => {
    beforeEach(() => init(makeLightOn()));

    it('shows the brightness row', () =>
      expect(fixture.debugElement.query(By.css('.brightness-row'))).toBeTruthy());

    it('brightness row is not disabled', () => {
      const row = fixture.debugElement.query(By.css('.brightness-row'));
      expect(row.nativeElement.classList).not.toContain('control-disabled');
    });

    it('shows the color row', () =>
      expect(fixture.debugElement.query(By.css('.color-row'))).toBeTruthy());

    it('color row is not disabled', () => {
      const row = fixture.debugElement.query(By.css('.color-row'));
      expect(row.nativeElement.classList).not.toContain('control-disabled');
    });

    it('power button has the power-on class', () =>
      expect(fixture.debugElement.query(By.css('.power-on'))).toBeTruthy());

    it('no latch button is shown', () =>
      expect(fixture.debugElement.query(By.css('.latch-button'))).toBeNull());
  });

  // ── Light card — OFF ─────────────────────────────────────────────────────────

  describe('Light card — OFF', () => {
    beforeEach(() => init(makeLightOff()));

    it('brightness row is still visible', () =>
      expect(fixture.debugElement.query(By.css('.brightness-row'))).toBeTruthy());

    it('brightness row carries control-disabled', () => {
      const row = fixture.debugElement.query(By.css('.brightness-row'));
      expect(row.nativeElement.classList).toContain('control-disabled');
    });

    it('color row carries control-disabled', () => {
      const row = fixture.debugElement.query(By.css('.color-row'));
      expect(row.nativeElement.classList).toContain('control-disabled');
    });

    it('power button has the power-off class', () =>
      expect(fixture.debugElement.query(By.css('.power-off'))).toBeTruthy());
  });

  // ── Fan card — ON ────────────────────────────────────────────────────────────

  describe('Fan card — ON', () => {
    beforeEach(() => init(makeFanOn()));

    it('shows the speed row', () =>
      expect(fixture.debugElement.query(By.css('.speed-row'))).toBeTruthy());

    it('speed row is not disabled', () => {
      const row = fixture.debugElement.query(By.css('.speed-row'));
      expect(row.nativeElement.classList).not.toContain('control-disabled');
    });

    it('renders three speed buttons (Low / Med / High)', () => {
      const buttons = fixture.debugElement.queryAll(By.css('.fan-speed-button'));
      expect(buttons.length).toBe(3);
    });

    it('all speed buttons are enabled', () => {
      fixture.debugElement.queryAll(By.css('.fan-speed-button')).forEach((b) =>
        expect((b.nativeElement as HTMLButtonElement).disabled).toBe(false),
      );
    });

    it('highlights the current speed (Medium → "Med" label)', () => {
      const selected = fixture.debugElement.queryAll(By.css('.fan-speed-button.selected'));
      expect(selected.length).toBe(1);
      expect(selected[0].nativeElement.textContent.trim()).toBe('Med');
    });
  });

  // ── Fan card — OFF ───────────────────────────────────────────────────────────

  describe('Fan card — OFF', () => {
    beforeEach(() => init(makeFanOff()));

    it('speed row carries control-disabled', () => {
      const row = fixture.debugElement.query(By.css('.speed-row'));
      expect(row.nativeElement.classList).toContain('control-disabled');
    });

    it('all speed buttons are disabled', () => {
      fixture.debugElement.queryAll(By.css('.fan-speed-button')).forEach((b) =>
        expect((b.nativeElement as HTMLButtonElement).disabled).toBe(true),
      );
    });
  });

  // ── Thermostat card — Idle ───────────────────────────────────────────────────

  describe('Thermostat card — Idle (powered on)', () => {
    beforeEach(() => init(makeThermostat('Idle')));

    it('shows the thermostat mode row', () =>
      expect(fixture.debugElement.query(By.css('.thermostat-mode-row'))).toBeTruthy());

    it('mode row is not disabled', () => {
      const row = fixture.debugElement.query(By.css('.thermostat-mode-row'));
      expect(row.nativeElement.classList).not.toContain('control-disabled');
    });

    it('renders Cool / Auto / Heat mode buttons in that order', () => {
      const labels = fixture.debugElement
        .queryAll(By.css('.thermostat-mode-button'))
        .map((b) => b.nativeElement.textContent.trim());
      expect(labels).toEqual(['Cool', 'Auto', 'Heat']);
    });

    it('highlights the current mode (Auto)', () => {
      const selected = fixture.debugElement.queryAll(By.css('.thermostat-mode-button.selected'));
      expect(selected.length).toBe(1);
      expect(selected[0].nativeElement.textContent.trim()).toBe('Auto');
    });

    it('all mode buttons are enabled', () => {
      fixture.debugElement.queryAll(By.css('.thermostat-mode-button')).forEach((b) =>
        expect((b.nativeElement as HTMLButtonElement).disabled).toBe(false),
      );
    });

    it('shows the target temperature row', () =>
      expect(fixture.debugElement.query(By.css('.temperature-row'))).toBeTruthy());

    it('temperature row is not disabled', () => {
      const row = fixture.debugElement.query(By.css('.temperature-row'));
      expect(row.nativeElement.classList).not.toContain('control-disabled');
    });
  });

  describe('Thermostat card — Off', () => {
    beforeEach(() => init(makeThermostat('Off')));

    it('mode row carries control-disabled', () => {
      const row = fixture.debugElement.query(By.css('.thermostat-mode-row'));
      expect(row.nativeElement.classList).toContain('control-disabled');
    });

    it('all mode buttons are disabled', () => {
      fixture.debugElement.queryAll(By.css('.thermostat-mode-button')).forEach((b) =>
        expect((b.nativeElement as HTMLButtonElement).disabled).toBe(true),
      );
    });

    it('temperature row carries control-disabled', () => {
      const row = fixture.debugElement.query(By.css('.temperature-row'));
      expect(row.nativeElement.classList).toContain('control-disabled');
    });
  });

  describe('Thermostat card — Heating', () => {
    it('isThermostatPoweredOn() is true', () => {
      init(makeThermostat('Heating'));
      expect(component.isThermostatPoweredOn()).toBe(true);
    });
  });

  describe('Thermostat card — Cooling', () => {
    it('isThermostatPoweredOn() is true', () => {
      init(makeThermostat('Cooling'));
      expect(component.isThermostatPoweredOn()).toBe(true);
    });
  });

  // ── DoorLock card — Locked ───────────────────────────────────────────────────

  describe('DoorLock card — Locked', () => {
    beforeEach(() => init(makeDoorLock(true)));

    it('shows the lock toggle row', () =>
      expect(fixture.debugElement.query(By.css('.power-row'))).toBeTruthy());

    it('does not render a power toggle button', () =>
      expect(fixture.debugElement.query(By.css('.power-on, .power-off'))).toBeNull());

    it('shows the pi-lock icon on the latch button', () =>
      expect(fixture.debugElement.query(By.css('.pi-lock'))).toBeTruthy());

    it('latch button carries latch-on class', () =>
      expect(fixture.debugElement.query(By.css('.latch-on'))).toBeTruthy());

    it('shows "Locked" in the card body', () => {
      const text = (fixture.nativeElement as HTMLElement).textContent;
      expect(text).toContain('Locked');
    });
  });

  // ── DoorLock card — Unlocked ─────────────────────────────────────────────────

  describe('DoorLock card — Unlocked', () => {
    beforeEach(() => init(makeDoorLock(false)));

    it('shows the pi-lock-open icon', () =>
      expect(fixture.debugElement.query(By.css('.pi-lock-open'))).toBeTruthy());

    it('latch button carries latch-off class', () =>
      expect(fixture.debugElement.query(By.css('.latch-off'))).toBeTruthy());

    it('shows "Unlocked" in the card body', () => {
      const text = (fixture.nativeElement as HTMLElement).textContent;
      expect(text).toContain('Unlocked');
    });
  });

  // ── toggleDevicePower() ──────────────────────────────────────────────────────

  describe('toggleDevicePower()', () => {
    it('calls controlDevice with TogglePower', () => {
      const device = makeLightOff();
      init(device, of({ ...device, isPoweredOn: true, isDeviceOn: true }));
      component.toggleDevicePower();
      expect(apiSpy.controlDevice).toHaveBeenCalledWith('light-1', { command: 'TogglePower' });
    });

    it('optimistically flips isPoweredOn before the API responds', () => {
      const device = makeLightOff();
      const pending = new Subject<DeviceResponse>();
      init(device, pending.asObservable());
      component.toggleDevicePower();
      expect(component.device.isPoweredOn).toBe(true);
    });

    it('reverts isPoweredOn when the API returns an error', () => {
      const device = makeLightOff();
      init(device, throwError(() => new Error('network')));
      component.toggleDevicePower();
      expect(component.device.isPoweredOn).toBe(false);
    });

    it('replaces the device with the API response on success', () => {
      const device = makeLightOff();
      const updated = { ...device, isPoweredOn: true, isDeviceOn: true };
      init(device, of(updated));
      component.toggleDevicePower();
      expect(component.device.isPoweredOn).toBe(true);
      expect(component.device.isDeviceOn).toBe(true);
    });
  });

  // ── toggleLatch() ─────────────────────────────────────────────────────────────

  describe('toggleLatch()', () => {
    it('calls controlDevice with ToggleLock', () => {
      const device = makeDoorLock(true);
      init(device, of({ ...device, isLocked: false }));
      component.toggleLatch();
      expect(apiSpy.controlDevice).toHaveBeenCalledWith('door-1', { command: 'ToggleLock' });
    });

    it('optimistically flips isLocked before the API responds', () => {
      const device = makeDoorLock(true);
      init(device, new Subject<DeviceResponse>().asObservable());
      component.toggleLatch();
      expect(component.device.isLocked).toBe(false);
    });

    it('reverts isLocked on API error', () => {
      const device = makeDoorLock(true);
      init(device, throwError(() => new Error('error')));
      component.toggleLatch();
      expect(component.device.isLocked).toBe(true);
    });
  });

  // ── setBrightness() ──────────────────────────────────────────────────────────

  describe('setBrightness()', () => {
    it('calls controlDevice with SetBrightness and the value', () => {
      init(makeLightOn(), of(makeLightOn({ lightBrightness: 80 })));
      component.setBrightness(80);
      expect(apiSpy.controlDevice).toHaveBeenCalledWith('light-1', { command: 'SetBrightness', brightness: 80 });
    });

    it('optimistically sets brightness before the API responds', () => {
      init(makeLightOn(), new Subject<DeviceResponse>().asObservable());
      component.setBrightness(90);
      expect(component.device.lightBrightness).toBe(90);
    });

    it('reverts brightness to its previous value on API error', () => {
      const device = makeLightOn();
      const original = device.lightBrightness;
      init(device, throwError(() => new Error('error')));
      component.setBrightness(90);
      expect(component.device.lightBrightness).toBe(original);
    });

    it('slider min/max are wired to device.minBrightness / maxBrightness (10–100)', () => {
      init(makeLightOn());
      expect(component.device.minBrightness).toBe(10);
      expect(component.device.maxBrightness).toBe(100);
    });
  });

  // ── setColor() ───────────────────────────────────────────────────────────────

  describe('setColor()', () => {
    it('sends the color unchanged when it already has a # prefix', () => {
      init(makeLightOn());
      component.setColor('#00FF00');
      expect(apiSpy.controlDevice).toHaveBeenCalledWith('light-1', { command: 'SetColor', color: '#00FF00' });
    });

    it('prepends # when the color string has no prefix', () => {
      init(makeLightOn());
      component.setColor('AABBCC');
      expect(apiSpy.controlDevice).toHaveBeenCalledWith('light-1', { command: 'SetColor', color: '#AABBCC' });
    });

    it('reverts lightColor on API error', () => {
      const device = makeLightOn();
      const original = device.lightColor;
      init(device, throwError(() => new Error('error')));
      component.setColor('#DEADBE');
      expect(component.device.lightColor).toBe(original);
    });
  });

  // ── setFanSpeed() ────────────────────────────────────────────────────────────

  describe('setFanSpeed()', () => {
    it('calls controlDevice with SetFanSpeed and the chosen speed', () => {
      init(makeFanOn(), of(makeFanOn({ fanSpeed: 'High' })));
      component.setFanSpeed('High');
      expect(apiSpy.controlDevice).toHaveBeenCalledWith('fan-1', { command: 'SetFanSpeed', fanSpeed: 'High' });
    });

    it('optimistically sets fanSpeed before the API responds', () => {
      init(makeFanOn(), new Subject<DeviceResponse>().asObservable());
      component.setFanSpeed('High');
      expect(component.device.fanSpeed).toBe('High');
    });

    it('reverts fanSpeed on API error', () => {
      const device = makeFanOn();
      const original = device.fanSpeed;
      init(device, throwError(() => new Error('error')));
      component.setFanSpeed('High');
      expect(component.device.fanSpeed).toBe(original);
    });
  });

  // ── setThermostatMode() ──────────────────────────────────────────────────────

  describe('setThermostatMode()', () => {
    it('calls controlDevice with SetThermostatMode and the new mode', () => {
      init(makeThermostat('Idle'), of(makeThermostat('Idle', { thermostatMode: 'Heat' })));
      component.setThermostatMode('Heat');
      expect(apiSpy.controlDevice).toHaveBeenCalledWith('thermo-1', { command: 'SetThermostatMode', thermostatMode: 'Heat' });
    });

    it('reverts thermostatMode on API error', () => {
      const device = makeThermostat('Idle');
      const original = device.thermostatMode;
      init(device, throwError(() => new Error('error')));
      component.setThermostatMode('Cool');
      expect(component.device.thermostatMode).toBe(original);
    });
  });

  // ── setTargetTemperature() ───────────────────────────────────────────────────

  describe('setTargetTemperature()', () => {
    it('calls controlDevice with SetTargetTemperature and the value', () => {
      init(makeThermostat('Idle'), of(makeThermostat('Idle', { targetTemperature: 75 })));
      component.setTargetTemperature(75);
      expect(apiSpy.controlDevice).toHaveBeenCalledWith('thermo-1', { command: 'SetTargetTemperature', targetTemperature: 75 });
    });

    it('optimistically updates targetTemperature before the API responds', () => {
      init(makeThermostat('Idle'), new Subject<DeviceResponse>().asObservable());
      component.setTargetTemperature(78);
      expect(component.device.targetTemperature).toBe(78);
    });

    it('reverts targetTemperature on API error', () => {
      const device = makeThermostat('Idle');
      const original = device.targetTemperature;
      init(device, throwError(() => new Error('error')));
      component.setTargetTemperature(78);
      expect(component.device.targetTemperature).toBe(original);
    });

    it('slider min/max are wired to device temperature bounds (60–80)', () => {
      init(makeThermostat('Idle'));
      expect(component.device.minTemperature).toBe(60);
      expect(component.device.maxTemperature).toBe(80);
    });
  });
});

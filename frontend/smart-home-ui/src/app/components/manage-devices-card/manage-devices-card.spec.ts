import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { of, throwError } from 'rxjs';

import { ManageDevicesCardComponent } from './manage-devices-card';
import { DeviceApiService } from '../../services/device.api.service';
import { DeviceResponse } from '../../devicemodels/deviceresponse';

// ── Test data factory ──────────────────────────────────────────────────────────

function makeDevice(overrides: Partial<DeviceResponse> = {}): DeviceResponse {
  return {
    id: 'd1',
    deviceName: 'Test Device',
    deviceLocation: 'Room',
    type: 'Light',
    isDeviceOn: true,
    isPoweredOn: true,
    createdAt: '',
    updatedAt: '',
    ...overrides,
  };
}

// ── Spy type ───────────────────────────────────────────────────────────────────

type ApiSpy = {
  getAllDevices: ReturnType<typeof vi.fn>;
  controlDevice: ReturnType<typeof vi.fn>;
  registerDevice: ReturnType<typeof vi.fn>;
  removeDevice: ReturnType<typeof vi.fn>;
  getCommandHistory: ReturnType<typeof vi.fn>;
};

// ── Tests ──────────────────────────────────────────────────────────────────────

describe('ManageDevicesCardComponent', () => {
  let fixture: ComponentFixture<ManageDevicesCardComponent>;
  let component: ManageDevicesCardComponent;
  let apiSpy: ApiSpy;

  beforeEach(async () => {
    apiSpy = {
      getAllDevices: vi.fn().mockReturnValue(of([])),
      controlDevice: vi.fn(),
      registerDevice: vi.fn().mockReturnValue(of(makeDevice())),
      removeDevice: vi.fn().mockReturnValue(of(undefined)),
      getCommandHistory: vi.fn().mockReturnValue(of([])),
    };

    await TestBed.configureTestingModule({
      imports: [ManageDevicesCardComponent],
      providers: [{ provide: DeviceApiService, useValue: apiSpy }],
    }).compileComponents();

    fixture = TestBed.createComponent(ManageDevicesCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // ── openAddDeviceModal ─────────────────────────────────────────────────────

  describe('openAddDeviceModal', () => {
    it('sets showAddDeviceModal to true', () => {
      component.openAddDeviceModal();
      expect(component.showAddDeviceModal).toBe(true);
    });

    it('clears newDeviceName', () => {
      component.newDeviceName = 'Old Name';
      component.openAddDeviceModal();
      expect(component.newDeviceName).toBe('');
    });

    it('clears newDeviceLocation', () => {
      component.newDeviceLocation = 'Old Location';
      component.openAddDeviceModal();
      expect(component.newDeviceLocation).toBe('');
    });

    it('resets selectedDeviceType to "Light"', () => {
      component.selectedDeviceType = 'Fan';
      component.openAddDeviceModal();
      expect(component.selectedDeviceType).toBe('Light');
    });
  });

  // ── openRemoveDeviceModal ──────────────────────────────────────────────────

  describe('openRemoveDeviceModal', () => {
    it('sets showRemoveDeviceModal to true', () => {
      component.openRemoveDeviceModal();
      expect(component.showRemoveDeviceModal).toBe(true);
    });

    it('clears selectedRemoveDeviceId', () => {
      component.selectedRemoveDeviceId = 'dev-999';
      component.openRemoveDeviceModal();
      expect(component.selectedRemoveDeviceId).toBe('');
    });
  });

  // ── openHistoryDeviceModal ─────────────────────────────────────────────────

  describe('openHistoryDeviceModal', () => {
    it('sets showHistoryDeviceModal to true', () => {
      component.openHistoryDeviceModal();
      expect(component.showHistoryDeviceModal).toBe(true);
    });

    it('clears historyEntries', () => {
      component.historyEntries = [{ id: 'h1', commandName: 'PowerOn', timestamp: '' }];
      component.openHistoryDeviceModal();
      expect(component.historyEntries.length).toBe(0);
    });

    it('resets hasLoadedHistory to false', () => {
      component.hasLoadedHistory = true;
      component.openHistoryDeviceModal();
      expect(component.hasLoadedHistory).toBe(false);
    });

    it('resets isLoadingHistory to false', () => {
      component.isLoadingHistory = true;
      component.openHistoryDeviceModal();
      expect(component.isLoadingHistory).toBe(false);
    });
  });

  // ── addDevice ──────────────────────────────────────────────────────────────

  describe('addDevice', () => {
    it('calls registerDevice with trimmed name and location', () => {
      component.newDeviceName = '  Desk Lamp  ';
      component.newDeviceLocation = '  Kitchen  ';
      component.selectedDeviceType = 'Light';
      component.addDevice();
      expect(apiSpy.registerDevice).toHaveBeenCalledWith({
        deviceName: 'Desk Lamp',
        deviceLocation: 'Kitchen',
        type: 'Light',
      });
    });

    it('calls registerDevice with the selected device type', () => {
      component.newDeviceName = 'Ceiling Fan';
      component.newDeviceLocation = 'Bedroom';
      component.selectedDeviceType = 'Fan';
      component.addDevice();
      expect(apiSpy.registerDevice).toHaveBeenCalledWith({
        deviceName: 'Ceiling Fan',
        deviceLocation: 'Bedroom',
        type: 'Fan',
      });
    });

    it('converts an all-lowercase device name to Title Case', () => {
      component.newDeviceName = 'desk lamp';
      component.newDeviceLocation = 'Office';
      component.addDevice();
      expect(apiSpy.registerDevice).toHaveBeenCalledWith(
        expect.objectContaining({ deviceName: 'Desk Lamp' }),
      );
    });

    it('converts an all-lowercase location to Title Case', () => {
      component.newDeviceName = 'Lamp';
      component.newDeviceLocation = 'living room';
      component.addDevice();
      expect(apiSpy.registerDevice).toHaveBeenCalledWith(
        expect.objectContaining({ deviceLocation: 'Living Room' }),
      );
    });

    it('normalises ALLCAPS input to Title Case', () => {
      component.newDeviceName = 'CEILING FAN';
      component.newDeviceLocation = 'BEDROOM';
      component.addDevice();
      expect(apiSpy.registerDevice).toHaveBeenCalledWith(
        expect.objectContaining({ deviceName: 'Ceiling Fan', deviceLocation: 'Bedroom' }),
      );
    });

    it('preserves already-correct Title Case unchanged', () => {
      component.newDeviceName = 'Front Door Lock';
      component.newDeviceLocation = 'Entryway';
      component.addDevice();
      expect(apiSpy.registerDevice).toHaveBeenCalledWith(
        expect.objectContaining({ deviceName: 'Front Door Lock', deviceLocation: 'Entryway' }),
      );
    });

    it('closes the modal on success', () => {
      component.showAddDeviceModal = true;
      component.newDeviceName = 'Lamp';
      component.newDeviceLocation = 'Kitchen';
      component.addDevice();
      expect(component.showAddDeviceModal).toBe(false);
    });

    it('resets the form fields on success', () => {
      component.newDeviceName = 'Lamp';
      component.newDeviceLocation = 'Kitchen';
      component.selectedDeviceType = 'Fan';
      component.addDevice();
      expect(component.newDeviceName).toBe('');
      expect(component.newDeviceLocation).toBe('');
      expect(component.selectedDeviceType).toBe('Light');
    });

    it('emits devicesChanged on success', () => {
      const emitSpy = vi.fn();
      component.devicesChanged.subscribe(emitSpy);
      component.newDeviceName = 'Lamp';
      component.newDeviceLocation = 'Kitchen';
      component.addDevice();
      expect(emitSpy).toHaveBeenCalledTimes(1);
    });

    it('keeps the modal open on API error', () => {
      apiSpy.registerDevice.mockReturnValue(throwError(() => new Error('fail')));
      component.showAddDeviceModal = true;
      component.newDeviceName = 'Lamp';
      component.newDeviceLocation = 'Kitchen';
      component.addDevice();
      expect(component.showAddDeviceModal).toBe(true);
    });

    it('does not emit devicesChanged on API error', () => {
      apiSpy.registerDevice.mockReturnValue(throwError(() => new Error('fail')));
      const emitSpy = vi.fn();
      component.devicesChanged.subscribe(emitSpy);
      component.newDeviceName = 'Lamp';
      component.newDeviceLocation = 'Kitchen';
      component.addDevice();
      expect(emitSpy).not.toHaveBeenCalled();
    });
  });

  // ── removeDevice ───────────────────────────────────────────────────────────

  describe('removeDevice', () => {
    it('is a no-op when selectedRemoveDeviceId is empty', () => {
      component.selectedRemoveDeviceId = '';
      component.removeDevice();
      expect(apiSpy.removeDevice).not.toHaveBeenCalled();
    });

    it('calls removeDevice with the selected device ID', () => {
      component.selectedRemoveDeviceId = 'dev-abc';
      component.removeDevice();
      expect(apiSpy.removeDevice).toHaveBeenCalledWith('dev-abc');
    });

    it('closes the modal on success', () => {
      component.selectedRemoveDeviceId = 'dev-abc';
      component.showRemoveDeviceModal = true;
      component.removeDevice();
      expect(component.showRemoveDeviceModal).toBe(false);
    });

    it('clears selectedRemoveDeviceId on success', () => {
      component.selectedRemoveDeviceId = 'dev-abc';
      component.removeDevice();
      expect(component.selectedRemoveDeviceId).toBe('');
    });

    it('emits devicesChanged on success', () => {
      const emitSpy = vi.fn();
      component.devicesChanged.subscribe(emitSpy);
      component.selectedRemoveDeviceId = 'dev-abc';
      component.removeDevice();
      expect(emitSpy).toHaveBeenCalledTimes(1);
    });

    it('keeps the modal open on API error', () => {
      apiSpy.removeDevice.mockReturnValue(throwError(() => new Error('fail')));
      component.selectedRemoveDeviceId = 'dev-abc';
      component.showRemoveDeviceModal = true;
      component.removeDevice();
      expect(component.showRemoveDeviceModal).toBe(true);
    });

    it('does not emit devicesChanged on API error', () => {
      apiSpy.removeDevice.mockReturnValue(throwError(() => new Error('fail')));
      const emitSpy = vi.fn();
      component.devicesChanged.subscribe(emitSpy);
      component.selectedRemoveDeviceId = 'dev-abc';
      component.removeDevice();
      expect(emitSpy).not.toHaveBeenCalled();
    });
  });

  // ── loadDeviceHistory ──────────────────────────────────────────────────────

  describe('loadDeviceHistory', () => {
    it('is a no-op when selectedHistoryDeviceId is empty', () => {
      component.selectedHistoryDeviceId = '';
      component.loadDeviceHistory();
      expect(apiSpy.getCommandHistory).not.toHaveBeenCalled();
    });

    it('calls getCommandHistory with the selected device ID', () => {
      component.selectedHistoryDeviceId = 'dev-xyz';
      component.loadDeviceHistory();
      expect(apiSpy.getCommandHistory).toHaveBeenCalledWith('dev-xyz');
    });

    it('populates historyEntries on success', () => {
      const entries = [
        { id: 'h1', commandName: 'PowerOn', timestamp: '2024-01-01T00:00:00Z' },
        { id: 'h2', commandName: 'SetBrightness', timestamp: '2024-01-02T00:00:00Z' },
      ];
      apiSpy.getCommandHistory.mockReturnValue(of(entries));
      component.selectedHistoryDeviceId = 'dev-xyz';
      component.loadDeviceHistory();
      expect(component.historyEntries).toEqual(entries);
    });

    it('sets hasLoadedHistory to true on success', () => {
      component.selectedHistoryDeviceId = 'dev-xyz';
      component.loadDeviceHistory();
      expect(component.hasLoadedHistory).toBe(true);
    });

    it('sets isLoadingHistory to false on success', () => {
      component.selectedHistoryDeviceId = 'dev-xyz';
      component.loadDeviceHistory();
      expect(component.isLoadingHistory).toBe(false);
    });

    it('sets hasLoadedHistory to true on error', () => {
      apiSpy.getCommandHistory.mockReturnValue(throwError(() => new Error('fail')));
      component.selectedHistoryDeviceId = 'dev-xyz';
      component.loadDeviceHistory();
      expect(component.hasLoadedHistory).toBe(true);
    });

    it('sets isLoadingHistory to false on error', () => {
      apiSpy.getCommandHistory.mockReturnValue(throwError(() => new Error('fail')));
      component.selectedHistoryDeviceId = 'dev-xyz';
      component.loadDeviceHistory();
      expect(component.isLoadingHistory).toBe(false);
    });

    it('leaves historyEntries empty on error', () => {
      apiSpy.getCommandHistory.mockReturnValue(throwError(() => new Error('fail')));
      component.selectedHistoryDeviceId = 'dev-xyz';
      component.loadDeviceHistory();
      expect(component.historyEntries.length).toBe(0);
    });
  });

  // ── selectDeviceType ───────────────────────────────────────────────────────

  describe('selectDeviceType', () => {
    it('sets the selected device type', () => {
      component.selectDeviceType('Fan');
      expect(component.selectedDeviceType).toBe('Fan');
    });

    it('closes the type picker', () => {
      component.showTypePicker = true;
      component.selectDeviceType('Thermostat');
      expect(component.showTypePicker).toBe(false);
    });
  });

  // ── selectRemoveDevice / selectHistoryDevice ───────────────────────────────

  describe('selectRemoveDevice', () => {
    it('sets selectedRemoveDeviceId', () => {
      component.selectRemoveDevice('dev-42');
      expect(component.selectedRemoveDeviceId).toBe('dev-42');
    });
  });

  describe('selectHistoryDevice', () => {
    it('sets selectedHistoryDeviceId', () => {
      component.selectHistoryDevice('dev-99');
      expect(component.selectedHistoryDeviceId).toBe('dev-99');
    });

    it('resets history state when switching device', () => {
      component.historyEntries = [{ id: 'h1', commandName: 'PowerOn', timestamp: '' }];
      component.hasLoadedHistory = true;
      component.selectHistoryDevice('dev-99');
      expect(component.historyEntries.length).toBe(0);
      expect(component.hasLoadedHistory).toBe(false);
    });
  });

  // ── getDeviceName / getDeviceLocation ──────────────────────────────────────

  describe('device lookup helpers', () => {
    beforeEach(() => {
      component.devices = [
        makeDevice({ id: 'x1', deviceName: 'Kitchen Light', deviceLocation: 'Kitchen' }),
        makeDevice({ id: 'x2', deviceName: 'Bedroom Fan', deviceLocation: 'Bedroom' }),
      ];
    });

    it('getDeviceName returns the device name for a known ID', () => {
      expect(component.getDeviceName('x1')).toBe('Kitchen Light');
    });

    it('getDeviceName returns "Choose Device" for an unknown ID', () => {
      expect(component.getDeviceName('unknown')).toBe('Choose Device');
    });

    it('getDeviceLocation returns the location for a known ID', () => {
      expect(component.getDeviceLocation('x2')).toBe('Bedroom');
    });

    it('getDeviceLocation returns an empty string for an unknown ID', () => {
      expect(component.getDeviceLocation('unknown')).toBe('');
    });
  });

  // ── Input Validation — Add Device form (DOM) ───────────────────────────────

  describe('Add Device form — DOM validation', () => {
    beforeEach(() => {
      component.openAddDeviceModal();
      fixture.detectChanges();
    });

    it('"Confirm Add" button is disabled when name is empty', () => {
      component.newDeviceName = '';
      component.newDeviceLocation = 'Kitchen';
      fixture.detectChanges();
      const btn = fixture.debugElement.query(By.css('.modal-confirm'));
      expect((btn.nativeElement as HTMLButtonElement).disabled).toBe(true);
    });

    it('"Confirm Add" button is disabled when location is empty', () => {
      component.newDeviceName = 'Lamp';
      component.newDeviceLocation = '';
      fixture.detectChanges();
      const btn = fixture.debugElement.query(By.css('.modal-confirm'));
      expect((btn.nativeElement as HTMLButtonElement).disabled).toBe(true);
    });

    it('"Confirm Add" button is disabled when name is whitespace only', () => {
      component.newDeviceName = '   ';
      component.newDeviceLocation = 'Kitchen';
      fixture.detectChanges();
      const btn = fixture.debugElement.query(By.css('.modal-confirm'));
      expect((btn.nativeElement as HTMLButtonElement).disabled).toBe(true);
    });

    it('"Confirm Add" button is disabled when location is whitespace only', () => {
      component.newDeviceName = 'Lamp';
      component.newDeviceLocation = '   ';
      fixture.detectChanges();
      const btn = fixture.debugElement.query(By.css('.modal-confirm'));
      expect((btn.nativeElement as HTMLButtonElement).disabled).toBe(true);
    });

    it('"Confirm Add" button is enabled when name and location are non-empty', () => {
      component.newDeviceName = 'Desk Lamp';
      component.newDeviceLocation = 'Office';
      fixture.detectChanges();
      const btn = fixture.debugElement.query(By.css('.modal-confirm'));
      expect((btn.nativeElement as HTMLButtonElement).disabled).toBe(false);
    });

    it('device type is always pre-selected and defaults to "Light"', () => {
      expect(component.selectedDeviceType).toBe('Light');
    });

    it('all four device types are available in the type list', () => {
      expect(component.deviceTypes).toEqual(['Light', 'Fan', 'Thermostat', 'DoorLock']);
    });

    it('"Cancel" button closes the add modal without calling the API', () => {
      const cancel = fixture.debugElement.query(By.css('.modal-cancel'));
      cancel.nativeElement.click();
      fixture.detectChanges();
      expect(component.showAddDeviceModal).toBe(false);
      expect(apiSpy.registerDevice).not.toHaveBeenCalled();
    });

    it('clicking the modal backdrop closes the add modal', () => {
      const backdrop = fixture.debugElement.query(By.css('.modal-backdrop'));
      backdrop.nativeElement.click();
      fixture.detectChanges();
      expect(component.showAddDeviceModal).toBe(false);
    });
  });

  // ── Input Validation — Remove Device form (DOM) ────────────────────────────

  describe('Remove Device form — DOM validation', () => {
    beforeEach(() => {
      component.devices = [
        makeDevice({ id: 'dev-1', deviceName: 'Kitchen Light', deviceLocation: 'Kitchen' }),
        makeDevice({ id: 'dev-2', deviceName: 'Bedroom Fan', deviceLocation: 'Bedroom' }),
      ];
      component.openRemoveDeviceModal();
      fixture.detectChanges();
    });

    it('"Confirm Remove" button is disabled when no device has been selected', () => {
      expect(component.selectedRemoveDeviceId).toBe('');
      const btn = fixture.debugElement.query(By.css('.modal-confirm'));
      expect((btn.nativeElement as HTMLButtonElement).disabled).toBe(true);
    });

    it('"Confirm Remove" button is enabled after selecting a device', () => {
      component.selectRemoveDevice('dev-1');
      fixture.detectChanges();
      const btn = fixture.debugElement.query(By.css('.modal-confirm'));
      expect((btn.nativeElement as HTMLButtonElement).disabled).toBe(false);
    });

    it('"Confirm Remove" button returns to disabled after clearing the selection', () => {
      component.selectRemoveDevice('dev-1');
      fixture.detectChanges();
      component.selectedRemoveDeviceId = '';
      fixture.detectChanges();
      const btn = fixture.debugElement.query(By.css('.modal-confirm'));
      expect((btn.nativeElement as HTMLButtonElement).disabled).toBe(true);
    });

    it('"Cancel" button closes the remove modal without calling the API', () => {
      const cancel = fixture.debugElement.query(By.css('.modal-cancel'));
      cancel.nativeElement.click();
      fixture.detectChanges();
      expect(component.showRemoveDeviceModal).toBe(false);
      expect(apiSpy.removeDevice).not.toHaveBeenCalled();
    });

    it('cancelling the remove modal preserves both devices', () => {
      component.selectRemoveDevice('dev-1');
      fixture.detectChanges();
      const cancel = fixture.debugElement.query(By.css('.modal-cancel'));
      cancel.nativeElement.click();
      fixture.detectChanges();
      expect(component.devices.length).toBe(2);
      expect(apiSpy.removeDevice).not.toHaveBeenCalled();
    });

    it('clicking the modal backdrop closes the remove modal', () => {
      const backdrop = fixture.debugElement.query(By.css('.modal-backdrop'));
      backdrop.nativeElement.click();
      fixture.detectChanges();
      expect(component.showRemoveDeviceModal).toBe(false);
    });

    it('device list shows all available devices as removal options', () => {
      const options = fixture.debugElement.queryAll(By.css('.modal-option'));
      expect(options.length).toBe(2);
    });
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Subject, of, throwError } from 'rxjs';

import { DashboardComponent } from './dashboard';
import { DeviceApiService } from '../../services/device.api.service';
import { SimulationApiService } from '../../services/simulation.api.service';
import { DeviceResponse } from '../../devicemodels/deviceresponse';

// ── Test data factories ────────────────────────────────────────────────────────

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

const makeLightOn = (overrides: Partial<DeviceResponse> = {}) =>
  makeDevice({ type: 'Light', isDeviceOn: true, isPoweredOn: true, ...overrides });

const makeLightOff = (overrides: Partial<DeviceResponse> = {}) =>
  makeDevice({ type: 'Light', isDeviceOn: false, isPoweredOn: false, ...overrides });

const makeFanOn = (overrides: Partial<DeviceResponse> = {}) =>
  makeDevice({ type: 'Fan', isDeviceOn: true, isPoweredOn: true, fanSpeed: 'Medium', ...overrides });

const makeFanOff = (overrides: Partial<DeviceResponse> = {}) =>
  makeDevice({ type: 'Fan', isDeviceOn: false, isPoweredOn: false, fanSpeed: 'Medium', ...overrides });

const makeThermostat = (isDeviceOn: boolean, overrides: Partial<DeviceResponse> = {}) =>
  makeDevice({
    type: 'Thermostat',
    isDeviceOn,
    isPoweredOn: true,
    thermostatMode: 'Auto',
    thermostatState: isDeviceOn ? 'Heating' : 'Idle',
    targetTemperature: 72,
    minTemperature: 60,
    maxTemperature: 80,
    ...overrides,
  });

const makeDoorLock = (overrides: Partial<DeviceResponse> = {}) =>
  makeDevice({ type: 'DoorLock', isDeviceOn: true, isPoweredOn: null, isLocked: true, ...overrides });

// ── Spy types ──────────────────────────────────────────────────────────────────

type DeviceApiSpy = {
  getAllDevices: ReturnType<typeof vi.fn>;
  controlDevice: ReturnType<typeof vi.fn>;
  registerDevice: ReturnType<typeof vi.fn>;
  removeDevice: ReturnType<typeof vi.fn>;
  getCommandHistory: ReturnType<typeof vi.fn>;
};

type SimulationApiSpy = {
  getAmbientTemperature: ReturnType<typeof vi.fn>;
  setAmbientTemperature: ReturnType<typeof vi.fn>;
  setSimulationSpeed: ReturnType<typeof vi.fn>;
  resetSimulation: ReturnType<typeof vi.fn>;
};

// ── Tests ──────────────────────────────────────────────────────────────────────

describe('DashboardComponent', () => {
  let fixture: ComponentFixture<DashboardComponent>;
  let component: DashboardComponent;
  let deviceApiSpy: DeviceApiSpy;
  let simulationApiSpy: SimulationApiSpy;

  beforeEach(async () => {
    vi.useFakeTimers();

    deviceApiSpy = {
      getAllDevices: vi.fn().mockReturnValue(of([])),
      controlDevice: vi.fn(),
      registerDevice: vi.fn(),
      removeDevice: vi.fn(),
      getCommandHistory: vi.fn(),
    };

    simulationApiSpy = {
      getAmbientTemperature: vi.fn().mockReturnValue(new Subject<never>().asObservable()),
      setAmbientTemperature: vi.fn().mockReturnValue(of({})),
      setSimulationSpeed: vi.fn().mockReturnValue(of({})),
      resetSimulation: vi.fn().mockReturnValue(of({})),
    };

    await TestBed.configureTestingModule({
      imports: [DashboardComponent],
      providers: [
        { provide: DeviceApiService, useValue: deviceApiSpy },
        { provide: SimulationApiService, useValue: simulationApiSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  // ── Initialization ─────────────────────────────────────────────────────────

  describe('initialization', () => {
    it('creates the component', () => {
      expect(component).toBeTruthy();
    });

    it('calls getAllDevices on init', () => {
      expect(deviceApiSpy.getAllDevices).toHaveBeenCalled();
    });

    it('populates devices from the API response', () => {
      const devices = [makeLightOn({ id: 'x1', deviceName: 'Lamp' })];
      deviceApiSpy.getAllDevices.mockReturnValue(of(devices));
      component.loadDevices();
      expect(component.devices).toEqual(devices);
    });

    it('clears isLoading after a successful load', () => {
      deviceApiSpy.getAllDevices.mockReturnValue(of([]));
      component.isLoading = true;
      component.loadDevices();
      expect(component.isLoading).toBe(false);
    });

    it('clears isLoading after a load error', () => {
      deviceApiSpy.getAllDevices.mockReturnValue(throwError(() => new Error('fail')));
      component.isLoading = true;
      component.loadDevices();
      expect(component.isLoading).toBe(false);
    });
  });

  // ── Power filter ───────────────────────────────────────────────────────────

  describe('power filter', () => {
    it('"All" returns every device', () => {
      component.devices = [makeLightOn({ id: 'a' }), makeLightOff({ id: 'b' })];
      component.selectedPowerFilter = 'All';
      expect(component.filteredDevices.length).toBe(2);
    });

    it('"On" includes a powered-on light', () => {
      component.devices = [makeLightOn({ id: 'a' })];
      component.selectedPowerFilter = 'On';
      expect(component.filteredDevices.length).toBe(1);
    });

    it('"On" excludes a powered-off light', () => {
      component.devices = [makeLightOff({ id: 'a' })];
      component.selectedPowerFilter = 'On';
      expect(component.filteredDevices.length).toBe(0);
    });

    it('"Off" includes a powered-off light', () => {
      component.devices = [makeLightOff({ id: 'a' })];
      component.selectedPowerFilter = 'Off';
      expect(component.filteredDevices.length).toBe(1);
    });

    it('"Off" excludes a powered-on light', () => {
      component.devices = [makeLightOn({ id: 'a' })];
      component.selectedPowerFilter = 'Off';
      expect(component.filteredDevices.length).toBe(0);
    });

    it('"On" always includes a DoorLock (latch device)', () => {
      component.devices = [makeDoorLock({ id: 'a' })];
      component.selectedPowerFilter = 'On';
      expect(component.filteredDevices.length).toBe(1);
    });

    it('"Off" never includes a DoorLock (latch device)', () => {
      component.devices = [makeDoorLock({ id: 'a' })];
      component.selectedPowerFilter = 'Off';
      expect(component.filteredDevices.length).toBe(0);
    });

    it('"On" includes a thermostat with isDeviceOn=true (Heating/Cooling)', () => {
      component.devices = [makeThermostat(true, { id: 'a' })];
      component.selectedPowerFilter = 'On';
      expect(component.filteredDevices.length).toBe(1);
    });

    it('"On" excludes a thermostat with isDeviceOn=false (Idle)', () => {
      component.devices = [makeThermostat(false, { id: 'a' })];
      component.selectedPowerFilter = 'On';
      expect(component.filteredDevices.length).toBe(0);
    });

    it('"Off" includes a thermostat with isDeviceOn=false (Idle)', () => {
      component.devices = [makeThermostat(false, { id: 'a' })];
      component.selectedPowerFilter = 'Off';
      expect(component.filteredDevices.length).toBe(1);
    });
  });

  // ── Location filter ────────────────────────────────────────────────────────

  describe('location filter', () => {
    it('"All" shows devices from every location', () => {
      component.devices = [
        makeLightOn({ id: 'a', deviceLocation: 'Kitchen' }),
        makeLightOn({ id: 'b', deviceLocation: 'Bedroom' }),
      ];
      component.selectedLocationFilter = 'All';
      expect(component.filteredDevices.length).toBe(2);
    });

    it('specific location shows only matching devices', () => {
      component.devices = [
        makeLightOn({ id: 'a', deviceLocation: 'Kitchen' }),
        makeLightOn({ id: 'b', deviceLocation: 'Bedroom' }),
      ];
      component.selectedLocationFilter = 'Kitchen';
      expect(component.filteredDevices.length).toBe(1);
      expect(component.filteredDevices[0].id).toBe('a');
    });

    it('specific location excludes devices in other locations', () => {
      component.devices = [makeLightOn({ id: 'b', deviceLocation: 'Bedroom' })];
      component.selectedLocationFilter = 'Kitchen';
      expect(component.filteredDevices.length).toBe(0);
    });
  });

  // ── Type filter ────────────────────────────────────────────────────────────

  describe('type filter', () => {
    it('"All" shows every device type', () => {
      component.devices = [
        makeLightOn({ id: 'a' }),
        makeFanOn({ id: 'b' }),
        makeThermostat(true, { id: 'c' }),
        makeDoorLock({ id: 'd' }),
      ];
      component.selectedTypeFilter = 'All';
      expect(component.filteredDevices.length).toBe(4);
    });

    it('"Light" shows only lights', () => {
      component.devices = [makeLightOn({ id: 'a' }), makeFanOn({ id: 'b' })];
      component.selectedTypeFilter = 'Light';
      expect(component.filteredDevices.length).toBe(1);
      expect(component.filteredDevices[0].type).toBe('Light');
    });

    it('"Fan" shows only fans', () => {
      component.devices = [makeLightOn({ id: 'a' }), makeFanOn({ id: 'b' })];
      component.selectedTypeFilter = 'Fan';
      expect(component.filteredDevices.length).toBe(1);
      expect(component.filteredDevices[0].type).toBe('Fan');
    });

    it('"Thermostat" shows only thermostats', () => {
      component.devices = [makeLightOn({ id: 'a' }), makeThermostat(true, { id: 'b' })];
      component.selectedTypeFilter = 'Thermostat';
      expect(component.filteredDevices.length).toBe(1);
    });

    it('"DoorLock" shows only door locks', () => {
      component.devices = [makeLightOn({ id: 'a' }), makeDoorLock({ id: 'b' })];
      component.selectedTypeFilter = 'DoorLock';
      expect(component.filteredDevices.length).toBe(1);
    });
  });

  // ── Combined filters ───────────────────────────────────────────────────────

  describe('combined filters', () => {
    it('applies all constraints simultaneously', () => {
      component.devices = [
        makeFanOn({ id: 'a', deviceLocation: 'Bedroom' }),
        makeFanOff({ id: 'b', deviceLocation: 'Bedroom' }),
        makeLightOn({ id: 'c', deviceLocation: 'Bedroom' }),
        makeFanOn({ id: 'd', deviceLocation: 'Kitchen' }),
      ];
      component.selectedPowerFilter = 'On';
      component.selectedLocationFilter = 'Bedroom';
      component.selectedTypeFilter = 'Fan';
      expect(component.filteredDevices.length).toBe(1);
      expect(component.filteredDevices[0].id).toBe('a');
    });

    it('returns nothing when no devices match all constraints', () => {
      component.devices = [makeLightOn({ id: 'a', deviceLocation: 'Kitchen' })];
      component.selectedPowerFilter = 'Off';
      component.selectedLocationFilter = 'Bedroom';
      component.selectedTypeFilter = 'Fan';
      expect(component.filteredDevices.length).toBe(0);
    });
  });

  // ── groupedDevices ─────────────────────────────────────────────────────────

  describe('groupedDevices', () => {
    it('groups devices by location', () => {
      component.devices = [
        makeLightOn({ id: 'a', deviceName: 'A', deviceLocation: 'Kitchen' }),
        makeLightOn({ id: 'b', deviceName: 'B', deviceLocation: 'Bedroom' }),
      ];
      expect(component.groupedDevices.length).toBe(2);
    });

    it('sorts groups alphabetically by location name', () => {
      component.devices = [
        makeLightOn({ id: 'a', deviceName: 'A', deviceLocation: 'Zzz Room' }),
        makeLightOn({ id: 'b', deviceName: 'B', deviceLocation: 'Aaa Room' }),
      ];
      const groups = component.groupedDevices;
      expect(groups[0].location).toBe('Aaa Room');
      expect(groups[1].location).toBe('Zzz Room');
    });

    it('sorts devices alphabetically by name within a group', () => {
      component.devices = [
        makeLightOn({ id: 'a', deviceName: 'Zebra', deviceLocation: 'Room' }),
        makeLightOn({ id: 'b', deviceName: 'Apple', deviceLocation: 'Room' }),
        makeLightOn({ id: 'c', deviceName: 'Mango', deviceLocation: 'Room' }),
      ];
      const [group] = component.groupedDevices;
      expect(group.devices[0].deviceName).toBe('Apple');
      expect(group.devices[1].deviceName).toBe('Mango');
      expect(group.devices[2].deviceName).toBe('Zebra');
    });

    it('uses "Unknown" for devices with no location', () => {
      component.devices = [makeDevice({ id: 'a', deviceLocation: '' })];
      const [group] = component.groupedDevices;
      expect(group.location).toBe('Unknown');
    });

    it('only includes devices that pass the active filters', () => {
      component.devices = [
        makeLightOn({ id: 'a', deviceName: 'A', deviceLocation: 'Room' }),
        makeLightOff({ id: 'b', deviceName: 'B', deviceLocation: 'Room' }),
      ];
      component.selectedPowerFilter = 'On';
      const [group] = component.groupedDevices;
      expect(group.devices.length).toBe(1);
      expect(group.devices[0].id).toBe('a');
    });
  });

  // ── locationFilters ────────────────────────────────────────────────────────

  describe('locationFilters', () => {
    it('always includes "All" as the first entry', () => {
      component.devices = [];
      expect(component.locationFilters[0]).toBe('All');
    });

    it('returns unique device locations', () => {
      component.devices = [
        makeLightOn({ deviceLocation: 'Kitchen' }),
        makeLightOn({ deviceLocation: 'Kitchen' }),
        makeLightOn({ deviceLocation: 'Bedroom' }),
      ];
      const filters = component.locationFilters;
      expect(filters.filter((f) => f === 'Kitchen').length).toBe(1);
      expect(filters).toContain('Bedroom');
    });
  });

  // ── thermostatLocations ────────────────────────────────────────────────────

  describe('thermostatLocations', () => {
    it('returns locations that have thermostats', () => {
      component.devices = [
        makeThermostat(true, { deviceLocation: 'Bedroom' }),
        makeLightOn({ deviceLocation: 'Kitchen' }),
      ];
      expect(component.thermostatLocations).toContain('Bedroom');
      expect(component.thermostatLocations).not.toContain('Kitchen');
    });

    it('de-duplicates locations with multiple thermostats', () => {
      component.devices = [
        makeThermostat(true, { id: 'a', deviceLocation: 'Bedroom' }),
        makeThermostat(false, { id: 'b', deviceLocation: 'Bedroom' }),
      ];
      expect(component.thermostatLocations.filter((l) => l === 'Bedroom').length).toBe(1);
    });
  });

  // ── clearFilters / hasActiveFilters ────────────────────────────────────────

  describe('clearFilters', () => {
    it('resets all filters to "All"', () => {
      component.selectedPowerFilter = 'On';
      component.selectedLocationFilter = 'Kitchen';
      component.selectedTypeFilter = 'Fan';
      component.clearFilters();
      expect(component.selectedPowerFilter).toBe('All');
      expect(component.selectedLocationFilter).toBe('All');
      expect(component.selectedTypeFilter).toBe('All');
    });

    it('hasActiveFilters is false after clearing', () => {
      component.selectedPowerFilter = 'On';
      component.clearFilters();
      expect(component.hasActiveFilters).toBe(false);
    });
  });

  describe('hasActiveFilters', () => {
    it('is false when all filters are "All"', () => {
      expect(component.hasActiveFilters).toBe(false);
    });

    it('is true when power filter is set', () => {
      component.selectedPowerFilter = 'On';
      expect(component.hasActiveFilters).toBe(true);
    });

    it('is true when location filter is set', () => {
      component.selectedLocationFilter = 'Kitchen';
      expect(component.hasActiveFilters).toBe(true);
    });

    it('is true when type filter is set', () => {
      component.selectedTypeFilter = 'Fan';
      expect(component.hasActiveFilters).toBe(true);
    });
  });

  // ── getLocationIcon ────────────────────────────────────────────────────────

  describe('getLocationIcon', () => {
    it('returns home icon for "Living Room"', () => {
      expect(component.getLocationIcon('Living Room')).toBe('pi pi-home');
    });

    it('returns moon icon for "Bedroom"', () => {
      expect(component.getLocationIcon('Bedroom')).toBe('pi pi-moon');
    });

    it('returns sign-in icon for a location containing "entry"', () => {
      expect(component.getLocationIcon('Entry Hall')).toBe('pi pi-sign-in');
    });

    it('returns map marker for an unrecognized location', () => {
      expect(component.getLocationIcon('Garage')).toBe('pi pi-map-marker');
    });
  });

  // ── handleSimulationReset ──────────────────────────────────────────────────

  describe('handleSimulationReset', () => {
    it('resets simulationSpeed to 1', () => {
      component.simulationSpeed = 10;
      component.handleSimulationReset();
      expect(component.simulationSpeed).toBe(1);
    });

    it('resets simulationSeconds to 0', () => {
      component.simulationSeconds = 9999;
      component.handleSimulationReset();
      expect(component.simulationSeconds).toBe(0);
    });

    it('reloads devices from the API', () => {
      const callsBefore = (deviceApiSpy.getAllDevices as ReturnType<typeof vi.fn>).mock.calls.length;
      component.handleSimulationReset();
      expect(deviceApiSpy.getAllDevices).toHaveBeenCalledTimes(callsBefore + 1);
    });
  });

  // ── handleSpeedChanged ─────────────────────────────────────────────────────

  describe('handleSpeedChanged', () => {
    it('updates simulationSpeed', () => {
      component.handleSpeedChanged(5);
      expect(component.simulationSpeed).toBe(5);
    });

    it('resets speed to 1x', () => {
      component.simulationSpeed = 10;
      component.handleSpeedChanged(1);
      expect(component.simulationSpeed).toBe(1);
    });
  });
});

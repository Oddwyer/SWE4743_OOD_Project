import { Component, OnDestroy, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { FormsModule } from '@angular/forms';

import { DeviceApiService } from '../../services/device.api.service';
import { DeviceCardComponent } from '../device-card/device-card';
import { DeviceResponse } from '../../devicemodels/deviceresponse';
import { SimulationCardComponent } from '../simulation-card/simulation-card';
import { ManageDevicesCardComponent } from '../manage-devices-card/manage-devices-card';
import { PowerFilter } from '../../types/powerfilter';
import { DeviceFilter } from '../../types/devicefilter';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DeviceCardComponent,
    CardModule,
    SimulationCardComponent,
    ManageDevicesCardComponent,
  ],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css'],
})
/**
 * Displays the smart home dashboard, including the simulation clock,
 * simulation controls, filters, and devices grouped by location.
 */
export class DashboardComponent implements OnInit, OnDestroy {
  devices: DeviceResponse[] = [];
  isLoading = true;
  currentTime = '00:00:00';
  simulationSpeed = 1;
  simulationSeconds = 0;
  showFilterModal = false;

  private simulationEvents?: EventSource;

  selectedPowerFilter: PowerFilter = 'All';
  readonly powerFilters: PowerFilter[] = ['All', 'On', 'Off'];

  selectedLocationFilter = 'All';

  selectedTypeFilter: DeviceFilter = 'All';
  readonly typeFilters: DeviceFilter[] = ['All', 'Light', 'Fan', 'Thermostat', 'DoorLock'];

  private clockIntervalId?: number;

  constructor(
    private readonly deviceApiService: DeviceApiService,
    private readonly cdr: ChangeDetectorRef,
  ) {}

  /**
   * Initializes dashboard data.
   */
  ngOnInit(): void {
    this.loadDevices();
    this.startClock();
    this.startSimulationEvents();
  }

  /**
   * Loads all devices from the backend API.
   */
  loadDevices(): void {
    this.deviceApiService.getAllDevices().subscribe({
      next: (data: DeviceResponse[]) => {
        this.devices = data;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err: unknown) => {
        console.error('Error loading devices:', err);
        this.isLoading = false;
        this.cdr.detectChanges();
      },
    });
  }

  /** Unique location names derived from loaded devices, prefixed with 'All'. */
  get locationFilters(): string[] {
    const locations = this.devices
      .map((device) => device.deviceLocation)
      .filter((location): location is string => !!location);

    return ['All', ...new Set(locations)];
  }

  /** Devices that pass all active power, location, and type filters. */
  get filteredDevices(): DeviceResponse[] {
    return this.devices.filter((device) => {
      return (
        this.matchesPowerFilter(device) &&
        this.matchesLocationFilter(device) &&
        this.matchesTypeFilter(device)
      );
    });
  }

  /** Filtered devices grouped by location, with each group sorted alphabetically by name. */
  get groupedDevices(): { location: string; devices: DeviceResponse[] }[] {
    const groups = new Map<string, DeviceResponse[]>();

    for (const device of this.filteredDevices) {
      const location = device.deviceLocation || 'Unknown Location';

      if (!groups.has(location)) {
        groups.set(location, []);
      }

      groups.get(location)!.push(device);
    }

    return Array.from(groups, ([location, devices]) => ({
      location,
      devices: devices.sort((a, b) => (a.deviceName ?? '').localeCompare(b.deviceName ?? '')),
    })).sort((a, b) => a.location.localeCompare(b.location));
  }

  /** Returns true when the device satisfies the active power filter. */
  private matchesPowerFilter(device: DeviceResponse): boolean {
    if (this.selectedPowerFilter === 'All') {
      return true;
    }

    if (this.selectedPowerFilter === 'On') {
      return this.isDeviceConsideredOn(device);
    }

    return this.isDeviceConsideredOff(device);
  }

  /** Returns true when the device belongs to the selected location (or when 'All' is selected). */
  private matchesLocationFilter(device: DeviceResponse): boolean {
    return (
      this.selectedLocationFilter === 'All' || device.deviceLocation === this.selectedLocationFilter
    );
  }

  /** Returns true when the device matches the selected type (or when 'All' is selected). */
  private matchesTypeFilter(device: DeviceResponse): boolean {
    return (
      this.selectedTypeFilter === 'All' ||
      this.getNormalizedDeviceType(device) === this.selectedTypeFilter.toLowerCase()
    );
  }

  /** Returns true when the device is considered "on" for power filter purposes. */
  private isDeviceConsideredOn(device: DeviceResponse): boolean {
    if (this.getNormalizedDeviceType(device) === 'doorlock') {
      return device.isLocked === true;
    }

    if (this.getNormalizedDeviceType(device) === 'thermostat') {
      return device.isDeviceOn === true;
    }

    return device.isPoweredOn === true || device.isDeviceOn === true;
  }

  /** Returns true when the device is considered "off" for power filter purposes. */
  private isDeviceConsideredOff(device: DeviceResponse): boolean {
    return !this.isDeviceConsideredOn(device);
  }

  /** Resets all active filters to their 'All' defaults. */
  clearFilters(): void {
    this.selectedPowerFilter = 'All';
    this.selectedLocationFilter = 'All';
    this.selectedTypeFilter = 'All';
  }

  /** True when any filter is set to a value other than 'All'. */
  get hasActiveFilters(): boolean {
    return (
      this.selectedPowerFilter !== 'All' ||
      this.selectedLocationFilter !== 'All' ||
      this.selectedTypeFilter !== 'All'
    );
  }

  /** Returns an icon for a location based on its name. */
  getLocationIcon(location: string): string {
    const loc = (location ?? '').toLowerCase();

    if (loc.includes('living')) {
      return 'living';
    }

    if (loc.includes('bedroom')) {
      return 'king_bed';
    }

    if (loc.includes('entry')) {
      return 'door_open';
    }

    if (loc.includes('kitchen')) {
      return 'kitchen';
    }

    if (loc.includes('garage')) {
      return 'garage';
    }

    if (loc.includes('media')) {
      return 'tv';
    }
    if (loc.includes('office')) {
      return 'desktop_mac';
    }
    return 'home';
  }

  /** Location names that contain at least one thermostat device. */
  get thermostatLocations(): string[] {
    const locations = this.devices
      .filter((device) => device.type?.toString().toLowerCase() === 'thermostat')
      .map((device) => device.deviceLocation)
      .filter((location): location is string => !!location);

    return [...new Set(locations)];
  }

  /**
   * Resets dashboard simulation display state after the backend simulation resets.
   */
  handleSimulationReset(): void {
    this.stopClock();
    this.simulationSpeed = 1;
    this.simulationSeconds = 0;
    this.currentTime = '00:00:00';

    this.clearFilters();
    this.startClock();
    this.loadDevices();
    this.cdr.detectChanges();
  }

  /**
   * Updates the speed multiplier.
   *
   * The clock interval stays stable. Speed only changes how much time
   * is added on each tick.
   */
  handleSpeedChanged(speed: number): void {
    this.simulationSpeed = speed;
    this.cdr.detectChanges();
  }

  /**
   * Starts or stops the dashboard clock based on whether ambient
   * temperature is still visibly adjusting.
   */
  handleSimulationRunningChanged(isRunning: boolean): void {
    if (isRunning) {
      this.startClock();
    }
  }

  /**
   * Starts the clock only if it is not already running.
   */
  private startClock(): void {
    if (this.clockIntervalId !== undefined) {
      return;
    }

    this.clockIntervalId = window.setInterval(() => {
      this.updateClock();
    }, 1000);
  }

  /**
   * Stops the clock when ambient temperature finishes adjusting.
   */
  private stopClock(): void {
    if (this.clockIntervalId !== undefined) {
      window.clearInterval(this.clockIntervalId);
      this.clockIntervalId = undefined;
    }
  }

  /**
   * Advances the displayed simulation clock based on the current speed multiplier.
   */
  private updateClock(): void {
    const speed = Number(this.simulationSpeed) || 1;

    this.simulationSeconds += speed;

    const hours = Math.floor(this.simulationSeconds / 3600) % 24;
    const minutes = Math.floor((this.simulationSeconds % 3600) / 60);
    const seconds = this.simulationSeconds % 60;

    this.currentTime =
      `${hours.toString().padStart(2, '0')}:` +
      `${minutes.toString().padStart(2, '0')}:` +
      `${seconds.toString().padStart(2, '0')}`;

    this.cdr.detectChanges();
  }

  private getNormalizedDeviceType(device: DeviceResponse): string {
    return device.type?.toString().toLowerCase() ?? '';
  }

  /**
   * Opens a Server-Sent Events connection so the dashboard can refresh
   * when the backend reports simulation updates.
   */
  private startSimulationEvents(): void {
    this.simulationEvents = new EventSource('http://localhost:5001/api/simulation/events');

    this.simulationEvents.addEventListener('simulation-update', () => {
      this.loadDevices();
    });

    this.simulationEvents.onerror = (error) => {
      console.error('Simulation SSE connection error.', error);
    };
  }

  /**
   * Clears the simulation clock interval when the dashboard is destroyed.
   */
  ngOnDestroy(): void {
    this.stopClock();
    this.simulationEvents?.close();
  }
}

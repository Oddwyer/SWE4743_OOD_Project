import { Component, OnDestroy, OnInit } from '@angular/core';
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

  selectedPowerFilter: PowerFilter = 'All';
  readonly powerFilters: PowerFilter[] = ['All', 'On', 'Off'];

  selectedLocationFilter = 'All';

  selectedTypeFilter: DeviceFilter = 'All';
  readonly typeFilters: DeviceFilter[] = ['All', 'Light', 'Fan', 'Thermostat', 'DoorLock'];

  private clockIntervalId?: number;

  constructor(private readonly deviceApiService: DeviceApiService) {}

  /**
   * Initializes dashboard data and starts the simulation clock.
   */
  ngOnInit(): void {
    this.loadDevices();
    this.startClock();
  }

  /**
   * Loads all devices from the backend API.
   */
  loadDevices(): void {
    this.deviceApiService.getAllDevices().subscribe({
      next: (data: DeviceResponse[]) => {
        this.devices = data;
        this.isLoading = false;
      },
      error: (err: unknown) => {
        console.error('Error loading devices:', err);
        this.isLoading = false;
      },
    });
  }

  /**
   * Returns unique locations from all loaded devices.
   */
  get locationFilters(): string[] {
    const locations = this.devices
      .map((device) => device.deviceLocation)
      .filter((location): location is string => !!location);

    return ['All', ...new Set(locations)];
  }

  /**
   * Returns devices that match all selected filters.
   */
  get filteredDevices(): DeviceResponse[] {
    return this.devices.filter((device) => {
      return (
        this.matchesPowerFilter(device) &&
        this.matchesLocationFilter(device) &&
        this.matchesTypeFilter(device)
      );
    });
  }

  /**
   * Groups filtered devices by location and sorts locations/devices alphabetically.
   */
  get groupedDevices() {
    const groups = new Map<string, DeviceResponse[]>();

    for (const device of this.filteredDevices) {
      const location = device.deviceLocation || 'Unknown';

      if (!groups.has(location)) {
        groups.set(location, []);
      }

      groups.get(location)!.push(device);
    }

    return Array.from(groups, ([location, devices]) => ({
      location,
      devices: devices.sort((a, b) => a.deviceName.localeCompare(b.deviceName)),
    })).sort((a, b) => a.location.localeCompare(b.location));
  }

  /**
   * Returns true when a device matches the selected power filter.
   */
  private matchesPowerFilter(device: DeviceResponse): boolean {
    if (this.selectedPowerFilter === 'All') {
      return true;
    }

    if (this.selectedPowerFilter === 'On') {
      return this.isDeviceConsideredOn(device);
    }

    return this.isDeviceConsideredOff(device);
  }

  /**
   * Returns true when a device matches the selected location filter.
   */
  private matchesLocationFilter(device: DeviceResponse): boolean {
    return (
      this.selectedLocationFilter === 'All' || device.deviceLocation === this.selectedLocationFilter
    );
  }

  /**
   * Returns true when a device matches the selected device type filter.
   */
  private matchesTypeFilter(device: DeviceResponse): boolean {
    return this.selectedTypeFilter === 'All' || device.type === this.selectedTypeFilter;
  }

  /**
   * Returns whether the device should count as on for filtering.
   */
  private isDeviceConsideredOn(device: DeviceResponse): boolean {
    if (device.type === 'DoorLock') {
      return true;
    }

    if (device.type === 'Thermostat') {
      return device.isDeviceOn === true;
    }

    return device.isPoweredOn === true || device.isDeviceOn === true;
  }

  /**
   * Returns whether the device should count as off for filtering.
   */
  private isDeviceConsideredOff(device: DeviceResponse): boolean {
    if (device.type === 'DoorLock') {
      return false;
    }

    return !this.isDeviceConsideredOn(device);
  }

  /**
   * Clears all selected filters.
   */
  clearFilters(): void {
    this.selectedPowerFilter = 'All';
    this.selectedLocationFilter = 'All';
    this.selectedTypeFilter = 'All';
  }

  /**
   * Returns whether any filter is currently active.
   */
  get hasActiveFilters(): boolean {
    return (
      this.selectedPowerFilter !== 'All' ||
      this.selectedLocationFilter !== 'All' ||
      this.selectedTypeFilter !== 'All'
    );
  }

  /**
   * Returns a location icon class based on the location name.
   */
  getLocationIcon(location: string): string {
    const loc = location?.toLowerCase();

    if (loc.includes('living')) return 'pi pi-home';
    if (loc.includes('bedroom')) return 'pi pi-moon';
    if (loc.includes('entry')) return 'pi pi-sign-in';

    return 'pi pi-map-marker';
  }

  /**
   * Returns unique locations that contain thermostats.
   */
  get thermostatLocations(): string[] {
    const locations = this.devices
      .filter((device) => device.type === 'Thermostat')
      .map((device) => device.deviceLocation)
      .filter((location): location is string => !!location);

    return [...new Set(locations)];
  }

  /**
   * Resets dashboard simulation display state after backend simulation reset.
   */
  handleSimulationReset(): void {
    this.simulationSpeed = 1;
    this.simulationSeconds = 0;

    queueMicrotask(() => {
      this.currentTime = '00:00:00';
    });

    this.restartClock();
    this.loadDevices();
  }

  /**
   * Updates the active simulation speed and restarts the clock interval.
   */
  handleSpeedChanged(speed: number): void {
    this.simulationSpeed = speed;
    this.restartClock();
  }

  /**
   * Starts the simulation clock interval.
   */
  private startClock(): void {
    this.restartClock();
  }

  /**
   * Restarts the simulation clock using the current speed multiplier.
   */
  private restartClock(): void {
    if (this.clockIntervalId !== undefined) {
      window.clearInterval(this.clockIntervalId);
      this.clockIntervalId = undefined;
    }

    const interval = 1000 / this.simulationSpeed;

    this.clockIntervalId = window.setInterval(() => {
      this.updateClock();
    }, interval);
  }

  /**
   * Advances the displayed simulation clock by one simulated second.
   */
  private updateClock(): void {
    this.simulationSeconds++;

    const hours = Math.floor(this.simulationSeconds / 3600) % 24;
    const minutes = Math.floor((this.simulationSeconds % 3600) / 60);
    const seconds = this.simulationSeconds % 60;

    queueMicrotask(() => {
      this.currentTime =
        `${hours.toString().padStart(2, '0')}:` +
        `${minutes.toString().padStart(2, '0')}:` +
        `${seconds.toString().padStart(2, '0')}`;
    });
  }

  /**
   * Clears the simulation clock interval when the dashboard is destroyed.
   */
  ngOnDestroy(): void {
    if (this.clockIntervalId !== undefined) {
      window.clearInterval(this.clockIntervalId);
      this.clockIntervalId = undefined;
    }
  }
}

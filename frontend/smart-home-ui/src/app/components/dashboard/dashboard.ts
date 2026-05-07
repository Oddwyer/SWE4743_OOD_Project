import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';

import { DeviceApiService } from '../../services/device.api.service';
import { DeviceCardComponent } from '../device-card/device-card';
import { DeviceResponse } from '../../devicemodels/deviceresponse';
import { SimulationCardComponent } from '../simulation-card/simulation-card';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, DeviceCardComponent, CardModule, SimulationCardComponent],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css'],
})
/**
 * Displays the smart home dashboard, including the simulation clock,
 * simulation controls, and devices grouped by location.
 */
export class DashboardComponent implements OnInit, OnDestroy {
  devices: DeviceResponse[] = [];
  isLoading = true;
  currentTime = '00:00:00';
  simulationSpeed = 1;
  simulationSeconds = 0;

  private clockIntervalId?: number;

  constructor(
    private readonly deviceApiService: DeviceApiService,
    private readonly cdr: ChangeDetectorRef,
  ) {}

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
        this.cdr.detectChanges();
      },
      error: (err: unknown) => {
        console.error('Error loading devices:', err);
        this.isLoading = false;
        this.cdr.detectChanges();
      },
    });
  }

  /**
   * Groups devices by location for display in the dashboard.
   */
  get groupedDevices(): { location: string; devices: DeviceResponse[] }[] {
    const groups = new Map<string, DeviceResponse[]>();

    for (const device of this.devices) {
      const location = device.deviceLocation || 'Unknown Location';

      if (!groups.has(location)) {
        groups.set(location, []);
      }

      groups.get(location)!.push(device);
    }

    return Array.from(groups, ([location, devices]) => ({
      location,
      devices,
    }));
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
      .filter((device) => device.type?.toString().toLowerCase() === 'thermostat')
      .map((device) => device.deviceLocation)
      .filter((location): location is string => !!location);

    return [...new Set(locations)];
  }

  /**
   * Resets dashboard simulation display state after the backend simulation resets.
   */
  handleSimulationReset(): void {
    this.simulationSpeed = 1;
    this.simulationSeconds = 0;
    this.currentTime = '00:00:00';
    this.loadDevices();
  }

  /**
   * Starts the simulation clock interval.
   */
  private startClock(): void {
    this.updateClock();

    this.clockIntervalId = window.setInterval(() => {
      this.updateClock();
    }, 1000);
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

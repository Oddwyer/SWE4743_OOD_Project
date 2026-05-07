import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DeviceApiService } from '../../services/device.api.service';
import { DeviceCardComponent } from '../device-card/device-card';
import { CardModule } from 'primeng/card';
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
 * Displays all smart home devices and organizes them by location.
 *
 * Also coordinates refreshing device data after simulation changes.
 */
export class DashboardComponent implements OnInit, OnDestroy {
  devices: any[] = [];
  isLoading = true;
  currentTime = '';
  simulationSpeed = 1;
  simulationSeconds = 0;

  constructor(
    private deviceApiService: DeviceApiService,
    private cdr: ChangeDetectorRef,
  ) {}

  /**
   * Loads all devices when the component initializes.
   */
  ngOnInit(): void {
    console.log('Dashboard INIT');
    this.loadDevices();

    if (!this.clockIntervalId) {
      this.startClock();
    }
  }

  private clockIntervalId?: number;

  private startClock(): void {
    this.updateClock();

    this.clockIntervalId = window.setInterval(() => {
      this.updateClock();
    }, 1000);
  }

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
   * Loads all devices from the backend API.
   */
  loadDevices() {
    this.deviceApiService.getAllDevices().subscribe({
      next: (data: DeviceResponse[]) => {
        this.devices = data;
        this.isLoading = false;
        console.log('Devices loaded:', data);
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
   * Groups devices by location for display in the UI.
   */
  get groupedDevices(): { location: string; devices: any[] }[] {
    const groups = new Map<string, any[]>();

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
   * Returns a PrimeIcons icon based on location name.
   */
  getLocationIcon(location: string): string {
    const loc = location?.toLowerCase();

    if (loc.includes('living')) return 'pi pi-home';

    if (loc.includes('bedroom')) return 'pi pi-moon';

    if (loc.includes('entry')) return 'pi pi-sign-in';

    return 'pi pi-map-marker';
  }

  /**
   * Returns all unique thermostat locations for simulation controls.
   */
  get thermostatLocations(): string[] {
    const locations = this.devices
      .filter((device) => device.type?.toString().toLowerCase() === 'thermostat')
      .map((device) => device.deviceLocation)
      .filter((location): location is string => !!location);

    return [...new Set(locations)];
  }

  ngOnDestroy(): void {
    if (this.clockIntervalId !== undefined) {
      window.clearInterval(this.clockIntervalId);
      this.clockIntervalId = undefined;
    }
  }
}

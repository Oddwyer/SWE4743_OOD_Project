import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DeviceApiService } from '../../services/device.api.service';
import { DeviceCardComponent } from '../device-card/device-card';
import { CardModule } from 'primeng/card';
import { DeviceResponse } from '../../devicemodels/deviceresponse';

@Component({
  selector: 'app-device-list',
  templateUrl: './device-list.html',
  standalone: true,
  imports: [CommonModule, DeviceCardComponent, CardModule],
  styleUrls: ['./device-list.css'],
})

// The DeviceList component is responsible for fetching and displaying all devices in the smart home system.
// It uses the DeviceApiService to retrieve device data and organizes it by location for better user experience.
export class DeviceListComponent implements OnInit {
  devices: any[] = [];
  isLoading = true;

  constructor(
    private deviceApiService: DeviceApiService,
    private cdr: ChangeDetectorRef,
  ) {}

  // On component initialization, load all devices from the API.
  ngOnInit(): void {
    console.log('DeviceList INIT');
    this.loadDevices();
  }

  // Helper: Load all devices from the API and handle loading state.
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

  // Group devices by location for better UI organization.
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

  // Helper method to get icon class based on device location.
  getLocationIcon(location: string): string {
    const loc = location?.toLowerCase();

    if (loc.includes('living')) return 'pi pi-home';

    if (loc.includes('bedroom')) return 'pi pi-moon';

    if (loc.includes('entry')) return 'pi pi-sign-in';

    return 'pi pi-map-marker'; // fallback
  }
}

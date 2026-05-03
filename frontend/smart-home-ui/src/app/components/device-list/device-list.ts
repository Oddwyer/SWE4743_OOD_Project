import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DeviceApiService } from '../../services/device.api.service';

// This component displays a list of all devices with their current status and controls.
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-device-list',
  templateUrl: './device-list.html',
  standalone: true,
  imports: [CommonModule, CardModule, TagModule, ButtonModule],
  styleUrls: ['./device-list.css']
})

// The DeviceList component is responsible for fetching and displaying all devices in the smart home system.
// It uses the DeviceApiService to retrieve device data and organizes it by location for better user experience.

export class DeviceList implements OnInit {
  devices: any[] = [];
  isLoading = true;

  constructor(
    private deviceService: DeviceApiService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    console.log('DeviceList INIT');
    this.loadDevices();
  }

  // Load all devices from the API and handle loading state.
  loadDevices() {
    this.deviceService.getAllDevices().subscribe({
      next: (data) => {
        this.devices = data;
        this.isLoading = false;
        console.log('Devices loaded:', data);
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading devices:', err);
        this.isLoading = false;
        this.cdr.detectChanges();
      }
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
      devices
    }));
  }

  // Helper method to get icon class based on device type.
  getDeviceIcon(type: string): string {
    switch (type?.toLowerCase()) {
      case 'thermostat':
        return 'pi pi-gauge';
      case 'fan':
        return 'pi pi-sync';
      case 'doorlock':
        return 'pi pi-lock';
      case 'light':
        return 'pi pi-lightbulb';
      default:
        return 'pi pi-home';
    }
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
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DeviceApiService } from '../../services/device.api.service';

// This component displays a list of all devices with their current status and controls.
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { FormsModule } from '@angular/forms';
import { ToggleSwitch } from 'primeng/toggleswitch';
import { SliderModule } from 'primeng/slider';
import { ColorPickerModule } from 'primeng/colorpicker';

@Component({
  selector: 'app-device-list',
  templateUrl: './device-list.html',
  standalone: true,
  imports: [CommonModule, CardModule, TagModule, ButtonModule, FormsModule,
    ToggleSwitch, SliderModule, ColorPickerModule],
  styleUrls: ['./device-list.css']
})

// The DeviceList component is responsible for fetching and displaying all devices in the smart home system.
// It uses the DeviceApiService to retrieve device data and organizes it by location for better user experience.

export class DeviceList implements OnInit {
  devices: any[] = [];
  isLoading = true;

  constructor(
    private deviceApiService: DeviceApiService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    console.log('DeviceList INIT');
    this.loadDevices();
  }

  // Load all devices from the API and handle loading state.
  loadDevices() {
    this.deviceApiService.getAllDevices().subscribe({
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

  // Device commands (e.g., toggle power) are sent to the API, and the device list is refreshed upon success.

  // Toggle the power state of a device and refresh the device list to reflect changes.
  toggleDevicePower(device: any): void {

    const previousPowerState = device.isDeviceOn;
    device.isDeviceOn = !device.isDeviceOn;

    const request = { command: 'togglePower' };

    // Send the control command to the API and refresh the device list on success.
    this.deviceApiService.controlDevice(device.id, request).subscribe({
      next: (updatedDevice: any) => {

        Object.assign(device, updatedDevice);
        console.log('FROM API:', updatedDevice);
      },
      error: (err) => {
        console.error('Failed to toggle power', err);
        console.log('AFTER MERGE (UI):', device);
        device.isDeviceOn = previousPowerState;
      }

    });
  }

  // Determine if the power toggle button should be shown for a device (e.g., not for door locks).
  canTogglePower(device: any): boolean {
    return device.type?.toLowerCase() !== 'doorlock';
  }

  // Set brightness for light devices.
  setBrightness(device: any, brightness: number): void {

    const previousBrightness = device.lightBrightness;

    const request = { command: 'setBrightness', brightness };

    this.deviceApiService.controlDevice(device.id, request).subscribe({
      next: (updatedDevice: any) => {
        Object.assign(device, updatedDevice);
        console.log('FROM API:', updatedDevice);
      },
      error: (err) => {
        console.error('Failed to set brightness', err);
        device.lightBrightness = previousBrightness;
        console.log('AFTER MERGE (UI):', device);
      }
    });
  }

  // Select color for light devices.
  changeColor(device: any, color: string): void {

    const previousColor = device.lightColor;

    const request = { command: 'setColor', color };

    this.deviceApiService.controlDevice(device.id, request).subscribe({
      next: (updatedDevice: any) => {
        Object.assign(device, updatedDevice);
        console.log('FROM API:', updatedDevice);
      },
      error: (err) => {
        console.error('Failed to change color', err);
        device.lightColor = previousColor;
        console.log('AFTER MERGE (UI):', device);
      }
    });
  }
}
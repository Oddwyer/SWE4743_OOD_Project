import { Component, Input } from '@angular/core';
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
import { SelectButtonModule } from 'primeng/selectbutton';

@Component({
  selector: 'app-device-card',
  standalone: true,
  imports: [
    CommonModule,
    CardModule,
    TagModule,
    ButtonModule,
    FormsModule,
    ToggleSwitch,
    SliderModule,
    ColorPickerModule,
    SelectButtonModule,
  ],
  templateUrl: './device-card.html',
  styleUrl: './device-card.css',
})
export class DeviceCardComponent {
  @Input({ required: true }) device!: any;

  fanSpeedOptions = [
    { label: 'Low', value: 'low' },
    { label: 'Medium', value: 'medium' },
    { label: 'High', value: 'high' },
  ];

  constructor(private deviceApiService: DeviceApiService) {}

  // Helper method to get icon class based on device type.
  getDeviceIcon(type: string): string {
    switch (type?.toLowerCase()) {
      case 'thermostat':
        return 'pi pi-gauge';

      case 'fan':
        return 'pi pi-sync';

      case 'doorlock':
        return 'pi pi-sign-in';

      case 'light':
        return 'pi pi-lightbulb';

      default:
        return 'pi pi-home';
    }
  }

  // Toggle the power state of a device and refresh the device list to reflect changes.
  toggleDevicePower(): void {
    const previousPowerState = this.device.isDeviceOn;

    this.device.isDeviceOn = !this.device.isDeviceOn;

    const request = { command: 'togglePower' };

    // Send the control command to the API and refresh the device list on success.
    this.deviceApiService.controlDevice(this.device.id, request).subscribe({
      next: (updatedDevice: any) => {
        Object.assign(this.device, updatedDevice);
        console.log('FROM API:', updatedDevice);
      },
      error: (err) => {
        console.error('Failed to toggle power', err);
        this.device.isDeviceOn = previousPowerState;
      },
    });
  }

  // Determine if the power toggle button should be shown for a device (e.g., not for door locks).
  canTogglePower(): boolean {
    return this.device.type?.toLowerCase() !== 'doorlock';
  }

  // Toggle the power state of a device and refresh the device list to reflect changes.
  toggleLatch(): void {
    const previousLatchState = this.device.isLocked;

    this.device.isLocked = !this.device.isLocked;

    const request = { command: 'toggleLock' };

    // Send the control command to the API and refresh the device list on success.
    this.deviceApiService.controlDevice(this.device.id, request).subscribe({
      next: (updatedDevice: any) => {
        Object.assign(this.device, updatedDevice);
        console.log('FROM API:', updatedDevice);
      },
      error: (err) => {
        console.error('Could not toggle door lock.', err);
        this.device.isLocked = previousLatchState;
      },
    });
  }

  // Determine if the power toggle button should be shown for a device (e.g., not for door locks).
  canToggleLatch(): boolean {
    return this.device.type?.toLowerCase() == 'doorlock';
  }

  // Set brightness for light devices.
  setBrightness(brightness: number): void {
    const previousBrightness = this.device.lightBrightness;

    const request = { command: 'setBrightness', brightness };

    this.deviceApiService.controlDevice(this.device.id, request).subscribe({
      next: (updatedDevice: any) => {
        Object.assign(this.device, updatedDevice);
        console.log('FROM API:', updatedDevice);
      },
      error: (err) => {
        console.error('Failed to set brightness', err);
        this.device.lightBrightness = previousBrightness;
      },
    });
  }

  // Get the display status of a device, showing "ON"/"OFF" for regular devices and "Locked"/"Unlocked" for door locks.
  getDeviceStatus(): string {
    if (this.canToggleLatch()) {
      return this.device.isLocked ? 'ON' : 'OFF';
    }

    return this.device.isDeviceOn ? 'ON' : 'OFF';
  }

  // Select color for light devices.
  setColor(color: string): void {
    const previousColor = this.device.lightColor;

    const normalizedColor = color.startsWith('#') ? color : `#${color}`;

    const request = { command: 'setColor', color: normalizedColor };

    this.deviceApiService.controlDevice(this.device.id, request).subscribe({
      next: (updatedDevice: any) => {
        Object.assign(this.device, updatedDevice);
        console.log('FROM API:', updatedDevice);
      },
      error: (err) => {
        console.error('Failed to change color', err);
        this.device.lightColor = previousColor;
      },
    });
  }

  setFanSpeed(speed: string): void {
    const previousSpeed = this.device.fanSpeed;

    const request = { command: 'setFanSpeed', speed: speed };

    this.deviceApiService.controlDevice(this.device.id, request).subscribe({
      next: (updatedDevice: any) => {
        Object.assign(this.device, updatedDevice);
        console.log('FROM API:', updatedDevice);
      },
      error: (err) => {
        console.error('Failed to set fan speed', err);
        this.device.fanSpeed = previousSpeed;
      },
    });
  }
}

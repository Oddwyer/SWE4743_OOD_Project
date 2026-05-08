import { ChangeDetectorRef, Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';

import { DeviceApiService } from '../../services/device.api.service';
import { DeviceResponse } from '../../devicemodels/deviceresponse';
import { RegisterDeviceRequest } from '../../devicemodels/registerdevicerequest';
import { CommandHistoryResponse } from '../../historymodels/commandhistoryresponse';
import { DeviceType } from '../../types/devicetype';

/**
 * Sidebar card for adding devices, removing devices, and viewing command history.
 */
@Component({
  selector: 'app-manage-devices-card',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonModule, InputTextModule],
  templateUrl: './manage-devices-card.html',
  styleUrls: ['./manage-devices-card.css'],
})
export class ManageDevicesCardComponent {
  /** Devices currently loaded by the dashboard. */
  @Input() devices: DeviceResponse[] = [];

  /** Tells the dashboard to reload devices after add/remove actions. */
  @Output() devicesChanged = new EventEmitter<void>();

  /** Device types available for registration. */
  readonly deviceTypes: DeviceType[] = ['Light', 'Fan', 'Thermostat', 'DoorLock'];

  /** Add device form values. */
  newDeviceName = '';
  newDeviceLocation = '';
  selectedDeviceType: DeviceType = 'Light';

  /** Selected devices for remove and history actions. */
  selectedRemoveDeviceId = '';
  selectedHistoryDeviceId = '';

  /** Command history shown after loading a selected device. */
  historyEntries: CommandHistoryResponse[] = [];

  /** Loading and loaded states for history requests. */
  isLoadingHistory = false;
  hasLoadedHistory = false;

  /** Modal visibility flags. */
  showAddDeviceModal = false;
  showRemoveDeviceModal = false;
  showHistoryDeviceModal = false;
  showTypePicker = false;

  constructor(
    private readonly deviceApiService: DeviceApiService,
    private readonly changeDetectorRef: ChangeDetectorRef,
  ) {}

  /** Open the add device modal. */
  openAddDeviceModal(): void {
    this.resetAddForm();
    this.showAddDeviceModal = true;
  }

  /** Open the remove device modal. */
  openRemoveDeviceModal(): void {
    this.selectedRemoveDeviceId = '';
    this.showRemoveDeviceModal = true;
  }

  /** Open the history modal. */
  openHistoryDeviceModal(): void {
    this.selectedHistoryDeviceId = '';
    this.historyEntries = [];
    this.hasLoadedHistory = false;
    this.isLoadingHistory = false;
    this.showHistoryDeviceModal = true;
  }

  /** Register a new device through the API. */
  addDevice(): void {
    const request: RegisterDeviceRequest = {
      deviceName: this.toTitleCase(this.newDeviceName.trim()),
      deviceLocation: this.toTitleCase(this.newDeviceLocation.trim()),
      type: this.selectedDeviceType,
    };

    this.deviceApiService.registerDevice(request).subscribe({
      next: () => {
        this.resetAddForm();
        this.showAddDeviceModal = false;
        this.devicesChanged.emit();
      },
      error: (error) => {
        console.error('Failed to register device.', error);
      },
    });
  }

  /** Remove the selected device through the API. */
  removeDevice(): void {
    if (!this.selectedRemoveDeviceId) {
      return;
    }

    this.deviceApiService.removeDevice(this.selectedRemoveDeviceId).subscribe({
      next: () => {
        this.selectedRemoveDeviceId = '';
        this.showRemoveDeviceModal = false;
        this.devicesChanged.emit();
      },
      error: (error) => {
        console.error('Failed to remove device.', error);
      },
    });
  }

  /** Load command history for the selected device. */
  loadDeviceHistory(): void {
    if (!this.selectedHistoryDeviceId) {
      return;
    }

    this.isLoadingHistory = true;
    this.hasLoadedHistory = false;
    this.historyEntries = [];
    this.changeDetectorRef.detectChanges();

    this.deviceApiService.getCommandHistory(this.selectedHistoryDeviceId).subscribe({
      next: (history) => {
        this.historyEntries = history;
        this.hasLoadedHistory = true;
        this.isLoadingHistory = false;
        this.changeDetectorRef.detectChanges();
      },
      error: (error) => {
        console.error('Failed to load device history.', error);

        this.hasLoadedHistory = true;
        this.isLoadingHistory = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  /** Select the device type used by the add form. */
  selectDeviceType(deviceType: DeviceType): void {
    this.selectedDeviceType = deviceType;
    this.showTypePicker = false;
  }

  /** Select the device to remove. */
  selectRemoveDevice(deviceId: string): void {
    this.selectedRemoveDeviceId = deviceId;
  }

  /** Select the device whose history should be viewed. */
  selectHistoryDevice(deviceId: string): void {
    this.selectedHistoryDeviceId = deviceId;
    this.historyEntries = [];
    this.hasLoadedHistory = false;
    this.isLoadingHistory = false;
  }

  /** Get display name for a selected device ID. */
  getDeviceName(deviceId: string): string {
    return this.devices.find((device) => device.id === deviceId)?.deviceName ?? 'Choose Device';
  }

  /** Get display location for a selected device ID. */
  getDeviceLocation(deviceId: string): string {
    return this.devices.find((device) => device.id === deviceId)?.deviceLocation ?? '';
  }

  /** Converts a string to Title Case (first letter of each word capitalised). */
  private toTitleCase(value: string): string {
    return value
      .split(' ')
      .map((word) => (word.length > 0 ? word[0].toUpperCase() + word.slice(1).toLowerCase() : ''))
      .join(' ');
  }

  /** Reset the add device form after a successful registration. */
  private resetAddForm(): void {
    this.newDeviceName = '';
    this.newDeviceLocation = '';
    this.selectedDeviceType = 'Light';
  }
}

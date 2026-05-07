import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';

import { DeviceApiService } from '../../services/device.api.service';

import { DeviceResponse } from '../../devicemodels/deviceresponse';
import { RegisterDeviceRequest } from '../../devicemodels/registerdevicerequest';
import { CommandHistoryResponse } from '../../historymodels/commandhistoryresponse';

import { DeviceType } from '../../types/devicetype';

/**
 * Sidebar card for managing smart home devices.
 *
 * Supports:
 * - adding devices
 * - removing devices
 * - viewing device command history
 */
@Component({
  selector: 'app-manage-devices-card',
  standalone: true,
  imports: [CommonModule, FormsModule, CardModule, ButtonModule, SelectModule],
  templateUrl: './manage-devices-card.html',
  styleUrls: ['./manage-devices-card.css'],
})
export class ManageDevicesCardComponent {
  /**
   * Current devices loaded by dashboard.
   */
  @Input() devices: DeviceResponse[] = [];

  /**
   * Notify dashboard to refresh devices.
   */
  @Output() devicesChanged = new EventEmitter<void>();

  /**
   * Available device types.
   */
  readonly deviceTypes: DeviceType[] = ['Light', 'Fan', 'Thermostat', 'DoorLock'];

  /**
   * New device form values.
   */
  newDeviceName = '';
  newDeviceLocation = '';
  selectedDeviceType: DeviceType = 'Light';

  /**
   * Selected device IDs for actions.
   */
  selectedRemoveDeviceId = '';
  selectedHistoryDeviceId = '';

  /**
   * Loaded command history entries.
   */
  historyEntries: CommandHistoryResponse[] = [];

  /**
   * Whether history is currently loading.
   */
  isLoadingHistory = false;

  constructor(private readonly deviceApiService: DeviceApiService) {}

  /**
   * Register a new smart home device.
   */
  addDevice(): void {
    const request: RegisterDeviceRequest = {
      deviceName: this.newDeviceName.trim(),
      deviceLocation: this.newDeviceLocation.trim(),
      type: this.selectedDeviceType,
    };

    this.deviceApiService.registerDevice(request).subscribe({
      next: () => {
        this.resetAddForm();
        this.devicesChanged.emit();
      },
      error: (error) => {
        console.error('Failed to register device.', error);
      },
    });
  }

  /**
   * Remove the selected device.
   */
  removeDevice(): void {
    if (!this.selectedRemoveDeviceId) {
      return;
    }

    this.deviceApiService.removeDevice(this.selectedRemoveDeviceId).subscribe({
      next: () => {
        this.selectedRemoveDeviceId = '';
        this.devicesChanged.emit();
      },
      error: (error) => {
        console.error('Failed to remove device.', error);
      },
    });
  }

  /**
   * Load command history for the selected device.
   */
  loadDeviceHistory(): void {
    if (!this.selectedHistoryDeviceId) {
      return;
    }

    this.isLoadingHistory = true;

    this.deviceApiService.getCommandHistory(this.selectedHistoryDeviceId).subscribe({
      next: (history) => {
        this.historyEntries = history;
        this.isLoadingHistory = false;
      },
      error: (error) => {
        console.error('Failed to load device history.', error);
        this.isLoadingHistory = false;
      },
    });
  }

  /**
   * Reset the add device form.
   */
  private resetAddForm(): void {
    this.newDeviceName = '';
    this.newDeviceLocation = '';
    this.selectedDeviceType = 'Light';
  }
}

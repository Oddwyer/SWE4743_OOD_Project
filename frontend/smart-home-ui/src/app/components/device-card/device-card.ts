import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DeviceApiService } from '../../services/device.api.service';

import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { FormsModule } from '@angular/forms';
import { ToggleSwitch } from 'primeng/toggleswitch';
import { SliderModule } from 'primeng/slider';
import { ColorPickerModule } from 'primeng/colorpicker';
import { SelectButtonModule } from 'primeng/selectbutton';

import { DeviceResponse } from '../../devicemodels/deviceresponse';
import { ControlDeviceRequest } from '../../devicemodels/controldevicerequest';
import { FanSpeed } from '../../types/fanspeed';
import { ThermostatMode } from '../../types/thermostatmode';

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

/**
 * Displays and controls an individual smart home device.
 *
 * Handles device-specific UI interactions such as:
 * - power controls
 * - lighting controls
 * - fan controls
 * - thermostat controls
 * - door lock controls
 */
export class DeviceCardComponent {
  @Input({ required: true }) device!: DeviceResponse;

  /**
   * Available fan speed options for select buttons.
   */
  fanSpeedOptions: { label: string; value: FanSpeed }[] = [
    { label: 'Low', value: 'Low' },
    { label: 'Med', value: 'Medium' },
    { label: 'High', value: 'High' },
  ];

  /**
   * Available thermostat mode options for select buttons.
   */
  thermostatModeOptions: { label: string; value: ThermostatMode }[] = [
    { label: 'Cool', value: 'Cool' },
    { label: 'Auto', value: 'Auto' },
    { label: 'Heat', value: 'Heat' },
  ];

  constructor(private readonly deviceApiService: DeviceApiService) {}

  /**
   * Returns the Material icon name for a device type.
   */
  getDeviceIcon(type: string): string {
    switch (type?.toLowerCase()) {
      case 'light':
        return 'emoji_objects';

      case 'fan':
        return 'mode_fan';

      case 'doorlock':
        return 'sensor_door';

      case 'thermostat':
        return 'thermostat';

      default:
        return 'devices';
    }
  }

  /**
   * Toggles device power using optimistic UI updates.
   */
  toggleDevicePower(): void {
    const previousPowerState = this.device.isPoweredOn ?? false;

    this.device.isPoweredOn = !previousPowerState;

    const request: ControlDeviceRequest = {
      command: 'TogglePower',
    };

    this.deviceApiService.controlDevice(this.device.id, request).subscribe({
      next: (updatedDevice: DeviceResponse) => {
        this.device = updatedDevice;
      },

      error: (err: unknown) => {
        console.error('Failed to toggle power.', err);
        this.device.isPoweredOn = previousPowerState;
      },
    });
  }

  /**
   * Returns whether the device supports power toggling.
   */
  canTogglePower(): boolean {
    return this.device.type?.toLowerCase() !== 'doorlock';
  }

  /**
   * Thermostats separate powered state from active heating/cooling state.
   *
   * isPoweredOn:
   * - determines control availability
   * - controls thermostat power state
   *
   * isDeviceOn:
   * - represents active heating/cooling
   * - used for filtering semantics
   */
  isThermostatPoweredOn(): boolean {
    return this.device.isPoweredOn ?? false;
  }

  /**
   * Toggles the lock state for door lock devices.
   */
  toggleLatch(): void {
    const previousLatchState = this.device.isLocked;

    this.device.isLocked = !this.device.isLocked;

    const request: ControlDeviceRequest = {
      command: 'ToggleLock',
    };

    this.deviceApiService.controlDevice(this.device.id, request).subscribe({
      next: (updatedDevice: DeviceResponse) => {
        this.device = updatedDevice;
      },

      error: (err: unknown) => {
        console.error('Failed to toggle door lock.', err);
        this.device.isLocked = previousLatchState;
      },
    });
  }

  /**
   * Returns whether the device supports latch controls.
   */
  canToggleLatch(): boolean {
    return this.device.type?.toLowerCase() === 'doorlock';
  }

  /**
   * Returns the display status for the device.
   *
   * Thermostats use isPoweredOn for display purposes,
   * while filtering logic still uses isDeviceOn.
   */
  getDeviceStatus(): string {
    if (this.canToggleLatch()) {
      return this.device.isLocked ? 'ON' : 'OFF';
    }

    if (this.device.type?.toLowerCase() === 'thermostat') {
      return this.isThermostatPoweredOn() ? 'ON' : 'OFF';
    }

    return this.device.isDeviceOn ? 'ON' : 'OFF';
  }

  /**
   * Updates light brightness using optimistic UI updates.
   */
  setBrightness(brightness: number): void {
    const previousBrightness = this.device.lightBrightness;

    this.device.lightBrightness = brightness;

    const request: ControlDeviceRequest = {
      command: 'SetBrightness',
      brightness,
    };

    this.deviceApiService.controlDevice(this.device.id, request).subscribe({
      next: (updatedDevice: DeviceResponse) => {
        this.device = updatedDevice;
      },

      error: (err: unknown) => {
        console.error('Failed to set brightness.', err);
        this.device.lightBrightness = previousBrightness;
      },
    });
  }

  /**
   * Updates light color using optimistic UI updates.
   */
  setColor(color: string): void {
    const previousColor = this.device.lightColor;

    const normalizedColor = color.startsWith('#') ? color : `#${color}`;

    this.device.lightColor = normalizedColor;

    const request: ControlDeviceRequest = {
      command: 'SetColor',
      color: normalizedColor,
    };

    this.deviceApiService.controlDevice(this.device.id, request).subscribe({
      next: (updatedDevice: DeviceResponse) => {
        this.device = updatedDevice;
      },

      error: (err: unknown) => {
        console.error('Failed to set color.', err);
        this.device.lightColor = previousColor;
      },
    });
  }

  /**
   * Updates fan speed using optimistic UI updates.
   */
  setFanSpeed(speed: FanSpeed): void {
    const previousSpeed = this.device.fanSpeed;

    this.device.fanSpeed = speed;

    const request: ControlDeviceRequest = {
      command: 'SetFanSpeed',
      fanSpeed: speed,
    };

    this.deviceApiService.controlDevice(this.device.id, request).subscribe({
      next: (updatedDevice: DeviceResponse) => {
        this.device = updatedDevice;
      },

      error: (err: unknown) => {
        console.error('Failed to set fan speed.', err);
        this.device.fanSpeed = previousSpeed;
      },
    });
  }

  /**
   * Updates thermostat operating mode.
   */
  setThermostatMode(mode: ThermostatMode): void {
    const previousMode = this.device.thermostatMode;

    this.device.thermostatMode = mode;

    const request: ControlDeviceRequest = {
      command: 'SetThermostatMode',
      thermostatMode: mode,
    };

    this.deviceApiService.controlDevice(this.device.id, request).subscribe({
      next: (updatedDevice: DeviceResponse) => {
        this.device = updatedDevice;
      },

      error: (err: unknown) => {
        console.error('Failed to set thermostat mode.', err);
        this.device.thermostatMode = previousMode;
      },
    });
  }

  /**
   * Updates thermostat target temperature.
   */
  setTargetTemperature(temperature: number): void {
    const previousTarget = this.device.targetTemperature;

    this.device.targetTemperature = temperature;

    const request: ControlDeviceRequest = {
      command: 'SetTargetTemperature',
      targetTemperature: temperature,
    };

    this.deviceApiService.controlDevice(this.device.id, request).subscribe({
      next: (updatedDevice: DeviceResponse) => {
        this.device = updatedDevice;
      },

      error: (err: unknown) => {
        console.error('Failed to set target temperature.', err);
        this.device.targetTemperature = previousTarget;
      },
    });
  }
}

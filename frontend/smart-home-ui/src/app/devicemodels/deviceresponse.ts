import { DeviceType } from '../types/devicetype';
import { FanSpeed } from '../types/fanspeed';
import { ThermostatMode } from '../types/thermostatmode';
import { ThermostatStateType } from '../types/thermostatstate';

/** API response shape for a single device. */
export interface DeviceResponse {
  id: string;
  deviceName: string;
  deviceLocation: string;
  type: DeviceType;

  /** True when the device is considered "on" for display and filtering purposes. */
  isDeviceOn: boolean;

  /** Power state for devices that have an explicit on/off switch (lights, fans, thermostats). */
  isPoweredOn?: boolean | null;

  /** Lock state for door lock devices. */
  isLocked?: boolean | null;

  createdAt: string;
  updatedAt: string;

  lightBrightness?: number | null;
  minBrightness?: number | null;
  maxBrightness?: number | null;
  lightColor?: string | null;

  fanSpeed?: FanSpeed | null;

  thermostatMode?: ThermostatMode | null;
  thermostatState?: ThermostatStateType | null;
  minTemperature?: number | null;
  maxTemperature?: number | null;
  targetTemperature?: number | null;
  defaultTemperature?: number | null;

  ambientTemperature?: number | null;
}

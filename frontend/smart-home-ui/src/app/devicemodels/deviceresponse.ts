import { DeviceType } from '../types/devicetype';
import { FanSpeed } from '../types/fanspeed';
import { ThermostatMode } from '../types/thermostatmode';
import { ThermostatStateType } from '../types/thermostatstate';

/** API response shape for a single device. */
export interface DeviceResponse {
  /** Unique device identifier (UUID). */
  id: string;
  /** Human-readable device name. */
  deviceName: string;
  /** Location the device is installed in. */
  deviceLocation: string;
  /** Device category. */
  type: DeviceType;

  /** True when the device is considered "on" for display and filtering purposes. */
  isDeviceOn: boolean;
  /** Power state for devices that have an explicit on/off switch (lights, fans, thermostats). */
  isPoweredOn?: boolean | null;
  /** ISO timestamp of when the device was registered. */
  createdAt: string;
  /** ISO timestamp of the most recent state change. */
  updatedAt: string;

  /** Current brightness level for light devices (10–100). */
  lightBrightness?: number | null;
  /** Minimum allowed brightness for light devices. */
  minBrightness?: number | null;
  /** Maximum allowed brightness for light devices. */
  maxBrightness?: number | null;
  /** Current color (hex string) for light devices. */
  lightColor?: string | null;

  /** Current fan speed for fan devices. */
  fanSpeed?: FanSpeed | null;

  /** Active operating mode for thermostat devices. */
  thermostatMode?: ThermostatMode | null;
  /** Runtime state (Heating / Cooling / Idle / Off) for thermostat devices. */
  thermostatState?: ThermostatStateType | null;
  /** Minimum allowed target temperature for thermostat devices (°F). */
  minTemperature?: number | null;
  /** Maximum allowed target temperature for thermostat devices (°F). */
  maxTemperature?: number | null;
  /** User-set target temperature for thermostat devices (°F). */
  targetTemperature?: number | null;
  /** Default target temperature for thermostat devices (°F). */
  defaultTemperature?: number | null;
  /** Current ambient temperature in the device's location (°F). */
  ambientTemperature?: number | null;

  /** Lock state for door lock devices. */
  isLocked?: boolean | null;
}

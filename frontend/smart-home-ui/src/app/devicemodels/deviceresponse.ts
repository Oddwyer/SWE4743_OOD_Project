import { DeviceType } from '../types/devicetype';
import { FanSpeed } from '../types/fanspeed';
import { ThermostatMode } from '../types/thermostatmode';
import { ThermostatStateType } from '../types/thermostatstate';

export interface DeviceResponse {
  id: string;
  deviceName: string;
  deviceLocation: string;
  type: DeviceType;

  isDeviceOn: boolean;
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

  isLocked?: boolean | null;
}

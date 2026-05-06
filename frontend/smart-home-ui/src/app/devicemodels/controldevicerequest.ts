import { FanSpeed } from '../types/fanspeed';
import { ThermostatMode } from '../types/thermostatmode';
import { DeviceCommandType } from '../types/devicecommandtype';

export interface ControlDeviceRequest {
  type: DeviceCommandType;

  lightBrightness?: number | null;

  lightColor?: string | null;

  fanSpeed?: FanSpeed | null;

  thermostatMode?: ThermostatMode | null;

  targetTemperature?: number | null;

  isLocked?: boolean | null;
}

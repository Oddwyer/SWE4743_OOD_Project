import { FanSpeed } from '../types/fanspeed';
import { ThermostatMode } from '../types/thermostatmode';
import { DeviceCommandType } from '../types/devicecommandtype';

/** Request body for sending a command to a device. */
export interface ControlDeviceRequest {
  command: DeviceCommandType;

  brightness?: number | null;

  color?: string | null;

  fanSpeed?: FanSpeed | null;

  thermostatMode?: ThermostatMode | null;

  targetTemperature?: number | null;

  isLocked?: boolean | null;
}

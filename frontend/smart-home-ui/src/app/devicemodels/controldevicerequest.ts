import { FanSpeed } from '../types/fanspeed';
import { ThermostatMode } from '../types/thermostatmode';
import { DeviceCommandType } from '../types/devicecommandtype';

/** Request body for sending a command to a device. */
export interface ControlDeviceRequest {
  /** The command to execute on the device. */
  command: DeviceCommandType;

  /** Brightness value for SetBrightness commands (10–100). */
  brightness?: number | null;

  /** Hex color string (e.g. #FF0000) for SetColor commands. */
  color?: string | null;

  /** Fan speed for SetFanSpeed commands. */
  fanSpeed?: FanSpeed | null;

  /** Thermostat mode for SetThermostatMode commands. */
  thermostatMode?: ThermostatMode | null;

  /** Target temperature (°F) for SetTargetTemperature commands (60–80). */
  targetTemperature?: number | null;

  /** Lock state for ToggleLock commands. */
  isLocked?: boolean | null;
}

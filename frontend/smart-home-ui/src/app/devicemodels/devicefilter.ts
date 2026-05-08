import { DeviceType } from '../types/devicetype';

/** Filter criteria for querying devices from the API. */
export interface DeviceFilter {
  /** Filter by on/off state when provided. */
  powerState?: boolean;
  /** Filter by location name when provided. */
  location?: string;
  /** Filter by device type when provided. */
  deviceType?: DeviceType;
}

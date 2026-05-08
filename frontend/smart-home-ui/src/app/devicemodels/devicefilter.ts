import { DeviceType } from '../types/devicetype';

/** Filter criteria for querying devices from the API. */
export interface DeviceFilter {
  powerState?: boolean;

  location?: string;

  deviceType?: DeviceType;
}

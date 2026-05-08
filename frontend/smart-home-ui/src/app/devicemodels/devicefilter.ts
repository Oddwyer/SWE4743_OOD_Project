import { DeviceType } from '../types/devicetype';

export interface DeviceFilter {
  powerState?: boolean;
  location?: string;
  deviceType?: DeviceType;
}

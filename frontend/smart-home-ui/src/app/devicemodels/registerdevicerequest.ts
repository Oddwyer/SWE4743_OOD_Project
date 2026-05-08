import { DeviceType } from '../types/devicetype';

export interface RegisterDeviceRequest {
  deviceName: string;
  deviceLocation: string;
  type: DeviceType;
}

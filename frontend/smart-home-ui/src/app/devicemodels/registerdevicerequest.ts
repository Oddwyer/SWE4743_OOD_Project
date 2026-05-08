import { DeviceType } from '../types/devicetype';

/** Request body for registering a new device. */
export interface RegisterDeviceRequest {
  deviceName: string;

  deviceLocation: string;

  type: DeviceType;
}

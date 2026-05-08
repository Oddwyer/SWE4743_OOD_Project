import { DeviceType } from '../types/devicetype';

/** Request body for registering a new device. */
export interface RegisterDeviceRequest {
  /** Human-readable device name (minimum 2 characters). */
  deviceName: string;
  /** Location the device will be installed in. */
  deviceLocation: string;
  /** Device category to register. */
  type: DeviceType;
}

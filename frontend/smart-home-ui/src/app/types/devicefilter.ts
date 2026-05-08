import { DeviceType } from './devicetype';

/** Dashboard filter that selects all devices or only those matching a specific type. */
export type DeviceFilter = 'All' | DeviceType;

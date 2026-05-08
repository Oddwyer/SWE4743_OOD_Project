import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

import { DeviceResponse } from '../devicemodels/deviceresponse';
import { RegisterDeviceRequest } from '../devicemodels/registerdevicerequest';
import { ControlDeviceRequest } from '../devicemodels/controldevicerequest';
import { CommandHistoryResponse } from '../historymodels/commandhistoryresponse';

/**
 * Handles device-related API calls.
 */
@Injectable({
  providedIn: 'root',
})
export class DeviceApiService {
  private readonly baseUrl = `${environment.apiBaseUrl}/devices`;

  constructor(private readonly http: HttpClient) {}

  /**
   * Retrieve all devices.
   */
  getAllDevices(): Observable<DeviceResponse[]> {
    return this.http.get<DeviceResponse[]>(this.baseUrl);
  }

  /**
   * Retrieve a device by ID.
   */
  getDeviceById(deviceId: string): Observable<DeviceResponse> {
    return this.http.get<DeviceResponse>(`${this.baseUrl}/${deviceId}`);
  }

  /**
   * Register a new device.
   */
  registerDevice(request: RegisterDeviceRequest): Observable<DeviceResponse> {
    return this.http.post<DeviceResponse>(this.baseUrl, request);
  }

  /**
   * Remove a device by ID.
   */
  removeDevice(deviceId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${deviceId}`);
  }

  /**
   * Send a control command to a device.
   */
  controlDevice(deviceId: string, request: ControlDeviceRequest): Observable<DeviceResponse> {
    return this.http.post<DeviceResponse>(`${this.baseUrl}/${deviceId}/commands`, request);
  }

  /**
   * Retrieve command history for a device.
   */
  getCommandHistory(deviceId: string): Observable<CommandHistoryResponse[]> {
    return this.http.get<CommandHistoryResponse[]>(`${this.baseUrl}/${deviceId}/history`);
  }
}

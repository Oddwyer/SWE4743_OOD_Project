import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class DeviceApiService {
  private readonly baseUrl = `${environment.apiBaseUrl}/devices`;

  constructor(private readonly http: HttpClient) {}

  getDevices(): Observable<DeviceResponse[]> {
    return this.http.get<DeviceResponse[]>(this.baseUrl);
  }

  getDeviceById(deviceId: string): Observable<DeviceResponse> {
    return this.http.get<DeviceResponse>(`${this.baseUrl}/${deviceId}`);
  }

  registerDevice(request: RegisterDeviceRequest): Observable<DeviceResponse> {
    return this.http.post<DeviceResponse>(this.baseUrl, request);
  }

  deleteDevice(deviceId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${deviceId}`);
  }

  controlDevice(deviceId: string, request: ControlDeviceRequest): Observable<DeviceResponse> {
    return this.http.put<DeviceResponse>(`${this.baseUrl}/${deviceId}/commands`, request);
  }

  getCommandHistory(deviceId: string): Observable<CommandHistoryResponse[]> {
    return this.http.get<CommandHistoryResponse[]>(`${this.baseUrl}/${deviceId}/history`);
  }
}

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class DeviceService {

  constructor(private http: HttpClient) { }

  getDevices() {
    return this.http.get<any[]>('/api/devices');
  }

  getDevice(id: string) {
    return this.http.get(`/api/devices/${id}`);
  }

  updateDevice(id: string, data: any) {
    return this.http.put(`/api/devices/${id}`, data);
  }

}

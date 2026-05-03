import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class DeviceService {

  constructor(private http: HttpClient) { }

  getAllDevices() {
    return this.http.get<any[]>(`http://localhost:5000/api/devices`);
  }

  getDeviceById(id: string) {
    return this.http.get(`http://localhost:5000/api/devices/${id}`);
  }

  controlDevice(id: string, data: any) {
    return this.http.put(`http://localhost:5000/api/devices/${id}/commands`, data);
  }

  getDeviceHistory(id: string) {
    return this.http.get(`http://localhost:5000/api/devices/${id}/history`);
  }

  registerDevice(data: any) {
    return this.http.post(`http://localhost:5000/api/devices`, data);
  }

  removeDevice(id: string) {
    return this.http.delete(`http://localhost:5000/api/devices/${id}`);
  }


}

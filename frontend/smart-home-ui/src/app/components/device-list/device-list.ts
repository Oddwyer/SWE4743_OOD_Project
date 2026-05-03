import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DeviceService } from '../../services/device.service';

@Component({
  selector: 'app-device-list',
  templateUrl: './device-list.html',
  standalone: true,
  imports: [CommonModule],
  styleUrls: ['./device-list.css']
})

export class DeviceList implements OnInit {

  devices: any[] = [];

  constructor(private deviceService: DeviceService) { }

  ngOnInit(): void {
    this.loadDevices();
  }

  loadDevices() {
    this.deviceService.getAllDevices().subscribe({
      next: (data) => { this.devices = data; console.log('Devices loaded:', data); },
      error: (err) => { console.error('Error loading devices:', err); alert('Failed to load devices. Please try again later.'); }
    });
  }
}
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

  isLoading = true;

  loadDevices() {
    this.deviceService.getAllDevices().subscribe({
      next: (data) => {
        this.devices = data;
        this.isLoading = false;
        console.log('Devices loaded:', data);
      },
      error: (err) => {
        console.error('Error loading devices:', err);
        this.isLoading = false;
      }
    });
  }
}
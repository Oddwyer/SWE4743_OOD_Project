import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DeviceService } from '../../services/device.service';

// This component displays a list of all devices with their current status and controls.
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-device-list',
  templateUrl: './device-list.html',
  standalone: true,
  imports: [CommonModule, CardModule, TagModule, ButtonModule],
  styleUrls: ['./device-list.css']
})

export class DeviceList implements OnInit {
  devices: any[] = [];
  isLoading = true;

  constructor(
    private deviceService: DeviceService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    console.log('DeviceList INIT');
    this.loadDevices();
  }

  loadDevices() {
    this.deviceService.getAllDevices().subscribe({
      next: (data) => {
        this.devices = data;
        this.isLoading = false;
        console.log('Devices loaded:', data);
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading devices:', err);
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }
}
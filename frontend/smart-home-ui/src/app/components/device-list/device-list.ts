import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DeviceApiService } from '../../services/device.api.service';

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
    private deviceService: DeviceApiService,
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

  getDeviceIcon(type: string): string {
    switch (type?.toLowerCase()) {
      case 'thermostat':
        return 'pi pi-gauge';
      case 'fan':
        return 'pi pi-sync';
      case 'doorlock':
        return 'pi pi-lock';
      case 'light':
        return 'pi pi-lightbulb';
      default:
        return 'pi pi-home';
    }
  }
}
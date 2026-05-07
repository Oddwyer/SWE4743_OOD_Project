import { Component, signal } from '@angular/core';
import { DeviceListComponent } from './components/device-list/device-list';

@Component({
  selector: 'app-root',
  imports: [DeviceListComponent],
  standalone: true,
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected readonly title = signal('smart-home-ui');
}

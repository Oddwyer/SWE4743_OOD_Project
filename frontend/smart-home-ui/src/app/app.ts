import { Component, signal } from '@angular/core';
import { DeviceList } from './components/device-list/device-list';

@Component({
  selector: 'app-root',
  imports: [DeviceList],
  standalone: true,
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('smart-home-ui');
}

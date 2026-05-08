import { Component, signal } from '@angular/core';
import { DashboardComponent } from './components/dashboard/dashboard';

/** Root application component — renders the smart home dashboard. */
@Component({
  selector: 'app-root',
  imports: [DashboardComponent],
  standalone: true,
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected readonly title = signal('smart-home-ui');
}

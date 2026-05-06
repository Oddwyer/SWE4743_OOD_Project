import { Component } from '@angular/core';
import { CardModule } from 'primeng/card';

@Component({
  selector: 'app-simulation-card',
  standalone: true,
  imports: [CardModule],
  templateUrl: './simulation-card.html',
  styleUrl: './simulation-card.css',
})
export class SimulationCard {}

import { Component } from '@angular/core';
import { CardModule } from 'primeng/card';
import { SimulationApiService } from '../../services/simulation.api.service';

@Component({
  selector: 'app-simulation-card',
  standalone: true,
  imports: [CardModule],
  templateUrl: './simulation-card.html',
  styleUrl: './simulation-card.css',
})
export class SimulationCard {
  ambientTemp = 72; // default

  constructor(private simulationApiService: SimulationApiService) {}

  setAmbientTemp(event: Event) {
    const input = event.target as HTMLInputElement;
    const value = Number(input.value);

    this.ambientTemp = value;

    this.simulationApiService.setAmbientTemperature('Living Room', value).subscribe();
  }
}

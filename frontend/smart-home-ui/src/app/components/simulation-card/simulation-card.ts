import { Component, Input, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { SliderModule } from 'primeng/slider';
import { FormsModule } from '@angular/forms';

import { SimulationApiService } from '../../services/simulation.api.service';
import { AmbientTemperatureResponse } from '../../locationmodels/ambienttemperatureresponse';

@Component({
  selector: 'app-simulation-card',
  standalone: true,
  imports: [CommonModule, CardModule, SliderModule, FormsModule],
  templateUrl: './simulation-card.html',
  styleUrl: './simulation-card.css',
})
export class SimulationCard implements OnChanges {
  @Input() locations: string[] = [];

  ambientTemps: Record<string, number> = {};
  minTemp = 60;
  maxTemp = 80;

  constructor(private readonly simulationApiService: SimulationApiService) {}

  ngOnChanges(): void {
    this.loadAmbientTemperatures();
  }

  /**
   * Load ambient temperature for each thermostat location.
   */
  loadAmbientTemperatures(): void {
    this.locations.forEach((location) => {
      if (this.ambientTemps[location] !== undefined) {
        return;
      }

      this.simulationApiService.getAmbientTemperature(location).subscribe({
        next: (response: AmbientTemperatureResponse) => {
          this.ambientTemps[location] = response.ambientTemperature;

          this.minTemp = response.minTemperature;

          this.maxTemp = response.maxTemperature;
        },
      });
    });
  }

  /**
   * Update ambient temperature for a location.
   */
  setAmbientTemperature(location: string, temperature: number): void {
    const previousTemperature = this.ambientTemps[location];

    this.ambientTemps[location] = temperature;

    this.simulationApiService.setAmbientTemperature(location, temperature).subscribe({
      error: () => {
        this.ambientTemps[location] = previousTemperature;
      },
    });
  }
}

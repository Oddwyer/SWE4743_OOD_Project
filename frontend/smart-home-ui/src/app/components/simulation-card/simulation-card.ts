import { Component, EventEmitter, Input, OnChanges, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { SliderModule } from 'primeng/slider';
import { FormsModule } from '@angular/forms';

import { SimulationApiService } from '../../services/simulation.api.service';
import { AmbientTemperatureResponse } from '../../locationmodels/ambienttemperatureresponse';
import { SimulationSpeedRequest } from '../../simulationmodels/simulationspeedrequest';
import { SimulationSpeed } from '../../types/simulationspeed';

@Component({
  selector: 'app-simulation-card',
  standalone: true,
  imports: [CommonModule, CardModule, SliderModule, FormsModule],
  templateUrl: './simulation-card.html',
  styleUrl: './simulation-card.css',
})
export class SimulationCard implements OnChanges {
  @Input() locations: string[] = [];

  @Output() simulationChanged = new EventEmitter<void>();

  ambientTemps: Record<string, number> = {};
  minTemp = 60;
  maxTemp = 80;

  simulationSpeed: SimulationSpeed = SimulationSpeed.OneX;

  speedOptions = [
    { label: '1x', value: SimulationSpeed.OneX },
    { label: '2x', value: SimulationSpeed.TwoX },
    { label: '5x', value: SimulationSpeed.FiveX },
    { label: '10x', value: SimulationSpeed.TenX },
  ];

  constructor(private readonly simulationApiService: SimulationApiService) {}

  ngOnChanges(): void {
    this.loadAmbientTemperatures();
  }

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

  setAmbientTemperature(location: string, temperature: number): void {
    const previousTemperature = this.ambientTemps[location];

    this.ambientTemps[location] = temperature;

    this.simulationApiService.setAmbientTemperature(location, temperature).subscribe({
      next: () => {
        this.simulationChanged.emit();
      },
      error: () => {
        this.ambientTemps[location] = previousTemperature;
      },
    });
  }

  setSimulationSpeed(speed: SimulationSpeed): void {
    const previousSpeed = this.simulationSpeed;

    this.simulationSpeed = speed;

    const request: SimulationSpeedRequest = {
      speedMultiplier: speed,
    };

    this.simulationApiService.setSimulationSpeed(request).subscribe({
      error: () => {
        this.simulationSpeed = previousSpeed;
      },
    });
  }

  resetSimulation(): void {
    this.simulationApiService.resetSimulation().subscribe({
      next: () => {
        this.ambientTemps = {};
        this.loadAmbientTemperatures();
        this.simulationChanged.emit();
      },
    });
  }
}

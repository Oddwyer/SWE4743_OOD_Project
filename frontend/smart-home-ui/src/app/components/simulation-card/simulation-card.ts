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
  minTemp = 0;
  maxTemp = 100;
  defaultTemperature?: number;

  simulationSpeed: SimulationSpeed = SimulationSpeed.OneX;

  speedOptions = [
    { label: '1x', value: SimulationSpeed.OneX },
    { label: '2x', value: SimulationSpeed.TwoX },
    { label: '5x', value: SimulationSpeed.FiveX },
    { label: '10x', value: SimulationSpeed.TenX },
  ];

  constructor(private readonly simulationApiService: SimulationApiService) {}

  ngOnChanges(): void {
    if (this.locations.length === 0) {
      return;
    }

    const unloadedLocations = this.locations.filter(
      (location) => this.ambientTemps[location] === undefined,
    );

    if (unloadedLocations.length === 0) {
      return;
    }

    setTimeout(() => {
      this.loadAmbientTemperatures(unloadedLocations);
    });
  }

  loadAmbientTemperatures(locations: string[] = this.locations): void {
    locations.forEach((location) => {
      this.simulationApiService.getAmbientTemperature(location).subscribe({
        next: (response: AmbientTemperatureResponse) => {
          setTimeout(() => {
            this.ambientTemps[location] = response.ambientTemperature;
            this.minTemp = response.minTemperature;
            this.maxTemp = response.maxTemperature;
            this.defaultTemperature = response.defaultTemperature;
            console.log('SETTING TEMP:', location, response.ambientTemperature);
            console.log(this.ambientTemps);
          });
        },
        error: (err: unknown) => {
          console.error('Failed to load ambient temperature for', location, err);
        },
      });
    });
  }

  hasAmbientTemperature(location: string): boolean {
    return this.ambientTemps[location] !== undefined;
  }

  getAmbientTemperature(location: string): number {
    return this.ambientTemps[location];
  }

  setAmbientTemperature(location: string, temperature: number): void {
    const previousTemperature = this.ambientTemps[location];

    this.ambientTemps[location] = temperature;

    this.simulationApiService.setAmbientTemperature(location, temperature).subscribe({
      next: () => {
        this.simulationChanged.emit();
      },
      error: (err: unknown) => {
        this.ambientTemps[location] = previousTemperature;
        console.error('Failed to set ambient temperatures.', err);
      },
    });
  }

  getSimulationSpeedLabel(): string {
    const selectedOption = this.speedOptions.find(
      (option) => option.value === this.simulationSpeed,
    );

    return selectedOption?.label ?? '1x';
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
        setTimeout(() => {
          this.loadAmbientTemperatures();
        });
        this.simulationChanged.emit();
      },
    });
  }
}

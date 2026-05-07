import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnDestroy,
  Output,
} from '@angular/core';
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
export class SimulationCard implements OnChanges, OnDestroy {
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

  private refreshIntervalId?: number;
  private refreshInProgress = false;

  constructor(
    private readonly simulationApiService: SimulationApiService,
    private readonly changeDetectorRef: ChangeDetectorRef,
  ) {}

  ngOnChanges(): void {
    if (this.locations.length === 0) {
      return;
    }

    const unloadedLocations = this.locations.filter(
      (location) => this.ambientTemps[location] === undefined,
    );

    if (unloadedLocations.length > 0) {
      this.loadAmbientTemperatures(unloadedLocations);
    }

    this.startAmbientRefresh();
  }

  loadAmbientTemperatures(locations: string[] = this.locations): void {
    if (locations.length === 0 || this.refreshInProgress) {
      return;
    }

    this.refreshInProgress = true;

    let completedRequests = 0;
    const locationsToRefresh = [...locations];

    locationsToRefresh.forEach((location) => {
      this.simulationApiService.getAmbientTemperature(location).subscribe({
        next: (response: AmbientTemperatureResponse) => {
          this.ambientTemps[location] = response.ambientTemperature;
          this.minTemp = response.minTemperature;
          this.maxTemp = response.maxTemperature;
          this.defaultTemperature = response.defaultTemperature;
        },
        error: (err: unknown) => {
          console.error('Failed to load ambient temperature for', location, err);
        },
        complete: () => {
          completedRequests++;

          if (completedRequests === locationsToRefresh.length) {
            this.refreshInProgress = false;
            this.changeDetectorRef.detectChanges();
          }
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
        this.changeDetectorRef.detectChanges();
      },
      error: (err: unknown) => {
        this.ambientTemps[location] = previousTemperature;
        console.error('Failed to set ambient temperatures.', err);
        this.changeDetectorRef.detectChanges();
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
      next: () => {
        this.restartAmbientRefresh();
      },
      error: () => {
        this.simulationSpeed = previousSpeed;
      },
    });
  }

  resetSimulation(): void {
    this.simulationApiService.resetSimulation().subscribe({
      next: () => {
        this.ambientTemps = {};
        this.refreshInProgress = false;
        this.loadAmbientTemperatures();
        this.simulationChanged.emit();
      },
    });
  }

  private getRefreshInterval(): number {
    const baseInterval = 5000;
    const speed = Number(this.simulationSpeed);

    return Math.max(baseInterval / speed, 2000);
  }

  private startAmbientRefresh(): void {
    if (this.refreshIntervalId !== undefined) {
      return;
    }

    this.refreshIntervalId = window.setInterval(() => {
      this.loadAmbientTemperatures();
    }, this.getRefreshInterval());
  }

  private restartAmbientRefresh(): void {
    if (this.refreshIntervalId !== undefined) {
      window.clearInterval(this.refreshIntervalId);
      this.refreshIntervalId = undefined;
    }

    this.refreshInProgress = false;
    this.startAmbientRefresh();
  }

  ngOnDestroy(): void {
    if (this.refreshIntervalId !== undefined) {
      window.clearInterval(this.refreshIntervalId);
      this.refreshIntervalId = undefined;
    }

    this.refreshInProgress = false;
  }
}

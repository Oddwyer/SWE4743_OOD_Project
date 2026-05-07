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

/**
 * Displays and manages simulation settings such as:
 * - ambient temperatures
 * - simulation speed
 * - simulation reset
 *
 * Also handles frontend polling to keep Angular synced
 * with backend simulation changes.
 */
export class SimulationCardComponent implements OnChanges, OnDestroy {
  @Input() locations: string[] = [];
  @Output() simulationChanged = new EventEmitter<void>();

  /**
   * Stores the active polling interval so it can be stopped/restarted safely.
   */
  private refreshIntervalId?: number;

  /**
   * Prevents overlapping ambient temperature requests.
   */
  private refreshInProgress = false;

  ambientTemps: Record<string, number> = {};
  displayAmbientTemps: Record<string, number> = {};
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

  constructor(
    private readonly simulationApiService: SimulationApiService,
    private readonly changeDetectorRef: ChangeDetectorRef,
  ) {}

  /**
   * Loads initial ambient temperatures and starts polling
   * when locations become available.
   */
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

  /**
   * Loads ambient temperatures for one or more locations.
   */
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

          if (this.displayAmbientTemps[location] === undefined) {
            this.displayAmbientTemps[location] = response.ambientTemperature;
          } else {
            this.updateDisplayedAmbientTemperature(location, response.ambientTemperature);
          }

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

  /**
   * Returns whether a location has a loaded ambient temperature.
   */
  hasAmbientTemperature(location: string): boolean {
    return this.ambientTemps[location] !== undefined;
  }

  /**
   * Gets the current ambient temperature for a location.
   */
  getAmbientTemperature(location: string): number {
    return this.ambientTemps[location];
  }

  /**
   * Updates the ambient temperature for a location.
   */
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

  /**
   * Gets the display label for the current simulation speed.
   */
  getSimulationSpeedLabel(): string {
    const selectedOption = this.speedOptions.find(
      (option) => option.value === this.simulationSpeed,
    );

    return selectedOption?.label ?? '1x';
  }

  /**
   * Updates backend simulation speed and restarts polling
   * using the new refresh interval.
   */
  setSimulationSpeed(speed: SimulationSpeed): void {
    const previousSpeed = this.simulationSpeed;

    this.simulationSpeed = speed;

    const request: SimulationSpeedRequest = {
      speedMultiplier: speed,
    };

    this.simulationApiService.setSimulationSpeed(request).subscribe({
      next: () => {
        this.restartAmbientRefresh();
        this.simulationChanged.emit();
      },
      error: () => {
        this.simulationSpeed = previousSpeed;
      },
    });
  }

  /**
   * Resets the simulation and reloads ambient temperatures.
   */
  resetSimulation(): void {
    this.simulationApiService.resetSimulation().subscribe({
      next: () => {
        this.ambientTemps = {};
        this.displayAmbientTemps = {};
        this.refreshInProgress = false;
        this.restartAmbientRefresh();
        this.loadAmbientTemperatures();
        this.simulationChanged.emit();
      },
    });
  }
  /**
   * Calculates the frontend polling interval based on simulation speed.
   * Uses a minimum interval to avoid excessive polling requests.
   */
  private getRefreshInterval(): number {
    const baseInterval = 5000;
    const speed = Number(this.simulationSpeed);

    return Math.max(baseInterval / speed, 2000);
  }

  /**
   * Starts frontend polling for ambient temperature updates.
   */
  private startAmbientRefresh(): void {
    if (this.refreshIntervalId !== undefined) {
      return;
    }

    this.refreshIntervalId = window.setInterval(() => {
      this.loadAmbientTemperatures();
    }, this.getRefreshInterval());
  }

  /**
   * Stops the current polling interval and starts a new one.
   */
  private restartAmbientRefresh(): void {
    if (this.refreshIntervalId !== undefined) {
      window.clearInterval(this.refreshIntervalId);
      this.refreshIntervalId = undefined;
    }

    this.refreshInProgress = false;
    this.startAmbientRefresh();
  }

  /**
   * Smoothly updates displayed ambient temperature values.
   */
  private updateDisplayedAmbientTemperature(location: string, targetTemperature: number): void {
    const currentTemperature = this.displayAmbientTemps[location] ?? targetTemperature;

    if (currentTemperature === targetTemperature) {
      this.displayAmbientTemps[location] = targetTemperature;
      return;
    }

    const direction = targetTemperature > currentTemperature ? 1 : -1;

    this.displayAmbientTemps[location] = currentTemperature + direction;
  }

  /**
   * Gets the smoothed display temperature for a location.
   */
  getDisplayAmbientTemperature(location: string): number {
    const displayTemperature = this.displayAmbientTemps[location];

    if (displayTemperature !== undefined) {
      return displayTemperature;
    }

    return this.getAmbientTemperature(location);
  }

  /**
   * Cleans up polling when the component is destroyed.
   */
  ngOnDestroy(): void {
    if (this.refreshIntervalId !== undefined) {
      window.clearInterval(this.refreshIntervalId);
      this.refreshIntervalId = undefined;
    }

    this.refreshInProgress = false;
  }
}

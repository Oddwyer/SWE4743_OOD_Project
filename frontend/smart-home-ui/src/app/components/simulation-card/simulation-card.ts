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
  styleUrls: ['./simulation-card.css'],
})
/**
 * Displays and manages simulation settings.
 *
 * Handles ambient temperature controls, simulation speed selection,
 * reset behavior, and frontend polling for simulation updates.
 */
export class SimulationCardComponent implements OnChanges, OnDestroy {
  @Input() locations: string[] = [];

  @Output() simulationChanged = new EventEmitter<void>();
  @Output() speedChanged = new EventEmitter<number>();
  @Output() simulationRunningChanged = new EventEmitter<boolean>();
  @Output() simulationReset = new EventEmitter<void>();

  ambientTemps: Record<string, number> = {};
  displayAmbientTemps: Record<string, number> = {};

  minTemp = 0;
  maxTemp = 100;

  simulationSpeed: SimulationSpeed = SimulationSpeed.OneX;

  speedOptions = [
    { label: '1x', value: SimulationSpeed.OneX },
    { label: '2x', value: SimulationSpeed.TwoX },
    { label: '5x', value: SimulationSpeed.FiveX },
    { label: '10x', value: SimulationSpeed.TenX },
  ];

  private refreshIntervalId?: number;
  private refreshInProgress = false;
  private simulationIsRunning = false;

  constructor(
    private readonly simulationApiService: SimulationApiService,
    private readonly changeDetectorRef: ChangeDetectorRef,
  ) {}

  /**
   * Loads ambient temperatures and starts polling when thermostat locations change.
   */
  ngOnChanges(): void {
    if (this.locations.length === 0) {
      this.setSimulationRunning(false);
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
   * Loads ambient temperatures for the provided locations.
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

          this.updateSimulationRunningState();
        },
        error: (err: unknown) => {
          console.error('Failed to load ambient temperature for', location, err);
        },
        complete: () => {
          completedRequests++;

          if (completedRequests === locationsToRefresh.length) {
            this.refreshInProgress = false;
            this.updateSimulationRunningState();
            this.changeDetectorRef.detectChanges();
          }
        },
      });
    });
  }

  /**
   * Returns true when the ambient temperature for a location has loaded.
   */
  hasAmbientTemperature(location: string): boolean {
    return this.ambientTemps[location] !== undefined;
  }

  /**
   * Returns the current ambient temperature for a location.
   */
  getAmbientTemperature(location: string): number {
    return this.ambientTemps[location];
  }

  /**
   * Returns the smoothed display temperature for a location.
   */
  getDisplayAmbientTemperature(location: string): number {
    const displayTemperature = this.displayAmbientTemps[location];

    if (displayTemperature !== undefined) {
      return displayTemperature;
    }

    return this.getAmbientTemperature(location);
  }

  /**
   * Updates the ambient temperature for a location.
   */
  setAmbientTemperature(location: string, temperature: number): void {
    const previousTemperature = this.ambientTemps[location];

    this.ambientTemps[location] = temperature;
    this.setSimulationRunning(true);

    this.simulationApiService.setAmbientTemperature(location, temperature).subscribe({
      next: () => {
        this.simulationChanged.emit();
        this.changeDetectorRef.detectChanges();
      },
      error: (err: unknown) => {
        this.ambientTemps[location] = previousTemperature;
        this.updateSimulationRunningState();

        console.error('Failed to set ambient temperature.', err);
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  /**
   * Updates the backend simulation speed and restarts frontend polling.
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
        this.speedChanged.emit(this.getSpeedMultiplier(speed));
        this.updateSimulationRunningState();
      },
      error: (err: unknown) => {
        this.simulationSpeed = previousSpeed;
        console.error('Failed to set simulation speed.', err);
      },
    });
  }

  /**
   * Resets the simulation and reloads dashboard simulation data.
   */
  resetSimulation(): void {
    this.simulationApiService.resetSimulation().subscribe({
      next: () => {
        this.simulationSpeed = SimulationSpeed.OneX;

        this.ambientTemps = {};
        this.displayAmbientTemps = {};
        this.refreshInProgress = false;

        this.setSimulationRunning(false);

        this.speedChanged.emit(this.getSpeedMultiplier(SimulationSpeed.OneX));
        this.simulationReset.emit();
        this.restartAmbientRefresh();
        this.loadAmbientTemperatures();
        this.simulationChanged.emit();

        this.changeDetectorRef.detectChanges();
      },
      error: (err: unknown) => {
        console.error('Failed to reset simulation.', err);
      },
    });
  }

  /**
   * Converts a simulation speed enum into its numeric multiplier.
   */
  private getSpeedMultiplier(speed: SimulationSpeed): number {
    const multipliers: Record<SimulationSpeed, number> = {
      [SimulationSpeed.OneX]: 1,
      [SimulationSpeed.TwoX]: 2,
      [SimulationSpeed.FiveX]: 5,
      [SimulationSpeed.TenX]: 10,
    };

    return multipliers[speed] ?? 1;
  }

  /**
   * Calculates the frontend polling interval from the current speed.
   */
  private getRefreshInterval(): number {
    const baseInterval = 5000;
    const speedMultiplier = this.getSpeedMultiplier(this.simulationSpeed);

    return Math.max(baseInterval / speedMultiplier, 2000);
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
   * Moves the displayed temperature one degree toward the latest backend value.
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
   * Emits whether the simulation is still visibly adjusting ambient temperature.
   */
  private updateSimulationRunningState(): void {
    const isRunning = this.locations.some((location) => {
      const actualTemperature = this.ambientTemps[location];
      const displayedTemperature = this.displayAmbientTemps[location];

      return (
        actualTemperature !== undefined &&
        displayedTemperature !== undefined &&
        actualTemperature !== displayedTemperature
      );
    });

    this.setSimulationRunning(isRunning);
  }

  /**
   * Emits simulation running changes only when the running state changes.
   */
  private setSimulationRunning(isRunning: boolean): void {
    if (this.simulationIsRunning === isRunning) {
      return;
    }

    this.simulationIsRunning = isRunning;
    this.simulationRunningChanged.emit(isRunning);
  }

  /**
   * Clears polling when the component is destroyed.
   */
  ngOnDestroy(): void {
    if (this.refreshIntervalId !== undefined) {
      window.clearInterval(this.refreshIntervalId);
      this.refreshIntervalId = undefined;
    }

    this.refreshInProgress = false;
    this.setSimulationRunning(false);
  }
}

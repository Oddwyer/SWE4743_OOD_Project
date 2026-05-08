import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

import { SimulationResponse } from '../simulationmodels/simulationresponse';
import { SimulationSpeedRequest } from '../simulationmodels/simulationspeedrequest';
import { AmbientTemperatureResponse } from '../locationmodels/ambienttemperatureresponse';

/**
 * Handles simulation-related API calls.
 */
@Injectable({
  providedIn: 'root',
})
export class SimulationApiService {
  private readonly simulationUrl = `${environment.apiBaseUrl}/simulation`;
  private readonly locationsUrl = `${environment.apiBaseUrl}/locations`;

  constructor(private readonly http: HttpClient) {}

  /**
   * Update simulation speed.
   */
  setSimulationSpeed(request: SimulationSpeedRequest): Observable<SimulationResponse> {
    return this.http.put<SimulationResponse>(`${this.simulationUrl}/speed`, request);
  }

  /**
   * Reset simulation state.
   */
  resetSimulation(): Observable<SimulationResponse> {
    return this.http.post<SimulationResponse>(`${this.simulationUrl}/reset`, {});
  }

  /**
   * Set ambient temperature for a location.
   */
  setAmbientTemperature(
    location: string,
    temperature: number,
  ): Observable<AmbientTemperatureResponse> {
    return this.http.put<AmbientTemperatureResponse>(
      `${this.locationsUrl}/${encodeURIComponent(location)}/ambient-temperature`,
      { temperature },
    );
  }

  /**
   * Get ambient temperature for a location.
   */
  getAmbientTemperature(location: string): Observable<AmbientTemperatureResponse> {
    return this.http.get<AmbientTemperatureResponse>(
      `${this.locationsUrl}/${encodeURIComponent(location)}/ambient-temperature`,
    );
  }
}

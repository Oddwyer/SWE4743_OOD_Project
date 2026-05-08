/** Request body for setting the ambient temperature of a location. */
export interface AmbientTemperatureRequest {
  /** New ambient temperature in °F (0–100). */
  temperature: number;
}

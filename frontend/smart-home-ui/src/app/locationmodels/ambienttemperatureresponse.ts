/** API response shape for ambient temperature data at a location. */
export interface AmbientTemperatureResponse {
  /** Location name. */
  location: string;
  /** Current ambient temperature in °F. */
  ambientTemperature: number;
  /** Minimum allowed ambient temperature (°F). */
  minTemperature: number;
  /** Maximum allowed ambient temperature (°F). */
  maxTemperature: number;
  /** Default ambient temperature used on reset (°F). */
  defaultTemperature: number;
}

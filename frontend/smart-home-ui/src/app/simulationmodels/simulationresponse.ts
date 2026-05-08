import { SimulationSpeed } from '../types/simulationspeed';

/** API response shape for simulation control operations. */
export interface SimulationResponse {
  /** Human-readable result message. */
  message: string;
  /** Speed multiplier active after the operation. */
  type: SimulationSpeed;
}

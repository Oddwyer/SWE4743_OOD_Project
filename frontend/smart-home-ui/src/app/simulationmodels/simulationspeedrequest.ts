import { SimulationSpeed } from '../types/simulationspeed';

/** Request body for changing the simulation speed. */
export interface SimulationSpeedRequest {
  /** Desired speed multiplier. */
  speedMultiplier: SimulationSpeed;
}

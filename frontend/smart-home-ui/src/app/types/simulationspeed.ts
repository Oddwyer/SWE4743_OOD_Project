/** Simulation speed multipliers controlling how fast simulation time advances. */
export enum SimulationSpeed {
  /** Normal speed — 1 second of simulation per real second. */
  OneX = 'OneX',
  /** 2× speed — simulation advances twice as fast as real time. */
  TwoX = 'TwoX',
  /** 5× speed — simulation advances five times faster than real time. */
  FiveX = 'FiveX',
  /** 10× speed — simulation advances ten times faster than real time. */
  TenX = 'TenX',
}

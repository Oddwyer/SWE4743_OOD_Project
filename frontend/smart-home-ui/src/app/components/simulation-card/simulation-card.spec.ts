import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SimulationCardComponent } from './simulation-card';
import { SimulationApiService } from '../../services/simulation.api.service';
import { Subject } from 'rxjs';

describe('SimulationCard', () => {
  let component: SimulationCardComponent;
  let fixture: ComponentFixture<SimulationCardComponent>;

  beforeEach(async () => {
    const simulationApiSpy = {
      getAmbientTemperature: vi.fn().mockReturnValue(new Subject<never>().asObservable()),
      setAmbientTemperature: vi.fn(),
      setSimulationSpeed: vi.fn(),
      resetSimulation: vi.fn(),
    };

    await TestBed.configureTestingModule({
      imports: [SimulationCardComponent],
      providers: [{ provide: SimulationApiService, useValue: simulationApiSpy }],
    }).compileComponents();

    fixture = TestBed.createComponent(SimulationCardComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  // ── onSliderDrag ─────────────────────────────────────────────────────────────

  describe('onSliderDrag', () => {
    it('immediately updates ambientTemps so the slider position is stable', () => {
      component['ambientTemps']['Bedroom'] = 65;
      component.onSliderDrag('Bedroom', 72);
      expect(component['ambientTemps']['Bedroom']).toBe(72);
    });

    it('immediately updates displayAmbientTemps so the number tracks the handle', () => {
      component['displayAmbientTemps']['Bedroom'] = 65;
      component.onSliderDrag('Bedroom', 72);
      expect(component['displayAmbientTemps']['Bedroom']).toBe(72);
    });

    it('keeps ambientTemps and displayAmbientTemps in sync after a drag', () => {
      component.onSliderDrag('Kitchen', 80);
      expect(component['ambientTemps']['Kitchen']).toBe(component['displayAmbientTemps']['Kitchen']);
    });
  });
});

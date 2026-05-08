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
});

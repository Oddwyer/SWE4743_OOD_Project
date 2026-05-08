import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SimulationCard } from './simulation-card';

describe('SimulationCard', () => {
  let component: SimulationCard;
  let fixture: ComponentFixture<SimulationCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SimulationCard],
    }).compileComponents();

    fixture = TestBed.createComponent(SimulationCard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

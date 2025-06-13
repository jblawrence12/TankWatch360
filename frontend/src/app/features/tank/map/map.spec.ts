import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TankMap } from './map';

describe('TankMap', () => {
  let component: TankMap;
  let fixture: ComponentFixture<TankMap>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TankMap]          // standalone component
    }).compileComponents();

    fixture = TestBed.createComponent(TankMap);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

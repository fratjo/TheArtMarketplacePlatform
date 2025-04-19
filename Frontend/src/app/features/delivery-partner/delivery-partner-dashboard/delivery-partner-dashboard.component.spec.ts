import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeliveryPartnerDashboardComponent } from './delivery-partner-dashboard.component';

describe('DeliveryPartnerDashboardComponent', () => {
  let component: DeliveryPartnerDashboardComponent;
  let fixture: ComponentFixture<DeliveryPartnerDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeliveryPartnerDashboardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DeliveryPartnerDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

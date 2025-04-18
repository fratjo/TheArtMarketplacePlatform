import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterDeliveryPartnerComponent } from './register-delivery-partner.component';

describe('RegisterDeliveryPartnerComponent', () => {
  let component: RegisterDeliveryPartnerComponent;
  let fixture: ComponentFixture<RegisterDeliveryPartnerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegisterDeliveryPartnerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RegisterDeliveryPartnerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

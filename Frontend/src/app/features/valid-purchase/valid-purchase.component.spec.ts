import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ValidPurchaseComponent } from './valid-purchase.component';

describe('ValidPurchaseComponent', () => {
  let component: ValidPurchaseComponent;
  let fixture: ComponentFixture<ValidPurchaseComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ValidPurchaseComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ValidPurchaseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserAdminPanelComponent } from './user-admin-panel.component';

describe('UserAdminPanelComponent', () => {
  let component: UserAdminPanelComponent;
  let fixture: ComponentFixture<UserAdminPanelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserAdminPanelComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserAdminPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

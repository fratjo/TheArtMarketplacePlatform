import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterArtisanComponent } from './register-artisan.component';

describe('RegisterArtisanComponent', () => {
  let component: RegisterArtisanComponent;
  let fixture: ComponentFixture<RegisterArtisanComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegisterArtisanComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RegisterArtisanComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

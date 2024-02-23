import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExtraFunctionsModalComponent } from './extra-functions-modal.component';

describe('ExtraFunctionsModalComponent', () => {
  let component: ExtraFunctionsModalComponent;
  let fixture: ComponentFixture<ExtraFunctionsModalComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ExtraFunctionsModalComponent]
    });
    fixture = TestBed.createComponent(ExtraFunctionsModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

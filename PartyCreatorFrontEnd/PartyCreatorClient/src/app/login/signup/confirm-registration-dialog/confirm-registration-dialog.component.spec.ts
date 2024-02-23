import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmRegistrationDialogComponent } from './confirm-registration-dialog.component';

describe('ConfirmRegistrationDialogComponent', () => {
  let component: ConfirmRegistrationDialogComponent;
  let fixture: ComponentFixture<ConfirmRegistrationDialogComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ConfirmRegistrationDialogComponent]
    });
    fixture = TestBed.createComponent(ConfirmRegistrationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

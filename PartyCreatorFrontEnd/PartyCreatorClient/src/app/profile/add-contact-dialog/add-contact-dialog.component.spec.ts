import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddContactDialogComponent } from './add-contact-dialog.component';

describe('AddContactDialogComponent', () => {
  let component: AddContactDialogComponent;
  let fixture: ComponentFixture<AddContactDialogComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AddContactDialogComponent]
    });
    fixture = TestBed.createComponent(AddContactDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

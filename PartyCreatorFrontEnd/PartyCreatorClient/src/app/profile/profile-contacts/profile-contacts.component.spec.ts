import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfileContactsComponent } from './profile-contacts.component';

describe('ProfileContactsComponent', () => {
  let component: ProfileContactsComponent;
  let fixture: ComponentFixture<ProfileContactsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ProfileContactsComponent]
    });
    fixture = TestBed.createComponent(ProfileContactsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

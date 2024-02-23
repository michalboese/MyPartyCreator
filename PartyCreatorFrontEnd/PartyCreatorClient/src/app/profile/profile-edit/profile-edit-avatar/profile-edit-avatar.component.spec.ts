import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfileEditAvatarComponent } from './profile-edit-avatar.component';

describe('ProfileEditAvatarComponent', () => {
  let component: ProfileEditAvatarComponent;
  let fixture: ComponentFixture<ProfileEditAvatarComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ProfileEditAvatarComponent]
    });
    fixture = TestBed.createComponent(ProfileEditAvatarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

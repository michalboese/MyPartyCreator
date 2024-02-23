import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NavMenuMainComponent } from './nav-menu-main.component';

describe('NavMenuMainComponent', () => {
  let component: NavMenuMainComponent;
  let fixture: ComponentFixture<NavMenuMainComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [NavMenuMainComponent]
    });
    fixture = TestBed.createComponent(NavMenuMainComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

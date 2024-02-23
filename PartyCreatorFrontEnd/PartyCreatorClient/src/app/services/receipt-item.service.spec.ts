import { TestBed } from '@angular/core/testing';

import { ReceiptItemService } from './receipt-item.service';

describe('ReceiptItemService', () => {
  let service: ReceiptItemService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ReceiptItemService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

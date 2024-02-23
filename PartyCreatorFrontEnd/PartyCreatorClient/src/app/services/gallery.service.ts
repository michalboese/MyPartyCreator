import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { PhotoDto } from '../interfaces/photo-dto';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class GalleryService {
  constructor(private http: HttpClient) {}
  private baseUrl = environment.apiUrl + 'BlobStorage/';

  GetImagesFromEvent(eventId: number) {
    return this.http.get<PhotoDto[]>(
      `${this.baseUrl}GetImagesFromEvent/${eventId}`
    );
  }

  UploadImage(formData: FormData) {
    return this.http.post<PhotoDto>(`${this.baseUrl}UploadBlobFile`, formData);
  }
  
  DeleteImage(imageId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}DeleteBlobFile/${imageId}`);
  }
}
  


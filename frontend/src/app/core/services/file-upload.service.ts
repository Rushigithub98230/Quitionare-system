import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface FileUploadResponse {
  success: boolean;
  fileUrl?: string;
  fileName?: string;
  message?: string;
}

@Injectable({
  providedIn: 'root'
})
export class FileUploadService {
  private apiUrl = `${environment.apiUrl}/upload`;

  constructor(private http: HttpClient) {}

  uploadFile(file: File): Observable<FileUploadResponse> {
    const formData = new FormData();
    formData.append('file', file);
    
    return this.http.post<FileUploadResponse>(this.apiUrl, formData);
  }

  uploadMultipleFiles(files: File[]): Observable<FileUploadResponse[]> {
    const formData = new FormData();
    files.forEach((file, index) => {
      formData.append(`files[${index}]`, file);
    });
    
    return this.http.post<FileUploadResponse[]>(`${this.apiUrl}/multiple`, formData);
  }
} 
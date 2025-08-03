import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { ApiResponse } from '../models/api-response.model';

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
  constructor(private apiService: ApiService) {}

  uploadFile(file: File): Observable<ApiResponse<FileUploadResponse>> {
    const formData = new FormData();
    formData.append('file', file);
    
    return this.apiService.post<FileUploadResponse>('/upload', formData);
  }

  uploadMultipleFiles(files: File[]): Observable<ApiResponse<FileUploadResponse[]>> {
    const formData = new FormData();
    files.forEach((file, index) => {
      formData.append(`files[${index}]`, file);
    });
    
    return this.apiService.post<FileUploadResponse[]>('/upload/multiple', formData);
  }
} 
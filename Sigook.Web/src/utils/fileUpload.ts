import http from '@/security/apiService';
import type { AxiosResponse } from 'axios';

interface UploadedFile {
  path: string;
}

export function uploadFile(evt: Event | File, type: string, name: string): Promise<string> {
  let urlImage: File;
  if (evt instanceof Event && (evt.target as HTMLInputElement).files) {
    urlImage = ((evt.target as HTMLInputElement).files as FileList)[0];
  } else {
    urlImage = evt as File;
  }
  return new Promise((resolve, reject) => {
    if (urlImage) {
      const formData = new FormData();
      formData.append('files', urlImage);
      http.post<UploadedFile[]>(`/api/file/${type}?tag=${name}`, formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
      })
        .then((response: AxiosResponse<UploadedFile[]>) => resolve(response.data[0].path))
        .catch((error: any) => reject(error));
    }
  });
}

export function deleteFile(fileName: string): Promise<void> {
  return new Promise((resolve, reject) => {
    http.delete(`/api/file/${fileName}`)
      .then(() => resolve())
      .catch(() => reject());
  });
}

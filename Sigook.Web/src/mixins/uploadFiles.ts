import http from '../security/apiService';

export default {
  methods: {
    uploadFile(evt: Event | File, type: string, name: string): Promise<string> {
      let urlImage: File;
      if (evt instanceof Event && (evt.target as HTMLInputElement).files) {
        urlImage = ((evt.target as HTMLInputElement).files as FileList)[0];
      } else {
        urlImage = evt as File;
      }
      return new Promise((resolve, reject) => {
        if (urlImage) {
          const formData = new FormData();
          formData.append("files", urlImage);
          http.post(`/api/file/${type}?tag=${name}`, formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
          })
            .then((response: any) => resolve(response.data[0].path))
            .catch((error: any) => reject(error.response));
        }
      });
    },
    deleteFile(fileName: string): Promise<void> {
      (this as any).isLoading = true;
      return new Promise((resolve, reject) => {
        http.delete(`/api/file/${fileName}`)
          .then(() => {
            (this as any).isLoading = false;
            resolve();
          })
          .catch(() => {
            (this as any).isLoading = false;
            reject();
          });
      });
    }
  }
};

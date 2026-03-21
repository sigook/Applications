import http from '../security/apiService';
import compress from './compressFiles';

export default {
  methods: {
    uploadImage(evt: File, type: string, name: string): Promise<string> {
      const urlImage = evt;
      return new Promise((resolve, reject) => {
        if (urlImage) {
          (this as any).compressFile(urlImage)
            .then((response: Blob) => {
              const compressed = response;
              if (compressed) {
                const formData = new FormData();
                formData.append("files", compressed, (compressed as any).name);
                http.post(`/api/file/${type}?tag=${name}`, formData, {
                  headers: { 'Content-Type': 'multipart/form-data' }
                })
                  .then((response: any) => resolve(response.data.path))
                  .catch((error: any) => reject(error.response));
              }
            });
        }
      });
    }
  },
  mixins: [compress]
};

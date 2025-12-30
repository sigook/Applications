import http from '../security/apiService';
import compress from './compressFiles';

export default {
  methods: {
    uploadImage(evt, type, name) {
      let urlImage = evt;
      return new Promise((resolve, reject) => {
        if (urlImage) {
          this.compressFile(urlImage)
            .then(response => {
              urlImage = response;
              if (urlImage) {
                const formData = new FormData();
                formData.append("files", urlImage, urlImage.name);
                http.post(`/api/file/${type}?tag=${name}`, formData, {
                  headers: { 'Content-Type': 'multipart/form-data' }
                })
                  .then(response => resolve(response.data.path))
                  .catch(error => reject(error.response));
              }
            });
        }
      });
    }
  },
  mixins: [compress]
};


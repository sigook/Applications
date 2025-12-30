import http from '../security/apiService';

export default {
  methods: {
    uploadFile(evt, type, name) {
      let urlImage;
      if (evt.target) {
        urlImage = evt.target.files[0];
      } else {
        urlImage = evt;
      }
      return new Promise((resolve, reject) => {
        if (urlImage) {
          const formData = new FormData();
          formData.append("files", urlImage);
          http.post(`/api/file/${type}?tag=${name}`, formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
          })
            .then(response => resolve(response.data[0].path))
            .catch(error => reject(error.response));
        }
      });
    },
    deleteFile(fileName) {
      this.isLoading = true;
      return new Promise((resolve, reject) => {
        http.delete(`/api/file/${fileName}`)
          .then(() => {
            this.isLoading = false;
            resolve();
          })
          .catch(() => {
            this.isLoading = false;
            reject();
          })
      })
    }
  }
};


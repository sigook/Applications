export default {
  methods: {
    downloadFile(response, name) {
      let blob = response;
      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(blob, name);
      } else {
        var downloadUrl = URL.createObjectURL(blob);
        //window.open(downloadUrl);
        var a = document.createElement("a");
        a.style.display = "none";
        a.href = downloadUrl;
        a.download = name + ".xlsx";
        document.body.appendChild(a);
        a.click();
      }
    },
    downloadFileGeneric(response, name) {
      let blob = response;
      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(blob, name);
      } else {
        var downloadUrl = URL.createObjectURL(blob);
        var a = document.createElement("a");
        a.style.display = "none";
        a.href = downloadUrl;
        a.download = name;
        document.body.appendChild(a);
        a.click();
      }
    },
    downloadPDF(response, name) {
      let blob = response;
      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(blob, name);
      } else {
        var downloadUrl = URL.createObjectURL(blob);
        var a = document.createElement("a");
        a.style.display = "none";
        a.href = downloadUrl;
        a.download = name + ".pdf";
        document.body.appendChild(a);
        a.click();
      }
    }
  }
};
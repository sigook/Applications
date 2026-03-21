export default {
  methods: {
    downloadFile(response: Blob, name: string): void {
      const blob = response;
      const downloadUrl = URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.style.display = "none";
      a.href = downloadUrl;
      a.download = name + ".xlsx";
      document.body.appendChild(a);
      a.click();
    },
    downloadFileGeneric(response: Blob, name: string): void {
      const blob = response;
      const downloadUrl = URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.style.display = "none";
      a.href = downloadUrl;
      a.download = name;
      document.body.appendChild(a);
      a.click();
    },
    downloadPDF(response: Blob, name: string): void {
      const blob = response;
      const downloadUrl = URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.style.display = "none";
      a.href = downloadUrl;
      a.download = name + ".pdf";
      document.body.appendChild(a);
      a.click();
    }
  }
};

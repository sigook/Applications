function triggerDownload(blob: Blob, fileName: string): void {
  const downloadUrl = URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.style.display = 'none';
  a.href = downloadUrl;
  a.download = fileName;
  document.body.appendChild(a);
  a.click();
}

export function downloadFile(response: Blob, name: string): void {
  triggerDownload(response, `${name}.xlsx`);
}

export function downloadFileGeneric(response: Blob, name: string): void {
  triggerDownload(response, name);
}

export function downloadPDF(response: Blob, name: string): void {
  triggerDownload(response, `${name}.pdf`);
}

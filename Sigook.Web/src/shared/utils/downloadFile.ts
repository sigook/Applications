function clickLink(href: string, fileName?: string): void {
  const a = document.createElement('a');
  a.style.display = 'none';
  a.href = href;
  if (fileName) {
    a.download = fileName;
  }
  document.body.appendChild(a);
  a.click();
  a.remove();
}

function triggerDownload(blob: Blob, fileName: string): void {
  clickLink(URL.createObjectURL(blob), fileName);
}

export function downloadFile(response: Blob, name: string): void {
  triggerDownload(response, `${name}.xlsx`);
}

export function downloadPDF(response: Blob, name: string): void {
  triggerDownload(response, `${name}.pdf`);
}

export function openFileUrl(url: string): void {
  clickLink(url);
}

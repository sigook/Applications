export function buildMultipartFormData(data: unknown, files: Record<string, File | null | undefined> = {}): FormData {
  const formData = new FormData();
  formData.append('data', JSON.stringify(data));
  Object.entries(files).forEach(([fileName, file]) => {
    if (file) formData.append(fileName, file);
  });
  return formData;
}

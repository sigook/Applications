export const UPLOAD_ACCEPT = '.pdf,.doc,.docx,.xls,.xlsx,image/*';

const ALLOWED_EXTENSIONS = ['pdf', 'jpeg', 'jpg', 'png', 'gif', 'doc', 'docx', 'xls', 'xlsx'];
const MAX_SIZE_KB = 15500;

export function validateUploadFile(file: File | null): string {
  if (!file) return 'File is required';

  const extension = file.name.split('.').pop()?.toLowerCase() || '';
  if (!ALLOWED_EXTENSIONS.includes(extension)) {
    return `File extension not allowed. Allowed: ${ALLOWED_EXTENSIONS.join(', ')}`;
  }

  if (file.size / 1024 > MAX_SIZE_KB) {
    return `File size must be less than ${MAX_SIZE_KB} KB`;
  }

  return '';
}

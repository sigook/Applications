import ImageCompressor from 'image-compressor.js';

export function compressFile(file: File): Promise<Blob> {
  return new Promise((resolve, reject) => {
    new ImageCompressor(file, {
      quality: 0.6,
      success(result: Blob) {
        resolve(result);
      },
      error(e: Error) {
        reject(e.message);
      },
    });
  });
}

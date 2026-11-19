import { describe, expect, it, vi } from 'vitest';
import ImageCompressor from 'image-compressor.js';
import { compressFile } from '@/shared/utils/compressFile';

type CompressorOptions = NonNullable<ConstructorParameters<typeof ImageCompressor>[1]>;

const { constructed } = vi.hoisted(() => ({
  constructed: [] as { file: File | Blob | undefined; options: CompressorOptions | undefined }[],
}));

vi.mock('image-compressor.js', () => ({
  default: class {
    constructor(file?: File | Blob, options?: CompressorOptions) {
      constructed.push({ file, options });
    }
  },
}));

function lastOptions(): CompressorOptions {
  const options = constructed.at(-1)?.options;
  if (!options) throw new Error('compressor was not created');
  return options;
}

describe('compressFile', () => {
  it('compresses the file with 60 percent quality and resolves the result', async () => {
    const file = new File(['raw'], 'photo.jpg', { type: 'image/jpeg' });
    const compressed = new Blob(['small'], { type: 'image/jpeg' });
    const result = compressFile(file);
    expect(constructed.at(-1)?.file).toBe(file);
    expect(lastOptions().quality).toBe(0.6);
    lastOptions().success?.(compressed);
    await expect(result).resolves.toBe(compressed);
  });

  it('rejects with the error message when compression fails', async () => {
    const result = compressFile(new File(['raw'], 'photo.png'));
    lastOptions().error?.(new Error('Unsupported image'));
    await expect(result).rejects.toBe('Unsupported image');
  });
});

import { beforeEach, describe, expect, it, vi } from 'vitest';
import { defineComponent } from 'vue';
import { flushPromises } from '@vue/test-utils';
import PreviewImage from '@/shared/ui/PreviewImage.vue';
import { compressFile } from '@/shared/utils/compressFile';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { CovenantFileModel } from '@/shared/types/common';

vi.mock('@/shared/utils/compressFile', () => ({ compressFile: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const CropImageStub = defineComponent({
  name: 'CropImage',
  props: { image: { type: String, default: '' } },
  emits: ['onCrop', 'closeModal'],
  template: '<div class="crop-image-stub" />',
});

let imageSize = { width: 400, height: 300 };

class FakeImage {
  width = imageSize.width;
  height = imageSize.height;
  onload: (() => void) | null = null;
  set src(_value: string) {
    queueMicrotask(() => this.onload?.());
  }
}

class FakeFileReader {
  result: string | null = null;
  onload: ((event: { target: FakeFileReader }) => void) | null = null;
  readAsDataURL(file: File) {
    this.result = `data:${file.type};base64,AAAA`;
    queueMicrotask(() => this.onload?.({ target: this }));
  }
}

interface Props {
  editedImage?: CovenantFileModel | null;
  required?: boolean;
  name?: string;
  showDefault?: boolean;
}

function setup(props: Props = {}) {
  return mountWithPlugins(PreviewImage, { props, global: { stubs: { CropImage: CropImageStub } } });
}

function pngFile(name = 'photo.png') {
  return new File(['img'], name, { type: 'image/png' });
}

async function pick(wrapper: ReturnType<typeof setup>, file?: File) {
  const input = wrapper.find('input[type="file"]');
  Object.defineProperty(input.element, 'files', { value: file ? [file] : [], configurable: true });
  await input.trigger('change');
  await flushPromises();
}

function preview(wrapper: ReturnType<typeof setup>) {
  return wrapper.find('img[alt="logo"]');
}

function modalOpen(wrapper: ReturnType<typeof setup>) {
  return wrapper.findComponent({ name: 'BModal' }).props('modelValue') as boolean;
}

function error(wrapper: ReturnType<typeof setup>) {
  return wrapper.find('.help.is-danger');
}

beforeEach(() => {
  imageSize = { width: 400, height: 300 };
  vi.stubGlobal('Image', FakeImage);
  vi.stubGlobal('FileReader', FakeFileReader);
  vi.spyOn(URL, 'createObjectURL').mockReturnValue('blob:preview');
});

describe('PreviewImage', () => {
  it('shows the current image', () => {
    expect(preview(setup({ editedImage: { pathFile: 'https://files/logo.png' } })).attributes('src')).toBe('https://files/logo.png');
  });

  it('shows the default image with initials when there is none', () => {
    const wrapper = setup({ showDefault: true, name: 'Acme' });

    expect(preview(wrapper).exists()).toBe(false);
    expect(wrapper.find('.container-default-image').text()).toBe('Ac');
  });

  it('rejects files with a disallowed extension', async () => {
    const wrapper = setup({ editedImage: { pathFile: 'https://files/logo.png' } });
    await pick(wrapper, new File(['x'], 'doc.pdf', { type: 'application/pdf' }));

    expect(error(wrapper).text()).toBe('Allowed formats: jpeg, jpg, png, gif, svg');
    expect(wrapper.find('.update-image').classes()).toContain('is-danger');
    expect(preview(wrapper).exists()).toBe(false);
  });

  it('rejects files over 10MB', async () => {
    const file = pngFile();
    Object.defineProperty(file, 'size', { value: 10001 * 1024 });
    const wrapper = setup();
    await pick(wrapper, file);

    expect(error(wrapper).text()).toBe('File exceeds 10MB limit');
  });

  it('requires an image when nothing was picked', async () => {
    const wrapper = setup({ required: true });
    await pick(wrapper);

    expect(error(wrapper).text()).toBe('Image is required');
  });

  it('accepts no file when the image is optional', async () => {
    const wrapper = setup();
    await pick(wrapper);

    expect(error(wrapper).text()).toBe('');
    expect(modalOpen(wrapper)).toBe(false);
  });

  it('rejects images smaller than 150px', async () => {
    imageSize = { width: 149, height: 300 };
    const wrapper = setup();
    await pick(wrapper, pngFile());

    expect(showAlertError).toHaveBeenCalledWith('This file is too small, please select a photo with height and width of at least 150 pixels');
    expect(modalOpen(wrapper)).toBe(false);
  });

  it('opens the cropper, then compresses and emits the cropped image', async () => {
    const compressed = pngFile('compressed.png');
    vi.mocked(compressFile).mockResolvedValue(compressed);
    const wrapper = setup();
    await pick(wrapper, pngFile());

    const crop = wrapper.findComponent(CropImageStub);
    expect(modalOpen(wrapper)).toBe(true);
    expect(crop.props('image')).toBe('blob:preview');

    const cropped = pngFile('cropped.png');
    crop.vm.$emit('onCrop', cropped);
    await flushPromises();

    expect(compressFile).toHaveBeenCalledWith(cropped);
    expect(wrapper.emitted('onUpload')).toEqual([[true]]);
    expect(wrapper.emitted('finishUpload')).toEqual([[true]]);
    expect(wrapper.emitted('imageSelected')).toEqual([[compressed]]);
    expect(preview(wrapper).attributes('src')).toBe('blob:preview');
    expect(modalOpen(wrapper)).toBe(false);
  });

  it('clears the preview when compression fails', async () => {
    const failure = new Error('too big');
    vi.mocked(compressFile).mockRejectedValue(failure);
    const wrapper = setup();
    await pick(wrapper, pngFile());
    wrapper.findComponent(CropImageStub).vm.$emit('onCrop', pngFile());
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(failure);
    expect(wrapper.emitted('finishUpload')).toEqual([[true]]);
    expect(wrapper.emitted('imageSelected')).toBeUndefined();
    expect(preview(wrapper).exists()).toBe(false);
  });

  it('closes the cropper without emitting', async () => {
    const wrapper = setup();
    await pick(wrapper, pngFile());
    wrapper.findComponent(CropImageStub).vm.$emit('closeModal');
    await flushPromises();

    expect(modalOpen(wrapper)).toBe(false);
    expect(wrapper.emitted('onUpload')).toBeUndefined();
  });

  it('skips the cropper on legacy Edge', async () => {
    vi.spyOn(navigator, 'userAgent', 'get').mockReturnValue('Mozilla/5.0 Edge/18.0');
    vi.mocked(compressFile).mockResolvedValue(pngFile());
    const wrapper = setup();
    await pick(wrapper, pngFile());

    expect(modalOpen(wrapper)).toBe(false);
    expect(wrapper.emitted('imageSelected')).toHaveLength(1);
  });
});

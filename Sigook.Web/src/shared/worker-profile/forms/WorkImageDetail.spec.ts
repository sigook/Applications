import { describe, expect, it, vi } from 'vitest';
import { defineComponent } from 'vue';
import WorkImageDetail from '@/shared/worker-profile/forms/WorkImageDetail.vue';
import { createWorkerImage } from '@/shared/worker-profile/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';

vi.mock('@/shared/worker-profile/api', () => ({ createWorkerImage: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const PreviewImageStub = defineComponent({
  name: 'PreviewImage',
  props: { editedImage: { type: Object, default: null }, required: Boolean },
  emits: ['imageSelected'],
  template: '<div class="preview-image-stub" />',
});

const imageFile = new File(['photo'], 'photo.jpg', { type: 'image/jpeg' });

function setup() {
  const data = { id: 'worker-profile-1', profileImage: { pathFile: 'https://files/photo.jpg' } } as WorkerProfileDetail;
  return mountWithPlugins(WorkImageDetail, {
    props: { data },
    global: { stubs: { UploadImage: PreviewImageStub } },
  });
}

async function openModal(wrapper: ReturnType<typeof setup>) {
  await wrapper.find('button[title="Edit"]').trigger('click');
  await vi.waitFor(() => { expect(wrapper.findComponent(PreviewImageStub).exists()).toBe(true); });
}

function buttonByText(text: string) {
  const button = Array.from(document.body.querySelectorAll('button')).find((b) => b.textContent.trim() === text);
  if (!button) throw new Error(`${text} button not found`);
  return button;
}

describe('WorkImageDetail', () => {
  it('shows the current profile image', () => {
    expect(setup().find('img').attributes('src')).toBe('https://files/photo.jpg');
  });

  it('asks for an image when none was selected', async () => {
    const wrapper = setup();
    await openModal(wrapper);
    buttonByText('Save').click();

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('Please select an image'); });
    expect(createWorkerImage).not.toHaveBeenCalled();
  });

  it('uploads the selected image once and emits updateProfile', async () => {
    vi.mocked(createWorkerImage).mockResolvedValue();
    const wrapper = setup();
    await openModal(wrapper);
    wrapper.findComponent(PreviewImageStub).vm.$emit('imageSelected', imageFile);
    buttonByText('Save').click();

    await vi.waitFor(() => { expect(wrapper.emitted('updateProfile')).toHaveLength(1); });
    expect(createWorkerImage).toHaveBeenCalledExactlyOnceWith('worker-profile-1', imageFile);
  });

  it('shows the error when the upload fails', async () => {
    const error = new Error('upload failed');
    vi.mocked(createWorkerImage).mockRejectedValue(error);
    const wrapper = setup();
    await openModal(wrapper);
    wrapper.findComponent(PreviewImageStub).vm.$emit('imageSelected', imageFile);
    buttonByText('Save').click();

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('updateProfile')).toBeUndefined();
  });
});

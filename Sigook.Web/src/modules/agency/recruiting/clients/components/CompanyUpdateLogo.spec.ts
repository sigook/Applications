import { describe, expect, it, vi } from 'vitest';
import { defineComponent } from 'vue';
import CompanyUpdateLogo from '@/modules/agency/recruiting/clients/components/CompanyUpdateLogo.vue';
import { updateAgencyCompanyProfileLogo } from '@/modules/agency/recruiting/clients/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/agency/recruiting/clients/api', () => ({ updateAgencyCompanyProfileLogo: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const PreviewImageStub = defineComponent({
  name: 'PreviewImage',
  props: { editedImage: { type: Object, default: null }, required: Boolean },
  emits: ['imageSelected'],
  template: '<div class="preview-image-stub" />',
});

const logoFile = new File(['logo'], 'logo.png', { type: 'image/png' });

function setup() {
  return mountWithPlugins(CompanyUpdateLogo, {
    props: { profileId: 'profile-1', logo: { pathFile: 'https://files/logo.png' } },
    global: { stubs: { UploadImage: PreviewImageStub } },
  });
}

function saveButton(wrapper: ReturnType<typeof setup>) {
  const button = wrapper.findAll('button').find((b) => b.text() === 'Save');
  if (!button) throw new Error('Save button not found');
  return button;
}

describe('CompanyUpdateLogo', () => {
  it('passes the current logo to the image picker as required', () => {
    const picker = setup().findComponent(PreviewImageStub);

    expect(picker.props('editedImage')).toEqual({ pathFile: 'https://files/logo.png' });
    expect(picker.props('required')).toBe(true);
  });

  it('asks for an image when none was selected', async () => {
    const wrapper = setup();
    await saveButton(wrapper).trigger('click');

    expect(showAlertError).toHaveBeenCalledWith('Please select an image');
    expect(updateAgencyCompanyProfileLogo).not.toHaveBeenCalled();
    expect(wrapper.emitted('updated')).toBeUndefined();
  });

  it('uploads the selected logo and emits updated', async () => {
    vi.mocked(updateAgencyCompanyProfileLogo).mockResolvedValue();
    const wrapper = setup();
    wrapper.findComponent(PreviewImageStub).vm.$emit('imageSelected', logoFile);
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(wrapper.emitted('updated')).toHaveLength(1); });
    expect(updateAgencyCompanyProfileLogo).toHaveBeenCalledWith('profile-1', logoFile);
  });

  it('shows the error and stays open when the upload fails', async () => {
    const error = new Error('upload failed');
    vi.mocked(updateAgencyCompanyProfileLogo).mockRejectedValue(error);
    const wrapper = setup();
    wrapper.findComponent(PreviewImageStub).vm.$emit('imageSelected', logoFile);
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('updated')).toBeUndefined();
  });

  it('emits cancel', async () => {
    const wrapper = setup();
    const cancel = wrapper.findAll('button').find((b) => b.text() === 'Cancel');
    await cancel?.trigger('click');

    expect(wrapper.emitted('cancel')).toHaveLength(1);
  });
});

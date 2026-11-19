import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { nextTick } from 'vue';
import CreateAgency from '@/modules/agency/sales/agencies/pages/CreateAgency.vue';
import { createAgency } from '@/modules/agency/sales/agencies/api';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/agency/sales/agencies/api', () => ({ createAgency: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

function setup() {
  return mountWithPlugins(CreateAgency);
}

type Wrapper = ReturnType<typeof setup>;

async function fill(wrapper: Wrapper, phone = '4165550199') {
  await wrapper.find('input[name="full name"]').setValue('North Staffing');
  await wrapper.find('input[name="email"]').setValue('ops@north.test');
  await wrapper.find('input[name="phone"]').setValue(phone);
  await wrapper.find('input[name="password"]').setValue('secret123');
  wrapper.findComponent({ name: 'BSelect' }).vm.$emit('update:modelValue', 3);
  await nextTick();
}

async function submit(wrapper: Wrapper) {
  await wrapper.find('form').trigger('submit');
}

describe('CreateAgency', () => {
  it('offers the agency types', () => {
    const wrapper = setup();
    expect(wrapper.findAll('option').map((o) => o.text())).toEqual(expect.arrayContaining(['Regular', 'Business Partner']));
  });

  it('creates the agency and goes back to the list', async () => {
    vi.mocked(createAgency).mockResolvedValue({ id: 'agency-9' });
    const wrapper = setup();
    const push = vi.spyOn(wrapper.vm.$router, 'push');
    await fill(wrapper);
    await submit(wrapper);

    await vi.waitFor(() => { expect(createAgency).toHaveBeenCalledOnce(); });
    expect(createAgency).toHaveBeenCalledWith({
      fullName: 'North Staffing',
      email: 'ops@north.test',
      agencyType: 3,
      password: 'secret123',
      phonePrincipal: '416 555-0199',
    });
    await flushPromises();
    expect(showAlertSuccess).toHaveBeenCalledWith('Agency created successfully');
    expect(push).toHaveBeenCalledWith('/sales/agencies');
  });

  it('shows the required errors', async () => {
    const wrapper = setup();
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly'); });
    await vi.waitFor(() => { expect(wrapper.text()).toContain('Full name is required'); });
    expect(wrapper.text()).toContain('Email is required');
    expect(wrapper.text()).toContain('Agency type is required');
    expect(wrapper.text()).toContain('Password is required');
    expect(createAgency).not.toHaveBeenCalled();
  });

  it('does not create the agency with an invalid phone', async () => {
    const wrapper = setup();
    await fill(wrapper, '416');
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly'); });
    expect(createAgency).not.toHaveBeenCalled();
  });

  it('shows the error when creation fails', async () => {
    const error = new Error('email taken');
    vi.mocked(createAgency).mockRejectedValue(error);
    const wrapper = setup();
    await fill(wrapper);
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
  });
});

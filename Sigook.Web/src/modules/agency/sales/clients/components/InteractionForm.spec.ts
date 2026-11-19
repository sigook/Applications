import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { nextTick } from 'vue';
import InteractionForm from '@/modules/agency/sales/clients/components/InteractionForm.vue';
import { createCompanyInteraction, updateCompanyInteraction } from '@/modules/agency/sales/clients/api';
import { getAgencyCompaniesList } from '@/modules/agency/recruiting/clients/api';
import { InteractionPurpose, InteractionStatus, InteractionType } from '@/modules/agency/sales/clients/types';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { CompanyInteraction } from '@/modules/agency/sales/clients/types';
import type { SalesClientReference } from '@/modules/agency/sales/dashboard/types';

vi.mock('@/modules/agency/sales/clients/api', () => ({ createCompanyInteraction: vi.fn(), updateCompanyInteraction: vi.fn() }));
vi.mock('@/modules/agency/recruiting/clients/api', () => ({ getAgencyCompaniesList: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

const interaction: CompanyInteraction = {
  id: 'int-1',
  companyProfileId: 'company-1',
  companyName: 'Acme Logistics',
  ownerId: 'user-1',
  description: 'Sent the rate card',
  interactionPurpose: InteractionPurpose.Proposal,
  interactionType: InteractionType.Mail,
  interactionStatus: InteractionStatus.Completed,
  createdAt: '2026-11-10T12:00:00',
  updatedAt: '2026-11-10T12:00:00',
};

const acme: SalesClientReference = { id: 'company-1', fullName: 'Acme Logistics' };

async function setup(props: { interaction?: CompanyInteraction | null; initialClient?: SalesClientReference | null } = {}) {
  const wrapper = mountWithPlugins(InteractionForm, { props });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function submit(wrapper: Wrapper) {
  return (wrapper.vm as unknown as { submit: () => Promise<boolean> }).submit();
}

function typeButton(wrapper: Wrapper, label: string) {
  const found = wrapper.findAll('.sd-choice').find((b) => b.text() === label);
  if (!found) throw new Error(`${label} not found`);
  return found;
}

function description(wrapper: Wrapper) {
  return wrapper.find('textarea[name="description"]');
}

describe('InteractionForm', () => {
  it('offers the interaction types with Call active by default', async () => {
    const wrapper = await setup();

    expect(wrapper.findAll('.sd-choice').map((b) => b.text())).toEqual(['Call', 'Email', 'SMS', 'LinkedIn']);
    expect(typeButton(wrapper, 'Call').classes()).toContain('is-active');
  });

  it('logs an interaction for the initial client and resets', async () => {
    vi.mocked(createCompanyInteraction).mockResolvedValue('int-9');
    const wrapper = await setup({ initialClient: acme });
    await typeButton(wrapper, 'LinkedIn').trigger('click');
    expect(typeButton(wrapper, 'LinkedIn').classes()).toContain('is-active');
    const [purposeSelect, statusSelect] = wrapper.findAllComponents({ name: 'BSelect' });
    purposeSelect.vm.$emit('update:modelValue', InteractionPurpose.Negotiation);
    statusSelect.vm.$emit('update:modelValue', InteractionStatus.InProgress);
    await description(wrapper).setValue('  Discussed rates  ');
    await nextTick();

    await expect(submit(wrapper)).resolves.toBe(true);
    expect(createCompanyInteraction).toHaveBeenCalledExactlyOnceWith('company-1', {
      description: 'Discussed rates',
      interactionPurpose: InteractionPurpose.Negotiation,
      interactionType: InteractionType.LinkedIn,
      interactionStatus: InteractionStatus.InProgress,
    });
    expect(showAlertSuccess).toHaveBeenCalledWith('Interaction logged');
    await flushPromises();
    expect((description(wrapper).element as HTMLTextAreaElement).value).toBe('');
    expect(wrapper.findComponent({ name: 'SearchSelect' }).props('modelValue')).toBe('company-1');
  });

  it('requires a client and a description', async () => {
    const wrapper = await setup();

    await expect(submit(wrapper)).resolves.toBe(false);
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    await flushPromises();
    expect(wrapper.text()).toContain('Client is required');
    expect(wrapper.text()).toContain('Description is required');
  });

  it('searches clients from three characters', async () => {
    vi.mocked(getAgencyCompaniesList).mockResolvedValue([{ id: 'company-2', value: 'Globex' }]);
    const wrapper = await setup({ initialClient: acme });
    const select = wrapper.findComponent({ name: 'SearchSelect' });

    select.vm.$emit('search', 'gl');
    await flushPromises();
    expect(getAgencyCompaniesList).not.toHaveBeenCalled();

    select.vm.$emit('search', 'globex');
    await flushPromises();
    expect(getAgencyCompaniesList).toHaveBeenCalledExactlyOnceWith('globex');
    expect(select.props('options')).toEqual([{ value: 'company-2', label: 'Globex' }]);
  });

  it('shows the error when the client search fails', async () => {
    const error = new Error('search');
    vi.mocked(getAgencyCompaniesList).mockRejectedValue(error);
    const wrapper = await setup();
    wrapper.findComponent({ name: 'SearchSelect' }).vm.$emit('search', 'acme');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('loads and updates an existing interaction', async () => {
    vi.mocked(updateCompanyInteraction).mockResolvedValue();
    const wrapper = await setup({ interaction });

    expect(wrapper.find('.sd-readonly').text()).toBe('Acme Logistics');
    expect(typeButton(wrapper, 'Email').classes()).toContain('is-active');
    expect((description(wrapper).element as HTMLTextAreaElement).value).toBe('Sent the rate card');
    await description(wrapper).setValue('Sent the updated rate card');

    await expect(submit(wrapper)).resolves.toBe(true);
    expect(updateCompanyInteraction).toHaveBeenCalledExactlyOnceWith('company-1', 'int-1', {
      description: 'Sent the updated rate card',
      interactionPurpose: InteractionPurpose.Proposal,
      interactionType: InteractionType.Mail,
      interactionStatus: InteractionStatus.Completed,
    });
    expect(showAlertSuccess).toHaveBeenCalledWith('Interaction updated');
  });

  it('returns false and shows the error when saving fails', async () => {
    const error = new Error('save');
    vi.mocked(updateCompanyInteraction).mockRejectedValue(error);
    const wrapper = await setup({ interaction });

    await expect(submit(wrapper)).resolves.toBe(false);
    expect(showAlertError).toHaveBeenCalledWith(error);
  });
});

import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import WorkerSettings from '@/modules/agency/recruiting/workers/components/WorkerSettings.vue';
import {
  addNewHoliday,
  addUpdateAgencyWorkerProfileHolidays,
  getAgencyWorkerProfileHolidays,
  updateAgencyWorkerContractor,
  updateAgencyWorkerSubContractor,
  updateWorkerProfileExternalId,
  updateWorkerProfileTaxCategory,
  updateWorkerProfileTaxRate,
  updateWorkerProfileWcCode,
} from '@/modules/agency/recruiting/workers/api';
import { getTaxCategories } from '@/shared/api/catalogApi';
import { getDialog } from '@/shared/utils/buefyProgrammatic';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail } from '@/testing/workerProfileFixtures';
import type { AgencyWorkerHoliday } from '@/modules/agency/recruiting/workers/types';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';

vi.mock('@/modules/agency/recruiting/workers/api', () => ({
  addNewHoliday: vi.fn(),
  addUpdateAgencyWorkerProfileHolidays: vi.fn(),
  getAgencyWorkerProfileHolidays: vi.fn(),
  updateAgencyWorkerContractor: vi.fn(),
  updateAgencyWorkerSubContractor: vi.fn(),
  updateWorkerProfileExternalId: vi.fn(),
  updateWorkerProfileTaxCategory: vi.fn(),
  updateWorkerProfileTaxRate: vi.fn(),
  updateWorkerProfileWcCode: vi.fn(),
}));
vi.mock('@/shared/api/catalogApi', () => ({ getTaxCategories: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/shared/utils/buefyProgrammatic', () => {
  const dialog = { prompt: vi.fn() };
  return { getDialog: () => dialog };
});

interface PromptOptions {
  message: string;
  confirmText: string;
  closeOnConfirm: boolean;
  inputAttrs: Record<string, unknown>;
  onConfirm: (value: string, dialog: { close: () => void }) => Promise<void>;
}

const holidays: AgencyWorkerHoliday[] = [
  { holidayId: 'h1', workerProfileId: 'worker-profile-1', date: '2026-05-18T12:00:00', statPaidWorker: 120 },
  { holidayId: 'h2', workerProfileId: 'worker-profile-1', date: '2026-07-01T12:00:00', statPaidWorker: 0 },
];

async function setup(overrides: Partial<WorkerProfileDetail> = {}, workerHolidays: AgencyWorkerHoliday[] = holidays) {
  vi.mocked(getTaxCategories).mockResolvedValue([{ id: 1, value: 'Claim code 1' }, { id: 2, value: 'Claim code 2' }]);
  vi.mocked(getAgencyWorkerProfileHolidays).mockResolvedValue(workerHolidays.map((h) => ({ ...h })));
  const worker = createWorkerProfileDetail({ externalId: 'EXT-1', wcCode: 'WC-1', cpp: 5.95, ei: 1.66, ...overrides });
  const wrapper = mountWithPlugins(WorkerSettings, { props: { worker } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function textInput(wrapper: Wrapper, placeholder: string) {
  return wrapper.find(`input[placeholder="${placeholder}"]`);
}

function lastEmitted(wrapper: Wrapper): WorkerProfileDetail {
  const events = wrapper.emitted('update:worker');
  if (!events) throw new Error('update:worker not emitted');
  return events.at(-1)?.[0] as WorkerProfileDetail;
}

describe('WorkerSettings', () => {
  it('fills the form with the worker settings and the tax categories', async () => {
    const wrapper = await setup({ federalTaxCategory: 2 });

    expect(getAgencyWorkerProfileHolidays).toHaveBeenCalledWith('worker-profile-1');
    expect((textInput(wrapper, 'External ID').element as HTMLInputElement).value).toBe('EXT-1');
    expect((textInput(wrapper, 'WC Code').element as HTMLInputElement).value).toBe('WC-1');
    expect((wrapper.find('input[name="cpp"]').element as HTMLInputElement).value).toBe('5.95');
    expect((wrapper.find('input[name="ei"]').element as HTMLInputElement).value).toBe('1.66');
    const selects = wrapper.findAll('select');
    expect(selects[0].findAll('option').map((o) => o.text())).toEqual(['Select', 'Claim code 1', 'Claim code 2']);
    expect((selects[0].element as HTMLSelectElement).value).toBe('2');
  });

  it('saves the external id on enter', async () => {
    vi.mocked(updateWorkerProfileExternalId).mockResolvedValue();
    const wrapper = await setup();
    await textInput(wrapper, 'External ID').setValue('EXT-9');
    await textInput(wrapper, 'External ID').trigger('keypress', { key: 'Enter' });
    await flushPromises();

    expect(updateWorkerProfileExternalId).toHaveBeenCalledWith(expect.objectContaining({ id: 'worker-profile-1', externalId: 'EXT-9' }));
    expect(lastEmitted(wrapper).externalId).toBe('EXT-9');
  });

  it('saves the WC code on enter', async () => {
    vi.mocked(updateWorkerProfileWcCode).mockResolvedValue();
    const wrapper = await setup();
    await textInput(wrapper, 'WC Code').setValue('WC-7');
    await textInput(wrapper, 'WC Code').trigger('keypress', { key: 'Enter' });
    await flushPromises();

    expect(updateWorkerProfileWcCode).toHaveBeenCalledWith(expect.objectContaining({ wcCode: 'WC-7' }));
    expect(lastEmitted(wrapper).wcCode).toBe('WC-7');
  });

  it('shows the error and does not emit when saving the external id fails', async () => {
    const error = new Error('save failed');
    vi.mocked(updateWorkerProfileExternalId).mockRejectedValue(error);
    const wrapper = await setup();
    await textInput(wrapper, 'External ID').trigger('keypress', { key: 'Enter' });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.emitted('update:worker')).toBeUndefined();
  });

  it.each([
    [0, updateAgencyWorkerContractor, 'isContractor'],
    [1, updateAgencyWorkerSubContractor, 'isSubcontractor'],
  ] as const)('toggles checkbox %i through its endpoint', async (index, request, field) => {
    vi.mocked(request).mockResolvedValue();
    const wrapper = await setup();
    await wrapper.findAll('input[type="checkbox"]')[index].setValue(true);
    await flushPromises();

    expect(request).toHaveBeenCalledExactlyOnceWith('worker-profile-1');
    expect(lastEmitted(wrapper)[field]).toBe(true);
  });

  it.each([
    [0, updateAgencyWorkerContractor],
    [1, updateAgencyWorkerSubContractor],
  ] as const)('shows the error when toggling checkbox %i fails', async (index, request) => {
    const error = new Error('toggle failed');
    vi.mocked(request).mockRejectedValue(error);
    const wrapper = await setup();
    await wrapper.findAll('input[type="checkbox"]')[index].setValue(true);
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('saves the tax categories when one changes', async () => {
    vi.mocked(updateWorkerProfileTaxCategory).mockResolvedValue();
    const wrapper = await setup();
    await wrapper.findAll('select')[1].setValue('1');
    await flushPromises();

    expect(updateWorkerProfileTaxCategory).toHaveBeenCalledWith(expect.objectContaining({ provincialTaxCategory: 1 }));
    expect(lastEmitted(wrapper).provincialTaxCategory).toBe(1);
  });

  it('shows the error when the tax category fails to save', async () => {
    const error = new Error('tax failed');
    vi.mocked(updateWorkerProfileTaxCategory).mockRejectedValue(error);
    const wrapper = await setup();
    await wrapper.findAll('select')[0].setValue('2');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('saves CPP and EI on enter', async () => {
    vi.mocked(updateWorkerProfileTaxRate).mockResolvedValue();
    const wrapper = await setup();
    await wrapper.find('input[name="cpp"]').setValue('4.5');
    await wrapper.find('input[name="ei"]').trigger('keypress', { key: 'Enter' });

    await vi.waitFor(() => { expect(updateWorkerProfileTaxRate).toHaveBeenCalledWith(expect.objectContaining({ cpp: 4.5, ei: 1.66 })); });
    expect(lastEmitted(wrapper).cpp).toBe(4.5);
  });

  it('rejects a negative CPP', async () => {
    const wrapper = await setup();
    await wrapper.find('input[name="cpp"]').setValue('-1');
    await wrapper.find('input[name="cpp"]').trigger('keypress', { key: 'Enter' });

    await vi.waitFor(() => { expect(wrapper.text()).toContain('Must be greater than or equal to 0'); });
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(updateWorkerProfileTaxRate).not.toHaveBeenCalled();
  });

  it('shows the error when the tax rate fails to save', async () => {
    const error = new Error('rate failed');
    vi.mocked(updateWorkerProfileTaxRate).mockRejectedValue(error);
    const wrapper = await setup();
    await wrapper.find('input[name="cpp"]').trigger('keypress', { key: 'Enter' });

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('update:worker')).toBeUndefined();
  });

  it('adds a holiday from the date prompt and reloads the holidays', async () => {
    vi.mocked(addNewHoliday).mockResolvedValue();
    const wrapper = await setup();
    const addButton = wrapper.findAll('button').find((b) => b.text() === 'Add Holiday');
    await addButton?.trigger('click');

    const options = vi.mocked(getDialog().prompt).mock.calls[0][0] as PromptOptions;
    expect(options.message).toBe('Holiday date');
    expect(options.inputAttrs).toEqual({ type: 'date', placeholder: 'Date' });
    expect(options.closeOnConfirm).toBe(false);
    expect(options.confirmText).toBe('Add');
    const dialog = { close: vi.fn() };
    await options.onConfirm('2026-12-26', dialog);

    expect(addNewHoliday).toHaveBeenCalledWith('worker-profile-1', { workerProfileId: 'worker-profile-1', date: '2026-12-26' });
    expect(getAgencyWorkerProfileHolidays).toHaveBeenCalledTimes(2);
    expect(dialog.close).toHaveBeenCalledOnce();
  });

  it('edits the amount paid for the selected holiday', async () => {
    vi.mocked(addUpdateAgencyWorkerProfileHolidays).mockResolvedValue();
    const wrapper = await setup();
    expect(wrapper.text()).not.toContain('Amount to pay');

    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', new Date('2026-05-18T00:00:00'));
    await flushPromises();
    expect(wrapper.text()).toContain('Amount to pay');
    const amount = wrapper.findAll('input').at(-1);
    expect((amount?.element as HTMLInputElement).value).toBe('120');

    await amount?.setValue('150');
    const save = wrapper.findAll('button').find((b) => b.text() === 'Save');
    await save?.trigger('click');
    await flushPromises();

    expect(addUpdateAgencyWorkerProfileHolidays).toHaveBeenCalledWith('worker-profile-1', expect.objectContaining({ holidayId: 'h1', statPaidWorker: '150' }));
    expect(showAlertSuccess).toHaveBeenCalledWith('Holiday updated');
    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(false);
  });

  it('selects the holiday of the same full date, not just the same day of month', async () => {
    const wrapper = await setup({}, [
      { holidayId: 'jan', workerProfileId: 'worker-profile-1', date: '2026-01-01T12:00:00', statPaidWorker: 10 },
      { holidayId: 'jul', workerProfileId: 'worker-profile-1', date: '2026-07-01T12:00:00', statPaidWorker: 70 },
    ]);

    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', new Date('2026-07-01T00:00:00'));
    await flushPromises();
    expect((wrapper.findAll('input').at(-1)?.element as HTMLInputElement).value).toBe('70');

    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', new Date('2026-03-01T00:00:00'));
    await flushPromises();
    expect(wrapper.text()).not.toContain('Amount to pay');
  });

  it('releases the loader and shows the error when saving the holiday fails', async () => {
    const error = new Error('boom');
    vi.mocked(addUpdateAgencyWorkerProfileHolidays).mockRejectedValue(error);
    const wrapper = await setup();

    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', new Date('2026-05-18T00:00:00'));
    await flushPromises();
    await wrapper.findAll('button').find((b) => b.text() === 'Save')?.trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(showAlertSuccess).not.toHaveBeenCalled();
    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(false);
  });

  it('refreshes the form when the worker prop changes', async () => {
    const wrapper = await setup();
    await wrapper.setProps({ worker: createWorkerProfileDetail({ externalId: 'EXT-2', cpp: 3 }) });
    await flushPromises();

    expect((textInput(wrapper, 'External ID').element as HTMLInputElement).value).toBe('EXT-2');
    expect((wrapper.find('input[name="cpp"]').element as HTMLInputElement).value).toBe('3');
  });
});

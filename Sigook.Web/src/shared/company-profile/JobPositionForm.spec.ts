import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import JobPositionForm from '@/shared/company-profile/JobPositionForm.vue';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { JobPositionModel } from '@/shared/company-profile/types';
import type { RequestShiftModel } from '@/shared/request-detail/types';

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/shared/request-detail/ShiftsForm.vue', () => ({
  default: {
    name: 'Shift',
    props: { isUpdate: Boolean, currentShift: Object },
    emits: ['updateModel'],
    template: '<div class="shift-stub" />',
  },
}));

type Create = (model: JobPositionModel) => Promise<{ id: string } | null>;
type Update = (id: string, model: JobPositionModel) => Promise<void>;
type Load = (id: string) => Promise<JobPositionModel>;

interface SetupOptions {
  kind?: 'full' | 'basic';
  create?: Create;
  update?: Update;
  load?: Load;
  currentPosition?: JobPositionModel | null;
  isDuplicate?: boolean;
}

const shift: RequestShiftModel = { monday: true, mondayStart: '08:00:00', mondayFinish: '16:00:00' };

const stored: JobPositionModel = {
  id: 'jp-1',
  companyProfileId: 'company-1',
  jobPosition: 'Forklift operator',
  rate: 30,
  workerRate: 20,
  workerRateMin: 20,
  workerRateMax: 25,
  overtimeStartsAfter: 40,
  description: 'Warehouse',
  shift,
};

function setup({ kind = 'full', create = vi.fn<Create>(), update, load, currentPosition, isDuplicate }: SetupOptions = {}) {
  return mountWithPlugins(JobPositionForm, { props: { kind, create, update, load, currentPosition, isDuplicate } });
}

function input(wrapper: ReturnType<typeof setup>, name: string) {
  return wrapper.find(`[name="${name}"]`);
}

function value(wrapper: ReturnType<typeof setup>, name: string) {
  return (input(wrapper, name).element as HTMLInputElement).value;
}

async function submit(wrapper: ReturnType<typeof setup>) {
  await wrapper.find('button.is-primary').trigger('click');
}

function errors(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.help.is-danger').map((e) => e.text());
}

describe('JobPositionForm', () => {
  it('shows only position, description and shift for the basic kind', () => {
    const wrapper = setup({ kind: 'basic' });

    expect(input(wrapper, 'positions').exists()).toBe(true);
    expect(input(wrapper, 'description').exists()).toBe(true);
    expect(input(wrapper, 'rate').exists()).toBe(false);
    expect(input(wrapper, 'overtimeStartsAfter').exists()).toBe(false);
    expect(wrapper.findComponent({ name: 'Shift' }).props('isUpdate')).toBe(false);
    expect(wrapper.find('button.is-primary').text()).toBe('Create');
  });

  it('creates a basic position without rates and emits its id', async () => {
    const create = vi.fn<Create>().mockResolvedValue({ id: 'jp-9' });
    const wrapper = setup({ kind: 'basic', create });
    await input(wrapper, 'positions').setValue('Packer');
    await input(wrapper, 'description').setValue('Line packing');
    wrapper.findComponent({ name: 'Shift' }).vm.$emit('updateModel', shift);
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('updateContent')).toEqual([['jp-9']]); });
    expect(create).toHaveBeenCalledExactlyOnceWith({
      jobPosition: 'Packer',
      description: 'Line packing',
      workerRateMin: null,
      workerRateMax: null,
      shift,
    });
  });

  it('requires the position', async () => {
    const create = vi.fn<Create>();
    const wrapper = setup({ kind: 'basic', create });
    await submit(wrapper);

    await vi.waitFor(() => { expect(errors(wrapper)).toContain('Position is required'); });
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(create).not.toHaveBeenCalled();
  });

  it('requires both rates for the full kind', async () => {
    const create = vi.fn<Create>();
    const wrapper = setup({ create });
    await input(wrapper, 'positions').setValue('Packer');
    await submit(wrapper);

    await vi.waitFor(() => { expect(errors(wrapper)).toEqual(expect.arrayContaining(['Rate is required', 'Worker rate is required'])); });
    expect(create).not.toHaveBeenCalled();
  });

  it('rejects a worker rate above the agency rate', async () => {
    const create = vi.fn<Create>();
    const wrapper = setup({ create });
    await input(wrapper, 'positions').setValue('Packer');
    await input(wrapper, 'rate').setValue('20');
    await input(wrapper, 'workerRate').setValue('25');
    await submit(wrapper);

    await vi.waitFor(() => { expect(errors(wrapper)).toContain('Cannot exceed agency rate'); });
    expect(create).not.toHaveBeenCalled();
  });

  it('rejects a max worker rate below the worker rate', async () => {
    const create = vi.fn<Create>();
    const wrapper = setup({ create });
    await input(wrapper, 'positions').setValue('Packer');
    await input(wrapper, 'rate').setValue('30');
    await input(wrapper, 'workerRate').setValue('20');
    await input(wrapper, 'workerRateMax').setValue('18');
    await submit(wrapper);

    await vi.waitFor(() => { expect(errors(wrapper)).toContain('Must be >= worker rate'); });
    expect(create).not.toHaveBeenCalled();
  });

  it('creates a full position with its rates', async () => {
    const create = vi.fn<Create>().mockResolvedValue({ id: 'jp-9' });
    const wrapper = setup({ create });
    await input(wrapper, 'positions').setValue('Packer');
    await input(wrapper, 'overtimeStartsAfter').setValue('44');
    await input(wrapper, 'rate').setValue('30');
    await input(wrapper, 'workerRate').setValue('20');
    await input(wrapper, 'workerRateMax').setValue('24');
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('updateContent')).toEqual([['jp-9']]); });
    expect(create.mock.calls[0][0]).toMatchObject({
      jobPosition: 'Packer',
      rate: 30,
      workerRate: 20,
      workerRateMin: 20,
      workerRateMax: 24,
      overtimeStartsAfter: 44,
    });
  });

  it('loads the position to edit and saves it with update', async () => {
    const load = vi.fn<Load>().mockResolvedValue(stored);
    const update = vi.fn<Update>().mockResolvedValue();
    const create = vi.fn<Create>();
    const wrapper = setup({ create, update, load, currentPosition: { id: 'jp-1' } });
    await flushPromises();

    expect(load).toHaveBeenCalledWith('jp-1');
    expect(value(wrapper, 'positions')).toBe('Forklift operator');
    expect(value(wrapper, 'description')).toBe('Warehouse');
    expect(wrapper.find('button.is-primary').text()).toBe('Save');
    expect(wrapper.findComponent({ name: 'Shift' }).props()).toEqual({ isUpdate: true, currentShift: shift });

    await input(wrapper, 'positions').setValue('Senior forklift operator');
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('updateContent')).toEqual([['jp-1']]); });
    expect(create).not.toHaveBeenCalled();
    expect(update).toHaveBeenCalledExactlyOnceWith('jp-1', expect.objectContaining({
      id: 'jp-1',
      jobPosition: 'Senior forklift operator',
      rate: 30,
      workerRate: 20,
      workerRateMax: 25,
      overtimeStartsAfter: 40,
      shift,
    }));
  });

  it('does not create a new position when editing without an update function', async () => {
    const load = vi.fn<Load>().mockResolvedValue(stored);
    const create = vi.fn<Create>();
    const wrapper = setup({ create, load, currentPosition: { id: 'jp-1' } });
    await flushPromises();
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('This role cannot be edited here'); });
    expect(create).not.toHaveBeenCalled();
    expect(wrapper.emitted('updateContent')).toBeUndefined();
  });

  it('creates a copy when duplicating', async () => {
    const load = vi.fn<Load>().mockResolvedValue(stored);
    const create = vi.fn<Create>().mockResolvedValue({ id: 'jp-2' });
    const update = vi.fn<Update>();
    const wrapper = setup({ create, update, load, currentPosition: { id: 'jp-1' }, isDuplicate: true });
    await flushPromises();

    expect(wrapper.find('button.is-primary').text()).toBe('Create');
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('updateContent')).toEqual([['jp-2']]); });
    expect(update).not.toHaveBeenCalled();
    expect(create.mock.calls[0][0]).toMatchObject({ id: undefined, jobPosition: 'Forklift operator', rate: 30 });
  });

  it('leaves zero rates empty when loading', async () => {
    const load = vi.fn<Load>().mockResolvedValue({ ...stored, rate: 0, workerRate: 0 });
    const wrapper = setup({ load, currentPosition: { id: 'jp-1' } });
    await flushPromises();

    expect(value(wrapper, 'rate')).toBe('');
    expect(value(wrapper, 'workerRate')).toBe('');
  });

  it('shows the error when loading fails', async () => {
    const error = new Error('load failed');
    setup({ load: vi.fn<Load>().mockRejectedValue(error), currentPosition: { id: 'jp-1' } });

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
  });

  it('shows the error when saving fails', async () => {
    const error = new Error('save failed');
    const wrapper = setup({ kind: 'basic', create: vi.fn<Create>().mockRejectedValue(error) });
    await input(wrapper, 'positions').setValue('Packer');
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('updateContent')).toBeUndefined();
  });
});

import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { nextTick } from 'vue';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import HoursWorkedReport from '@/modules/agency/accounting/reports/components/HoursWorkedReport.vue';
import { getHoursWorkedReport, getJobPositionsHoursWorked } from '@/modules/agency/accounting/reports/api';
import { getAgencyCompanyProfileWithRequests } from '@/modules/agency/recruiting/clients/api';
import { downloadAgencyReport } from '@/shared/api/reportApi';
import { downloadFile } from '@/shared/utils/downloadFile';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { HoursWorkedResume } from '@/modules/agency/accounting/reports/types';
import type { AgencyCompanyJobPosition, CompanyProfileListItem } from '@/modules/agency/recruiting/clients/types';

vi.mock('@/modules/agency/accounting/reports/api', () => ({ getHoursWorkedReport: vi.fn(), getJobPositionsHoursWorked: vi.fn() }));
vi.mock('@/modules/agency/recruiting/clients/api', () => ({ getAgencyCompanyProfileWithRequests: vi.fn() }));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/downloadFile', () => ({ downloadFile: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const acme = { id: 'company-1', fullName: 'Acme Logistics' } as CompanyProfileListItem;
const globex = { id: 'company-2', fullName: 'Globex' } as CompanyProfileListItem;
const forklift = { id: 'jp-1', jobPosition: 'Forklift' } as AgencyCompanyJobPosition;
const packer = { id: 'jp-2', jobPosition: 'Packer' } as AgencyCompanyJobPosition;

const resume: HoursWorkedResume = {
  totalRegularHours: 40,
  totalOvertimeHours: 4,
  totalHolidayHours: 0,
  totalNightHours: 0,
  totalHours: 44,
  totalPayRegular: 800,
  totalPayOvertime: 120,
  totalPayHoliday: 0,
  totalPayNight: 0,
  totalPay: 920,
  detail: [{
    workerName: 'John Smith',
    jobPosition: 'Forklift',
    payRate: 20,
    billRate: 28.5,
    regularHoursWorked: 40,
    overtimeHoursWorked: 4,
    holidayHoursWorked: 0,
    nightHoursWorked: 0,
    totalPayRegularRate: 800,
    totalPayOvertimeRate: 120,
    totalPayHolidayRate: 0,
    totalPayNightRate: 0,
    totalHoursWorked: 44,
    totalPayRate: 920,
  }],
};

let pinia: Pinia;

async function setup() {
  vi.mocked(getAgencyCompanyProfileWithRequests).mockResolvedValue([acme, globex]);
  vi.mocked(getJobPositionsHoursWorked).mockResolvedValue([forklift, packer]);
  const wrapper = mountWithPlugins(HoursWorkedReport, { global: { plugins: [pinia] } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function autocompletes(wrapper: Wrapper) {
  return wrapper.findAllComponents({ name: 'BAutocomplete' });
}

async function pickDates(wrapper: Wrapper) {
  wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', [new Date(2026, 10, 1), new Date(2026, 10, 14)]);
  await flushPromises();
}

async function pickCompany(wrapper: Wrapper, company: CompanyProfileListItem | null) {
  autocompletes(wrapper)[0].vm.$emit('update:modelValue', company?.fullName ?? '');
  autocompletes(wrapper)[0].vm.$emit('select', company);
  await flushPromises();
}

async function generate(wrapper: Wrapper) {
  await wrapper.findAll('button').find((b) => b.text() === 'Generate')?.trigger('click');
}

beforeEach(() => {
  pinia = createPinia();
  setActivePinia(pinia);
});

describe('HoursWorkedReport', () => {
  it('filters the companies by name', async () => {
    const wrapper = await setup();
    autocompletes(wrapper)[0].vm.$emit('update:modelValue', 'glo');
    await nextTick();

    expect(autocompletes(wrapper)[0].props('data')).toEqual([globex]);
  });

  it('loads job positions once both company and dates are set', async () => {
    const wrapper = await setup();
    await pickCompany(wrapper, acme);
    expect(getJobPositionsHoursWorked).not.toHaveBeenCalled();

    await pickDates(wrapper);
    expect(getJobPositionsHoursWorked).toHaveBeenCalledExactlyOnceWith({ startDate: '2026-11-01', endDate: '2026-11-14', companyProfileId: 'company-1' });

    autocompletes(wrapper)[1].vm.$emit('update:modelValue', 'pack');
    await nextTick();
    expect(autocompletes(wrapper)[1].props('data')).toEqual([packer]);
  });

  it('reloads job positions when the company changes after the dates', async () => {
    const wrapper = await setup();
    await pickDates(wrapper);
    await pickCompany(wrapper, globex);

    expect(getJobPositionsHoursWorked).toHaveBeenCalledExactlyOnceWith(expect.objectContaining({ companyProfileId: 'company-2' }));
  });

  it('clears the job positions when the company is cleared', async () => {
    const wrapper = await setup();
    await pickDates(wrapper);
    await pickCompany(wrapper, acme);
    await pickCompany(wrapper, null);

    expect(autocompletes(wrapper)[1].props('data')).toEqual([]);
  });

  it('drops the job position when the company changes', async () => {
    vi.mocked(getHoursWorkedReport).mockResolvedValue(resume);
    const wrapper = await setup();
    await pickDates(wrapper);
    await pickCompany(wrapper, acme);
    autocompletes(wrapper)[1].vm.$emit('update:modelValue', 'Forklift');
    autocompletes(wrapper)[1].vm.$emit('select', forklift);
    await pickCompany(wrapper, globex);
    await generate(wrapper);

    await vi.waitFor(() => { expect(getHoursWorkedReport).toHaveBeenCalledOnce(); });
    expect(vi.mocked(getHoursWorkedReport).mock.calls[0][0]).toEqual({
      startDate: '2026-11-01',
      endDate: '2026-11-14',
      companyProfileId: 'company-2',
    });
    expect(autocompletes(wrapper)[1].props('modelValue')).toBe('');
  });

  it('drops the job position when the company is cleared', async () => {
    vi.mocked(getHoursWorkedReport).mockResolvedValue(resume);
    const wrapper = await setup();
    await pickDates(wrapper);
    await pickCompany(wrapper, acme);
    autocompletes(wrapper)[1].vm.$emit('update:modelValue', 'Forklift');
    autocompletes(wrapper)[1].vm.$emit('select', forklift);
    await pickCompany(wrapper, null);
    await generate(wrapper);

    await vi.waitFor(() => { expect(getHoursWorkedReport).toHaveBeenCalledOnce(); });
    expect(vi.mocked(getHoursWorkedReport).mock.calls[0][0]).toEqual({ startDate: '2026-11-01', endDate: '2026-11-14' });
    expect(autocompletes(wrapper)[1].props('modelValue')).toBe('');
  });

  it('shows the error when the companies cannot load', async () => {
    const error = new Error('companies');
    vi.mocked(getAgencyCompanyProfileWithRequests).mockRejectedValue(error);
    const wrapper = mountWithPlugins(HoursWorkedReport, { global: { plugins: [pinia] } });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(false);
  });

  it('shows the error when the job positions cannot load', async () => {
    const error = new Error('positions');
    const wrapper = await setup();
    vi.mocked(getJobPositionsHoursWorked).mockRejectedValue(error);
    await pickCompany(wrapper, acme);
    await pickDates(wrapper);

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(autocompletes(wrapper)[1].props('loading')).toBe(false);
  });

  it('requires the dates', async () => {
    const wrapper = await setup();
    await generate(wrapper);

    await vi.waitFor(() => { expect(wrapper.text()).toContain('Dates are required'); });
    expect(getHoursWorkedReport).not.toHaveBeenCalled();
  });

  it('shows the hours and pay per worker with totals', async () => {
    vi.mocked(getHoursWorkedReport).mockResolvedValue(resume);
    const wrapper = await setup();
    await pickDates(wrapper);
    await pickCompany(wrapper, acme);
    autocompletes(wrapper)[1].vm.$emit('select', forklift);
    await generate(wrapper);

    await vi.waitFor(() => { expect(wrapper.find('tbody tr').exists()).toBe(true); });
    expect(getHoursWorkedReport).toHaveBeenCalledExactlyOnceWith({
      startDate: '2026-11-01',
      endDate: '2026-11-14',
      companyProfileId: 'company-1',
      jobPositionRateId: 'jp-1',
    });
    expect(wrapper.findAll('tbody tr td').map((c) => c.text())).toEqual([
      'John Smith', 'Forklift', '$28.50', '40', '$800.00', '4', '$120.00', '0', '$0.00', '44', '$920.00',
    ]);
    expect(wrapper.findAll('tfoot th').map((c) => c.text())).toEqual(['', '', '', '40', '$800.00', '4', '$120.00', '0', '$0.00', '44', '$920.00']);
  });

  it('drops the job position when it is cleared', async () => {
    vi.mocked(getHoursWorkedReport).mockResolvedValue(resume);
    const wrapper = await setup();
    await pickDates(wrapper);
    await pickCompany(wrapper, acme);
    autocompletes(wrapper)[1].vm.$emit('select', forklift);
    autocompletes(wrapper)[1].vm.$emit('select', null);
    await generate(wrapper);

    await vi.waitFor(() => { expect(getHoursWorkedReport).toHaveBeenCalledOnce(); });
    expect(vi.mocked(getHoursWorkedReport).mock.calls[0][0].jobPositionRateId).toBeUndefined();
  });

  it('shows no footer when there are no rows', async () => {
    vi.mocked(getHoursWorkedReport).mockResolvedValue({ ...resume, detail: [] });
    const wrapper = await setup();
    await pickDates(wrapper);
    await generate(wrapper);

    await vi.waitFor(() => { expect(getHoursWorkedReport).toHaveBeenCalledOnce(); });
    await flushPromises();
    expect(wrapper.findAll('tfoot th')).toHaveLength(0);
  });

  it('shows the error when the report fails', async () => {
    const error = new Error('report');
    vi.mocked(getHoursWorkedReport).mockRejectedValue(error);
    const wrapper = await setup();
    await pickDates(wrapper);
    await generate(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.find('table').exists()).toBe(false);
  });

  it('exports the report with the same filter', async () => {
    const blob = new Blob(['x']);
    vi.mocked(getHoursWorkedReport).mockResolvedValue(resume);
    vi.mocked(downloadAgencyReport).mockResolvedValue(blob);
    const wrapper = await setup();
    await pickDates(wrapper);
    await generate(wrapper);
    await vi.waitFor(() => { expect(wrapper.find('tbody tr').exists()).toBe(true); });

    await wrapper.find('.sigook-grid-toolbar .dropdown-trigger').trigger('click');
    await flushPromises();
    const exportItem = Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).find((i) => i.textContent.trim() === 'Export');
    exportItem?.click();
    await flushPromises();

    expect(downloadAgencyReport).toHaveBeenCalledWith('/api/agency/accounting/reports/hours-worked/file', { startDate: '2026-11-01', endDate: '2026-11-14' });
    expect(downloadFile).toHaveBeenCalledWith(blob, expect.stringMatching(/^Hours Worked Report_/));
  });

  it('shows the error when the export fails', async () => {
    const error = new Error('export');
    vi.mocked(getHoursWorkedReport).mockResolvedValue(resume);
    vi.mocked(downloadAgencyReport).mockRejectedValue(error);
    const wrapper = await setup();
    await pickDates(wrapper);
    await generate(wrapper);
    await vi.waitFor(() => { expect(wrapper.find('tbody tr').exists()).toBe(true); });

    await wrapper.find('.sigook-grid-toolbar .dropdown-trigger').trigger('click');
    await flushPromises();
    Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).find((i) => i.textContent.trim() === 'Export')?.click();
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });
});

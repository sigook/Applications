import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import ApplicantComplianceItems from '@/modules/agency/recruiting/applicants/components/ApplicantComplianceItems.vue';
import {
  completeApplicantComplianceItem,
  getApplicantComplianceItems,
  uncompleteApplicantComplianceItem,
} from '@/modules/agency/recruiting/requests/api';
import { getIdentificationTypes } from '@/shared/api/catalogApi';
import { ComplianceDocumentTarget, RequestApplicantStatus } from '@/shared/request-detail/requestApplicant';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { ApplicantComplianceItem } from '@/modules/agency/recruiting/requests/types';

vi.mock('@/modules/agency/recruiting/requests/api', () => ({
  completeApplicantComplianceItem: vi.fn(),
  getApplicantComplianceItems: vi.fn(),
  uncompleteApplicantComplianceItem: vi.fn(),
}));
vi.mock('@/shared/api/catalogApi', () => ({ getIdentificationTypes: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const items: ApplicantComplianceItem[] = [
  { id: 'i1', name: 'ID', isMandatory: true, documentTarget: ComplianceDocumentTarget.Identification1, isCompleted: false, canUpload: true },
  { id: 'i2', name: 'SIN/SSN', isMandatory: true, documentTarget: ComplianceDocumentTarget.SocialInsurance, isCompleted: false, canUpload: true },
  { id: 'i3', name: 'W4', isMandatory: true, documentTarget: ComplianceDocumentTarget.OtherDocument, isCompleted: false, canUpload: true, existingFileUrl: 'https://files/w4.pdf' },
  { id: 'i4', name: 'Banking', isMandatory: true, documentTarget: ComplianceDocumentTarget.OtherDocument, isCompleted: true, canUpload: true, completedBy: 'jane.doe@sigook.com', completedAt: '2026-03-01T12:00:00' },
  { id: 'i5', name: 'WP', isMandatory: false, documentTarget: ComplianceDocumentTarget.None, isCompleted: false, canUpload: false },
];

interface Options {
  status?: RequestApplicantStatus;
  workerProfileId?: string | null;
  response?: ApplicantComplianceItem[];
}

async function setup({ status = RequestApplicantStatus.InProgress, workerProfileId = 'wp-1', response = items }: Options = {}) {
  vi.mocked(getApplicantComplianceItems).mockResolvedValue(response.map((i) => ({ ...i })));
  vi.mocked(getIdentificationTypes).mockResolvedValue([{ id: 't1', value: 'Passport' }, { id: 't2', value: 'Driver licence' }]);
  const wrapper = mountWithPlugins(ApplicantComplianceItems, {
    props: { requestId: 'req-1', applicantId: 'app-1', status, workerProfileId },
  });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function row(wrapper: Wrapper, name: string) {
  const found = wrapper.findAll('.compliance-item').find((r) => r.find('.checkbox').text() === name);
  if (!found) throw new Error(`${name} row not found`);
  return found;
}

function checkbox(wrapper: Wrapper, name: string) {
  return row(wrapper, name).find('input[type="checkbox"]');
}

function blockedReason(wrapper: Wrapper, name: string) {
  return row(wrapper, name).findComponent({ name: 'BTooltip' }).props('label') as string;
}

async function attach(wrapper: Wrapper, name: string, file: File | null) {
  row(wrapper, name).findComponent({ name: 'BUpload' }).vm.$emit('update:modelValue', file);
  await flushPromises();
}

function pdf(name = 'doc.pdf') {
  return new File(['x'], name, { type: 'application/pdf' });
}

describe('ApplicantComplianceItems', () => {
  it('loads the checklist and reports its progress', async () => {
    const wrapper = await setup();

    expect(getApplicantComplianceItems).toHaveBeenCalledWith('req-1', 'app-1');
    expect(getIdentificationTypes).toHaveBeenCalledOnce();
    expect(wrapper.emitted('loaded')).toEqual([[{ mandatoryCompleted: false, itemsCount: 5, completedCount: 1 }]]);
    expect(row(wrapper, 'Banking').text()).toContain('jane.doe');
    expect(row(wrapper, 'W4').find('a').attributes('href')).toBe('https://files/w4.pdf');
    expect(wrapper.findAll('.tag').map((t) => t.text())).toEqual(['Mandatory', 'Mandatory', 'Mandatory', 'Mandatory']);
  });

  it('reports a complete checklist when only optional items are pending', async () => {
    const wrapper = await setup({ response: [items[3], items[4]] });

    expect(wrapper.emitted('loaded')).toEqual([[{ mandatoryCompleted: true, itemsCount: 2, completedCount: 1 }]]);
    expect(getIdentificationTypes).not.toHaveBeenCalled();
  });

  it('says when the request has no requirements', async () => {
    const wrapper = await setup({ response: [] });

    expect(wrapper.text()).toContain('No requirements configured for this request.');
  });

  it('shows the error when the checklist fails to load', async () => {
    const error = new Error('load failed');
    vi.mocked(getApplicantComplianceItems).mockRejectedValue(error);
    mountWithPlugins(ApplicantComplianceItems, { props: { requestId: 'req-1', applicantId: 'app-1', status: RequestApplicantStatus.InProgress } });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('shows the error when the identification types fail to load', async () => {
    const error = new Error('types failed');
    vi.mocked(getApplicantComplianceItems).mockResolvedValue([items[0]]);
    vi.mocked(getIdentificationTypes).mockRejectedValue(error);
    mountWithPlugins(ApplicantComplianceItems, { props: { requestId: 'req-1', applicantId: 'app-1', status: RequestApplicantStatus.InProgress } });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('locks every item until the applicant is started', async () => {
    const wrapper = await setup({ status: RequestApplicantStatus.Pending });

    expect(wrapper.find('.message').text()).toBe('Start the applicant to mark compliance items.');
    expect(wrapper.findAll('input[type="checkbox"]').every((c) => c.attributes('disabled') !== undefined)).toBe(true);
    expect(wrapper.findComponent({ name: 'BUpload' }).exists()).toBe(false);
  });

  it('explains that a candidate cannot upload documents yet', async () => {
    const wrapper = await setup({ workerProfileId: null });

    expect(wrapper.find('.message').text()).toBe('Convert to worker to upload documents. Checks can still be marked.');
  });

  it('offers to attach or replace documents on pending items', async () => {
    const wrapper = await setup();

    expect(wrapper.find('.message').text()).toContain('Attach the document and fill the information');
    expect(row(wrapper, 'ID').find('input[placeholder="Identification number"]').exists()).toBe(true);
    expect(row(wrapper, 'ID').findAll('option').map((o) => o.text())).toEqual(['Type', 'Passport', 'Driver licence']);
    expect(row(wrapper, 'SIN/SSN').find('input[placeholder="Social insurance number"]').exists()).toBe(true);
    expect(row(wrapper, 'ID').find('.file-label').text()).toContain('Attach');
    expect(row(wrapper, 'W4').find('.file-label').text()).toContain('Replace');
    expect(row(wrapper, 'WP').findComponent({ name: 'BUpload' }).exists()).toBe(false);
    expect(row(wrapper, 'Banking').findComponent({ name: 'BUpload' }).exists()).toBe(false);
  });

  it('only lets a mandatory item be checked once its document and data are filled', async () => {
    const wrapper = await setup();

    expect(checkbox(wrapper, 'ID').attributes('disabled')).toBeDefined();
    expect(blockedReason(wrapper, 'ID')).toBe('Attach the document to complete this item');
    expect(checkbox(wrapper, 'W4').attributes('disabled')).toBeUndefined();
    expect(checkbox(wrapper, 'WP').attributes('disabled')).toBeUndefined();

    await attach(wrapper, 'ID', pdf('passport.pdf'));
    expect(row(wrapper, 'ID').find('.file-name').text()).toBe('passport.pdf');
    expect(blockedReason(wrapper, 'ID')).toBe('Enter the identification number and type to complete this item');

    await row(wrapper, 'ID').find('input[placeholder="Identification number"]').setValue('P123');
    await row(wrapper, 'ID').find('select').setValue('t2');
    expect(blockedReason(wrapper, 'ID')).toBe('');
    expect(checkbox(wrapper, 'ID').attributes('disabled')).toBeUndefined();

    await attach(wrapper, 'SIN/SSN', pdf());
    expect(blockedReason(wrapper, 'SIN/SSN')).toBe('Enter the social insurance number to complete this item');
    await row(wrapper, 'SIN/SSN').find('input[placeholder="Social insurance number"]').setValue('123456789');
    expect(blockedReason(wrapper, 'SIN/SSN')).toBe('');
  });

  it('completes the identification with its document and data, then reloads', async () => {
    vi.mocked(completeApplicantComplianceItem).mockResolvedValue();
    const wrapper = await setup();
    const file = pdf('passport.pdf');
    await attach(wrapper, 'ID', file);
    await row(wrapper, 'ID').find('input[placeholder="Identification number"]').setValue('P123');
    await row(wrapper, 'ID').find('select').setValue('t1');
    await checkbox(wrapper, 'ID').setValue(true);
    await flushPromises();

    expect(completeApplicantComplianceItem).toHaveBeenCalledWith('req-1', 'app-1', 'i1', {
      fileName: expect.stringMatching(/^Document_\w+\.pdf$/) as unknown as string,
      identificationNumber: 'P123',
      identificationTypeId: 't1',
    }, file);
    expect(wrapper.emitted('updated')).toHaveLength(1);
    expect(getApplicantComplianceItems).toHaveBeenCalledTimes(2);
  });

  it('completes the social insurance with its number', async () => {
    vi.mocked(completeApplicantComplianceItem).mockResolvedValue();
    const wrapper = await setup();
    const file = pdf('sin.pdf');
    await attach(wrapper, 'SIN/SSN', file);
    await row(wrapper, 'SIN/SSN').find('input[placeholder="Social insurance number"]').setValue('123456789');
    await checkbox(wrapper, 'SIN/SSN').setValue(true);
    await flushPromises();

    expect(completeApplicantComplianceItem).toHaveBeenCalledWith('req-1', 'app-1', 'i2', expect.objectContaining({ socialInsuranceNumber: '123456789' }), file);
  });

  it('completes an item with a document on file as is', async () => {
    vi.mocked(completeApplicantComplianceItem).mockResolvedValue();
    const wrapper = await setup();
    await checkbox(wrapper, 'W4').setValue(true);
    await flushPromises();

    expect(completeApplicantComplianceItem).toHaveBeenCalledWith('req-1', 'app-1', 'i3', {}, null);
  });

  it('completes an item without uploads with an empty model', async () => {
    vi.mocked(completeApplicantComplianceItem).mockResolvedValue();
    const wrapper = await setup();
    await checkbox(wrapper, 'WP').setValue(true);
    await flushPromises();

    expect(completeApplicantComplianceItem).toHaveBeenCalledWith('req-1', 'app-1', 'i5', {}, null);
  });

  it('reverts the check and shows the error when completing fails', async () => {
    const error = new Error('complete failed');
    vi.mocked(completeApplicantComplianceItem).mockRejectedValue(error);
    const wrapper = await setup();
    await checkbox(wrapper, 'W4').setValue(true);
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect((checkbox(wrapper, 'W4').element as HTMLInputElement).checked).toBe(false);
    expect(wrapper.emitted('updated')).toBeUndefined();
  });

  it('uncompletes a checked item', async () => {
    vi.mocked(uncompleteApplicantComplianceItem).mockResolvedValue();
    const wrapper = await setup();
    await checkbox(wrapper, 'Banking').setValue(false);
    await flushPromises();

    expect(uncompleteApplicantComplianceItem).toHaveBeenCalledWith('req-1', 'app-1', 'i4');
    expect(wrapper.emitted('updated')).toHaveLength(1);
    expect(getApplicantComplianceItems).toHaveBeenCalledTimes(2);
  });

  it('keeps the item checked when uncompleting fails', async () => {
    const error = new Error('uncomplete failed');
    vi.mocked(uncompleteApplicantComplianceItem).mockRejectedValue(error);
    const wrapper = await setup();
    await checkbox(wrapper, 'Banking').setValue(false);
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect((checkbox(wrapper, 'Banking').element as HTMLInputElement).checked).toBe(true);
  });

  it('rejects a file with a forbidden extension', async () => {
    const wrapper = await setup();
    await attach(wrapper, 'ID', new File(['x'], 'virus.exe'));

    expect(showAlertError).toHaveBeenCalledWith(expect.stringContaining('File extension not allowed'));
    expect(row(wrapper, 'ID').find('.file-name').exists()).toBe(false);
  });

  it('ignores an empty file selection and clears an attached file', async () => {
    const wrapper = await setup();
    await attach(wrapper, 'W4', null);
    expect(showAlertError).not.toHaveBeenCalled();

    await attach(wrapper, 'W4', pdf('w4-new.pdf'));
    expect(row(wrapper, 'W4').find('.file-name').text()).toBe('w4-new.pdf');
    expect(blockedReason(wrapper, 'W4')).toBe('');
    await row(wrapper, 'W4').find('button').trigger('click');
    expect(row(wrapper, 'W4').find('.file-name').exists()).toBe(false);
  });

  it('reloads when the applicant changes', async () => {
    const wrapper = await setup();
    await wrapper.setProps({ applicantId: 'app-2' });
    await flushPromises();

    expect(getApplicantComplianceItems).toHaveBeenLastCalledWith('req-1', 'app-2');
  });
});

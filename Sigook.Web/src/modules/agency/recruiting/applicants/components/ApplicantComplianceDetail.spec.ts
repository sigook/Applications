import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import ApplicantComplianceDetail from '@/modules/agency/recruiting/applicants/components/ApplicantComplianceDetail.vue';
import { changeApplicantStatus } from '@/modules/agency/recruiting/requests/api';
import { RequestApplicantStatus } from '@/shared/request-detail/requestApplicant';
import { getDialog } from '@/shared/utils/buefyProgrammatic';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/agency/recruiting/requests/api', () => ({ changeApplicantStatus: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/shared/utils/buefyProgrammatic', () => {
  const dialog = { confirm: vi.fn() };
  return { getDialog: () => dialog };
});
vi.mock('@/modules/agency/recruiting/applicants/components/ApplicantComplianceItems.vue', () => ({
  default: {
    name: 'ApplicantComplianceItems',
    props: { requestId: String, applicantId: String, status: Number, workerProfileId: String },
    emits: ['loaded'],
    template: '<div class="items-stub" />',
  },
}));

interface Props {
  status: RequestApplicantStatus;
  workerProfileId?: string | null;
}

function setup({ status, workerProfileId = 'wp-1' }: Props) {
  return mountWithPlugins(ApplicantComplianceDetail, {
    props: { requestId: 'req-1', applicantId: 'app-1', name: 'Ana Lopez', status, workerProfileId },
  });
}

type Wrapper = ReturnType<typeof setup>;

function buttons(wrapper: Wrapper) {
  return wrapper.findAll('.applicants-detail-actions button').map((b) => b.text());
}

function button(wrapper: Wrapper, text: string) {
  const found = wrapper.findAll('button').find((b) => b.text() === text);
  if (!found) throw new Error(`${text} not found`);
  return found;
}

async function loadItems(wrapper: Wrapper, mandatoryCompleted: boolean) {
  wrapper.findComponent({ name: 'ApplicantComplianceItems' }).vm.$emit('loaded', { mandatoryCompleted, itemsCount: 3, completedCount: 2 });
  await flushPromises();
}

function tooltipLabel(wrapper: Wrapper) {
  return wrapper.findComponent({ name: 'BTooltip' }).props('label') as string;
}

describe('ApplicantComplianceDetail', () => {
  it('passes the applicant to the checklist', () => {
    const wrapper = setup({ status: RequestApplicantStatus.InProgress });

    expect(wrapper.findComponent({ name: 'ApplicantComplianceItems' }).props()).toEqual({
      requestId: 'req-1',
      applicantId: 'app-1',
      status: RequestApplicantStatus.InProgress,
      workerProfileId: 'wp-1',
    });
  });

  it.each([
    [RequestApplicantStatus.Pending, ['Cancel applicant', 'Start']],
    [RequestApplicantStatus.InProgress, ['Cancel applicant', 'Confirm applicant']],
    [RequestApplicantStatus.Confirmed, []],
    [RequestApplicantStatus.Cancelled, ['Reopen']],
  ])('status %s offers %j', (status, expected) => {
    expect(buttons(setup({ status }))).toEqual(expected);
  });

  it('blocks the confirmation until the mandatory items are complete', async () => {
    const wrapper = setup({ status: RequestApplicantStatus.InProgress });

    expect(button(wrapper, 'Confirm applicant').attributes('disabled')).toBeDefined();
    expect(tooltipLabel(wrapper)).toBe('Complete all mandatory items first');

    await loadItems(wrapper, true);
    expect(button(wrapper, 'Confirm applicant').attributes('disabled')).toBeUndefined();
    expect(wrapper.emitted('loaded')).toEqual([[{ mandatoryCompleted: true, itemsCount: 3, completedCount: 2 }]]);
  });

  it('blocks the confirmation of a candidate', async () => {
    const wrapper = setup({ status: RequestApplicantStatus.InProgress, workerProfileId: null });
    await loadItems(wrapper, true);

    expect(button(wrapper, 'Confirm applicant').attributes('disabled')).toBeDefined();
    expect(tooltipLabel(wrapper)).toBe('Convert the candidate to a worker first');
  });

  it('starts the applicant and moves to its new actions', async () => {
    vi.mocked(changeApplicantStatus).mockResolvedValue();
    const wrapper = setup({ status: RequestApplicantStatus.Pending });
    await button(wrapper, 'Start').trigger('click');
    await flushPromises();

    expect(changeApplicantStatus).toHaveBeenCalledWith('req-1', 'app-1', { status: RequestApplicantStatus.InProgress });
    expect(wrapper.emitted('statusChanged')).toHaveLength(1);
    expect(buttons(wrapper)).toEqual(['Cancel applicant', 'Confirm applicant']);
  });

  it('confirms an applicant with every mandatory item done', async () => {
    vi.mocked(changeApplicantStatus).mockResolvedValue();
    const wrapper = setup({ status: RequestApplicantStatus.InProgress });
    await loadItems(wrapper, true);
    await button(wrapper, 'Confirm applicant').trigger('click');
    await flushPromises();

    expect(changeApplicantStatus).toHaveBeenCalledWith('req-1', 'app-1', { status: RequestApplicantStatus.Confirmed });
    expect(buttons(wrapper)).toEqual([]);
  });

  it('reopens a cancelled applicant', async () => {
    vi.mocked(changeApplicantStatus).mockResolvedValue();
    const wrapper = setup({ status: RequestApplicantStatus.Cancelled });
    await button(wrapper, 'Reopen').trigger('click');
    await flushPromises();

    expect(changeApplicantStatus).toHaveBeenCalledWith('req-1', 'app-1', { status: RequestApplicantStatus.InProgress });
  });

  it('asks before cancelling', async () => {
    const wrapper = setup({ status: RequestApplicantStatus.Pending });
    await button(wrapper, 'Cancel applicant').trigger('click');

    expect(getDialog().confirm).toHaveBeenCalledWith(expect.objectContaining({ title: 'Cancel applicant' }));
    expect(changeApplicantStatus).not.toHaveBeenCalled();
  });

  it('follows the status prop when the list reloads', async () => {
    const wrapper = setup({ status: RequestApplicantStatus.Pending });
    await wrapper.setProps({ status: RequestApplicantStatus.Cancelled });

    expect(buttons(wrapper)).toEqual(['Reopen']);
  });
});

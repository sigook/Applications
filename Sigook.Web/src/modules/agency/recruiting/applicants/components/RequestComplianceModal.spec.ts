import { describe, expect, it } from 'vitest';
import RequestComplianceModal from '@/modules/agency/recruiting/applicants/components/RequestComplianceModal.vue';
import { DEFAULT_COMPLIANCE_ITEMS } from '@/modules/agency/recruiting/applicants/compliance';
import { ComplianceDocumentTarget } from '@/shared/request-detail/requestApplicant';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { RequestComplianceItem } from '@/shared/request-detail/types';

const stored: RequestComplianceItem[] = [
  { id: 'c1', name: 'ID', isMandatory: true, documentTarget: ComplianceDocumentTarget.Identification1 },
  { id: 'c2', name: 'Banking', isMandatory: false, documentTarget: ComplianceDocumentTarget.OtherDocument },
];

function setup(items?: RequestComplianceItem[]) {
  return mountWithPlugins(RequestComplianceModal, { props: { items } });
}

type Wrapper = ReturnType<typeof setup>;

function names(wrapper: Wrapper) {
  return wrapper.findAll('.compliance-name').map((n) => n.text());
}

function button(wrapper: Wrapper, text: string) {
  const found = wrapper.findAll('button').find((b) => b.text() === text);
  if (!found) throw new Error(`${text} button not found`);
  return found;
}

function saved(wrapper: Wrapper): RequestComplianceItem[] {
  const events = wrapper.emitted('save');
  if (!events) throw new Error('save not emitted');
  return events.at(-1)?.[0] as RequestComplianceItem[];
}

async function add(wrapper: Wrapper, name: string) {
  await wrapper.find('.compliance-new-name input').setValue(name);
  await button(wrapper, 'Add').trigger('click');
}

describe('RequestComplianceModal', () => {
  it('says when no requirement is configured', () => {
    const wrapper = setup();

    expect(wrapper.text()).toContain('No requirements configured.');
    expect(names(wrapper)).toEqual([]);
  });

  it('lists the configured requirements with their mandatory flag', () => {
    const wrapper = setup(stored);

    expect(names(wrapper)).toEqual(['ID', 'Banking']);
    expect(wrapper.findAll('.compliance-row').map((r) => r.find('.switch').text())).toEqual(['Mandatory', 'Optional']);
  });

  it('saves a copy of the edited items without touching the props', async () => {
    const items = stored.map((i) => ({ ...i }));
    const wrapper = setup(items);
    await wrapper.findAll('.compliance-row')[1].find('input[type="checkbox"]').setValue(true);
    await wrapper.findAll('.compliance-row')[1].find('select').setValue(String(ComplianceDocumentTarget.Resume));
    await button(wrapper, 'Save').trigger('click');

    expect(saved(wrapper)[1]).toEqual({ id: 'c2', name: 'Banking', isMandatory: true, documentTarget: ComplianceDocumentTarget.Resume });
    expect(items[1].isMandatory).toBe(false);
  });

  it('adds a requirement and resets the new item fields', async () => {
    const wrapper = setup(stored);
    await wrapper.find('.compliance-add select').setValue(String(ComplianceDocumentTarget.PoliceCheck));
    await wrapper.find('.compliance-add input[type="checkbox"]').setValue(false);
    await add(wrapper, '  Police check  ');

    expect(names(wrapper)).toEqual(['ID', 'Banking', 'Police check']);
    expect((wrapper.find('.compliance-new-name input').element as HTMLInputElement).value).toBe('');
    await button(wrapper, 'Save').trigger('click');
    expect(saved(wrapper)[2]).toEqual({ name: 'Police check', isMandatory: false, documentTarget: ComplianceDocumentTarget.PoliceCheck });
  });

  it('adds with enter', async () => {
    const wrapper = setup();
    await wrapper.find('.compliance-new-name input').setValue('WHMIS');
    await wrapper.find('.compliance-new-name input').trigger('keyup', { key: 'Enter' });

    expect(names(wrapper)).toEqual(['WHMIS']);
  });

  it('requires a name', async () => {
    const wrapper = setup();
    await add(wrapper, '   ');

    expect(wrapper.find('.help.is-danger').text()).toBe('Requirement name is required');
    expect(names(wrapper)).toEqual([]);
  });

  it('rejects a duplicated name ignoring case and clears the error on the next add', async () => {
    const wrapper = setup(stored);
    await add(wrapper, 'banking');

    expect(wrapper.find('.help.is-danger').text()).toBe('That requirement already exists');
    await add(wrapper, 'W4');
    expect(wrapper.find('.help.is-danger').exists()).toBe(false);
  });

  it('removes a requirement', async () => {
    const wrapper = setup(stored);
    await add(wrapper, 'ID');
    await wrapper.findAll('.compliance-row')[0].find('button').trigger('click');

    expect(names(wrapper)).toEqual(['Banking']);
    expect(wrapper.find('.help.is-danger').exists()).toBe(false);
  });

  it('resets to the default requirements', async () => {
    const wrapper = setup(stored);
    await button(wrapper, 'Reset to defaults').trigger('click');
    await button(wrapper, 'Save').trigger('click');

    expect(saved(wrapper)).toEqual(DEFAULT_COMPLIANCE_ITEMS);
  });
});

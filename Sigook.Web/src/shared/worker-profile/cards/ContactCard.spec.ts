import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import ContactCard from '@/shared/worker-profile/cards/ContactCard.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail } from '@/testing/workerProfileFixtures';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';

const { formStub } = vi.hoisted(() => ({
  formStub: (name: string) => ({ name, props: { data: Object }, emits: ['closeModal'], template: `<div class="${name}" />` }),
}));

vi.mock('@/shared/worker-profile/forms/WorkContactInformationForm.vue', () => ({ default: formStub('ContactInformationForm') }));
vi.mock('@/shared/worker-profile/forms/WorkEmergencyInformationForm.vue', () => ({ default: formStub('EmergencyInformationForm') }));

function setup(overrides: Partial<WorkerProfileDetail> = {}) {
  return mountWithPlugins(ContactCard, {
    props: { worker: createWorkerProfileDetail(overrides) },
  });
}

function field(wrapper: ReturnType<typeof setup>, group: number, label: string) {
  const row = wrapper.findAll('.contact-group')[group].findAll('.profile-field').find((f) => f.find('dt').text() === label);
  if (!row) throw new Error(`${label} not found`);
  return row.find('dd');
}

function menuItem(text: string) {
  const item = Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).find((i) => i.textContent.trim() === text);
  if (!item) throw new Error(`${text} not found`);
  return item;
}

describe('ContactCard', () => {
  it('shows placeholders for missing contact data', () => {
    const wrapper = setup();

    expect(field(wrapper, 0, 'Address').text()).toBe('—');
    expect(field(wrapper, 0, 'Mobile').text()).toBe('—');
    expect(wrapper.findAll('.contact-group')[0].text()).not.toContain('Phone');
    expect(field(wrapper, 1, 'Name').text()).toBe('—');
    expect(field(wrapper, 1, 'Phone').text()).toBe('—');
    expect(field(wrapper, 1, 'Health problems / allergies').text()).toBe('None');
  });

  it('formats the address, phones and emergency contact', () => {
    const wrapper = setup({
      location: {
        address: '12 King St',
        city: { id: 'c1', value: 'Toronto', province: { id: 'p1', value: 'Ontario', code: 'ON' } },
        postalCode: 'M5V 1A1',
      },
      mobileNumber: '4165550000',
      phone: '4165551111',
      phoneExt: 12,
      contactEmergencyName: 'Mary',
      contactEmergencyLastName: 'Doe',
      contactEmergencyPhone: '4165552222',
      haveAnyHealthProblem: true,
      healthProblem: 'Asthma',
      otherHealthProblem: 'Peanuts',
    });

    expect(field(wrapper, 0, 'Address').text()).toBe('12 King St Toronto, ON M5V 1A1');
    expect(field(wrapper, 0, 'Mobile').find('a').attributes('href')).toBe('tel:4165550000');
    expect(field(wrapper, 0, 'Phone').find('a').text()).toBe('4165551111 Ext. 12');
    expect(field(wrapper, 1, 'Phone').find('a').attributes('href')).toBe('tel:4165552222');
    expect(field(wrapper, 1, 'Name').text()).toBe('Mary Doe');
    expect(field(wrapper, 1, 'Health problems / allergies').text()).toBe('Asthma · Peanuts');
  });

  it('says Yes when a health problem has no description', () => {
    expect(field(setup({ haveAnyHealthProblem: true }), 1, 'Health problems / allergies').text()).toBe('Yes');
  });

  it('opens the chosen form and emits updateProfile once it closes', async () => {
    const wrapper = setup();
    menuItem('Emergency information').click();
    await nextTick();

    const form = wrapper.findComponent({ name: 'EmergencyInformationForm' });
    expect(form.exists()).toBe(true);
    expect(wrapper.findComponent({ name: 'ContactInformationForm' }).exists()).toBe(false);
    form.vm.$emit('closeModal');

    expect(wrapper.emitted('updateProfile')).toHaveLength(1);
  });
});

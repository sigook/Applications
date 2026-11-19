import { describe, expect, it, vi } from 'vitest';
import CompanyRequests from '@/modules/agency/recruiting/clients/components/CompanyRequests.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createAgencyCompany } from '@/testing/agencyCompanyFixtures';

vi.mock('@/modules/agency/recruiting/requests/components/TableRequests.vue', () => ({
  default: {
    name: 'TableRequests',
    props: { companyProfileId: String },
    emits: ['onDataLoading'],
    template: '<div class="table-requests-stub" />',
  },
}));

function setup() {
  return mountWithPlugins(CompanyRequests, { props: { company: createAgencyCompany() } });
}

function isLoading(wrapper: ReturnType<typeof setup>) {
  return wrapper.findComponent({ name: 'BLoading' }).props('modelValue') as boolean;
}

describe('CompanyRequests', () => {
  it('lists the requests of the company', () => {
    const wrapper = setup();

    expect(wrapper.findComponent({ name: 'TableRequests' }).props('companyProfileId')).toBe('company-profile-1');
  });

  it('shows the loader while the table is loading', async () => {
    const wrapper = setup();
    expect(isLoading(wrapper)).toBe(true);

    wrapper.findComponent({ name: 'TableRequests' }).vm.$emit('onDataLoading', false);
    await wrapper.vm.$nextTick();

    expect(isLoading(wrapper)).toBe(false);
  });
});

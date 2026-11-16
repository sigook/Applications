import { describe, it, expect, beforeEach } from 'vitest';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { useAgencyStore } from '@/modules/agency/store';
import { agencyCode } from '@/modules/agency/shared/useAgencyName';
import AgencyBadge from '@/modules/agency/shared/AgencyBadge.vue';

const covenant = { id: 'a1', name: 'Covenant' };
const sigook = { id: 'a2', name: 'Sigook Staffing' };

let pinia: Pinia;

function mountBadge(agencyId?: string | null) {
  return mountWithPlugins(AgencyBadge, {
    props: { agencyId },
    global: { plugins: [pinia] },
  });
}

describe('AgencyBadge', () => {
  beforeEach(() => {
    pinia = createPinia();
    setActivePinia(pinia);
    useAgencyStore().setAgencies([covenant, sigook]);
  });

  it('shows a flag with the agency code and color when the user has several agencies', () => {
    const flag = mountBadge('a2').find('.request-flag');
    expect(flag.text()).toBe('SS');
    expect(flag.attributes('aria-label')).toBe('Agency: Sigook Staffing');
    expect(flag.classes()).toContain('request-flag--agency-1');
  });

  it('is hidden when the user has a single agency', () => {
    useAgencyStore().setAgencies([covenant]);
    expect(mountBadge('a1').find('.request-flag').exists()).toBe(false);
  });

  it('is hidden when the agency is unknown or missing', () => {
    expect(mountBadge('zz').find('.request-flag').exists()).toBe(false);
    expect(mountBadge(null).find('.request-flag').exists()).toBe(false);
  });
});

describe('agencyCode', () => {
  it('uses the first three letters of a single word', () => {
    expect(agencyCode('Covenant')).toBe('COV');
  });

  it('uses the initials of up to three words', () => {
    expect(agencyCode('Sigook Staffing Solutions Inc')).toBe('SSS');
  });

  it('returns an empty code for a blank name', () => {
    expect(agencyCode('  ')).toBe('');
  });
});

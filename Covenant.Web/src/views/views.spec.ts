import { describe, expect, it, vi } from 'vitest'
import { flushPromises } from '@vue/test-utils'
import type { Component } from 'vue'
import { mountAt } from '@/testing/mountWithPlugins'
import AboutUsView from './AboutUsView.vue'
import BecomePartnerView from './BecomePartnerView.vue'
import DisclaimerView from './DisclaimerView.vue'
import EmployersView from './EmployersView.vue'
import HomeView from './HomeView.vue'
import IndustriesView from './IndustriesView.vue'
import LicensedCertifiedView from './LicensedCertifiedView.vue'
import OpenPositionsView from './OpenPositionsView.vue'
import PrivacyPolicyView from './PrivacyPolicyView.vue'
import TalentsView from './TalentsView.vue'
import TermsConditionsView from './TermsConditionsView.vue'

vi.mock('@/services/locationService', () => ({
  locationService: {
    getCountries: vi.fn().mockResolvedValue([]),
    getProvinces: vi.fn().mockResolvedValue([]),
    getCities: vi.fn().mockResolvedValue([]),
    getSkills: vi.fn().mockResolvedValue([]),
    getSources: vi.fn().mockResolvedValue([]),
  },
}))

vi.mock('@/services/jobService', () => ({
  jobService: { getJobs: vi.fn().mockResolvedValue([]) },
}))

const views: [string, string, Component, string][] = [
  ['AboutUsView', '/about', AboutUsView, 'footer'],
  ['BecomePartnerView', '/become-partner', BecomePartnerView, 'footer'],
  ['DisclaimerView', '/disclaimer', DisclaimerView, 'Disclaimer'],
  ['EmployersView', '/employers', EmployersView, 'footer'],
  ['HomeView', '/', HomeView, 'footer'],
  ['IndustriesView', '/industries', IndustriesView, 'footer'],
  ['LicensedCertifiedView', '/licensed-certified', LicensedCertifiedView, 'footer'],
  ['OpenPositionsView', '/open-positions', OpenPositionsView, 'No Jobs Available'],
  ['PrivacyPolicyView', '/privacy-policy', PrivacyPolicyView, 'Privacy'],
  ['TalentsView', '/talents', TalentsView, 'footer'],
  ['TermsConditionsView', '/terms-conditions', TermsConditionsView, 'Terms'],
]

describe('views', () => {
  it.each(views)('%s renders its page', async (_name, path, view, marker) => {
    const { wrapper } = await mountAt(view, path)
    await flushPromises()

    if (marker === 'footer') {
      expect(wrapper.find('footer').exists()).toBe(true)
    } else {
      expect(wrapper.text()).toContain(marker)
    }
  })
})

import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'
import HomeView from '@/views/HomeView.vue'
import LicensedCertifiedView from '@/views/LicensedCertifiedView.vue'
import AboutUsView from '@/views/AboutUsView.vue'
import IndustriesView from '@/views/IndustriesView.vue'
import BecomePartnerView from '@/views/BecomePartnerView.vue'
import EmployersView from '@/views/EmployersView.vue'
import TalentsView from '@/views/TalentsView.vue'
import OpenPositionsView from '@/views/OpenPositionsView.vue'
import DisclaimerView from '@/views/DisclaimerView.vue'
import PrivacyPolicyView from '@/views/PrivacyPolicyView.vue'
import TermsConditionsView from '@/views/TermsConditionsView.vue'

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'home',
    component: HomeView,
  },
  {
    path: '/licensed-certified',
    name: 'licensed',
    component: LicensedCertifiedView,
  },
  {
    path: '/about',
    name: 'about',
    component: AboutUsView,
  },
  {
    path: '/industries',
    name: 'industries',
    component: IndustriesView,
  },
  {
    path: '/become-partner',
    name: 'become_partner',
    component: BecomePartnerView,
  },
  {
    path: '/employers',
    name: 'employers',
    component: EmployersView,
  },
  {
    path: '/talents',
    name: 'talents',
    component: TalentsView,
  },
  {
    path: '/open-positions',
    name: 'open_positions',
    component: OpenPositionsView,
  },
  {
    path: '/disclaimer',
    name: 'disclaimer',
    component: DisclaimerView,
  },
  {
    path: '/privacy-policy',
    name: 'privacy_policy',
    component: PrivacyPolicyView,
  },
  {
    path: '/terms-conditions',
    name: 'terms_conditions',
    component: TermsConditionsView,
  },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
  scrollBehavior(to, from, savedPosition) {
    if (to.hash) {
      return {
        el: to.hash,
        behavior: 'smooth',
      }
    } else if (savedPosition) {
      return savedPosition
    } else if (to.path === from.path && to.name === 'open_positions') {
      return false
    } else {
      return { top: 0, behavior: 'smooth' }
    }
  },
})

export default router


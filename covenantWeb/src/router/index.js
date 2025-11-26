import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '@/views/HomeView.vue'
import LicensedCertifiedView from '@/views/LicensedCertifiedView.vue'
import AboutUsView from '@/views/AboutUsView.vue'
import IndustriesView from '@/views/IndustriesView.vue'
import BecomePartnerView from '@/views/BecomePartnerView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
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
  ],
})

export default router

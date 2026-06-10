import { RouteRecordRaw } from 'vue-router';

const Home               = () => import('@/pages/v2/landing/Home/Home.vue');
const AboutUs            = () => import('@/pages/v2/landing/About/AboutUs.vue');
const Industries         = () => import('@/pages/v2/landing/Industries/Industries.vue');
const Talents            = () => import('@/pages/v2/landing/Talents/Talents.vue');
const Employers          = () => import('@/pages/v2/landing/Employers/Employers.vue');
const News               = () => import('@/pages/v2/landing/News/News.vue');
const OpenPositions      = () => import('@/pages/v2/landing/OpenPositions/OpenPositions.vue');
const Apply              = () => import('@/pages/v2/landing/Apply/Apply.vue');
const Partner            = () => import('@/pages/v2/landing/Partner/Partner.vue');
const SpecialProjects    = () => import('@/pages/v2/landing/SpecialProjects/SpecialProjects.vue');
const PrivacyPolicy      = () => import('@/pages/v2/landing/Legal/PrivacyPolicy.vue');
const TermsAndConditions = () => import('@/pages/v2/landing/Legal/TermsAndConditions.vue');
const ComingSoon         = () => import('@/pages/v2/landing/ComingSoon.vue');

const cs = (path: string, name: string, title: string): RouteRecordRaw => ({
    path,
    name,
    component: ComingSoon,
    meta: { layout: 'v2', requiresAuth: false, title },
});

const routesV2: RouteRecordRaw[] = [
    {
        path: '/',
        name: 'v2-home',
        component: Home,
        meta: { layout: 'v2', requiresAuth: false },
    },
    {
        path: '/open-positions',
        name: 'v2-open-positions',
        component: OpenPositions,
        meta: { layout: 'v2', requiresAuth: false, title: 'Open Positions' },
    },
    {
        path: '/industries',
        name: 'v2-industries',
        component: Industries,
        meta: { layout: 'v2', requiresAuth: false, title: 'Industries' },
    },
    {
        path: '/news',
        name: 'v2-news',
        component: News,
        meta: { layout: 'v2', requiresAuth: false, title: 'News' },
    },
    {
        path: '/about',
        name: 'v2-about',
        component: AboutUs,
        meta: { layout: 'v2', requiresAuth: false, title: 'About Us' },
    },
    {
        path: '/employers',
        name: 'v2-employers',
        component: Employers,
        meta: { layout: 'v2', requiresAuth: false, title: 'For Employers' },
    },
    {
        path: '/talents',
        name: 'v2-talents',
        component: Talents,
        meta: { layout: 'v2', requiresAuth: false, title: 'For Talents' },
    },
    {
        path: '/special-projects',
        name: 'v2-special-projects',
        component: SpecialProjects,
        meta: { layout: 'v2', requiresAuth: false },
    },
    {
        path: '/partner',
        name: 'v2-partner',
        component: Partner,
        meta: { layout: 'v2', requiresAuth: false, title: 'Become a Partner' },
    },
    {
        path: '/apply',
        name: 'v2-apply',
        component: Apply,
        meta: { layout: 'v2', requiresAuth: false, title: 'Apply' },
    },
    {
        path: '/privacy-policy',
        name: 'v2-privacy-policy',
        component: PrivacyPolicy,
        meta: { layout: 'v2', requiresAuth: false, title: 'Privacy Policy' },
    },
    {
        path: '/terms-and-conditions',
        name: 'v2-terms',
        component: TermsAndConditions,
        meta: { layout: 'v2', requiresAuth: false, title: 'Terms and Conditions' },
    },
    cs('/certified',  'v2-certified',  'Licensed & Certified'),
    cs('/payroll',    'v2-payroll',    'Payroll Solutions'),
    cs('/disclaimer', 'v2-disclaimer', 'Disclaimer'),
    cs('/sign-up',    'v2-sign-up',    'Sign Up'),
    cs('/sign-in',    'v2-sign-in',    'Sign In'),

    // Backward-compatibility: the v2 landing used to live under /v2/*. Redirect
    // those old paths to their canonical equivalents so existing links/bookmarks
    // keep working. Specific cases first, then a catch-all.
    { path: '/v2/home', redirect: '/' },
    { path: '/v2/terms', redirect: '/terms-and-conditions' },
    { path: '/v2/:rest(.*)', redirect: (to) => `/${to.params.rest}` },
];

export default routesV2;

import { RouteRecordRaw } from 'vue-router';

const Home             = () => import('@/pages/v2/landing/Home/Home.vue');
const AboutUs          = () => import('@/pages/v2/landing/About/AboutUs.vue');
const Industries       = () => import('@/pages/v2/landing/Industries/Industries.vue');
const Talents          = () => import('@/pages/v2/landing/Talents/Talents.vue');
const Employers        = () => import('@/pages/v2/landing/Employers/Employers.vue');
const News             = () => import('@/pages/v2/landing/News/News.vue');
const Partner          = () => import('@/pages/v2/landing/Partner/Partner.vue');
const SpecialProjects  = () => import('@/pages/v2/landing/SpecialProjects/SpecialProjects.vue');
const ComingSoon       = () => import('@/pages/v2/landing/ComingSoon.vue');

const cs = (path: string, name: string, title: string): RouteRecordRaw => ({
    path,
    name,
    component: ComingSoon,
    meta: { layout: 'v2', requiresAuth: false, title },
});

const routesV2: RouteRecordRaw[] = [
    {
        path: '/v2/home',
        name: 'v2-home',
        component: Home,
        meta: { layout: 'v2', requiresAuth: false },
    },
    cs('/v2/open-positions',   'v2-open-positions',   'Open Positions'),
    {
        path: '/v2/industries',
        name: 'v2-industries',
        component: Industries,
        meta: { layout: 'v2', requiresAuth: false, title: 'Industries' },
    },
    {
        path: '/v2/news',
        name: 'v2-news',
        component: News,
        meta: { layout: 'v2', requiresAuth: false, title: 'News' },
    },
    {
        path: '/v2/about',
        name: 'v2-about',
        component: AboutUs,
        meta: { layout: 'v2', requiresAuth: false, title: 'About Us' },
    },
    {
        path: '/v2/employers',
        name: 'v2-employers',
        component: Employers,
        meta: { layout: 'v2', requiresAuth: false, title: 'For Employers' },
    },
    {
        path: '/v2/talents',
        name: 'v2-talents',
        component: Talents,
        meta: { layout: 'v2', requiresAuth: false, title: 'For Talents' },
    },
    {
        path: '/v2/special-projects',
        name: 'v2-special-projects',
        component: SpecialProjects,
        meta: { layout: 'v2', requiresAuth: false },
    },
    {
        path: '/v2/partner',
        name: 'v2-partner',
        component: Partner,
        meta: { layout: 'v2', requiresAuth: false, title: 'Become a Partner' },
    },
    cs('/v2/certified',        'v2-certified',        'Licensed & Certified'),
    cs('/v2/payroll',          'v2-payroll',          'Payroll Solutions'),
    cs('/v2/privacy-policy',   'v2-privacy-policy',   'Privacy Policy'),
    cs('/v2/terms',            'v2-terms',            'Terms of Use'),
    cs('/v2/sign-up',          'v2-sign-up',          'Sign Up'),
    cs('/v2/sign-in',          'v2-sign-in',          'Sign In'),
];

export default routesV2;

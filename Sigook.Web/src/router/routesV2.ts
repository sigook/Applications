import { RouteRecordRaw } from 'vue-router';

const HomeV2             = () => import('@/pages/v2/HomeV2.vue');
const AboutUsV2          = () => import('@/pages/v2/AboutUsV2.vue');
const IndustriesV2       = () => import('@/pages/v2/IndustriesV2.vue');
const TalentsV2          = () => import('@/pages/v2/TalentsV2.vue');
const SpecialProjectsV2  = () => import('@/pages/v2/SpecialProjectsV2.vue');
const ComingSoonV2       = () => import('@/pages/v2/ComingSoonV2.vue');

const cs = (path: string, name: string, title: string): RouteRecordRaw => ({
    path,
    name,
    component: ComingSoonV2,
    meta: { layout: 'v2', requiresAuth: false, title },
});

const routesV2: RouteRecordRaw[] = [
    {
        path: '/v2/home',
        name: 'v2-home',
        component: HomeV2,
        meta: { layout: 'v2', requiresAuth: false },
    },
    cs('/v2/open-positions',   'v2-open-positions',   'Open Positions'),
    {
        path: '/v2/industries',
        name: 'v2-industries',
        component: IndustriesV2,
        meta: { layout: 'v2', requiresAuth: false, title: 'Industries' },
    },
    cs('/v2/news',             'v2-news',             'News'),
    {
        path: '/v2/about',
        name: 'v2-about',
        component: AboutUsV2,
        meta: { layout: 'v2', requiresAuth: false, title: 'About Us' },
    },
    cs('/v2/employers',        'v2-employers',        'For Employers'),
    {
        path: '/v2/talents',
        name: 'v2-talents',
        component: TalentsV2,
        meta: { layout: 'v2', requiresAuth: false, title: 'For Talents' },
    },
    {
        path: '/v2/special-projects',
        name: 'v2-special-projects',
        component: SpecialProjectsV2,
        meta: { layout: 'v2', requiresAuth: false },
    },
    cs('/v2/partner',          'v2-partner',          'Become a Partner'),
    cs('/v2/certified',        'v2-certified',        'Licensed & Certified'),
    cs('/v2/payroll',          'v2-payroll',          'Payroll Solutions'),
    cs('/v2/privacy-policy',   'v2-privacy-policy',   'Privacy Policy'),
    cs('/v2/terms',            'v2-terms',            'Terms of Use'),
    cs('/v2/sign-up',          'v2-sign-up',          'Sign Up'),
    cs('/v2/sign-in',          'v2-sign-in',          'Sign In'),
];

export default routesV2;

import { RouteRecordRaw } from 'vue-router';

const Home               = () => import('@/pages/landing/Home/Home.vue');
const AboutUs            = () => import('@/pages/landing/About/AboutUs.vue');
const Industries         = () => import('@/pages/landing/Industries/Industries.vue');
const Talents            = () => import('@/pages/landing/Talents/Talents.vue');
const Employers          = () => import('@/pages/landing/Employers/Employers.vue');
const OpenPositions      = () => import('@/pages/landing/OpenPositions/OpenPositions.vue');
const Apply              = () => import('@/pages/landing/Apply/Apply.vue');
const Partner            = () => import('@/pages/landing/Partner/Partner.vue');
const SpecialProjects    = () => import('@/pages/landing/SpecialProjects/SpecialProjects.vue');
const PrivacyPolicy      = () => import('@/pages/landing/Legal/PrivacyPolicy.vue');
const TermsAndConditions = () => import('@/pages/landing/Legal/TermsAndConditions.vue');
const Disclaimer         = () => import('@/pages/landing/Legal/Disclaimer.vue');
const ComingSoon         = () => import('@/pages/landing/ComingSoon.vue');

const cs = (path: string, name: string, title: string): RouteRecordRaw => ({
    path,
    name,
    component: ComingSoon,
    meta: { layout: 'landing', requiresAuth: false, title },
});

const routesLanding: RouteRecordRaw[] = [
    // ── Canonical public landing pages ──────────────────────────────────────
    {
        path: '/',
        name: 'home',
        component: Home,
        meta: { layout: 'landing', requiresAuth: false },
    },
    {
        path: '/open-positions',
        name: 'open-positions',
        component: OpenPositions,
        meta: { layout: 'landing', requiresAuth: false, title: 'Open Positions' },
    },
    {
        path: '/industries',
        name: 'industries',
        component: Industries,
        meta: { layout: 'landing', requiresAuth: false, title: 'Industries' },
    },
    {
        path: '/about',
        name: 'about',
        component: AboutUs,
        meta: { layout: 'landing', requiresAuth: false, title: 'About Us' },
    },
    {
        path: '/employers',
        name: 'employers',
        component: Employers,
        meta: { layout: 'landing', requiresAuth: false, title: 'For Employers' },
    },
    {
        path: '/talents',
        name: 'talents',
        component: Talents,
        meta: { layout: 'landing', requiresAuth: false, title: 'For Talents' },
    },
    {
        path: '/special-projects',
        name: 'special-projects',
        component: SpecialProjects,
        meta: { layout: 'landing', requiresAuth: false },
    },
    {
        path: '/partner',
        name: 'partner',
        component: Partner,
        meta: { layout: 'landing', requiresAuth: false, title: 'Become a Partner' },
    },
    {
        path: '/apply',
        name: 'apply',
        component: Apply,
        meta: { layout: 'landing', requiresAuth: false, title: 'Apply' },
    },
    {
        path: '/privacy-policy',
        name: 'privacy-policy',
        component: PrivacyPolicy,
        meta: { layout: 'landing', requiresAuth: false, title: 'Privacy Policy' },
    },
    {
        path: '/terms-and-conditions',
        name: 'terms-and-conditions',
        component: TermsAndConditions,
        meta: { layout: 'landing', requiresAuth: false, title: 'Terms and Conditions' },
    },
    {
        path: '/disclaimer',
        name: 'disclaimer',
        component: Disclaimer,
        meta: { layout: 'landing', requiresAuth: false, title: 'Disclaimer' },
    },
    cs('/certified',  'certified',  'Licensed & Certified'),
    cs('/payroll',    'payroll',    'Payroll Solutions'),
    cs('/sign-up',    'sign-up',    'Sign Up'),
    cs('/sign-in',    'sign-in',    'Sign In'),

    // ── Legacy slug redirects → canonical landing (preserve inbound links/SEO) ─
    // Discontinued pages (direct-hiring, contact, atas) redirect to the closest
    // page. The legal slugs are served directly by the canonical routes above.
    { path: '/home', redirect: '/' },
    { path: '/jobSeekers', redirect: '/open-positions' },
    { path: '/jobSeekers/:position', redirect: '/open-positions' },
    { path: '/business', redirect: '/employers' },
    { path: '/business/:position', redirect: '/employers' },
    { path: '/direct-hiring', redirect: '/employers' },
    { path: '/about-us', redirect: '/about' },
    { path: '/contact', redirect: '/' },
    { path: '/atas', redirect: '/' },
    { path: '/news', redirect: '/' },

    // ── Backward-compatibility: the landing previously lived under /v2/* ──────
    { path: '/v2/home', redirect: '/' },
    { path: '/v2/terms', redirect: '/terms-and-conditions' },
    { path: '/v2/:rest(.*)', redirect: (to) => `/${to.params.rest}` },
];

export default routesLanding;

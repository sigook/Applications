import { RouteRecordRaw } from 'vue-router';

const HomeV2 = () => import('@/pages/v2/HomeV2.vue');

const routesV2: RouteRecordRaw[] = [
    {
        path: '/v2/home',
        name: 'v2-home',
        component: HomeV2,
        meta: {
            layout: 'v2',
            requiresAuth: false,
        },
    },
];

export default routesV2;

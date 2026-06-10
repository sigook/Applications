import { RouteRecordRaw } from 'vue-router';

// Legacy landing routes now redirect to their v2 canonical equivalents.
// The page components under src/pages/landing/ remain on disk but are no
// longer routed as pages. Discontinued pages (direct-hiring, contact, atas)
// redirect to the closest v2 page. The legal slugs /privacy-policy and
// /terms-and-conditions are owned by routesV2 now.
const routesLanding: RouteRecordRaw[] = [
    { path: '/home', redirect: '/' },
    { path: '/jobSeekers', redirect: '/open-positions' },
    { path: '/jobSeekers/:position', redirect: '/open-positions' },
    { path: '/business', redirect: '/employers' },
    { path: '/business/:position', redirect: '/employers' },
    { path: '/direct-hiring', redirect: '/employers' },
    { path: '/about-us', redirect: '/about' },
    { path: '/contact', redirect: '/' },
    { path: '/atas', redirect: '/' },
];

export default routesLanding;

import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";
import NotFound from "@/pages/NotFound.vue";
import SilentRefresh from "@/pages/SilentRefresh.vue";
import Unauthorized from "@/pages/Unauthorized.vue";
import EmailPreferences from "@/pages/EmailPreferences.vue";
import Callback from "@/pages/Callback.vue";
import routesCompany from "@/router/routesCompany";
import routesAgency from "@/router/routesAgency";
import routesWorker from "@/router/routesWorker";
import routesLanding from "@/router/routesLanding";
import routesV2 from "@/router/routesV2";
import pinia from "@/stores";
import { useSecurityStore } from "@/stores/security";

const routes: RouteRecordRaw[] = [
  {
    path: "/callback",
    name: 'callback',
    component: Callback
  },
  {
    path: "/",
    redirect: () => 'home',
    meta: {
      layout: "web",
      requiresAuth: false,
    },
  },
  {
    path: "/silent-refresh",
    name: "silent-refresh",
    component: SilentRefresh,
  },
  {
    path: "/unauthorized",
    name: "unauthorized",
    component: Unauthorized,
  },
  {
    path: "/:pathMatch(.*)*",
    name: "not-found",
    component: NotFound,
  },
  {
    path: "/email-preferences",
    component: EmailPreferences,
    meta: {
      requiresAuth: false,
      layout: "web",
    },
  },
];
const router = createRouter({
  history: createWebHistory(),
  routes: routes
    .concat(routesAgency as RouteRecordRaw[])
    .concat(routesCompany as RouteRecordRaw[])
    .concat(routesWorker as RouteRecordRaw[])
    .concat(routesLanding as RouteRecordRaw[])
    .concat(routesV2 as RouteRecordRaw[]),
});
router.beforeEach(async (to, from, next) => {
  if (from.name !== 'jobSeekers') {
    setTimeout(() => {
      window.scrollTo(0, 0);
    }, 0);
  }
  if (to.meta?.requiresAuth) {
    const securityStore = useSecurityStore(pinia);
    const user = await securityStore.getUser();
    if (!user) {
      securityStore.signIn();
    } else {
      next();
    }
  } else {
    next();
  }
});

export default router;

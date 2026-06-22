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
import pinia from "@/stores";
import { useSecurityStore } from "@/stores/security";

const routes: RouteRecordRaw[] = [
  {
    path: "/callback",
    name: 'callback',
    component: Callback
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
  scrollBehavior(to) {
    if (to.hash) {
      return { el: to.hash, top: 90, behavior: 'smooth' };
    }
    return false;
  },
  routes: routes
    .concat(routesAgency as RouteRecordRaw[])
    .concat(routesCompany as RouteRecordRaw[])
    .concat(routesWorker as RouteRecordRaw[])
    .concat(routesLanding as RouteRecordRaw[]),
});
router.beforeEach(async (to, from, next) => {
  if (from.name !== 'open-positions' && !to.hash) {
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

// Per-route canonical + og:url. index.html ships a single static canonical
// pointing at the homepage; without this every SPA route would claim the
// homepage as its canonical and search engines would treat them as duplicates.
// The origin is the production host (not window.location.origin) so canonicals
// stay correct on staging/preview domains.
const CANONICAL_ORIGIN = "https://www.sigook.com";

function setCanonical(path: string): void {
  const url = `${CANONICAL_ORIGIN}${path}`;
  let link = document.querySelector<HTMLLinkElement>('link[rel="canonical"]');
  if (!link) {
    link = document.createElement("link");
    link.rel = "canonical";
    document.head.appendChild(link);
  }
  link.href = url;
  const ogUrl = document.querySelector<HTMLMetaElement>('meta[property="og:url"]');
  if (ogUrl) ogUrl.setAttribute("content", url);
}

router.afterEach((to) => {
  // Don't self-canonicalize unknown URLs (the SPA soft-serves 404s with HTTP 200).
  if (to.name === "not-found") return;
  setCanonical(to.path);
});

export default router;

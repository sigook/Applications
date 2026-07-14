import { createRouter, createWebHistory, RouteRecordRaw, RouteLocationNormalized } from "vue-router";
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
      next(false);
      securityStore.signIn();
    } else {
      next();
    }
  } else {
    next();
  }
});

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

const SITE_NAME = "Sigook";
const DEFAULT_TITLE = "Sigook® - Connecting Talent with Opportunity";
const DEFAULT_DESCRIPTION =
  "Sigook connects job seekers with top employers across the United States. Find temporary, contract, and permanent jobs in skilled trades, industrial, and professional sectors.";

function setMetaContent(selector: string, value: string): void {
  const el = document.querySelector<HTMLMetaElement>(selector);
  if (el) el.setAttribute("content", value);
}

function setPageMeta(to: RouteLocationNormalized): void {
  const isLanding = to.meta?.layout === "landing";
  const pageTitle = to.meta?.title as string | undefined;
  const pageDescription = to.meta?.description as string | undefined;
  const title = isLanding && pageTitle ? `${pageTitle} | ${SITE_NAME}` : DEFAULT_TITLE;
  const description = (isLanding && pageDescription) || DEFAULT_DESCRIPTION;

  document.title = title;
  setMetaContent('meta[name="description"]', description);
  setMetaContent('meta[property="og:title"]', title);
  setMetaContent('meta[property="og:description"]', description);
  setMetaContent('meta[name="twitter:title"]', title);
  setMetaContent('meta[name="twitter:description"]', description);
}

router.afterEach((to) => {
  setPageMeta(to);
  if (to.name === "not-found") return;
  setCanonical(to.path);
});

export default router;

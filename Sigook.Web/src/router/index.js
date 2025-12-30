import Vue from "vue";
import Router from "vue-router";
import NotFound from "@/pages/NotFound.vue";
import SilentRefresh from "@/pages/SilentRefresh";
import Unauthorized from "@/pages/Unauthorized";
import EmailPreferences from "@/pages/EmailPreferences";
import Callback from "@/pages/Callback";
import routesCompany from "@/router/routesCompany";
import routesAgency from "@/router/routesAgency";
import routesWorker from "@/router/routesWorker";
import routesLanding from "@/router/routesLanding";
import store from "../store";

Vue.use(Router);
const routes = [
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
    path: "*",
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
]
const router = new Router({
  mode: "history",
  routes: routes
    .concat(routesAgency)
    .concat(routesCompany)
    .concat(routesWorker)
    .concat(routesLanding),
});
router.beforeEach(async (to, from, next) => {
  if (from.name !== 'jobSeekers') {
    setTimeout(() => {
      window.scrollTo(0, 0);
    }, 0);
  }
  if (to.meta.requiresAuth) {
    const user = await store.dispatch("getUser");
    if (!user) {
      await store.dispatch('signIn');
    } else {
      next();
    }
  } else {
    next();
  }
});

export default router;

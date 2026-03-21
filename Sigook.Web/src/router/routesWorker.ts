import { RouteConfig } from 'vue-router';
import roles from "@/security/roles";

// Lazy loading para rutas de trabajador
const WorkerRegister = () => import("@/pages/worker/Register.vue");
const WorkerRequests = () => import("@/pages/worker/Requests.vue");
const WorkerRequest = () => import("@/pages/worker/Request.vue");
const WorkerRequestApplied = () => import("@/pages/worker/RequestApplied.vue");
const PunchCard = () => import("@/pages/worker/PunchCard.vue");
const WorkerTimeSheet = () => import("@/pages/worker/TimeSheet.vue");
const WorkerHistory = () => import("@/pages/worker/History.vue");
const WorkerProfile = () => import("@/pages/worker/WorkerProfile.vue");
const WorkerApply = () => import("@/pages/worker/WorkerApply.vue");

const worker = roles.worker;

const routesWorker: RouteConfig[] = [
  {
    path: "/register-worker",
    name: "register-worker",
    component: WorkerRegister,
    meta: {
      layout: "web",
    },
  },
  {
    path: "/register-worker/:orderId",
    name: "register-worker-with-orderId",
    component: WorkerRegister,
    meta: {
      layout: "web",
    },
  },
  {
    path: "/worker-requests",
    component: WorkerRequests,
    meta: {
      requiresAuth: true,
      role: [worker],
    },
  },
  {
    path: "/worker-request/:id",
    component: WorkerRequest,
    name: "worker-request",
    meta: {
      requiresAuth: true,
      role: [worker],
    },
  },
  {
    path: "/worker-request-applied/:id",
    component: WorkerRequestApplied,
    name: "worker-applied",
    meta: {
      requiresAuth: true,
      role: [worker],
    },
  },
  {
    path: "/punch-card",
    component: PunchCard,
    meta: {
      requiresAuth: true,
      role: [worker],
    },
  },
  {
    path: "/timesheet",
    component: WorkerTimeSheet,
    meta: {
      requiresAuth: true,
      role: [worker],
    },
  },
  {
    path: "/worker-history",
    component: WorkerHistory,
    meta: {
      requiresAuth: true,
      role: [worker],
    },
  },
  {
    path: "/worker-profile",
    component: WorkerProfile,
    meta: {
      requiresAuth: true,
      role: [worker],
    },
  },
  {
    path: "/worker-apply",
    component: WorkerApply,
    meta: {
      requiresAuth: false,
      role: [],
      layout: "web",
    },
  },
];

export default routesWorker;

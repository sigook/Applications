import { RouteRecordRaw } from 'vue-router';
import roles from "@/app/security/roles";

const WorkerRegister = () => import("@/modules/worker/register/pages/Register.vue");
const WorkerRequests = () => import("@/modules/worker/requests/pages/Requests.vue");
const WorkerRequest = () => import("@/modules/worker/requests/pages/Request.vue");
const WorkerRequestApplied = () => import("@/modules/worker/requests/pages/RequestApplied.vue");
const WorkerTimeSheet = () => import("@/modules/worker/requests/pages/TimeSheet.vue");
const WorkerHistory = () => import("@/modules/worker/history/pages/History.vue");
const WorkerProfile = () => import("@/modules/worker/profile/pages/WorkerProfile.vue");
const WorkerApply = () => import("@/modules/worker/requests/pages/Apply.vue");

const worker = roles.worker;

const routesWorker: RouteRecordRaw[] = [
  {
    path: "/register-worker",
    name: "register-worker",
    component: WorkerRegister,
    meta: {
      layout: "landing-anonymous",
      resolveUser: true,
    },
  },
  {
    path: "/register-worker/:requestId",
    name: "register-worker-with-requestId",
    component: WorkerRegister,
    meta: {
      layout: "landing-anonymous",
      resolveUser: true,
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
    path: "/worker-requests/applied/:id",
    component: WorkerRequestApplied,
    name: "worker-applied",
    meta: {
      requiresAuth: true,
      role: [worker],
    },
  },
  {
    path: "/worker-requests/:id",
    component: WorkerRequest,
    name: "worker-request",
    meta: {
      requiresAuth: true,
      role: [worker],
    },
  },
  { path: "/worker-request/:id", redirect: (to) => `/worker-requests/${to.params.id}` },
  { path: "/worker-request-applied/:id", redirect: (to) => `/worker-requests/applied/${to.params.id}` },
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
    name: "worker-apply",
    component: WorkerApply,
    meta: {
      requiresAuth: false,
      role: [],
      layout: "landing",
      title: "Apply",
    },
  },
];

export default routesWorker;

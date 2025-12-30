// Lazy loading para rutas de trabajador - chunks se cargan solo cuando se necesitan
const WorkerRegister = () => import("@/pages/worker/Register");
const WorkerRequests = () => import("@/pages/worker/Requests");
const WorkerRequest = () => import("@/pages/worker/Request");
const WorkerRequestApplied = () => import("@/pages/worker/RequestApplied");
const PunchCard = () => import("@/pages/worker/PunchCard");
const WorkerTimeSheet = () => import("@/pages/worker/TimeSheet");
const WorkerHistory = () => import("@/pages/worker/History");
const WorkerProfile = () => import("@/pages/worker/WorkerProfile");
const WorkerApply = () => import("@/pages/worker/WorkerApply");

import roles from "@/security/roles";

let worker = roles.worker;

let routesWorker = [
  // worker
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

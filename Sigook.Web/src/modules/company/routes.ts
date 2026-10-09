import { RouteRecordRaw } from 'vue-router';
import roles from "@/app/security/roles";

// Lazy loading para rutas de empresa
const CompanyRequests = () => import("@/modules/company/requests/pages/Requests.vue");
const CompanyRequest = () => import("@/modules/company/requests/pages/Request.vue");
const CreateRequest = () => import("@/modules/company/requests/pages/CreateRequest.vue");
const CompanyReports = () => import("@/modules/company/invoices/pages/Invoices.vue");
const CompanyProfile = () => import("@/modules/company/profile/pages/CompanyProfile.vue");
const CompanyUserProfile = () => import("@/modules/company/profile/pages/CompanyUserProfile.vue");

const companyUser = roles.companyUser;
const company = roles.company;

const routesCompany: RouteRecordRaw[] = [
  {
    path: "/company-requests",
    component: CompanyRequests,
    meta: {
      requiresAuth: true,
      role: [company, companyUser],
    },
  },
  {
    path: "/company-requests/create",
    component: CreateRequest,
    meta: {
      requiresAuth: true,
      role: [company, companyUser],
    },
  },
  {
    path: "/company-requests/:id",
    component: CompanyRequest,
    meta: {
      requiresAuth: true,
      role: [company, companyUser],
    },
  },
  { path: "/request/:id", redirect: (to) => `/company-requests/${to.params.id}` },
  { path: "/create-request", redirect: "/company-requests/create" },
  {
    path: "/company-invoices",
    component: CompanyReports,
    meta: {
      requiresAuth: true,
      role: [company, companyUser],
    },
  },
  {
    path: "/company-profile",
    component: CompanyProfile,
    meta: {
      requiresAuth: true,
      role: [company, companyUser],
    },
  },
  {
    path: "/company-user-profile",
    component: CompanyUserProfile,
    meta: {
      requiresAuth: true,
      role: [company, companyUser],
    },
  },
];

export default routesCompany;

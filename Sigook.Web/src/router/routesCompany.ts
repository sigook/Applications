import { RouteRecordRaw } from 'vue-router';
import roles from "@/security/roles";

// Lazy loading para rutas de empresa
const CompanyRequests = () => import("../pages/company/Requests.vue");
const CompanyRequest = () => import("../pages/company/Request.vue");
const CreateRequest = () => import("../pages/company/CreateRequest.vue");
const CompanyReports = () => import("../pages/company/CompanyReports.vue");
const CompanyProfile = () => import("../pages/company/CompanyProfile.vue");
const CompanyUserProfile = () => import("@/pages/company/CompanyUserProfile.vue");

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
    path: "/request/:id",
    component: CompanyRequest,
    meta: {
      requiresAuth: true,
      role: [company, companyUser],
    },
  },
  {
    path: "/create-request",
    component: CreateRequest,
    meta: {
      requiresAuth: true,
      role: [company, companyUser],
    },
  },
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

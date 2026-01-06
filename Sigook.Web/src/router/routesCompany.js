// Lazy loading para rutas de empresa - chunks se cargan solo cuando se necesitan
const CompanyRequests = () => import("../pages/company/Requests");
const CompanyRequest = () => import("../pages/company/Request");
const CreateRequest = () => import("../pages/company/CreateRequest");
const CompanyReports = () => import("../pages/company/CompanyReports");
const CompanyProfile = () => import("../pages/company/CompanyProfile");
const CompanyUserProfile = () => import("@/pages/company/CompanyUserProfile");

import roles from "@/security/roles";

let companyUser = roles.companyUser;
let company = roles.company;

let routesCompany = [
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

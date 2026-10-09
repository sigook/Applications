import { RouteRecordRaw } from 'vue-router';
import { recruitingAccess, agencyStaff, salesAccess, adminAccess } from "@/app/security/roles";
import {
  loadAgencyCompaniesResolver,
  loadCompanyToUpdateResolver,
  loadAgencyRequestFormResolver
} from "@/modules/agency/resolvers";

const AgencyRequests = () => import("@/modules/agency/recruiting/requests/pages/Requests.vue");
const AgencyWeeklyBoard = () => import("@/modules/agency/recruiting/weekly-board/pages/WeeklyBoard.vue");
const AgencyApplicants = () => import("@/modules/agency/recruiting/applicants/pages/Applicants.vue");
const AgencyRequest = () => import("@/modules/agency/recruiting/requests/pages/Request.vue");
const AgencyCreateRequest = () => import("@/modules/agency/recruiting/requests/pages/CreateRequest.vue");
const AgencyWorkers = () => import("@/modules/agency/recruiting/workers/pages/Workers.vue");
const AgencyDetailWorker = () => import("@/modules/agency/recruiting/workers/pages/WorkerDetail.vue");
const AgencyCompanies = () => import("@/modules/agency/recruiting/clients/pages/Clients.vue");
const CreateCompany = () => import("@/modules/agency/recruiting/clients/pages/CreateClient.vue");
const AgencyDetailCompany = () => import("@/modules/agency/recruiting/clients/pages/ClientDetail.vue");
const AgencyProfile = () => import("@/modules/agency/profile/pages/AgencyProfile.vue");
const AgencyCandidates = () => import("@/modules/agency/recruiting/candidates/pages/Candidates.vue");
const SalesDashboard = () => import("@/modules/agency/sales/dashboard/pages/Dashboard.vue");
const AgencyAgencies = () => import("@/modules/agency/sales/agencies/pages/Agencies.vue");
const CreateAgency = () => import("@/modules/agency/sales/agencies/pages/CreateAgency.vue");
const DetailAgency = () => import("@/modules/agency/sales/agencies/pages/AgencyDetail.vue");
const AgencyInvoices = () => import("@/modules/agency/accounting/invoices/pages/Invoices.vue");
const CreateInvoice = () => import("@/modules/agency/accounting/invoices/pages/CreateInvoice.vue");
const AgencyPayStubs = () => import("@/modules/agency/accounting/paystubs/pages/PayStubs.vue");
const CreatePayStub = () => import("@/modules/agency/accounting/paystubs/pages/CreatePayStub.vue");
const WorkerRegister = () => import("@/modules/worker/register/pages/Register.vue");
const Reports = () => import("@/modules/agency/accounting/reports/pages/Reports.vue");

const routesAgency: RouteRecordRaw[] = [
  {
    path: "/recruiting/requests",
    component: AgencyRequests,
    name: "agency-requests",
    meta: {
      requiresAuth: true,
      role: recruitingAccess,
    },
  },
  { path: "/agency-requests", redirect: "/recruiting/requests" },
  {
    path: "/recruiting/weekly-board",
    component: AgencyWeeklyBoard,
    name: "agency-weekly-board",
    meta: {
      requiresAuth: true,
      role: recruitingAccess,
    },
  },
  {
    path: "/recruiting/applicants",
    component: AgencyApplicants,
    name: "agency-applicants",
    meta: {
      requiresAuth: true,
      role: recruitingAccess,
    },
  },
  {
    path: "/recruiting/requests/create/:companyProfileId",
    component: AgencyCreateRequest,
    name: "agency-create-request",
    meta: {
      requiresAuth: true,
      role: recruitingAccess,
    },
    beforeEnter: loadAgencyRequestFormResolver
  },
  {
    path: "/recruiting/requests/update/:companyProfileId/:requestId",
    component: AgencyCreateRequest,
    name: "agency-update-request",
    meta: {
      requiresAuth: true,
      role: recruitingAccess,
    },
    beforeEnter: loadAgencyRequestFormResolver
  },
  {
    path: "/recruiting/requests/duplicate/:companyProfileId/:requestId",
    component: AgencyCreateRequest,
    name: "agency-duplicate-request",
    meta: {
      requiresAuth: true,
      role: recruitingAccess,
      isDuplicate: true,
    },
    beforeEnter: loadAgencyRequestFormResolver
  },
  {
    path: "/recruiting/requests/:id",
    component: AgencyRequest,
    name: "agency-request",
    meta: {
      requiresAuth: true,
      role: recruitingAccess,
    },
  },
  { path: "/agency-request/:id", redirect: (to) => `/recruiting/requests/${to.params.id}` },
  {
    path: "/sales/requests",
    component: AgencyRequests,
    name: "sales-requests",
    meta: {
      requiresAuth: true,
      role: salesAccess,
    },
  },
  {
    path: "/sales/requests/create/:companyProfileId",
    component: AgencyCreateRequest,
    name: "sales-create-request",
    meta: {
      requiresAuth: true,
      role: salesAccess,
    },
    beforeEnter: loadAgencyRequestFormResolver
  },
  {
    path: "/sales/requests/update/:companyProfileId/:requestId",
    component: AgencyCreateRequest,
    name: "sales-update-request",
    meta: {
      requiresAuth: true,
      role: salesAccess,
    },
    beforeEnter: loadAgencyRequestFormResolver
  },
  {
    path: "/sales/requests/duplicate/:companyProfileId/:requestId",
    component: AgencyCreateRequest,
    name: "sales-duplicate-request",
    meta: {
      requiresAuth: true,
      role: salesAccess,
      isDuplicate: true,
    },
    beforeEnter: loadAgencyRequestFormResolver
  },
  {
    path: "/sales/requests/:id",
    component: AgencyRequest,
    name: "sales-request",
    meta: {
      requiresAuth: true,
      role: salesAccess,
    },
  },
  {
    path: "/recruiting/workers",
    component: AgencyWorkers,
    name: "workers",
    meta: {
      requiresAuth: true,
      role: recruitingAccess,
    },
  },
  { path: "/agency-workers", redirect: "/recruiting/workers" },
  {
    path: "/recruiting/workers/register",
    name: "agency-register-worker",
    component: WorkerRegister,
    meta: {
      requiresAuth: true,
      role: recruitingAccess,
    },
  },
  {
    path: "/recruiting/workers/:id",
    component: AgencyDetailWorker,
    name: "workerDetail",
    meta: {
      requiresAuth: true,
      role: agencyStaff,
    },
  },
  { path: "/agency-workers/worker/:id", redirect: (to) => `/recruiting/workers/${to.params.id}` },
  {
    path: "/recruiting/companies",
    name: "recruiting-companies",
    component: AgencyCompanies,
    meta: {
      requiresAuth: true,
      role: recruitingAccess,
    },
    beforeEnter: loadAgencyCompaniesResolver
  },
  {
    path: "/sales/dashboard",
    name: "sales-dashboard",
    component: SalesDashboard,
    meta: {
      requiresAuth: true,
      role: salesAccess,
    },
  },
  {
    path: "/sales/companies",
    name: "sales-companies",
    component: AgencyCompanies,
    meta: {
      requiresAuth: true,
      role: salesAccess,
    },
    beforeEnter: loadAgencyCompaniesResolver
  },
  { path: "/agency-companies", redirect: "/recruiting/companies" },
  {
    path: "/recruiting/companies/create",
    component: CreateCompany,
    meta: {
      requiresAuth: true,
      role: recruitingAccess,
    },
  },
  {
    path: "/recruiting/companies/update/:companyProfileId",
    component: CreateCompany,
    meta: {
      requiresAuth: true,
      role: recruitingAccess,
    },
    beforeEnter: loadCompanyToUpdateResolver
  },
  {
    path: "/recruiting/companies/:id",
    component: AgencyDetailCompany,
    meta: {
      requiresAuth: true,
      role: recruitingAccess,
    },
  },
  {
    path: "/sales/companies/create",
    component: CreateCompany,
    meta: {
      requiresAuth: true,
      role: salesAccess,
    },
  },
  {
    path: "/sales/companies/update/:companyProfileId",
    component: CreateCompany,
    meta: {
      requiresAuth: true,
      role: salesAccess,
    },
    beforeEnter: loadCompanyToUpdateResolver
  },
  {
    path: "/sales/companies/:id",
    component: AgencyDetailCompany,
    meta: {
      requiresAuth: true,
      role: salesAccess,
    },
  },
  { path: "/agency-companies/company/:id", redirect: (to) => `/recruiting/companies/${to.params.id}` },
  { path: "/create-company", redirect: "/recruiting/companies/create" },
  {
    path: "/agency-profile",
    component: AgencyProfile,
    meta: {
      requiresAuth: true,
      role: recruitingAccess,
    },
  },
  {
    path: "/recruiting/candidates",
    component: AgencyCandidates,
    meta: {
      requiresAuth: true,
      role: recruitingAccess,
    },
  },
  { path: "/agency-candidates", redirect: "/recruiting/candidates" },
  {
    path: "/sales/agencies",
    component: AgencyAgencies,
    meta: {
      requiresAuth: true,
      role: agencyStaff,
    },
  },
  { path: "/agency-agencies", redirect: "/sales/agencies" },
  {
    path: "/sales/agencies/create",
    component: CreateAgency,
    meta: {
      requiresAuth: true,
      role: agencyStaff,
    },
  },
  {
    path: "/sales/agencies/:id",
    component: DetailAgency,
    meta: {
      requiresAuth: true,
      role: agencyStaff,
    },
  },
  { path: "/agency-detail/:id", redirect: (to) => `/sales/agencies/${to.params.id}` },
  { path: "/create-agency", redirect: "/sales/agencies/create" },
  {
    path: "/accounting/invoices",
    component: AgencyInvoices,
    name: "agency-invoices",
    meta: {
      requiresAuth: true,
      role: adminAccess,
    },
  },
  {
    path: "/accounting/invoices/create",
    component: CreateInvoice,
    name: "create-invoice",
    meta: {
      requiresAuth: true,
      role: adminAccess,
    },
  },
  {
    path: "/accounting/paystubs",
    component: AgencyPayStubs,
    name: "agency-paystubs",
    meta: {
      requiresAuth: true,
      role: adminAccess,
    },
  },
  {
    path: "/accounting/paystubs/create",
    component: CreatePayStub,
    name: "create-paystub",
    meta: {
      requiresAuth: true,
      role: adminAccess,
    },
  },
  {
    path: "/accounting/reports",
    component: Reports,
    name: "reports",
    meta: {
      requiresAuth: true,
      role: adminAccess
    },
  }
];

export default routesAgency;

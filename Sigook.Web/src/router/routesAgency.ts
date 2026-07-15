import { RouteRecordRaw } from 'vue-router';
import { roleGroups } from "@/security/roles";
import {
  loadAgencyCompaniesResolver,
  loadCompanyToUpdateResolver,
  loadAgencyRequestToUpdateResolver
} from "@/resolvers/agencyResolvers";

const AgencyRequests = () => import("@/pages/agency/Requests.vue");
const AgencyWeeklyBoard = () => import("@/pages/agency/WeeklyBoard.vue");
const AgencyAttendanceReview = () => import("@/pages/agency/AttendanceReview.vue");
const AgencyRequest = () => import("@/pages/agency/Request.vue");
const AgencyCreateRequest = () => import("@/pages/agency/AgencyCreateRequest.vue");
const AgencyWorkers = () => import("@/pages/agency/Workers.vue");
const AgencyDetailWorker = () => import("@/pages/agency/DetailWorker.vue");
const AgencyCompanies = () => import("@/pages/agency/Companies.vue");
const CreateCompany = () => import("@/pages/agency/CreateCompany.vue");
const AgencyDetailCompany = () => import("@/pages/agency/DetailCompany.vue");
const AgencyProfile = () => import("@/pages/agency/AgencyProfile.vue");
const AgencyCandidates = () => import("@/pages/agency/Candidates.vue");
const SalesDashboard = () => import("@/pages/agency/Dashboard.vue");
const AgencyAgencies = () => import("@/pages/agency/Agencies.vue");
const CreateAgency = () => import("@/pages/agency/CreateAgency.vue");
const DetailAgency = () => import("@/pages/agency/DetailAgency.vue");
const AgencyInvoices = () => import("@/pages/agency/accounting/Invoices.vue");
const CreateInvoice = () => import("@/pages/agency/accounting/CreateInvoice.vue");
const AgencyPayStubs = () => import("@/pages/agency/accounting/PayStubs.vue");
const CreatePayStub = () => import("@/pages/agency/accounting/CreatePayStub.vue");
const WorkerRegister = () => import("@/pages/worker/Register.vue");
const Reports = () => import("@/pages/agency/accounting/Reports.vue");

const agencyAccess = [...roleGroups.agencyAccess];
const salesAccess = [...roleGroups.salesAccess];
const accounting = [...roleGroups.accounting];
const agencyStaff = [...roleGroups.agencyStaff];

const routesAgency: RouteRecordRaw[] = [
  {
    path: "/recruiting/requests",
    component: AgencyRequests,
    name: "agency-requests",
    meta: {
      requiresAuth: true,
      role: agencyAccess,
    },
  },
  { path: "/agency-requests", redirect: "/recruiting/requests" },
  {
    path: "/recruiting/weekly-board",
    component: AgencyWeeklyBoard,
    name: "agency-weekly-board",
    meta: {
      requiresAuth: true,
      role: agencyAccess,
    },
  },
  {
    path: "/recruiting/attendance-review",
    component: AgencyAttendanceReview,
    name: "agency-attendance-review",
    meta: {
      requiresAuth: true,
      role: agencyAccess,
    },
  },
  {
    path: "/recruiting/requests/create/:companyProfileId",
    component: AgencyCreateRequest,
    name: "agency-create-request",
    meta: {
      requiresAuth: true,
      role: agencyAccess,
    },
  },
  {
    path: "/recruiting/requests/update/:companyProfileId/:requestId",
    component: AgencyCreateRequest,
    name: "agency-update-request",
    meta: {
      requiresAuth: true,
      role: agencyAccess,
    },
    beforeEnter: loadAgencyRequestToUpdateResolver
  },
  {
    path: "/recruiting/requests/:id",
    component: AgencyRequest,
    name: "agency-request",
    meta: {
      requiresAuth: true,
      role: agencyAccess,
    },
  },
  { path: "/agency-request/:id", redirect: (to) => `/recruiting/requests/${to.params.id}` },
  {
    path: "/recruiting/workers",
    component: AgencyWorkers,
    name: "workers",
    meta: {
      requiresAuth: true,
      role: agencyAccess,
    },
  },
  { path: "/agency-workers", redirect: "/recruiting/workers" },
  {
    path: "/recruiting/workers/register",
    name: "agency-register-worker",
    component: WorkerRegister,
    meta: {
      requiresAuth: true,
      role: agencyAccess,
    },
  },
  {
    path: "/recruiting/workers/:id",
    component: AgencyDetailWorker,
    name: "workerDetail",
    meta: {
      requiresAuth: true,
      role: agencyAccess,
    },
  },
  { path: "/agency-workers/worker/:id", redirect: (to) => `/recruiting/workers/${to.params.id}` },
  {
    path: "/recruiting/companies",
    name: "recruiting-companies",
    component: AgencyCompanies,
    meta: {
      requiresAuth: true,
      role: agencyAccess,
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
      role: agencyAccess,
    },
  },
  {
    path: "/recruiting/companies/update/:companyProfileId",
    component: CreateCompany,
    meta: {
      requiresAuth: true,
      role: agencyAccess,
    },
    beforeEnter: loadCompanyToUpdateResolver
  },
  {
    path: "/recruiting/companies/:id",
    component: AgencyDetailCompany,
    meta: {
      requiresAuth: true,
      role: agencyAccess,
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
      role: agencyStaff,
    },
  },
  {
    path: "/recruiting/candidates",
    component: AgencyCandidates,
    meta: {
      requiresAuth: true,
      role: agencyAccess,
    },
  },
  { path: "/agency-candidates", redirect: "/recruiting/candidates" },
  {
    path: "/sales/agencies",
    component: AgencyAgencies,
    meta: {
      requiresAuth: true,
      role: salesAccess,
    },
  },
  { path: "/agency-agencies", redirect: "/sales/agencies" },
  {
    path: "/sales/agencies/create",
    component: CreateAgency,
    meta: {
      requiresAuth: true,
      role: salesAccess,
    },
  },
  {
    path: "/sales/agencies/:id",
    component: DetailAgency,
    meta: {
      requiresAuth: true,
      role: salesAccess,
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
      role: accounting,
    },
  },
  {
    path: "/accounting/invoices/create",
    component: CreateInvoice,
    name: "create-invoice",
    meta: {
      requiresAuth: true,
      role: accounting,
    },
  },
  {
    path: "/accounting/paystubs",
    component: AgencyPayStubs,
    name: "agency-paystubs",
    meta: {
      requiresAuth: true,
      role: accounting,
    },
  },
  {
    path: "/accounting/paystubs/create",
    component: CreatePayStub,
    name: "create-paystub",
    meta: {
      requiresAuth: true,
      role: accounting,
    },
  },
  {
    path: "/accounting/reports",
    component: Reports,
    name: "reports",
    meta: {
      requiresAuth: true,
      role: accounting
    },
  }
];

export default routesAgency;

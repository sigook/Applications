const AgencyRequests = () => import("@/pages/agency/Requests");
const WorkersBoard = () => import("@/pages/agency/WeeklyBoard");
const AgencyRequest = () => import("@/pages/agency/Request");
const AgencyCreateRequest = () => import("@/pages/agency/AgencyCreateRequest");
const AgencyWorkers = () => import("@/pages/agency/Workers");
const AgencyDetailWorker = () => import("@/pages/agency/DetailWorker");
const AgencyCompanies = () => import("@/pages/agency/Companies");
const CreateCompany = () => import("@/pages/agency/CreateCompany");
const AgencyDetailCompany = () => import("@/pages/agency/DetailCompany");
const AgencyProfile = () => import("@/pages/agency/AgencyProfile");
const AgencyCandidates = () => import("@/pages/agency/Candidates");
const AgencyAgencies = () => import("@/pages/agency/Agencies");
const CreateAgency = () => import("@/pages/agency/CreateAgency");
const DetailAgency = () => import("@/pages/agency/DetailAgency");
const AgencyInvoices = () => import("@/pages/agency/accounting/Invoices");
const CreateInvoice = () => import("@/pages/agency/accounting/CreateInvoice");
const AgencyPayStubs = () => import("@/pages/agency/accounting/PayStubs");
const CreatePayStub = () => import("@/pages/agency/accounting/CreatePayStub");
const WorkerRegister = () => import("@/pages/worker/Register");
const Reports = () => import("@/pages/agency/accounting/Reports");

import roles from "@/security/roles";
import {
  loadAgencyCompaniesResolver,
  loadCompanyToUpdateResolver,
  loadAgencyRequestToUpdateResolver
} from "@/resolvers/agencyResolvers";

const agency = roles.agency;
const agencyPersonnel = roles.agencyPersonnel;
const payroll = roles.payroll;
const admin = roles.admin;

let routesAgency = [
  {
    path: "/agency-requests",
    component: AgencyRequests,
    name: "agency-requests",
    meta: {
      requiresAuth: true,
      role: [agency, agencyPersonnel],
    },
  },
  {
    path: "/board",
    component: WorkersBoard,
    name: "agency-board",
    meta: {
      requiresAuth: true,
      role: [agency, agencyPersonnel],
    },
  },
  {
    path: "/agency-request/:id",
    component: AgencyRequest,
    name: "agency-request",
    meta: {
      requiresAuth: true,
      role: [agency, agencyPersonnel],
    },
  },
  {
    path: "/agency-create-request/:companyProfileId",
    component: AgencyCreateRequest,
    name: "agency-create-request",
    meta: {
      requiresAuth: true,
      role: [agency, agencyPersonnel],
    },
  },
  {
    path: "/agency-update-request/:companyProfileId/:requestId",
    component: AgencyCreateRequest,
    name: "agency-update-request",
    meta: {
      requiresAuth: true,
      role: [agency, agencyPersonnel],
    },
    beforeEnter: loadAgencyRequestToUpdateResolver
  },
  {
    path: "/agency-workers",
    component: AgencyWorkers,
    name: "workers",
    meta: {
      requiresAuth: true,
      role: [agency, agencyPersonnel],
    },
  },
  {
    path: "/agency-workers/worker/:id",
    component: AgencyDetailWorker,
    name: "workerDetail",
    meta: {
      requiresAuth: true,
      role: [agency, agencyPersonnel],
    },
  },
  {
    path: "/agency-workers/register-worker",
    name: "agency-register-worker",
    component: WorkerRegister,
    meta: {
      requiresAuth: true,
      role: [agency, agencyPersonnel],
    },
  },
  {
    path: "/agency-companies",
    component: AgencyCompanies,
    meta: {
      requiresAuth: true,
      role: [agency, agencyPersonnel],
    },
    beforeEnter: loadAgencyCompaniesResolver
  },
  {
    path: "/create-company",
    component: CreateCompany,
    meta: {
      requiresAuth: true,
      role: [agency, agencyPersonnel],
    },
  },
  {
    path: "/update-company/:companyProfileId",
    component: CreateCompany,
    meta: {
      requiresAuth: true,
      role: [agency, agencyPersonnel],
    },
    beforeEnter: loadCompanyToUpdateResolver
  },
  {
    path: "/agency-companies/company/:id",
    component: AgencyDetailCompany,
    meta: {
      requiresAuth: true,
      role: [agency, agencyPersonnel],
    },
  },
  {
    path: "/agency-profile",
    component: AgencyProfile,
    meta: {
      requiresAuth: true,
      role: [agency, agencyPersonnel],
    },
  },
  {
    path: "/agency-candidates",
    component: AgencyCandidates,
    meta: {
      requiresAuth: true,
      role: [agency, agencyPersonnel],
    },
  },
  {
    path: "/agency-agencies",
    component: AgencyAgencies,
    meta: {
      requiresAuth: true,
      role: [agency, agencyPersonnel],
    },
  },
  {
    path: "/create-agency",
    component: CreateAgency,
    meta: {
      requiresAuth: true,
      role: [agency, agencyPersonnel],
    },
  },
  {
    path: "/agency-detail/:id",
    component: DetailAgency,
    meta: {
      requiresAuth: true,
      role: [agency, agencyPersonnel],
    },
  },
  {
    path: "/accounting/invoices",
    component: AgencyInvoices,
    name: "agency-invoices",
    meta: {
      requiresAuth: true,
      role: [agency, agencyPersonnel],
    },
  },
  {
    path: "/accounting/create-invoice",
    component: CreateInvoice,
    name: "create-invoice",
    meta: {
      requiresAuth: true,
      role: [agency, agencyPersonnel, payroll, admin],
    },
  },
  {
    path: "/accounting/paystubs",
    component: AgencyPayStubs,
    name: "agency-paystubs",
    meta: {
      requiresAuth: true,
      role: [agency, agencyPersonnel, payroll, admin],
    },
  },
  {
    path: "/accounting/create-paystub",
    component: CreatePayStub,
    name: "create-paystub",
    meta: {
      requiresAuth: true,
      role: [agency, agencyPersonnel, payroll, admin],
    },
  },
  {
    path: "/accounting/reports",
    component: Reports,
    name: "reports",
    meta: {
      requiresAuth: true,
      role: [agency, agencyPersonnel, payroll, admin]
    },
  }
];

export default routesAgency;

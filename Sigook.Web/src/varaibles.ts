import type { App } from 'vue';
import type { AgencyTypeOption } from '@/types/agency';

export const appGlobals = {
  // Request Status — string values returned by worker-facing endpoints
  // (WorkerRequestListModel / WorkerRequestDetailModel).
  // Agency/company pages use the numeric enum from src/constants/enums.ts.
  $statusOpen: "Open",
  $statusFilled: "Filled",
  $statusCancelled: "Cancelled",

  // Request Status Display Labels
  $statusDisplayOpen: "Open",
  $statusDisplayFilled: "Filled",
  $statusDisplayCancelled: "Cancelled",

  // worker status — string values returned by worker-facing endpoints
  // (WorkerRequestListModel / WorkerRequestDetailModel).
  // Agency/company pages use the numeric enum from src/constants/enums.ts.
  $statusNone: "None",
  $statusApply: "Applied",
  $statusDecline: "Declined",
  $statusReject: "Rejected",
  $statusBook: "Booked",
  $statusInQueue: "InQueue",
  $statusNotWorking: "NotWorking",
  $statusWorking: "Working",

  // worker status Display
  $statusDisplayNone: "None",
  $statusDisplayApply: "Applied",
  $statusDisplayDecline: "Declined",
  $statusDisplayReject: "Rejected",
  $statusDisplayBook: "Booked",
  $statusDisplayInQueue: "InQueue",
  $statusDisplayNotWorking: "NotWorking",
  $statusDisplayWorking: "Working",

  // Company sort by
  $companySortByName: "Name",
  $companySortByNumberId: "NumberId",
  $companySortByLocation: "Location",

  // Worker sort by
  $workerSortById: "Id",
  $workerSortByName: "Name",
  $workerSortByNumberId: "NumberId",
  $workerSortByPhone: "Phone",
  $workerSortByRequestId: "RequestId",
  $workerSortByCreateAt: "CreateAt",

  // Request sort by
  $requestSortByClient: "Client",
  $requestSortByJobTitle: "JobTitle",
  $requestSortByStartAt: "StartAt",
  $requestSortByRecruiter: "Recruiter",
  $requestSortByRate: "Rate",
  $requestSortByWorkersQuantity: "WorkersQuantity",
  $requestSortByUpdateAt: "UpdatedAt",

  // Candidates Sort By
  $candidatesSortByName: 'Name',
  $candidatesSortByPhone: 'Phone',
  $candidatesSortByLocation: 'Location',
  $candidatesSortBySkills: 'Skills',
  $candidatesSortByLastRequest: 'LastRequest',
  $candidatesSortByStatus: 'Status',
  $candidatesSortByCreateAt: 'CreateAt',
  $candidatesSortByRecruiter: 'Recruiter',

  // regex
  $regexAddress: /^[-.# a-zA-Z0-9]+$/,
  $regexEmail: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,

  //Users
  $agency: "agency",
  $company: "company",
  $worker: "worker",
  $companyUser: "company.user",

  // Duration Term — string values returned by worker-facing endpoints.
  // Agency/company pages (and create payloads) use the numeric enum from src/constants/enums.ts.
  $longTerm: "LongTerm",
  $shortTerm: "ShortTerm",

  // Employment Type — display/legacy values used by worker-facing pages.
  // Agency/company pages (and create payloads) use the numeric enum from src/constants/enums.ts.
  $fullTime: "FullTime",
  $partTime: "PartTime",
  $contractor: "Contractor",
  $temporary: "Temporary",

  // Agency Type
  $agencyTypeMaster: 1,
  $agencyTypeRegular: 2,
  $agencyTypeBusinessPartner: 3,

  $agencyTypes: [
    { value: 2, label: 'Regular' },
    { value: 3, label: 'Business Partner' },
  ] as AgencyTypeOption[],
};

export function registerAppGlobals(app: App): void {
  for (const [key, value] of Object.entries(appGlobals)) {
    app.config.globalProperties[key] = value;
  }
}

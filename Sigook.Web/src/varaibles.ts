import Vue from 'vue';

// Request Status — string values returned by worker-facing endpoints
// (WorkerRequestListModel / WorkerRequestDetailModel).
// Agency/company pages use the numeric enum from src/constants/enums.ts.
Vue.prototype.$statusOpen = "Open";
Vue.prototype.$statusFilled = "Filled";
Vue.prototype.$statusCancelled = "Cancelled";

// Request Status Display Labels
Vue.prototype.$statusDisplayOpen = "Open";
Vue.prototype.$statusDisplayFilled = "Filled";
Vue.prototype.$statusDisplayCancelled = "Cancelled";

// worker status — string values returned by worker-facing endpoints
// (WorkerRequestListModel / WorkerRequestDetailModel).
// Agency/company pages use the numeric enum from src/constants/enums.ts.
Vue.prototype.$statusNone = "None";
Vue.prototype.$statusApply = "Applied";
Vue.prototype.$statusDecline = "Declined";
Vue.prototype.$statusReject = "Rejected";
Vue.prototype.$statusBook = "Booked";
Vue.prototype.$statusInQueue = "InQueue";
Vue.prototype.$statusNotWorking = "NotWorking";
Vue.prototype.$statusWorking = "Working";

// worker status Display
Vue.prototype.$statusDisplayNone = "None";
Vue.prototype.$statusDisplayApply = "Applied";
Vue.prototype.$statusDisplayDecline = "Declined";
Vue.prototype.$statusDisplayReject = "Rejected";
Vue.prototype.$statusDisplayBook = "Booked";
Vue.prototype.$statusDisplayInQueue = "InQueue";
Vue.prototype.$statusDisplayNotWorking = "NotWorking";
Vue.prototype.$statusDisplayWorking = "Working";

// Company sort by
Vue.prototype.$companySortByName = "Name";
Vue.prototype.$companySortByNumberId = "NumberId";
Vue.prototype.$companySortByLocation = "Location";

// Worker sort by
Vue.prototype.$workerSortById = "Id";
Vue.prototype.$workerSortByName = "Name";
Vue.prototype.$workerSortByNumberId = "NumberId";
Vue.prototype.$workerSortByPhone = "Phone";
Vue.prototype.$workerSortByRequestId = "RequestId";
Vue.prototype.$workerSortByCreateAt = "CreateAt";

// Request sort by
Vue.prototype.$requestSortByClient = "Client";
Vue.prototype.$requestSortByJobTitle = "JobTitle";
Vue.prototype.$requestSortByStartAt = "StartAt";
Vue.prototype.$requestSortByRecruiter = "Recruiter";
Vue.prototype.$requestSortByRate = "Rate";
Vue.prototype.$requestSortByWorkersQuantity = "WorkersQuantity";
Vue.prototype.$requestSortByUpdateAt = "UpdatedAt";

// Candidates Sort By
Vue.prototype.$candidatesSortByName = 'Name';
Vue.prototype.$candidatesSortByPhone = 'Phone';
Vue.prototype.$candidatesSortByLocation = 'Location';
Vue.prototype.$candidatesSortBySkills = 'Skills';
Vue.prototype.$candidatesSortByLastRequest = 'LastRequest';
Vue.prototype.$candidatesSortByStatus = 'Status';
Vue.prototype.$candidatesSortByCreateAt = 'CreateAt';
Vue.prototype.$candidatesSortByRecruiter = 'Recruiter';

// regex
Vue.prototype.$regexAddress = /^[-.# a-zA-Z0-9]+$/;
Vue.prototype.$regexEmail = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

//Users
Vue.prototype.$agency = "agency";
Vue.prototype.$company = "company";
Vue.prototype.$worker = "worker";
Vue.prototype.$companyUser = "company.user";

// Duration Term — string values returned by worker-facing endpoints.
// Agency/company pages (and create payloads) use the numeric enum from src/constants/enums.ts.
Vue.prototype.$longTerm = "LongTerm";
Vue.prototype.$shortTerm = "ShortTerm";

// Employment Type — display/legacy values used by worker-facing pages.
// Agency/company pages (and create payloads) use the numeric enum from src/constants/enums.ts.
Vue.prototype.$fullTime = "FullTime";
Vue.prototype.$partTime = "PartTime";
Vue.prototype.$contractor = "Contractor";
Vue.prototype.$temporary = "Temporary";

// Agency Type
Vue.prototype.$agencyTypeMaster = 1;
Vue.prototype.$agencyTypeRegular = 2;
Vue.prototype.$agencyTypeBusinessPartner = 3;

Vue.prototype.$agencyTypes = [
  { value: 2, label: 'Regular' },
  { value: 3, label: 'Business Partner' }
];

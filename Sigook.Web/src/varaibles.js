import Vue from 'vue';

// request status
Vue.prototype.$statusRequested = "Requested";
Vue.prototype.$statusFinalized = "Finalized";
Vue.prototype.$statusCancelled = "Cancelled";
Vue.prototype.$statusInProcess = "InProcess";

// request
Vue.prototype.$statusOpen = "Open";
Vue.prototype.$statusFilled = "Filled";
Vue.prototype.$statusNotFilled = "Not Filled";

// requestDisplay
Vue.prototype.$statusDisplayRequested = "Requested";
Vue.prototype.$statusDisplayCancelled = "Cancelled";
Vue.prototype.$statusDisplayInProcess = "In Process";
Vue.prototype.$statusDisplayOpen = "Open";
Vue.prototype.$statusDisplayNoOpen = "No Open";

// worker status
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

// Duration Term
Vue.prototype.$longTerm = "LongTerm";
Vue.prototype.$shortTerm = "ShortTerm";

// Employment Type
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
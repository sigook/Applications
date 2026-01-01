import http from "../../security/apiService";
import Vue from "vue";

export default {
  namespaced: true,
  state: {
    agency: {},
    personnelAgencies: [],
    agencyRequestFilter: null,
    agencyCandidateFilter: null,
    agencyWorkerProfileFilter: null,
    agencyCompanyProfileFilter: null,
    agencyInvoiceFilter: null,
    agencyPayStubFilter: null,
    agencyListFilter: null,
  },
  mutations: {
    setAgency(state, data) {
      state.agency = {
        ...data,
        usaAgency: data.locations.some((l) => l.isUSA),
        masterAgency: data.agencyType === Vue.prototype.$agencyTypeMaster
      }
    },
    setPersonnelAgencies(state, data) {
      state.agency.agencies = data;
    },
    setAgencyRequestFilter(state, data) {
      state.agencyRequestFilter = data;
    },
    setAgencyCandidateFilter(state, data) {
      state.agencyCandidateFilter = data;
    },
    setAgencyWorkerProfileFilter(state, data) {
      state.agencyWorkerProfileFilter = data;
    },
    setAgencyCompanyProfileFilter(state, data) {
      state.agencyCompanyProfileFilter = data;
    },
    setAgencyInvoiceFilter(state, data) {
      state.agencyInvoiceFilter = data;
    },
    setAgencyPayStubFilter(state, data) {
      state.agencyPayStubFilter = data;
    },
    setAgencyListFilter(state, data) {
      state.agencyListFilter = data;
    },
  },
  actions: {
    updateAgencyRequestFilter(context, data) {
      context.commit("setAgencyRequestFilter", data);
    },
    updateAgencyCandidateFilter(context, data) {
      context.commit("setAgencyCandidateFilter", data);
    },
    updateAgencyWorkerProfileFilter(context, data) {
      context.commit("setAgencyWorkerProfileFilter", data);
    },
    updateAgencyCompanyProfileFilter(context, data) {
      context.commit("setAgencyCompanyProfileFilter", data);
    },
    updateAgencyInvoiceFilter(context, data) {
      context.commit("setAgencyInvoiceFilter", data);
    },
    updateAgencyPayStubFilter(context, data) {
      context.commit("setAgencyPayStubFilter", data);
    },
    updateAgencyListFilter(context, data) {
      context.commit("setAgencyListFilter", data);
    },
    getAgencyProfile(context) {
      return new Promise((resolve, reject) => {
        http
          .get("/api/Agency/Profile")
          .then((response) => {
            context.commit("setAgency", response.data);
            resolve(response.data);
          })
          .catch((error) => reject(error.response));
      });
    },
    getAgency(context, id) {
      return new Promise((resolve, reject) => {
        http.get(`/api/Agency/${id}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateAgency(context, agency) {
      return new Promise((resolve, reject) => {
        http
          .put("/api/Agency", agency)
          .then((response) => {
            context.dispatch("getAgency");
            resolve(response);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    getWorkers(context, filter) {
      return new Promise((resolve, reject) => {
        http
          .get("api/AgencyWorkerProfile", {
            params: { ...filter },
          })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    createWorker(context, worker) {
      return new Promise((resolve, reject) => {
        http.post(`api/AgencyWorkerProfile`, worker, {
          headers: { 'Content-Type': 'multipart/form-data' }
        })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      })
    },
    getAllWorkers(context, filter) {
      return new Promise((resolve, reject) => {
        http
          .get("api/AgencyWorkerProfile/Dropdown", {
            params: { ...filter },
          })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getAgencyReport(context, { filter, url }) {
      return new Promise((resolve, reject) => {
        http
          .get(url, {
            params: { ...filter },
            responseType: "blob",
          })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getWorker(context, id) {
      return new Promise((resolve, reject) => {
        http
          .get("/api/AgencyWorkerProfile/" + id)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateWorker(context, { id, worker }) {
      return new Promise((resolve, reject) => {
        http
          .put("/api/AgencyWorkerProfile/" + id, worker)
          .then((response) => resolve(response))
          .catch((error) => reject(error.response));
      });
    },
    updateApprovedToWork(context, id) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyWorkerProfile/${id}/ApprovedToWork`)
          .then((response) => resolve(response))
          .catch((error) => reject(error.response));
      });
    },
    agencyCommentWorker(context, { id, comment }) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyWorker/${id}/Comment`, comment)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    createCompany(context, company) {
      return new Promise((resolve, reject) => {
        http
          .post("/api/v2/AgencyCompanyProfile", company)
          .then((response) => resolve(response))
          .catch((error) => reject(error.response));
      });
    },
    getCompanies(context, filter) {
      return new Promise((resolve, reject) => {
        http
          .get("api/v2/AgencyCompanyProfile", {
            params: { ...filter },
          })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getAgencies(context, filter) {
      return new Promise((resolve, reject) => {
        http
          .get("api/Agency", {
            params: { ...filter },
          })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    createAgency(context, model) {
      return new Promise((resolve, reject) => {
        http
          .post("/api/Agency", model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getCompany({ commit }, companyId) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/v2/AgencyCompanyProfile/${companyId}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getCompanyJobPosition(context, companyProfileId) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyCompanyProfile/${companyProfileId}/JobPosition`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getCompanyLocation(context, companyProfileId) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyCompanyProfile/${companyProfileId}/Location`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateCompany({ commit }, { companyId, company }) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/v2/AgencyCompanyProfile/${companyId}`, company)
          .then((response) => resolve(response))
          .catch((error) => reject(error.response));
      });
    },
    updateCompanyVaccinationRequired(context, { companyProfileId, model }) {
      return new Promise((resolve, reject) => {
        http
          .put(
            `/api/v2/AgencyCompanyProfile/${companyProfileId}/VaccinationRequired`,
            model
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    postAgencyRequest(context, model) {
      return new Promise((resolve, reject) => {
        http
          .post(`api/AgencyRequest`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getAgencyRequests({ commit }, filter) {
      return new Promise((resolve, reject) => {
        http
          .get("/api/AgencyRequest", {
            params: { ...filter },
          })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getAllAgencyRequests({ commit }, filter) {
      return new Promise((resolve, reject) => {
        http
          .get("/api/AgencyRequest/all", {
            params: { ...filter },
          })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getAgencyRequest({ commit }, id) {
      return new Promise((resolve, reject) => {
        http
          .get("/api/AgencyRequest/" + id)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    cancelRequest(
      context,
      { id, cancellationReasonId, otherCancellationReason }
    ) {
      return new Promise((resolve, reject) => {
        http
          .put("/api/AgencyRequest/" + id + "/Cancel", {
            cancellationReasonId: cancellationReasonId,
            otherCancellationReason: otherCancellationReason,
          })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getAgencyRequestsWorkers(context, filter) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyRequest/${filter.requestId}/Worker`, {
            params: { ...filter },
          })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getAgencyRequestsWorker(context, { requestId, workerId }) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyRequest/${requestId}/Worker/${workerId}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getAgencyRequestWorkersAll(context, { requestId, pagination, filter }) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyRequest/${requestId}/Worker/All`, {
            params: { ...pagination, filter },
          })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    bookAgencyRequestWorker(context, { requestId, workerId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post(
            `/api/AgencyRequest/${requestId}/Worker/${workerId}/Book`,
            model
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateAgencyRequestWorkerStartDate(context, { requestId, id, model }) {
      return new Promise((resolve, reject) => {
        http
          .put(`api/AgencyRequest/${requestId}/Worker/${id}`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    rejectAgencyRequestWorker(context, { requestId, workerId, model }) {
      return new Promise((resolve, reject) => {
        http
          .put(
            `/api/AgencyRequest/${requestId}/Worker/${workerId}/Reject`,
            model
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteJobPosition(context, { companyProfileId, jobPositionRateId }) {
      return new Promise((resolve, reject) => {
        http
          .delete(
            `/api/AgencyJobPosition/${companyProfileId}/${jobPositionRateId}`
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getAgencyWorkerTimeSheet(context, { requestId, workerId }) {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/v2/AgencyRequest/${requestId}/Worker/${workerId}/TimeSheet`
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getAgencyWorkerTimeSheetByDate(context, { requestId, workerId, date }) {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/v2/AgencyRequest/${requestId}/Worker/${workerId}/TimeSheet`,
            {
              params: { ...date },
            }
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    postAgencyWorkerTimeSheet(context, { requestId, workerId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post(
            `/api/v2/AgencyRequest/${requestId}/Worker/${workerId}/TimeSheet`,
            model
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateWorkerTimeSheet(context, { requestId, workerId, id, model }) {
      return new Promise((resolve, reject) => {
        http
          .put(
            `/api/v2/AgencyRequest/${requestId}/Worker/${workerId}/TimeSheet/${id}`,
            model
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteWorkerTimeSheet(context, { requestId, workerId, id }) {
      return new Promise((resolve, reject) => {
        http
          .delete(
            `/api/v2/AgencyRequest/${requestId}/Worker/${workerId}/TimeSheet/${id}`
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getInvoiceNotes(context, id) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/CompanyProfile/${id}/InvoiceNotes`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    postInvoiceNotes(context, { id, model }) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/CompanyProfile/${id}/InvoiceNotes`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getCompanyInvoiceRecipients(context, companyProfileId) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/CompanyProfile/${companyProfileId}/InvoiceRecipient`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    postCompanyInvoiceRecipient(context, { companyProfileId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post(
            `/api/CompanyProfile/${companyProfileId}/InvoiceRecipient`,
            model
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteCompanyInvoiceRecipient(context, { companyProfileId, id }) {
      return new Promise((resolve, reject) => {
        http
          .delete(
            `/api/CompanyProfile/${companyProfileId}/InvoiceRecipient/${id}`
          )
          .then((response) => resolve(response.data))
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    updateCompanyInvoiceRecipient(context, { companyProfileId, id, model }) {
      return new Promise((resolve, reject) => {
        http
          .put(
            `/api/CompanyProfile/${companyProfileId}/InvoiceRecipient/${id}`,
            model
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getWeeklyPayrollByWeekEnding(context, { pagination }) {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/WeeklyPayroll/ByWeekEnding?PageSize=${pagination.size}&PageIndex=${pagination.page}`
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateManageCompanyProfile(context, { companyProfileId, model }) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/ManagerCompanyProfile/${companyProfileId}`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getManagerWorkerProfile() {
      return new Promise((resolve, reject) => {
        http
          .get("/api/ManagerWorkerProfile")
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getSkipPayrollNumber() {
      return new Promise((resolve, reject) => {
        http
          .get("/api/SkipPayrollNumber")
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    postSkipPayrollNumber(context, { model }) {
      return new Promise((resolve, reject) => {
        http
          .post("/api/SkipPayrollNumber", model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getWorkerProfilePunchCardId(context, { profileId }) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/WorkerProfile/${profileId}/PunchCardId`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateWorkerProfilePunchCardId(context, { profileId, model }) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/WorkerProfile/${profileId}/PunchCardId`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getUserNotification() {
      return new Promise((resolve, reject) => {
        http
          .get("/api/UserNotification")
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateUserNotification(context, model) {
      return new Promise((resolve, reject) => {
        http
          .put("/api/UserNotification", model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getRequestTimeSheetDocument(context, requestId) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/Request/${requestId}/TimeSheet/Document`, {
            responseType: "blob",
          })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getWorkersReportDocument(context, requestId) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/WorkersReportDocument/${requestId}/Document`, {
            responseType: "blob",
          })
          .then((response) => resolve(response.data))
          .catch((err) => reject(err.response));
      });
    },
    getAgencyWorkerProfileRequestHistory(context, { pagination, workerId }) {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/AgencyWorkerProfile/${workerId}/RequestHistory?PageSize=${pagination.size}&PageIndex=${pagination.page}`
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getAgencyWorkerProfileNote(context, { userId, pagination }) {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/AgencyWorkerProfile/${userId}/Note?PageSize=${pagination.size}&PageIndex=${pagination.page}`
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    createAgencyWorkerProfileNote(context, { userId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyWorkerProfile/${userId}/Note`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getAgencyCandidates(context, filter) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyCandidate`, { params: { ...filter } })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getAgencyCandidate(context, candidateId) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyCandidate/${candidateId}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    createAgencyCandidate(context, model) {
      return new Promise((resolve, reject) => {
        http
          .post("/api/AgencyCandidate", model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    addCandidateNumber(context, { candidateId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyCandidate/${candidateId}/PhoneNumber`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteCandidateNumber(context, { candidateId, number }) {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyCandidate/${candidateId}/PhoneNumber/${number}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    addCandidateSkill(context, { candidateId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyCandidate/${candidateId}/Skill`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteCandidateSkill(context, { candidateId, skill }) {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyCandidate/${candidateId}/Skill/${skill}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getCandidateDocuments(context, candidateId) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyCandidate/${candidateId}/Document`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    addCandidateDocument(context, { candidateId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyCandidate/${candidateId}/Document`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteCandidateDocument(context, { candidateId, id }) {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyCandidate/${candidateId}/Document/${id}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getCandidateNotes(context, { userId, pagination }) {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/AgencyCandidate/${userId}/Note?PageSize=${pagination.size}&PageIndex=${pagination.page}`
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    addCandidateNote(context, { userId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyCandidate/${userId}/Note`, model)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    deleteCandidateNote(context, { userId, id }) {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyCandidate/${userId}/Note/${id}`)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    updateCandidate(context, { candidateId, model }) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyCandidate/${candidateId}`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteCandidate(context, candidateId) {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyCandidate/${candidateId}`)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    updateCandidateRecruiter(context, candidateId) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyCandidate/${candidateId}/Recruiter`, null)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    convertToWorker(context, candidateId) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyCandidate/${candidateId}/convert-to-worker`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getTimesheetUsages(context, { requestId, workerId, id }) {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/v2/AgencyRequest/${requestId}/Worker/${workerId}/TimeSheet/${id}/Usages`
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    /*Agency Company Contact person*/
    getAgencyCompanyContactPerson(context, profileId) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyCompanyProfile/${profileId}/ContactPerson`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    createAgencyCompanyContactPerson(context, { profileId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyCompanyProfile/${profileId}/ContactPerson`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateAgencyCompanyContactPerson(context, { profileId, personId, model }) {
      return new Promise((resolve, reject) => {
        http
          .put(
            `/api/AgencyCompanyProfile/${profileId}/ContactPerson/${personId}`,
            model
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteAgencyCompanyContactPerson(context, { profileId, personId }) {
      return new Promise((resolve, reject) => {
        http
          .delete(
            `/api/AgencyCompanyProfile/${profileId}/ContactPerson/${personId}`
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    /*Agency Company Location*/
    getAgencyCompanyLocation(context, profileId) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyCompanyProfile/${profileId}/Location`)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    createAgencyCompanyLocation(context, { profileId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyCompanyProfile/${profileId}/Location`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateAgencyCompanyLocation(context, { profileId, locationId, model }) {
      return new Promise((resolve, reject) => {
        http
          .put(
            `/api/AgencyCompanyProfile/${profileId}/Location/${locationId}`,
            model
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteAgencyCompanyLocation(context, { profileId, locationId }) {
      return new Promise((resolve, reject) => {
        http
          .delete(
            `/api/AgencyCompanyProfile/${profileId}/Location/${locationId}`
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateAgencyCompanyContactInformation(context, { profileId, model }) {
      return new Promise((resolve, reject) => {
        http
          .put(
            `/api/AgencyCompanyProfile/${profileId}/ContactInformation`,
            model
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    /*Job position*/
    getAgencyCompanyJobPositions(context, companyProfileId) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyCompanyProfile/${companyProfileId}/JobPosition`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getAgencyCompanyJobPositionById(context, { profileId, id }) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyCompanyProfile/${profileId}/JobPosition/${id}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    createAgencyCompanyJobPosition(context, { profileId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyCompanyProfile/${profileId}/JobPosition`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateAgencyCompanyJobPosition(context, { profileId, id, model }) {
      return new Promise((resolve, reject) => {
        http
          .put(
            `/api/AgencyCompanyProfile/${profileId}/JobPosition/${id}`,
            model
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteAgencyCompanyJobPosition(context, { profileId, id }) {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyCompanyProfile/${profileId}/JobPosition/${id}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    petitionAgencyCompanyJobPosition(context, { profileId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post(
            `/api/AgencyCompanyProfile/${profileId}/JobPosition/Petition`,
            model
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    /*Job position Documents*/
    getAgencyCompanyJobPositionDocuments(
      context,
      { profileId, jobPositionId, pagination }
    ) {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/AgencyCompanyProfile/${profileId}/JobPosition/${jobPositionId}/Document?PageSize=${pagination.size}&PageIndex=${pagination.page}`
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    createAgencyCompanyJobPositionDocuments(
      context,
      { profileId, jobPositionId, model }
    ) {
      return new Promise((resolve, reject) => {
        http
          .post(
            `/api/AgencyCompanyProfile/${profileId}/JobPosition/${jobPositionId}/Document`,
            model
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteAgencyCompanyJobPositionDocuments(
      context,
      { profileId, jobPositionId, id }
    ) {
      return new Promise((resolve, reject) => {
        http
          .delete(
            `/api/AgencyCompanyProfile/${profileId}/JobPosition/${jobPositionId}/Document/${id}`
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    /*Documents*/
    getAgencyCompanyDocument(context, { profileId, pagination }) {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/AgencyCompanyProfile/${profileId}/Document?PageSize=${pagination.size}&PageIndex=${pagination.page}`
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    createAgencyCompanyDocument(context, { profileId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyCompanyProfile/${profileId}/Document`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteAgencyCompanyDocument(context, { profileId, id }) {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyCompanyProfile/${profileId}/Document/${id}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    /*Company Notes*/
    getAgencyCompanyNote(context, { userId, pagination }) {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/AgencyCompanyProfile/${userId}/Note?PageSize=${pagination.size}&PageIndex=${pagination.page}`
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    createAgencyCompanyNote(context, { userId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyCompanyProfile/${userId}/Note`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateAgencyCompanyNote(context, { userId, id, model }) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyCompanyProfile/${userId}/Note/${id}`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteAgencyCompanyNote(context, { userId, id }) {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyCompanyProfile/${userId}/Note/${id}`)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    /*Request Notes*/
    getAgencyRequestNote(context, { userId, pagination }) {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/AgencyRequest/${userId}/Note?PageSize=${pagination.size}&PageIndex=${pagination.page}`
          )
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    createAgencyRequestNote(context, { userId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyRequest/${userId}/Note`, model)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    updateAgencyRequestNote(context, { userId, id, model }) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyRequest/${userId}/Note/${id}`, model)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    deleteAgencyRequestNote(context, { userId, id }) {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyRequest/${userId}/Note/${id}`)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    /*Request Worker Notes*/
    getAgencyRequestWorkerNote(context, { requestId, userId, pagination }) {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/AgencyRequest/${requestId}/Worker/${userId}/Note?PageSize=${pagination.size}&PageIndex=${pagination.page}`
          )
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    createAgencyRequestWorkerNote(context, { requestId, userId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyRequest/${requestId}/Worker/${userId}/Note`, model)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    updateAgencyRequestWorkerNote(context, { requestId, userId, id, model }) {
      return new Promise((resolve, reject) => {
        http
          .put(
            `/api/AgencyRequest/${requestId}/Worker/${userId}/Note/${id}`,
            model
          )
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    deleteAgencyRequestWorkerNote(context, { requestId, userId, id }) {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyRequest/${requestId}/Worker/${userId}/Note/${id}`)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    /*Requested by*/
    getAgencyRequestRequestedBy(context, requestId) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyRequest/${requestId}/RequestedBy`)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    postAgencyRequestRequestedBy(context, { requestId, contactPersonId }) {
      return new Promise((resolve, reject) => {
        http
          .post(
            `/api/AgencyRequest/${requestId}/RequestedBy/${contactPersonId}`
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteAgencyRequestRequestedBy(context, { requestId, contactPersonId }) {
      return new Promise((resolve, reject) => {
        http
          .delete(
            `/api/AgencyRequest/${requestId}/RequestedBy/${contactPersonId}`
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getAgencyRequestReportTo(context, requestId) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyRequest/${requestId}/ReportTo`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    postAgencyRequestReportTo(context, { requestId, contactPersonId }) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyRequest/${requestId}/ReportTo/${contactPersonId}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteAgencyRequestReportTo(context, { requestId, contactPersonId }) {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyRequest/${requestId}/ReportTo/${contactPersonId}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    /*Personnel*/
    getAgencyPersonnel(context) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyPersonnel`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteAgencyPersonnel(context, id) {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyPersonnel/${id}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    createAgencyPersonnel(context, model) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyPersonnel`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getAgencyRequestRecruiter(context, requestId) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyRequest/${requestId}/Recruiter`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    postAgencyRequestRecruiter(context, { requestId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyRequest/${requestId}/Recruiter`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteAgencyRequestRecruiter(context, { requestId, id }) {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyRequest/${requestId}/Recruiter/${id}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    increaseWorkersQuantityByOne(context, requestId) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyRequest/${requestId}/IncreaseWorkersQuantityByOne`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    reduceWorkersQuantityByOne(context, requestId) {
      return new Promise((resolve, reject) => {
        http
          .put(`api/AgencyRequest/${requestId}/ReduceWorkersQuantityByOne`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getAgencySkill(context, { requestId }) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyRequest/${requestId}/Skill`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    postAgencySkill(context, { requestId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyRequest/${requestId}/Skill`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteAgencySkill(context, { requestId, id }) {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyRequest/${requestId}/Skill/${id}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getAgencyRequestApplicant(context, filter) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyRequest/${filter.requestId}/Applicant`, {
            params: { ...filter },
          })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    postAgencyRequestApplicant(context, { requestId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyRequest/${requestId}/Applicant`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteAgencyRequestApplicant(context, { requestId, id }) {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyRequest/${requestId}/Applicant/${id}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateAgencyRequestApplicant(context, { requestId, id, model }) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyRequest/${requestId}/Applicant/${id}`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateAgencyWorkerProfileDNU(context, id) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyWorkerProfile/${id}/Dnu`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateAgencyCompanyProfileLogo(context, { profileId, model }) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyCompanyProfile/${profileId}/Logo`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateAgencyRequestShift(context, { requestId, model }) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyRequest/${requestId}/Shift`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getAgencyRequestBoard(context, { pagination }) {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/AgencyRequest/Board?PageSize=${pagination.size}&PageIndex=${pagination.page}`
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateAgencyRequest(context, { requestId, model }) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyRequest/${requestId}`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateAgencyRequestIsAsap(context, requestId) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyRequest/${requestId}/IsAsap`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateAgencyPunchCardVisibilityStatusInApp(context, requestId) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyRequest/${requestId}/PunchCardVisibilityStatusInApp`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateAgencyWorkerContractor(context, id) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyWorkerProfile/${id}/IsContractor`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateAgencyWorkerSubContractor(context, id) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyWorkerProfile/${id}/IsSubcontractor`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateWorkerProfileTaxCategory(context, payload) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyWorkerProfile/${payload.id}/tax-category`, payload)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateWorkerProfileTaxRate(context, payload) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyWorkerProfile/${payload.id}/tax-rate`, payload)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getAgencyWorkerOtherDocuments(context, id) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyWorkerProfile/${id}/OtherDocument`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    agencyRequestOpen(context, id) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyRequest/${id}/Open`, id)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateAgencyWorkerEmail(context, { workerProfileId, model }) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyWorkerProfile/${workerProfileId}/Email`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateAgencyCompanyEmail(context, { companyProfileId, model }) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/v2/AgencyCompanyProfile/${companyProfileId}/Email`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    agencyRequestSendInvitation(context, requestId) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyRequest/${requestId}/SendInvitation`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getAgencyLocation() {
      return new Promise((resolve, reject) => {
        http
          .get("/api/Agency/Location")
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    createAgencyLocation(context, model) {
      return new Promise((resolve, reject) => {
        http
          .post("/api/Agency/Location", model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateAgencyLocation(context, { id, model }) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/Agency/Location/${id}`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteAgencyLocation(context, id) {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/Agency/Location/${id}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getPersonnelAgency(context) {
      return new Promise((resolve, reject) => {
        http
          .get("/api/PersonnelAgency")
          .then((response) => {
            context.commit("setPersonnelAgencies", response.data);
            resolve(response.data);
          })
          .catch((error) => reject(error.response));
      });
    },
    putPersonnelAgency(context, id) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/PersonnelAgency/${id}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updatePermissionToSeeOrders(context, { companyId, settings }) {
      return new Promise((resolve, reject) => {
        http
          .patch(
            `/api/V2/AgencyCompanyProfile/${companyId}/RequiresPermissionToSeeOrders`,
            settings
          )
          .then((response) => resolve(response))
          .catch((error) => reject(error.response));
      });
    },
    updatePaidHolidays(context, { companyId, settings }) {
      return new Promise((resolve, reject) => {
        http
          .patch(
            `/api/V2/AgencyCompanyProfile/${companyId}/PaidHolidays`,
            settings
          )
          .then((response) => resolve(response))
          .catch((error) => reject(error.response));
      });
    },
    updateOvertime(context, { companyId, settings }) {
      return new Promise((resolve, reject) => {
        http
          .patch(`/api/V2/AgencyCompanyProfile/${companyId}/Overtime`, settings)
          .then((response) => resolve(response))
          .catch((error) => reject(error.response));
      });
    },
    getCompanyUsers(context, id) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/V2/AgencyCompanyProfile/${id}/CompanyUsers`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateIsAsapRequests(context, model) {
      return new Promise((resolve, reject) => {
        http
          .put("/api/AgencyRequest/is-asap", model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getCompanyProfileUsers(context, profileId) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/agency-company-profile-user/${profileId}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    createCompanyProfileUser(context, payload) {
      return new Promise((resolve, reject) => {
        http
          .post(
            `/api/agency-company-profile-user/${payload.companyId}`,
            payload.user
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteCompanyProfileUser(context, payload) {
      return new Promise((resolve, reject) => {
        http
          .delete(
            `/api/agency-company-profile-user/${payload.companyId}/users/${payload.userId}`
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getAgencyWorkerProfileHolidays(context, workerProfileId) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/agency-worker-profile-holiday/${workerProfileId}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    addUpdateAgencyWorkerProfileHolidays(context, payload) {
      return new Promise((resolve, reject) => {
        http
          .post(
            `/api/agency-worker-profile-holiday/${payload.workerProfileId}`,
            payload.data
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    addNewHoliday(context, payload) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/agency-worker-profile-holiday/new-holiday`, payload)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    bulkCandidates(context, payload) {
      return new Promise((resolve, reject) => {
        const formData = new FormData();
        formData.append("file", payload.file);
        http
          .post(`/api/AgencyCandidate/bulk/${payload.agencyId}`, formData, {
            responseType: "blob",
            headers: {
              "Content-Type": "multipart/form-data",
            },
          })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    bulkCompanies(context, payload) {
      return new Promise((resolve, reject) => {
        const formData = new FormData();
        formData.append("file", payload.file);
        http
          .post(
            `/api/v2/AgencyCompanyProfile/bulk/${payload.agencyId}`,
            formData,
            {
              responseType: "blob",
              headers: {
                "Content-Type": "multipart/form-data",
              },
            }
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getInvoices(context, filter) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/agency/accounting/Invoices`, {
            params: { ...filter },
          })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getAgencyCompanyProfileWithRequests(context) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/v2/AgencyCompanyProfile/company-with-requests`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getCompanyProvinceWithTaxes(context, companyProfileId) {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/v2/AgencyCompanyProfile/${companyProfileId}/company-provinces-taxes`
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    previewInvoice(context, payload) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/agency/accounting/Invoices/Preview`, payload)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    createInvoice(context, payload) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/agency/accounting/Invoices`, payload)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    downloadInvoicePdf(context, invoiceId) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/v4/Accounting/Invoice/${invoiceId}/Document/PDF`, {
            responseType: "blob",
          })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getVerificationCode(context, invoiceId) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/v4/Accounting/Invoice/${invoiceId}/SendVerificationCode`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getPayStubsByInvoice(context, invoiceId) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/v4/Accounting/Invoice/${invoiceId}/PayStub`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteInvoice(context, payload) {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/v4/Accounting/Invoice/${payload.invoiceId}`, {
            data: payload,
          })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getInvoiceRecipients(context, companyProfileId) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/CompanyProfile/${companyProfileId}/InvoiceRecipient`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    sendInvoiceEmail(context, payload) {
      return new Promise((resolve, reject) => {
        const formData = new FormData();
        payload.recipients.forEach((recipient) =>
          formData.append("cc", recipient)
        );
        formData.append("subject", payload.subject);
        formData.append("message", payload.body);
        payload.attachments.forEach((file) => formData.append("files", file));
        http
          .post(
            `/api/v4/Accounting/Invoice/${payload.invoiceId}/Document/Email`,
            formData,
            {
              headers: {
                "Content-Type": "multipart/form-data",
              },
            }
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getPayStubs(context, filter) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/agency/accounting/PayStubs`, {
            params: { ...filter },
          })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    downloadPayStubPdf(context, payStubId) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/v4/Accounting/PayStub/${payStubId}/Document/PDF`, {
            responseType: "blob",
          })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deletePayStub(context, payStubId) {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/v4/Accounting/PayStub/${payStubId}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    sendPayStubEmail(context, payStubId) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/v4/Accounting/PayStub/${payStubId}/Document/Email`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getWorkersReadyForPayStub(context) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/agency/accounting/PayStubs/WorkersReadyForPayStub`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    generatePayStubs(context, workerIds) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/agency/accounting/PayStubs/Generate`, workerIds)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getPayrollSubcontractors(context, filter) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/agency/accounting/reports/subcontractors`, {
            params: { ...filter },
          })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    downloadSubcontractorReport(context, weekEnding) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/agency/accounting/reports/subcontractors/file`, {
            params: { weekEnding },
            responseType: "blob",
          })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getJobPositionsHoursWorked(context, filter) {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/agency/accounting/reports/${filter.companyId}/job-positions`,
            {
              params: { ...filter },
            }
          )
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getHoursWorkedReport(context, filter) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/agency/accounting/reports/hours-worked`, {
            params: { ...filter },
          })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getT4Report(context, filter) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/agency/accounting/reports/t4`, {
            params: { ...filter },
            responseType: "blob",
          })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getPaymentReport(context, filter) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/agency/accounting/reports/payments`, {
            params: { ...filter },
          })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    downloadWeeklyPayrollReport(context, weekEnding) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/agency/accounting/reports/payments/file`, {
            params: { weekEnding },
            responseType: "blob",
          })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getSkipPayrollNumbers(context, filter) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/agency/accounting/PayStubs/skip-payroll-number`, {
            params: { ...filter },
          })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    addSkipPayrollNumber(context, payload) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/agency/accounting/PayStubs/skip-payroll-number`, payload)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    createPayStub(context, payload) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/v4/accounting/PayStub`, payload)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
  },
};

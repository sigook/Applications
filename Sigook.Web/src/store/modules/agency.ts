import http from "../../security/apiService";
import Vue from "vue";

export interface AgencyState {
  agency: any;
  personnelAgencies: any[];
  agencyRequestFilter: any;
  agencyCandidateFilter: any;
  agencyWorkerProfileFilter: any;
  agencyCompanyProfileFilter: any;
  agencyInvoiceFilter: any;
  agencyPayStubFilter: any;
  agencyListFilter: any;
}

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
  } as AgencyState,
  mutations: {
    setAgency(state: AgencyState, data: any) {
      state.agency = {
        ...data,
        agencies: data.agencies || (state.agency && state.agency.agencies) || [],
        usaAgency: data.locations.some((l: any) => l.isUSA),
        masterAgency: data.agencyType === (Vue.prototype as any).$agencyTypeMaster
      }
    },
    setPersonnelAgencies(state: AgencyState, data: any) {
      state.agency.agencies = data;
    },
    setAgencyRequestFilter(state: AgencyState, data: any) {
      state.agencyRequestFilter = data;
    },
    setAgencyCandidateFilter(state: AgencyState, data: any) {
      state.agencyCandidateFilter = data;
    },
    setAgencyWorkerProfileFilter(state: AgencyState, data: any) {
      state.agencyWorkerProfileFilter = data;
    },
    setAgencyCompanyProfileFilter(state: AgencyState, data: any) {
      state.agencyCompanyProfileFilter = data;
    },
    setAgencyInvoiceFilter(state: AgencyState, data: any) {
      state.agencyInvoiceFilter = data;
    },
    setAgencyPayStubFilter(state: AgencyState, data: any) {
      state.agencyPayStubFilter = data;
    },
    setAgencyListFilter(state: AgencyState, data: any) {
      state.agencyListFilter = data;
    },
  },
  actions: {
    updateAgencyRequestFilter(context: any, data: any) {
      context.commit("setAgencyRequestFilter", data);
    },
    updateAgencyCandidateFilter(context: any, data: any) {
      context.commit("setAgencyCandidateFilter", data);
    },
    updateAgencyWorkerProfileFilter(context: any, data: any) {
      context.commit("setAgencyWorkerProfileFilter", data);
    },
    updateAgencyCompanyProfileFilter(context: any, data: any) {
      context.commit("setAgencyCompanyProfileFilter", data);
    },
    updateAgencyInvoiceFilter(context: any, data: any) {
      context.commit("setAgencyInvoiceFilter", data);
    },
    updateAgencyPayStubFilter(context: any, data: any) {
      context.commit("setAgencyPayStubFilter", data);
    },
    updateAgencyListFilter(context: any, data: any) {
      context.commit("setAgencyListFilter", data);
    },
    getAgencyProfile(context: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get("/api/Agency/Profile")
          .then((response: any) => {
            context.commit("setAgency", response.data);
            resolve(response.data);
          })
          .catch((error: any) => reject(error.response));
      });
    },
    getAgency(context: any, id: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get(`/api/Agency/${id}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateAgency(context: any, agency: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put("/api/Agency", agency)
          .then((response: any) => {
            context.dispatch("getAgency");
            resolve(response);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    getWorkers(context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get("api/AgencyWorkerProfile", {
            params: { ...filter },
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    createWorker(context: any, worker: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.post(`api/AgencyWorkerProfile`, worker, {
          headers: { 'Content-Type': 'multipart/form-data' }
        })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      })
    },
    getAllWorkers(context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get("api/AgencyWorkerProfile/Dropdown", {
            params: { ...filter },
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAgencyReport(context: any, { filter, url }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(url, {
            params: { ...filter },
            responseType: "blob",
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getWorker(context: any, id: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get("/api/AgencyWorkerProfile/" + id)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateWorker(context: any, { id, worker }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put("/api/AgencyWorkerProfile/" + id, worker)
          .then((response: any) => resolve(response))
          .catch((error: any) => reject(error.response));
      });
    },
    updateApprovedToWork(context: any, id: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyWorkerProfile/${id}/ApprovedToWork`)
          .then((response: any) => resolve(response))
          .catch((error: any) => reject(error.response));
      });
    },
    agencyCommentWorker(context: any, { id, comment }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyWorker/${id}/Comment`, comment)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    createCompany(context: any, company: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post("/api/v2/AgencyCompanyProfile", company)
          .then((response: any) => resolve(response))
          .catch((error: any) => reject(error.response));
      });
    },
    getCompanies(context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get("api/v2/AgencyCompanyProfile", {
            params: { ...filter },
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAgencies(context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get("api/Agency", {
            params: { ...filter },
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    createAgency(context: any, model: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post("/api/Agency", model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getCompany({ commit: _commit }: any, companyId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/v2/AgencyCompanyProfile/${companyId}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getCompanyJobPosition(context: any, companyProfileId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyCompanyProfile/${companyProfileId}/JobPosition`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getCompanyLocation(context: any, companyProfileId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyCompanyProfile/${companyProfileId}/Location`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateCompany({ commit: _commit }: any, { companyId, company }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/v2/AgencyCompanyProfile/${companyId}`, company)
          .then((response: any) => resolve(response))
          .catch((error: any) => reject(error.response));
      });
    },
    updateCompanyVaccinationRequired(context: any, { companyProfileId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(
            `/api/v2/AgencyCompanyProfile/${companyProfileId}/VaccinationRequired`,
            model
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    postAgencyRequest(context: any, model: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`api/AgencyRequest`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAgencyRequests({ commit: _commit }: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get("/api/AgencyRequest", {
            params: { ...filter },
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAllAgencyRequests({ commit: _commit }: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get("/api/AgencyRequest/all", {
            params: { ...filter },
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAgencyRequest({ commit: _commit }: any, id: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get("/api/AgencyRequest/" + id)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    cancelRequest(
      context: any,
      { id, cancellationReasonId, otherCancellationReason }: any
    ): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put("/api/AgencyRequest/" + id + "/Cancel", {
            cancellationReasonId: cancellationReasonId,
            otherCancellationReason: otherCancellationReason,
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAgencyRequestsWorkers(context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyRequest/${filter.requestId}/Worker`, {
            params: { ...filter },
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAgencyRequestsWorker(context: any, { requestId, workerId }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyRequest/${requestId}/Worker/${workerId}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAgencyRequestWorkersAll(context: any, { requestId, pagination, filter }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyRequest/${requestId}/Worker/All`, {
            params: { ...pagination, filter },
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    bookAgencyRequestWorker(context: any, { requestId, workerId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(
            `/api/AgencyRequest/${requestId}/Worker/${workerId}/Book`,
            model
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateAgencyRequestWorkerStartDate(context: any, { requestId, id, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`api/AgencyRequest/${requestId}/Worker/${id}`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    rejectAgencyRequestWorker(context: any, { requestId, workerId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(
            `/api/AgencyRequest/${requestId}/Worker/${workerId}/Reject`,
            model
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteJobPosition(context: any, { companyProfileId, jobPositionRateId }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(
            `/api/AgencyJobPosition/${companyProfileId}/${jobPositionRateId}`
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAgencyWorkerTimeSheet(context: any, { requestId, workerId }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/v2/AgencyRequest/${requestId}/Worker/${workerId}/TimeSheet`
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAgencyWorkerTimeSheetByDate(context: any, { requestId, workerId, date }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/v2/AgencyRequest/${requestId}/Worker/${workerId}/TimeSheet`,
            {
              params: { ...date },
            }
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    postAgencyWorkerTimeSheet(context: any, { requestId, workerId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(
            `/api/v2/AgencyRequest/${requestId}/Worker/${workerId}/TimeSheet`,
            model
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateWorkerTimeSheet(context: any, { requestId, workerId, id, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(
            `/api/v2/AgencyRequest/${requestId}/Worker/${workerId}/TimeSheet/${id}`,
            model
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteWorkerTimeSheet(context: any, { requestId, workerId, id }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(
            `/api/v2/AgencyRequest/${requestId}/Worker/${workerId}/TimeSheet/${id}`
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getInvoiceNotes(context: any, id: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/CompanyProfile/${id}/InvoiceNotes`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    postInvoiceNotes(context: any, { id, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/CompanyProfile/${id}/InvoiceNotes`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getCompanyInvoiceRecipients(context: any, companyProfileId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/CompanyProfile/${companyProfileId}/InvoiceRecipient`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    postCompanyInvoiceRecipient(context: any, { companyProfileId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(
            `/api/CompanyProfile/${companyProfileId}/InvoiceRecipient`,
            model
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteCompanyInvoiceRecipient(context: any, { companyProfileId, id }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(
            `/api/CompanyProfile/${companyProfileId}/InvoiceRecipient/${id}`
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    updateCompanyInvoiceRecipient(context: any, { companyProfileId, id, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(
            `/api/CompanyProfile/${companyProfileId}/InvoiceRecipient/${id}`,
            model
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getWeeklyPayrollByWeekEnding(context: any, { pagination }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/WeeklyPayroll/ByWeekEnding?PageSize=${pagination.size}&PageIndex=${pagination.page}`
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateManageCompanyProfile(context: any, { companyProfileId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/ManagerCompanyProfile/${companyProfileId}`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getManagerWorkerProfile(): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get("/api/ManagerWorkerProfile")
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getSkipPayrollNumber(): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get("/api/SkipPayrollNumber")
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    postSkipPayrollNumber(context: any, { model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post("/api/SkipPayrollNumber", model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getWorkerProfilePunchCardId(context: any, { profileId }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/WorkerProfile/${profileId}/PunchCardId`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateWorkerProfilePunchCardId(context: any, { profileId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/WorkerProfile/${profileId}/PunchCardId`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getUserNotification(): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get("/api/UserNotification")
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateUserNotification(context: any, model: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put("/api/UserNotification", model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getRequestTimeSheetDocument(context: any, requestId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/Request/${requestId}/TimeSheet/Document`, {
            responseType: "blob",
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getWorkersReportDocument(context: any, requestId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/WorkersReportDocument/${requestId}/Document`, {
            responseType: "blob",
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAgencyWorkerProfileRequestHistory(context: any, { pagination, workerId }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/AgencyWorkerProfile/${workerId}/RequestHistory?PageSize=${pagination.size}&PageIndex=${pagination.page}`
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAgencyWorkerProfileNote(context: any, { userId, pagination }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/AgencyWorkerProfile/${userId}/Note?PageSize=${pagination.size}&PageIndex=${pagination.page}`
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    createAgencyWorkerProfileNote(context: any, { userId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyWorkerProfile/${userId}/Note`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAgencyCandidates(context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyCandidate`, { params: { ...filter } })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAgencyCandidate(context: any, candidateId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyCandidate/${candidateId}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    createAgencyCandidate(context: any, model: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post("/api/AgencyCandidate", model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    addCandidateNumber(context: any, { candidateId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyCandidate/${candidateId}/PhoneNumber`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteCandidateNumber(context: any, { candidateId, number }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyCandidate/${candidateId}/PhoneNumber/${number}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    addCandidateSkill(context: any, { candidateId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyCandidate/${candidateId}/Skill`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteCandidateSkill(context: any, { candidateId, skill }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyCandidate/${candidateId}/Skill/${skill}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getCandidateDocuments(context: any, candidateId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyCandidate/${candidateId}/Document`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    addCandidateDocument(context: any, { candidateId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyCandidate/${candidateId}/Document`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteCandidateDocument(context: any, { candidateId, id }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyCandidate/${candidateId}/Document/${id}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getCandidateNotes(context: any, { userId, pagination }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/AgencyCandidate/${userId}/Note?PageSize=${pagination.size}&PageIndex=${pagination.page}`
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    addCandidateNote(context: any, { userId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyCandidate/${userId}/Note`, model)
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    deleteCandidateNote(context: any, { userId, id }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyCandidate/${userId}/Note/${id}`)
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    updateCandidate(context: any, { candidateId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyCandidate/${candidateId}`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteCandidate(context: any, candidateId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyCandidate/${candidateId}`)
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    updateCandidateRecruiter(context: any, candidateId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyCandidate/${candidateId}/Recruiter`, null)
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    convertToWorker(context: any, candidateId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyCandidate/${candidateId}/convert-to-worker`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getTimesheetUsages(context: any, { requestId, workerId, id }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/v2/AgencyRequest/${requestId}/Worker/${workerId}/TimeSheet/${id}/Usages`
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    /*Agency Company Contact person*/
    getAgencyCompanyContactPerson(context: any, profileId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyCompanyProfile/${profileId}/ContactPerson`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    createAgencyCompanyContactPerson(context: any, { profileId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyCompanyProfile/${profileId}/ContactPerson`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateAgencyCompanyContactPerson(context: any, { profileId, personId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(
            `/api/AgencyCompanyProfile/${profileId}/ContactPerson/${personId}`,
            model
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteAgencyCompanyContactPerson(context: any, { profileId, personId }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(
            `/api/AgencyCompanyProfile/${profileId}/ContactPerson/${personId}`
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    /*Agency Company Location*/
    getAgencyCompanyLocation(context: any, profileId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyCompanyProfile/${profileId}/Location`)
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    createAgencyCompanyLocation(context: any, { profileId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyCompanyProfile/${profileId}/Location`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateAgencyCompanyLocation(context: any, { profileId, locationId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(
            `/api/AgencyCompanyProfile/${profileId}/Location/${locationId}`,
            model
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteAgencyCompanyLocation(context: any, { profileId, locationId }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(
            `/api/AgencyCompanyProfile/${profileId}/Location/${locationId}`
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateAgencyCompanyContactInformation(context: any, { profileId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(
            `/api/AgencyCompanyProfile/${profileId}/ContactInformation`,
            model
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    /*Job position*/
    getAgencyCompanyJobPositions(context: any, companyProfileId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyCompanyProfile/${companyProfileId}/JobPosition`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAgencyCompanyJobPositionById(context: any, { profileId, id }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyCompanyProfile/${profileId}/JobPosition/${id}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    createAgencyCompanyJobPosition(context: any, { profileId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyCompanyProfile/${profileId}/JobPosition`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateAgencyCompanyJobPosition(context: any, { profileId, id, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(
            `/api/AgencyCompanyProfile/${profileId}/JobPosition/${id}`,
            model
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteAgencyCompanyJobPosition(context: any, { profileId, id }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyCompanyProfile/${profileId}/JobPosition/${id}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    petitionAgencyCompanyJobPosition(context: any, { profileId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(
            `/api/AgencyCompanyProfile/${profileId}/JobPosition/Petition`,
            model
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    /*Job position Documents*/
    getAgencyCompanyJobPositionDocuments(
      context: any,
      { profileId, jobPositionId, pagination }: any
    ): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/AgencyCompanyProfile/${profileId}/JobPosition/${jobPositionId}/Document?PageSize=${pagination.size}&PageIndex=${pagination.page}`
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    createAgencyCompanyJobPositionDocuments(
      context: any,
      { profileId, jobPositionId, model }: any
    ): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(
            `/api/AgencyCompanyProfile/${profileId}/JobPosition/${jobPositionId}/Document`,
            model
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteAgencyCompanyJobPositionDocuments(
      context: any,
      { profileId, jobPositionId, id }: any
    ): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(
            `/api/AgencyCompanyProfile/${profileId}/JobPosition/${jobPositionId}/Document/${id}`
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    /*Documents*/
    getAgencyCompanyDocument(context: any, { profileId, pagination }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/AgencyCompanyProfile/${profileId}/Document?PageSize=${pagination.size}&PageIndex=${pagination.page}`
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    createAgencyCompanyDocument(context: any, { profileId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyCompanyProfile/${profileId}/Document`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteAgencyCompanyDocument(context: any, { profileId, id }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyCompanyProfile/${profileId}/Document/${id}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    /*Company Notes*/
    getAgencyCompanyNote(context: any, { userId, pagination }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/AgencyCompanyProfile/${userId}/Note?PageSize=${pagination.size}&PageIndex=${pagination.page}`
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    createAgencyCompanyNote(context: any, { userId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyCompanyProfile/${userId}/Note`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateAgencyCompanyNote(context: any, { userId, id, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyCompanyProfile/${userId}/Note/${id}`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteAgencyCompanyNote(context: any, { userId, id }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyCompanyProfile/${userId}/Note/${id}`)
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    /*Request Notes*/
    getAgencyRequestNote(context: any, { userId, pagination }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/AgencyRequest/${userId}/Note?PageSize=${pagination.size}&PageIndex=${pagination.page}`
          )
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    createAgencyRequestNote(context: any, { userId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyRequest/${userId}/Note`, model)
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    updateAgencyRequestNote(context: any, { userId, id, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyRequest/${userId}/Note/${id}`, model)
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    deleteAgencyRequestNote(context: any, { userId, id }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyRequest/${userId}/Note/${id}`)
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    /*Request Worker Notes*/
    getAgencyRequestWorkerNote(context: any, { requestId, userId, pagination }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/AgencyRequest/${requestId}/Worker/${userId}/Note?PageSize=${pagination.size}&PageIndex=${pagination.page}`
          )
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    createAgencyRequestWorkerNote(context: any, { requestId, userId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyRequest/${requestId}/Worker/${userId}/Note`, model)
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    updateAgencyRequestWorkerNote(context: any, { requestId, userId, id, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(
            `/api/AgencyRequest/${requestId}/Worker/${userId}/Note/${id}`,
            model
          )
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    deleteAgencyRequestWorkerNote(context: any, { requestId, userId, id }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyRequest/${requestId}/Worker/${userId}/Note/${id}`)
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    /*Requested by*/
    getAgencyRequestRequestedBy(context: any, requestId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyRequest/${requestId}/RequestedBy`)
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    postAgencyRequestRequestedBy(context: any, { requestId, contactPersonId }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(
            `/api/AgencyRequest/${requestId}/RequestedBy/${contactPersonId}`
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteAgencyRequestRequestedBy(context: any, { requestId, contactPersonId }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(
            `/api/AgencyRequest/${requestId}/RequestedBy/${contactPersonId}`
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAgencyRequestReportTo(context: any, requestId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyRequest/${requestId}/ReportTo`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    postAgencyRequestReportTo(context: any, { requestId, contactPersonId }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyRequest/${requestId}/ReportTo/${contactPersonId}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteAgencyRequestReportTo(context: any, { requestId, contactPersonId }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyRequest/${requestId}/ReportTo/${contactPersonId}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    /*Personnel*/
    getAgencyPersonnel(_context: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyPersonnel`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteAgencyPersonnel(context: any, id: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyPersonnel/${id}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    createAgencyPersonnel(context: any, model: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyPersonnel`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAgencyRequestRecruiter(context: any, requestId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyRequest/${requestId}/Recruiter`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    postAgencyRequestRecruiter(context: any, { requestId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyRequest/${requestId}/Recruiter`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteAgencyRequestRecruiter(context: any, { requestId, id }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyRequest/${requestId}/Recruiter/${id}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    increaseWorkersQuantityByOne(context: any, requestId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyRequest/${requestId}/IncreaseWorkersQuantityByOne`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    reduceWorkersQuantityByOne(context: any, requestId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`api/AgencyRequest/${requestId}/ReduceWorkersQuantityByOne`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAgencySkill(context: any, { requestId }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyRequest/${requestId}/Skill`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    postAgencySkill(context: any, { requestId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyRequest/${requestId}/Skill`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteAgencySkill(context: any, { requestId, id }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyRequest/${requestId}/Skill/${id}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAgencyRequestApplicant(context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyRequest/${filter.requestId}/Applicant`, {
            params: { ...filter },
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    postAgencyRequestApplicant(context: any, { requestId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyRequest/${requestId}/Applicant`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteAgencyRequestApplicant(context: any, { requestId, id }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/AgencyRequest/${requestId}/Applicant/${id}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateAgencyRequestApplicant(context: any, { requestId, id, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyRequest/${requestId}/Applicant/${id}`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateAgencyWorkerProfileDNU(context: any, id: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyWorkerProfile/${id}/Dnu`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateAgencyCompanyProfileLogo(context: any, { profileId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyCompanyProfile/${profileId}/Logo`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateAgencyRequestShift(context: any, { requestId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyRequest/${requestId}/Shift`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAgencyRequestBoard(context: any, { pagination }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/AgencyRequest/Board?PageSize=${pagination.size}&PageIndex=${pagination.page}`
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateAgencyRequest(context: any, { requestId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyRequest/${requestId}`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateAgencyRequestIsAsap(context: any, requestId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyRequest/${requestId}/IsAsap`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateAgencyPunchCardVisibilityStatusInApp(context: any, requestId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyRequest/${requestId}/PunchCardVisibilityStatusInApp`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateAgencyWorkerContractor(context: any, id: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyWorkerProfile/${id}/IsContractor`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateAgencyWorkerSubContractor(context: any, id: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyWorkerProfile/${id}/IsSubcontractor`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateWorkerProfileTaxCategory(context: any, payload: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyWorkerProfile/${payload.id}/tax-category`, payload)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateWorkerProfileTaxRate(context: any, payload: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyWorkerProfile/${payload.id}/tax-rate`, payload)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateWorkerProfileExternalId(context: any, payload: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyWorkerProfile/${payload.id}/ExternalId`, payload)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAgencyWorkerOtherDocuments(context: any, id: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/AgencyWorkerProfile/${id}/OtherDocument`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    agencyRequestOpen(context: any, id: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyRequest/${id}/Open`, id)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateAgencyWorkerEmail(context: any, { workerProfileId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/AgencyWorkerProfile/${workerProfileId}/Email`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateAgencyCompanyEmail(context: any, { companyProfileId, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/v2/AgencyCompanyProfile/${companyProfileId}/Email`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    agencyRequestSendInvitation(context: any, requestId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/AgencyRequest/${requestId}/SendInvitation`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAgencyLocation(): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get("/api/Agency/Location")
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    createAgencyLocation(context: any, model: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post("/api/Agency/Location", model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateAgencyLocation(context: any, { id, model }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/Agency/Location/${id}`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteAgencyLocation(context: any, id: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/Agency/Location/${id}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getPersonnelAgency(context: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get("/api/PersonnelAgency")
          .then((response: any) => {
            context.commit("setPersonnelAgencies", response.data);
            resolve(response.data);
          })
          .catch((error: any) => reject(error.response));
      });
    },
    putPersonnelAgency(context: any, id: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/PersonnelAgency/${id}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updatePermissionToSeeOrders(context: any, { companyId, settings }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .patch(
            `/api/V2/AgencyCompanyProfile/${companyId}/RequiresPermissionToSeeOrders`,
            settings
          )
          .then((response: any) => resolve(response))
          .catch((error: any) => reject(error.response));
      });
    },
    updatePaidHolidays(context: any, { companyId, settings }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .patch(
            `/api/V2/AgencyCompanyProfile/${companyId}/PaidHolidays`,
            settings
          )
          .then((response: any) => resolve(response))
          .catch((error: any) => reject(error.response));
      });
    },
    updateOvertime(context: any, { companyId, settings }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .patch(`/api/V2/AgencyCompanyProfile/${companyId}/Overtime`, settings)
          .then((response: any) => resolve(response))
          .catch((error: any) => reject(error.response));
      });
    },
    getCompanyUsers(context: any, id: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/V2/AgencyCompanyProfile/${id}/CompanyUsers`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateIsAsapRequests(context: any, model: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put("/api/AgencyRequest/is-asap", model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getCompanyProfileUsers(context: any, profileId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/agency-company-profile-user/${profileId}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    createCompanyProfileUser(context: any, payload: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(
            `/api/agency-company-profile-user/${payload.companyId}`,
            payload.user
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteCompanyProfileUser(context: any, payload: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(
            `/api/agency-company-profile-user/${payload.companyId}/users/${payload.userId}`
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAgencyWorkerProfileHolidays(context: any, workerProfileId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/agency-worker-profile-holiday/${workerProfileId}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    addUpdateAgencyWorkerProfileHolidays(context: any, payload: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(
            `/api/agency-worker-profile-holiday/${payload.workerProfileId}`,
            payload.data
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    addNewHoliday(context: any, payload: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/agency-worker-profile-holiday/new-holiday`, payload)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    bulkCandidates(context: any, payload: any): Promise<any> {
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
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    bulkCompanies(context: any, payload: any): Promise<any> {
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
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getInvoices(context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/agency/accounting/Invoices`, {
            params: { ...filter },
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAgencyCompanyProfileWithRequests(_context: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/v2/AgencyCompanyProfile/company-with-requests`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getCompanyProvinceWithTaxes(context: any, companyProfileId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/v2/AgencyCompanyProfile/${companyProfileId}/company-provinces-taxes`
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    previewInvoice(context: any, payload: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/agency/accounting/Invoices/Preview`, payload)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    createInvoice(context: any, payload: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/agency/accounting/Invoices`, payload)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    downloadInvoicePdf(context: any, invoiceId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/v4/Accounting/Invoice/${invoiceId}/Document/PDF`, {
            responseType: "blob",
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getVerificationCode(context: any, invoiceId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/v4/Accounting/Invoice/${invoiceId}/SendVerificationCode`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getPayStubsByInvoice(context: any, invoiceId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/v4/Accounting/Invoice/${invoiceId}/PayStub`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteInvoice(context: any, payload: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/v4/Accounting/Invoice/${payload.invoiceId}`, {
            data: payload,
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getInvoiceRecipients(context: any, companyProfileId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/CompanyProfile/${companyProfileId}/InvoiceRecipient`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    sendInvoiceEmail(context: any, payload: any): Promise<any> {
      return new Promise((resolve, reject) => {
        const formData = new FormData();
        payload.recipients.forEach((recipient: any) =>
          formData.append("cc", recipient)
        );
        formData.append("subject", payload.subject);
        formData.append("message", payload.body);
        payload.attachments.forEach((file: any) => formData.append("files", file));
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
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getPayStubs(context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/agency/accounting/PayStubs`, {
            params: { ...filter },
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    downloadPayStubPdf(context: any, payStubId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/v4/Accounting/PayStub/${payStubId}/Document/PDF`, {
            responseType: "blob",
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deletePayStub(context: any, payStubId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/v4/Accounting/PayStub/${payStubId}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    sendPayStubEmail(context: any, payStubId: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/v4/Accounting/PayStub/${payStubId}/Document/Email`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getWorkersReadyForPayStub(_context: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/agency/accounting/PayStubs/WorkersReadyForPayStub`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    generatePayStubs(context: any, workerIds: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/agency/accounting/PayStubs/generate-v2`, workerIds)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getPayrollSubcontractors(context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/agency/accounting/reports/subcontractors`, {
            params: { ...filter },
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    downloadSubcontractorReport(context: any, weekEnding: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/agency/accounting/reports/subcontractors/file`, {
            params: { weekEnding },
            responseType: "blob",
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getJobPositionsHoursWorked(context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(
            `/api/agency/accounting/reports/${filter.companyId}/job-positions`,
            {
              params: { ...filter },
            }
          )
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getHoursWorkedReport(context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/agency/accounting/reports/hours-worked`, {
            params: { ...filter },
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getT4Report(context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/agency/accounting/reports/t4`, {
            params: { ...filter },
            responseType: "blob",
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getCraPayrollReport(context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/agency/accounting/reports/cra-payroll`, {
            params: { ...filter },
            responseType: "blob",
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getPaymentReport(context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/agency/accounting/reports/payments`, {
            params: { ...filter },
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    downloadWeeklyPayrollReport(context: any, weekEnding: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/agency/accounting/reports/payments/file`, {
            params: { weekEnding },
            responseType: "blob",
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getSkipPayrollNumbers(context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/agency/accounting/PayStubs/skip-payroll-number`, {
            params: { ...filter },
          })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    addSkipPayrollNumber(context: any, payload: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/agency/accounting/PayStubs/skip-payroll-number`, payload)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    createPayStub(context: any, payload: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/v4/accounting/PayStub`, payload)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
  },
};

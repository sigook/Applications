import http from "../../security/apiService";

export default {
  namespaced: true,
  state: {
    companyWorkers: {},
    companyProfile: {},
    companyProfileImage: "",
    companyName: "",
    companyWorker: {},
    companyIsActive: false,
    companyRequestFilter: null
  },
  mutations: {
    setCompanyRequestFilter(state, data) {
      state.companyRequestFilter = data;
    },
    setCompanyWorkers(state, data) {
      state.companyWorkers = data;
    },
    setRequests(state, data) {
      state.requests = data;
    },
    setCompanyProfile(state, data) {
      state.companyProfile = data;
    },
    setCompanyProfileImage(state, data) {
      state.companyProfileImage = data;
    },
    setCompanyName(state, data) {
      state.companyName = data;
    },
    setCompanyWorker(state, data) {
      state.companyWorker = data;
    },
    setCompanyIsActive(state, data) {
      state.companyIsActive = data;
    }
  },
  actions: {
    updateCompanyRequestFilter(context, data) {
      context.commit("setCompanyRequestFilter", data);
    },
    getLocations(context) {
      return new Promise((resolve, reject) => {
        http.get("/api/CompanyLocation")
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getProfileLocations(context) {
      return new Promise((resolve, reject) => {
        http.get(`/api/CompanyProfile/Location`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    createProfileLocation(context,  model) {
      return new Promise((resolve, reject) => {
        http.post(`/api/CompanyProfile/Location`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateProfileLocation(context, { id, model }) {
      return new Promise((resolve, reject) => {
        http.put(`/api/CompanyProfile/Location/${id}`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteProfileLocation(context, { id }) {
      return new Promise((resolve, reject) => {
        http.delete(`/api/CompanyProfile/Location/${id}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getCompanyJobPositions(context) {
      return new Promise((resolve, reject) => {
        http.get("/api/CompanyJobPosition")
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getCompanyJobPositionById(context, id) {
      return new Promise((resolve, reject) => {
        http.get(`/api/CompanyJobPosition/${id}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    createRequest(context, request) {
      return new Promise((resolve, reject) => {
        http
          .post("/api/CompanyRequest", request)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    //This is action is called dynamically
    companyCommentWorker(context, { id, comment }) {
      return new Promise((resolve, reject) => {
        http.post(`/api/CompanyWorker/${id}/Comment`, comment)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getRequests(context, filter) {
      return new Promise((resolve, reject) => {
        http.get("/api/CompanyRequest", {
          params: { ...filter }
        })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getRequest(context, id) {
      return new Promise((resolve, reject) => {
        http.get("/api/CompanyRequest/" + id)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    cancelRequest(context, { id, cancellationReasonId, otherCancellationReason }) {
      return new Promise((resolve, reject) => {
        http.put(`/api/CompanyRequest/${id}/Cancel`, {
          cancellationReasonId: cancellationReasonId,
          otherCancellationReason: otherCancellationReason,
        })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    editRequest(context, { id, model }) {
      return new Promise((resolve, reject) => {
        http
          .put("/api/CompanyRequest/" + id, model)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    getRequestWorkers(context, filter) {
      return new Promise((resolve, reject) => {
        http.get(`/api/CompanyRequest/${filter.requestId}/Worker`, {
          params: { ...filter }
        })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getRequestWorker(context, { requestId, workerId }) {
      return new Promise((resolve, reject) => {
        http.get(`/api/CompanyRequest/${requestId}/Worker/${workerId}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getCompanyWorkerTimeSheetByDate(context, { requestId, workerId, date }) {
      return new Promise((resolve, reject) => {
        http.get(`/api/v2/CompanyRequest/${requestId}/Worker/${workerId}/TimeSheet`, {
          params: { ...date }
        })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    validateAllHoursTimeSheet(context, { requestId, workerId }) {
      return new Promise((resolve, reject) => {
        http.put(`/api/v2/CompanyRequest/${requestId}/Worker/${workerId}/TimeSheet`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    validateHoursTimeSheet(context, { requestId, workerId, id, model }) {
      return new Promise((resolve, reject) => {
        http.put(`/api/v2/CompanyRequest/${requestId}/Worker/${workerId}/TimeSheet/${id}`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    postCompanyWorkerTimeSheet(context, { requestId, workerId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post(
            "/api/v2/CompanyRequest/" +
            requestId +
            "/Worker/" +
            workerId +
            "/TimeSheet",
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
    deleteCompanyWorkerTimeSheet(context, { requestId, workerId, id }) {
      return new Promise((resolve, reject) => {
        http
          .delete(
            "/api/v2/CompanyRequest/" +
            requestId +
            "/Worker/" +
            workerId +
            "/TimeSheet/" +
            id
          )
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    getProfile(context, id) {
      return new Promise((resolve, reject) => {
        http.get(`/api/CompanyProfile`)
          .then((response) => {
            context.commit("setCompanyProfile", response.data);
            context.commit("setCompanyProfileImage", response.data.logo.pathFile);
            context.commit("setCompanyName", response.data.fullName);
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    updateProfile(context, { id: id, company: company }) {
      return new Promise((resolve, reject) => {
        http.put(`/api/CompanyProfile/${id}`, company)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    RequestAnotherWorker(context, { requestId, comment }) {
      return new Promise((resolve, reject) => {
        http.post(`/api/CompanyRequest/${requestId}/Worker/RequestNewWorker`, comment)
          .then((response) => resolve(response))
          .catch((error) => reject(error.response));
      });
    },
    registerCompany(context, company) {
      return new Promise((resolve, reject) => {
        http.post("/api/CompanyProfile", company)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getCompanyInvoice(context, filter) {
      return new Promise((resolve, reject) => {
        http.get("/api/CompanyInvoice", {
          params: { ...filter }
        }).then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    rejectCompanyRequestWorker(context, { requestId, workerId, model }) {
      return new Promise((resolve, reject) => {
        http
          .put(
            "/api/CompanyRequest/" +
            requestId +
            "/Worker/" +
            workerId +
            "/Reject",
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
    getCompanyInvoiceDetail(context, requestId) {
      return new Promise((resolve, reject) => {
        http.get(`/api/CompanyInvoice/${requestId}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    companyTimeSheetClockIn(context, { requestId, workerId, model }) {
      return new Promise((resolve, reject) => {
        http.post(`/api/v2/CompanyRequest/${requestId}/Worker/${workerId}/TimeSheet/ClockIn`, model)
          .then((r) => resolve(r.data))
          .catch((r) => reject(r.response));
      });
    },
    createCompanyUser(context, model) {
      return new Promise((resolve, reject) => {
        http.post("/api/CompanyUser", model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getCompanyUser(context) {
      return new Promise((resolve, reject) => {
        http.get(`/api/CompanyUser`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getCompanyUserDetail(context) {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/CompanyUser/detail`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateCompanyUser(context, { id: id, user: user }) {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/CompanyUser/${id}`, user)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteCompanyUser(context, id) {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/CompanyUser/${id}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    requestNewPosition(context, data) {
      return new Promise((resolve, reject) => {
        http.post("/api/CompanyJobPosition/request-new-position", data)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    contactPeople(context) {
      return new Promise((resolve, reject) => {
        http.get(`/api/CompanyProfileContactPerson`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteContactPerson(context, id) {
      return new Promise((resolve, reject) => {
        http.delete(`/api/CompanyProfileContactPerson/${id}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      })
    },
    saveContactPerson(context, model) {
      return new Promise((resolve, reject) => {
        http.post(`/api/CompanyProfileContactPerson`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    updateCompanyRequestWorkerTimeSheet(context, { requestId, workerId, id, model }) {
      return new Promise((resolve, reject) => {
        http.put(`/api/v2/CompanyRequest/${requestId}/Worker/${workerId}/TimeSheet/${id}`, model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    }
  },
};

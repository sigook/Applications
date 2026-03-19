import http from "../../security/apiService";
import { CompanyProfile } from "@/types/company";

interface CompanyState {
  companyWorkers: any;
  companyProfile: Partial<CompanyProfile>;
  companyProfileImage: string;
  companyName: string;
  companyWorker: any;
  companyIsActive: boolean;
  companyRequestFilter: any;
  requests?: any;
}

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
  } as CompanyState,
  mutations: {
    setCompanyRequestFilter(state: CompanyState, data: any) {
      state.companyRequestFilter = data;
    },
    setCompanyWorkers(state: CompanyState, data: any) {
      state.companyWorkers = data;
    },
    setRequests(state: CompanyState, data: any) {
      state.requests = data;
    },
    setCompanyProfile(state: CompanyState, data: any) {
      state.companyProfile = data;
    },
    setCompanyProfileImage(state: CompanyState, data: string) {
      state.companyProfileImage = data;
    },
    setCompanyName(state: CompanyState, data: string) {
      state.companyName = data;
    },
    setCompanyWorker(state: CompanyState, data: any) {
      state.companyWorker = data;
    },
    setCompanyIsActive(state: CompanyState, data: boolean) {
      state.companyIsActive = data;
    }
  },
  actions: {
    updateCompanyRequestFilter(context: any, data: any): void {
      context.commit("setCompanyRequestFilter", data);
    },
    getLocations(_context: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get("/api/CompanyLocation")
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getProfileLocations(_context: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get(`/api/CompanyProfile/Location`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    createProfileLocation(context: any, model: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.post(`/api/CompanyProfile/Location`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateProfileLocation(context: any, { id, model }: { id: string; model: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http.put(`/api/CompanyProfile/Location/${id}`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteProfileLocation(context: any, { id }: { id: string }): Promise<any> {
      return new Promise((resolve, reject) => {
        http.delete(`/api/CompanyProfile/Location/${id}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getCompanyJobPositions(_context: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get("/api/CompanyJobPosition")
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getCompanyJobPositionById(context: any, id: string): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get(`/api/CompanyJobPosition/${id}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    createRequest(context: any, request: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post("/api/CompanyRequest", request)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    //This is action is called dynamically
    companyCommentWorker(context: any, { id, comment }: { id: string; comment: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http.post(`/api/CompanyWorker/${id}/Comment`, comment)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getRequests(context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get("/api/CompanyRequest", {
          params: { ...filter }
        })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getRequest(context: any, id: string): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get("/api/CompanyRequest/" + id)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    cancelRequest(context: any, { id, cancellationReasonId, otherCancellationReason }: { id: string; cancellationReasonId: string; otherCancellationReason: string }): Promise<any> {
      return new Promise((resolve, reject) => {
        http.put(`/api/CompanyRequest/${id}/Cancel`, {
          cancellationReasonId: cancellationReasonId,
          otherCancellationReason: otherCancellationReason,
        })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    editRequest(context: any, { id, model }: { id: string; model: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put("/api/CompanyRequest/" + id, model)
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    getRequestWorkers(context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get(`/api/CompanyRequest/${filter.requestId}/Worker`, {
          params: { ...filter }
        })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getRequestWorker(context: any, { requestId, workerId }: { requestId: string; workerId: string }): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get(`/api/CompanyRequest/${requestId}/Worker/${workerId}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getCompanyWorkerTimeSheetByDate(context: any, { requestId, workerId, date }: { requestId: string; workerId: string; date: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get(`/api/v2/CompanyRequest/${requestId}/Worker/${workerId}/TimeSheet`, {
          params: { ...date }
        })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    validateAllHoursTimeSheet(context: any, { requestId, workerId }: { requestId: string; workerId: string }): Promise<any> {
      return new Promise((resolve, reject) => {
        http.put(`/api/v2/CompanyRequest/${requestId}/Worker/${workerId}/TimeSheet`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    validateHoursTimeSheet(context: any, { requestId, workerId, id, model }: { requestId: string; workerId: string; id: string; model: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http.put(`/api/v2/CompanyRequest/${requestId}/Worker/${workerId}/TimeSheet/${id}`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    postCompanyWorkerTimeSheet(context: any, { requestId, workerId, model }: { requestId: string; workerId: string; model: any }): Promise<any> {
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
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    deleteCompanyWorkerTimeSheet(context: any, { requestId, workerId, id }: { requestId: string; workerId: string; id: string }): Promise<any> {
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
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    getProfile(context: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get(`/api/CompanyProfile`)
          .then((response: any) => {
            context.commit("setCompanyProfile", response.data);
            context.commit("setCompanyProfileImage", response.data.logo.pathFile);
            context.commit("setCompanyName", response.data.fullName);
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    updateProfile(context: any, { id, company }: { id: string; company: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http.put(`/api/CompanyProfile/${id}`, company)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    RequestAnotherWorker(context: any, { requestId, comment }: { requestId: string; comment: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http.post(`/api/CompanyRequest/${requestId}/Worker/RequestNewWorker`, comment)
          .then((response: any) => resolve(response))
          .catch((error: any) => reject(error.response));
      });
    },
    registerCompany(context: any, company: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.post("/api/CompanyProfile", company)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getCompanyInvoice(context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get("/api/CompanyInvoice", {
          params: { ...filter }
        }).then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    rejectCompanyRequestWorker(context: any, { requestId, workerId, model }: { requestId: string; workerId: string; model: any }): Promise<any> {
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
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    getCompanyInvoiceDetail(context: any, requestId: string): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get(`/api/CompanyInvoice/${requestId}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    companyTimeSheetClockIn(context: any, { requestId, workerId, model }: { requestId: string; workerId: string; model: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http.post(`/api/v2/CompanyRequest/${requestId}/Worker/${workerId}/TimeSheet/ClockIn`, model)
          .then((r: any) => resolve(r.data))
          .catch((r: any) => reject(r.response));
      });
    },
    createCompanyUser(context: any, model: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.post("/api/CompanyUser", model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getCompanyUser(_context: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get(`/api/CompanyUser`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getCompanyUserDetail(_context: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/CompanyUser/detail`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateCompanyUser(context: any, { id, user }: { id: string; user: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(`/api/CompanyUser/${id}`, user)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteCompanyUser(context: any, id: string): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete(`/api/CompanyUser/${id}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    requestNewPosition(context: any, data: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.post("/api/CompanyJobPosition/request-new-position", data)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    contactPeople(_context: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get(`/api/CompanyProfileContactPerson`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteContactPerson(context: any, id: string): Promise<any> {
      return new Promise((resolve, reject) => {
        http.delete(`/api/CompanyProfileContactPerson/${id}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      })
    },
    saveContactPerson(context: any, model: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.post(`/api/CompanyProfileContactPerson`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    updateCompanyRequestWorkerTimeSheet(context: any, { requestId, workerId, id, model }: { requestId: string; workerId: string; id: string; model: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http.put(`/api/v2/CompanyRequest/${requestId}/Worker/${workerId}/TimeSheet/${id}`, model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    }
  },
};

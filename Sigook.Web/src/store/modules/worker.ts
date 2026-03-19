import http from "../../security/apiService";
import { WorkerProfile, WorkerBasicInfo } from "@/types/worker";

interface WorkerState {
  workerComments: any;
  workerProfiles: any;
  workerBasic: WorkerBasicInfo;
  workerProfile: Partial<WorkerProfile>;
  workerProfileImage: string;
  workerName: string;
  profileSelected: any;
}

export default {
  namespaced: true,
  state: {
    workerComments: {},
    workerProfiles: {},
    workerBasic: {
      approvedToWork: true,
      hasSocialInsurance: true,
      hasSocialInsuranceFile: true,
      hasIdentificationType1File: true,
      hasIdentificationNumber1: true,
      hasIdentificationType2File: true,
      hasIdentificationNumber2: true,
      hasResume: true,
    },
    workerProfile: {},
    workerProfileImage: "",
    workerName: "",
    profileSelected: {}
  } as WorkerState,
  mutations: {
    setWokerComments(state: WorkerState, data: any) {
      state.workerComments = data;
    },
    setWorkerProfiles(state: WorkerState, data: any) {
      state.workerProfiles = data;
    },
    setWorkerProfile(state: WorkerState, data: any) {
      state.workerProfile = data;
    },
    setWorkerBasicInfo(state: WorkerState, data: any) {
      state.workerBasic = data;
    },
    setWorkerProfileImage(state: WorkerState, data: string) {
      state.workerProfileImage = data;
    },
    setWorkerName(state: WorkerState, data: string) {
      state.workerName = data;
    },
    setProfileSelected(state: WorkerState, data: any) {
      state.profileSelected = data;
    }
  },
  actions: {
    getJobs(context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get("/api/WorkerRequest", {
          params: { ...filter }
        })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getWorkerRequest(context: any, id: string): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get("/api/WorkerRequest/" + id)
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    WorkerRequestApply(context: any, { requestId, model }: { requestId: string; model: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/WorkerRequest/${requestId}/Apply/`, model)
          .then((response: any) => {
            resolve(response);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    workerRequestApply(context: any, { workerId, requestId, model }: { workerId: string; requestId: string; model: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/WorkerRequest/${workerId}/${requestId}/Apply`, model)
          .then((response: any) => {
            resolve(response);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    workerRequestDecline(context: any, id: string): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete("/api/WorkerRequest/Decline/" + id)
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    workerRegisterTime(context: any, { requestId, latitude, longitude }: { requestId: string; latitude: number; longitude: number }): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/WorkerRequest/${requestId}/TimeSheet`, { latitude: latitude, longitude: longitude })
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    workerGetTimeSheet(context: any, requestId: string): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get(`/api/WorkerRequest/${requestId}/TimeSheet`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getCommentsWorker(context: any, { workerId, size, pageIndex }: { workerId: string; size: number; pageIndex: number }): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get(`/api/worker/${workerId}/comment?PageSize=${size}&PageIndex=${pageIndex}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getProfiles(context: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get("/api/WorkerProfile")
          .then((response: any) => {
            context.commit("setWorkerProfiles", response.data);
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    getProfile(context: any, id: string): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get("/api/WorkerProfile/" + id)
          .then((response: any) => {
            context.commit("setWorkerProfile", response.data);
            context.commit("setWorkerProfileImage", response.data.profileImage.pathFile);
            context.commit("setWorkerName", response.data.firstName + " " + response.data.lastName);
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    getProfileBasicInfo(context: any, id: string): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get("/api/WorkerProfile/" + id + "/BasicInfo")
          .then((response: any) => {
            context.commit("setWorkerBasicInfo", response.data);
            if (response.data.profileImage) {
              context.commit("setWorkerProfileImage", response.data.profileImage.pathFile);
            }
            context.commit("setWorkerName", response.data.firstName + " " + response.data.lastName);
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    registerWorker(context: any, payload: FormData): Promise<any> {
      return new Promise((resolve, reject) => {
        const url = `/api/WorkerProfile`;
        http.post(url, payload, {
          headers: { 'Content-Type': 'multipart/form-data' }
        })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    uploadWorker(context: any, { profileId, worker }: { profileId: string; worker: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http.put("/api/WorkerProfile/" + profileId, worker)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getWorkerRequestHistory(context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get("/api/WorkerRequestHistory", {
          params: { ...filter }
        })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getWorkerRequestHistoryDetail(context: any, id: string): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .get("/api/WorkerRequestHistory/" + id)
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    createWorkerWorkExperience(context: any, { id, model }: { id: string; model: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post("/api/WorkerProfile/" + id + "/JobExperience", model)
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    editWorkerWorkExperience(context: any, { profileId, id, model }: { profileId: string; id: string; model: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .put(
            "/api/WorkerProfile/" + profileId + "/JobExperience/" + id,
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
    deleteWorkerWorkExperience(context: any, { profileId, id }: { profileId: string; id: string }): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .delete("/api/WorkerProfile/" + profileId + "/JobExperience/" + id)
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    createWorkerSin(context: any, { profileId, formData }: { profileId: string; formData: FormData }): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/WorkerProfile/${profileId}/SinInformation`, formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
          })
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    createWorkerBasicInformation(context: any, { profileId, model }: { profileId: string; model: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post("/api/WorkerProfile/" + profileId + "/BasicInformation", model)
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    createWorkerEmergencyInformation(context: any, { profileId, model }: { profileId: string; model: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(
            "/api/WorkerProfile/" + profileId + "/EmergencyInformation",
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
    createWorkerDocuments(context: any, { profileId, formData }: { profileId: string; formData: FormData }): Promise<any> {
      return new Promise((resolve, reject) => {
        http.post(`/api/WorkerProfile/${profileId}/Documents`, formData, {
          headers: { 'Content-Type': 'multipart/form-data' }
        })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    createWorkerResume(context: any, { profileId, formData }: { profileId: string; formData: FormData }): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/WorkerProfile/${profileId}/Resume`, formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
          })
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    createWorkerContactInformation(context: any, { profileId, model }: { profileId: string; model: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(
            "/api/WorkerProfile/" + profileId + "/ContactInformation",
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
    createWorkerAvailabilities(context: any, { profileId, model }: { profileId: string; model: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post("/api/WorkerProfile/" + profileId + "/Availabilities", model)
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    createWorkerAvailabilityTimes(context: any, { profileId, model }: { profileId: string; model: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post("/api/WorkerProfile/" + profileId + "/AvailabilityTimes", model)
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    createWorkerAvailabilityDays(context: any, { profileId, model }: { profileId: string; model: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http.post("/api/WorkerProfile/" + profileId + "/AvailabilityDays", model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    createWorkerLocationPreferences(context: any, { profileId, model }: { profileId: string; model: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http.post("/api/WorkerProfile/" + profileId + "/LocationPreferences", model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    createWorkerLanguages(context: any, { profileId, model }: { profileId: string; model: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http.post("/api/WorkerProfile/" + profileId + "/Languages", model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    createWorkerOther(context: any, { profileId, model }: { profileId: string; model: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http.post("/api/WorkerProfile/" + profileId + "/OtherInformation", model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    createWorkerSkills(context: any, { profileId, model }: { profileId: string; model: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http.post("/api/WorkerProfile/" + profileId + "/Skills", model)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    createWorkerLicenses(context: any, { profileId, formData }: { profileId: string; formData: FormData }): Promise<any> {
      return new Promise((resolve, reject) => {
        http.post(`/api/WorkerProfile/${profileId}/Licenses`, formData, {
          headers: { 'Content-Type': 'multipart/form-data' }
        })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    deleteWorkerLicenses(context: any, { profileId, licenseId }: { profileId: string; licenseId: string }): Promise<any> {
      return new Promise((resolve, reject) => {
        http.delete(`/api/WorkerProfile/${profileId}/Licenses/${licenseId}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      })
    },
    createWorkerCertificates(context: any, { profileId, formData }: { profileId: string; formData: FormData }): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/WorkerProfile/${profileId}/Certificates`, formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
          })
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    deleteWorkerCertificates(context: any, { profileId, certificateId }: { profileId: string; certificateId: string }): Promise<any> {
      return new Promise((resolve, reject) => {
        http.delete(`/api/WorkerProfile/${profileId}/Certificates/${certificateId}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      })
    },
    createWorkerOtherDocuments(context: any, { profileId, formData }: { profileId: string; formData: FormData }): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/WorkerProfile/${profileId}/OtherDocument`, formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
          })
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    deleteWorkerOtherDocuments(context: any, { profileId, otherDocumentId }: { profileId: string; otherDocumentId: string }): Promise<any> {
      return new Promise((resolve, reject) => {
        http.delete(`/api/WorkerProfile/${profileId}/OtherDocument/${otherDocumentId}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      })
    },
    createWorkerImage(context: any, { profileId, formData }: { profileId: string; formData: FormData }): Promise<any> {
      return new Promise((resolve, reject) => {
        http
          .post("/api/WorkerProfile/" + profileId + "/ProfileImage", formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
          })
          .then((response: any) => {
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    getWorkerProfileWageHistory(context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get(`/api/WorkerProfile/${filter.profileId}/WageHistory`, {
          params: { ...filter }
        })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getWorkerProfileWageHistoryAccumulated(context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get(`/api/WorkerProfile/${filter.profileId}/WageHistory/${filter.rowNumber}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getWorkerProfileTimeSheetHistory(context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get(`/api/WorkerProfile/${filter.profileId}/TimeSheetHistory`, {
          params: { ...filter }
        })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getWorkerProfileTimeSheetHistoryAccumulated(context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get(`/api/WorkerProfile/${filter.profileId}/TimeSheetHistory/${filter.rowNumber}`)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getClockType(context: any, { requestId, date }: { requestId: string; date: string }): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get(`/api/WorkerRequest/${requestId}/TimeSheet/clock-type`, {
          params: { date }
        })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    }
  },
};

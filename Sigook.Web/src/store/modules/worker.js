import http from "../../security/apiService";

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
  },
  mutations: {
    setWokerComments(state, data) {
      state.workerComments = data;
    },
    setWorkerProfiles(state, data) {
      state.workerProfiles = data;
    },
    setWorkerProfile(state, data) {
      state.workerProfile = data;
    },
    setWorkerBasicInfo(state, data) {
      state.workerBasic = data;
    },
    setWorkerProfileImage(state, data) {
      state.workerProfileImage = data;
    },
    setWorkerName(state, data) {
      state.workerName = data;
    },
    setProfileSelected(state, data) {
      state.profileSelected = data;
    }
  },
  actions: {
    getJobs(context, filter) {
      return new Promise((resolve, reject) => {
        http.get("/api/WorkerRequest", {
          params: { ...filter }
        })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getWorkerRequest(context, id) {
      return new Promise((resolve, reject) => {
        http
          .get("/api/WorkerRequest/" + id)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    WorkerRequestApply(context, { requestId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/WorkerRequest/${requestId}/Apply/`, model)
          .then((response) => {
            resolve(response);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    workerRequestApply(context, { workerId, requestId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/WorkerRequest/${workerId}/${requestId}/Apply`, model)
          .then((response) => {
            resolve(response);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    workerRequestDecline(context, id) {
      return new Promise((resolve, reject) => {
        http
          .delete("/api/WorkerRequest/Decline/" + id)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    workerRegisterTime(context, { requestId, latitude, longitude }) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/WorkerRequest/${requestId}/TimeSheet`, { latitude: latitude, longitude: longitude })
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    workerGetTimeSheet(context, requestId) {
      return new Promise((resolve, reject) => {
        http.get(`/api/WorkerRequest/${requestId}/TimeSheet`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getCommentsWorker(context, { workerId, size, pageIndex }) {
      return new Promise((resolve, reject) => {
        http
          .get(
            "/api/worker/" +
            workerId +
            "/comment?PageSize=" +
            size +
            "&PageIndex=" +
            pageIndex
          )
          .then((response) => {
            context.commit("setWokerComments", response.data);
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    getProfiles(context) {
      return new Promise((resolve, reject) => {
        http.get("/api/WorkerProfile")
          .then((response) => {
            context.commit("setWorkerProfiles", response.data);
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    getProfile(context, id) {
      return new Promise((resolve, reject) => {
        http.get("/api/WorkerProfile/" + id)
          .then((response) => {
            context.commit("setWorkerProfile", response.data);
            context.commit("setWorkerProfileImage", response.data.profileImage.pathFile);
            context.commit("setWorkerName", response.data.firstName + " " + response.data.lastName);
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    getProfileBasicInfo(context, id) {
      return new Promise((resolve, reject) => {
        http
          .get("/api/WorkerProfile/" + id + "/BasicInfo")
          .then((response) => {
            context.commit("setWorkerBasicInfo", response.data);
            if (response.data.profileImage) {
              context.commit("setWorkerProfileImage", response.data.profileImage.pathFile);
            }
            context.commit("setWorkerName", response.data.firstName + " " + response.data.lastName);
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    registerWorker(context, payload) {
      return new Promise((resolve, reject) => {
        const url = `/api/WorkerProfile`;
        http.post(url, payload, {
          headers: { 'Content-Type': 'multipart/form-data' }
        })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    uploadWorker(context, { profileId, worker }) {
      return new Promise((resolve, reject) => {
        http.put("/api/WorkerProfile/" + profileId, worker)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getWorkerRequestHistory(context, filter) {
      return new Promise((resolve, reject) => {
        http.get("/api/WorkerRequestHistory", {
          params: { ...filter }
        })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getWorkerRequestHistoryDetail(context, id) {
      return new Promise((resolve, reject) => {
        http
          .get("/api/WorkerRequestHistory/" + id)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    createWorkerWorkExperience(context, { id, model }) {
      return new Promise((resolve, reject) => {
        http
          .post("/api/WorkerProfile/" + id + "/JobExperience", model)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    editWorkerWorkExperience(context, { profileId, id, model }) {
      return new Promise((resolve, reject) => {
        http
          .put(
            "/api/WorkerProfile/" + profileId + "/JobExperience/" + id,
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
    deleteWorkerWorkExperience(context, { profileId, id }) {
      return new Promise((resolve, reject) => {
        http
          .delete("/api/WorkerProfile/" + profileId + "/JobExperience/" + id)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    createWorkerSin(context, { profileId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post("/api/WorkerProfile/" + profileId + "/SinInformation", model)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    createWorkerBasicInformation(context, { profileId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post("/api/WorkerProfile/" + profileId + "/BasicInformation", model)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    createWorkerEmergencyInformation(context, { profileId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post(
            "/api/WorkerProfile/" + profileId + "/EmergencyInformation",
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
    createWorkerDocuments(context, { profileId, model }) {
      return new Promise((resolve, reject) => {
        http.post("/api/WorkerProfile/" + profileId + "/Documents", model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    createWorkerResume(context, { profileId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post("/api/WorkerProfile/" + profileId + "/Resume", model)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    createWorkerContactInformation(context, { profileId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post(
            "/api/WorkerProfile/" + profileId + "/ContactInformation",
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
    createWorkerAvailabilities(context, { profileId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post("/api/WorkerProfile/" + profileId + "/Availabilities", model)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    createWorkerAvailabilityTimes(context, { profileId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post("/api/WorkerProfile/" + profileId + "/AvailabilityTimes", model)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    createWorkerAvailabilityDays(context, { profileId, model }) {
      return new Promise((resolve, reject) => {
        http.post("/api/WorkerProfile/" + profileId + "/AvailabilityDays", model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    createWorkerLocationPreferences(context, { profileId, model }) {
      return new Promise((resolve, reject) => {
        http.post("/api/WorkerProfile/" + profileId + "/LocationPreferences", model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    createWorkerLanguages(context, { profileId, model }) {
      return new Promise((resolve, reject) => {
        http.post("/api/WorkerProfile/" + profileId + "/Languages", model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    createWorkerOther(context, { profileId, model }) {
      return new Promise((resolve, reject) => {
        http.post("/api/WorkerProfile/" + profileId + "/OtherInformation", model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    createWorkerSkills(context, { profileId, model }) {
      return new Promise((resolve, reject) => {
        http.post("/api/WorkerProfile/" + profileId + "/Skills", model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    createWorkerLicenses(context, { profileId, model }) {
      return new Promise((resolve, reject) => {
        http.post("/api/WorkerProfile/" + profileId + "/Licenses", model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    deleteWorkerLicenses(context, { profileId, licenseId }) {
      return new Promise((resolve, reject) => {
        http.delete(`/api/WorkerProfile/${profileId}/Licenses/${licenseId}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      })
    },
    createWorkerCertificates(context, { profileId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post("/api/WorkerProfile/" + profileId + "/Certificates", model)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    deleteWorkerCertificates(context, { profileId, certificateId }) {
      return new Promise((resolve, reject) => {
        http.delete(`/api/WorkerProfile/${profileId}/Certificates/${certificateId}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      })
    },
    createWorkerOtherDocuments(context, { profileId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post(`/api/WorkerProfile/${profileId}/OtherDocument`, model)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    deleteWorkerOtherDocuments(context, { profileId, otherDocumentId }) {
      return new Promise((resolve, reject) => {
        http.delete(`/api/WorkerProfile/${profileId}/OtherDocument/${otherDocumentId}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      })
    },
    createWorkerImage(context, { profileId, model }) {
      return new Promise((resolve, reject) => {
        http
          .post("/api/WorkerProfile/" + profileId + "/ProfileImage", model)
          .then((response) => {
            resolve(response.data);
          })
          .catch((error) => {
            reject(error.response);
          });
      });
    },
    getWorkerProfileWageHistory(context, filter) {
      return new Promise((resolve, reject) => {
        http.get(`/api/WorkerProfile/${filter.profileId}/WageHistory`, {
          params: { ...filter }
        })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getWorkerProfileWageHistoryAccumulated(context, filter) {
      return new Promise((resolve, reject) => {
        http.get(`/api/WorkerProfile/${filter.profileId}/WageHistory/${filter.rowNumber}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getWorkerProfileTimeSheetHistory(context, filter) {
      return new Promise((resolve, reject) => {
        http.get(`/api/WorkerProfile/${filter.profileId}/TimeSheetHistory`, {
          params: { ...filter }
        })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getWorkerProfileTimeSheetHistoryAccumulated(context, filter) {
      return new Promise((resolve, reject) => {
        http.get(`/api/WorkerProfile/${filter.profileId}/TimeSheetHistory/${filter.rowNumber}`)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getClockType(context, { requestId, date }) {
      return new Promise((resolve, reject) => {
        http.get(`/api/WorkerRequest/${requestId}/TimeSheet/clock-type`, {
          params: { date }
        })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    }
  },
};

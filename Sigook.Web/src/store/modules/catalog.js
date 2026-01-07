import http from "../../security/apiService";
import axios from 'axios';

export default {
  state: {
    countries: {},
    provinces: [],
    cities: [],
    wsibgroups: {},
    industries: {},
    subIndustries: {},
    jobPositions: {},
    maximumHoursPerDay: process.env.VUE_APP_MAXIMUM_HOURS_DAY,
    residencyList: ['Citizen', 'Work Permit', 'Student', 'Permanent Resident'],
    sourceList: [
      "Google Search",
      "Online Advert",
      "Friend Recommendation",
      "Zip Recruiter",
      "Glassdoor",
      "Indeed",
      "Linkedin",
      "Kijiji",
      "Email",
      "Other"
    ]
  },
  mutations: {
    updateCountries(state, countries) {
      state.countries = countries;
    },
    updateProvinces(state, provinces) {
      state.provinces = provinces;
    },
    updateCities(state, cities) {
      state.cities = cities;
    },
    updateWsibGroups(state, wsibgroups) {
      state.wsibgroups = wsibgroups;
    },
    updateAvailabilityTimes(state, times) {
      state.availabilityTimes = times;
    },
    updateIndustries(state, data) {
      state.industries = data;
    },
    updateSubindustries(state, data) {
      state.subIndustries = data;
    },
    updateJobPositions(state, data) {
      state.jobPositions = data;
    },
  },
  actions: {
    getCountries({ commit }) {
      return new Promise((resolve, reject) => {
        http.get('api/Location/country')
          .then(result => {
            commit('updateCountries', result.data);
            resolve(result.data);
          })
          .catch(error => reject(error.response));
      });
    },
    getProvinces({ commit }, countryId) {
      return new Promise((resolve, reject) => {
        http.get(`api/Location/province/${countryId}`)
          .then(result => {
            commit('updateProvinces', result.data);
            resolve(result.data);
          })
          .catch(error => {
            reject(error.response);
          });
      });

    },
    getCities({ commit }, provinceId) {
      return new Promise((resolve, reject) => {
        http.get(`api/Location/city/${provinceId}`)
          .then(result => {
            commit('updateCities', result.data);
            resolve(result.data);
          })
          .catch(error => {
            reject(error.response);
          });
      });

    },
    getWsibGroups({ commit }) {
      return new Promise((resolve, reject) => {
        http.get('api/Catalog/wsibgroup')
          .then(response => {
            commit('updateWsibGroups', response.data);
            resolve(response.data);
          })
          .catch(error => {
            reject(error.response);
          });
      });
    },
    getGenders({ commit }) {
      return new Promise((resolve, reject) => {
        http.get('/api/Catalog/gender')
          .then(result => resolve(result.data))
          .catch(error => reject(error.response));
      });
    },
    getIdentificationTypes({ commit }) {
      return new Promise((resolve, reject) => {
        http.get('/api/Catalog/identificationType')
          .then(response => resolve(response.data))
          .catch(error => reject(error.response));
      });
    },
    getAvailability({ commit }) {
      return new Promise((resolve, reject) => {
        http.get('/api/Catalog/availability')
          .then(response => resolve(response.data))
          .catch(error => reject(error.response));
      });
    },
    getAvailabilityTimes({ commit }) {
      return new Promise((resolve, reject) => {
        http.get('/api/Catalog/availabilityTime')
          .then(response => resolve(response.data))
          .catch(error => reject(error.response));
      });
    },
    getDays({ commit }) {
      return new Promise((resolve, reject) => {
        http.get('/api/Catalog/day')
          .then(response => resolve(response.data))
          .catch(error => reject(error.response));
      });
    },
    getLifts({ commit }) {
      return new Promise((resolve, reject) => {
        http.get('/api/Catalog/lift')
          .then(response => resolve(response.data))
          .catch(error => reject(error.response));
      });
    },
    getLanguages({ commit }) {
      return new Promise((resolve, reject) => {
        http.get('/api/Catalog/language')
          .then(response => resolve(response.data))
          .catch(error => { reject(error.response); });
      });
    },
    getJobPositions({ commit }) {
      return new Promise((resolve, reject) => {
        http.get('/api/Catalog/jobPosition')
          .then(response => {
            commit('updateJobPositions', response.data);
            resolve(response.data);
          })
          .catch(error => {
            reject(error);
          });
      });
    },
    getReasonCancellationRequest({ commit }) {
      return new Promise((resolve, reject) => {
        http.get('/api/Catalog/reasonCancellationRequest')
          .then(response => resolve(response.data))
          .catch(error => reject(error.response));
      });
    },
    getSkills({ commit }) {
      return new Promise((resolve, reject) => {
        http.get('api/Catalog/skills')
          .then(response => resolve(response.data))
          .catch(error => reject(error.response));
      });
    },
    getCompanyIndustry() {
      return new Promise((resolve, reject) => {
        http.get("/api/Catalog/industry")
          .then(response => resolve(response.data))
          .catch(error => reject(error.response));
      });
    },
    getCompanyStatus() {
      return new Promise((resolve, reject) => {
        http.get("/api/Catalog/companyStatus")
          .then(response => resolve(response.data))
          .catch(error => reject(error.response));
      })
    },
    downloadInvoicePdf(context, { invoiceId }) {
      return new Promise((resolve, reject) => {
        http.get("/api/Invoice/" + invoiceId + "/Document/PDF", { responseType: 'blob' })
          .then(response => resolve(response.data))
          .catch(error => reject(error.response));
      });
    },
    downloadPayrollSubcontractor(context, { weekEnding }) {
      return new Promise((resolve, reject) => {
        http.get('/api/PayrollSubcontractor/' + weekEnding + '/Document/EXCEL', { responseType: 'blob' })
          .then(response => resolve(response.data))
          .catch(error => reject(error.response));
      });
    },
    downloadWeeklyPayrollExcel(context, { weekEnding }) {
      return new Promise((resolve, reject) => {
        http.get(`/api/WeeklyPayroll/${weekEnding}/Document/EXCEL`, { responseType: 'blob' })
          .then(response => resolve(response.data))
          .catch(error => reject(error.response));
      });
    },
    downloadWeeklyPayrollExcelByWeekEnding(context, { date }) {
      return new Promise((resolve, reject) => {
        http.get(`/api/WeeklyPayroll/${date}/Document/EXCEL/ByWeekEnding`, { responseType: 'blob' })
          .then(response => resolve(response.data))
          .catch(error => reject(error.response));
      });
    },
    downloadWeeklyPayrollExcelByPaymentDate(context, { date }) {
      return new Promise((resolve, reject) => {
        http.get(`/api/WeeklyPayroll/${date}/Document/EXCEL/ByPaymentDate`, { responseType: 'blob' })
          .then(response => resolve(response.data))
          .catch(error => reject(error.response));
      });
    },
    getLasVersion() {
      return new Promise((resolve, reject) => {
        axios.get("/version.json")
          .then(response => resolve(response.data))
          .catch(error => reject(error.response));
      });
    },
    getRequestShift(context, requestId) {
      return new Promise((resolve, reject) => {
        http.get(`/api/Request/${requestId}/Shift`)
          .then(result => resolve(result.data))
          .catch(error => reject(error.response));
      });
    },
    getQRCode(context, text) {
      return new Promise((resolve, reject) => {
        http.get(`/api/QrCode/${text}`, { responseType: 'blob' })
          .then(result => resolve(result.data))
          .catch(error => reject(error.response));
      });
    },
    getJobs(context, filter) {
      return new Promise((resolve, reject) => {
        filter.countries = ['USA', 'CA'];
        http.get(`/api/WebSite/jobs`, {
          params: { ...filter }
        }).then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getLandingJobPositions() {
      return new Promise((resolve, reject) => {
        axios.get("/data/job-positions.json")
          .then(response => resolve(response.data))
          .catch(error => reject(error.response));
      });
    },
    sendForm(context, contact) {
      return new Promise((resolve, reject) => {
        http.post('/api/WebSite/contact', contact)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      })
    },
    createCandidate(context, formData) {
      return new Promise((resolve, reject) => {
        http.post('/api/website/candidate', formData, {
          headers: { 'Content-Type': 'multipart/form-data' }
        })
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response))
      });
    },
    getTaxCategories() {
      return new Promise((resolve, reject) => {
        http.get('/api/catalog/tax-categories')
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response))
      });
    },
    addCity(context, city) {
      return new Promise((resolve, reject) => {
        http.post(`/api/Location/city`, city)
          .then(response => resolve(response.data))
          .catch((error) => reject(error.response))
      });
    },
    addIndustry(context, industry) {
      return new Promise((resolve, reject) => {
        http.post('/api/catalog/industry', industry)
          .then(response => resolve(response.data))
          .catch((error) => reject(error.response))
      });
    },
    addProvinceSetting(context, provinceSetting) {
      return new Promise((resolve, reject) => {
        http.post(`/api/Location/province/${provinceSetting.provinceId}/settings`, provinceSetting.settings)
          .then(response => resolve(response.data))
          .catch((error) => reject(error.response))
      });
    }
  }
};
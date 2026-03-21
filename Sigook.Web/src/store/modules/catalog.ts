import http from "../../security/apiService";
import axios from 'axios';
import { Country, Province, City, WsibGroup, Industry, JobPosition } from "../../types/common";

export interface CatalogState {
  countries: Record<string, any>;
  provinces: Province[];
  cities: City[];
  wsibgroups: Record<string, any>;
  industries: Record<string, any>;
  subIndustries: Record<string, any>;
  jobPositions: Record<string, any>;
  maximumHoursPerDay: string | undefined;
  residencyList: string[];
  sourceList: string[];
  availabilityTimes?: any;
}

const catalogModule = {
  state: (): CatalogState => ({
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
  }),
  mutations: {
    updateCountries(state: CatalogState, countries: Country[]) {
      state.countries = countries;
    },
    updateProvinces(state: CatalogState, provinces: Province[]) {
      state.provinces = provinces;
    },
    updateCities(state: CatalogState, cities: City[]) {
      state.cities = cities;
    },
    updateWsibGroups(state: CatalogState, wsibgroups: WsibGroup[]) {
      state.wsibgroups = wsibgroups;
    },
    updateAvailabilityTimes(state: CatalogState, times: any) {
      state.availabilityTimes = times;
    },
    updateIndustries(state: CatalogState, data: Industry[]) {
      state.industries = data;
    },
    updateSubindustries(state: CatalogState, data: any) {
      state.subIndustries = data;
    },
    updateJobPositions(state: CatalogState, data: JobPosition[]) {
      state.jobPositions = data;
    },
  },
  actions: {
    getCountries({ commit }: any): Promise<Country[]> {
      return new Promise((resolve, reject) => {
        http.get('api/Location/country')
          .then((result: any) => {
            commit('updateCountries', result.data);
            resolve(result.data);
          })
          .catch((error: any) => reject(error.response));
      });
    },
    getProvinces({ commit }: any, countryId: string): Promise<Province[]> {
      return new Promise((resolve, reject) => {
        http.get(`api/Location/province/${countryId}`)
          .then((result: any) => {
            commit('updateProvinces', result.data);
            resolve(result.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });

    },
    getCities({ commit }: any, provinceId: string): Promise<City[]> {
      return new Promise((resolve, reject) => {
        http.get(`api/Location/city/${provinceId}`)
          .then((result: any) => {
            commit('updateCities', result.data);
            resolve(result.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });

    },
    getWsibGroups({ commit }: any): Promise<WsibGroup[]> {
      return new Promise((resolve, reject) => {
        http.get('api/Catalog/wsibgroup')
          .then((response: any) => {
            commit('updateWsibGroups', response.data);
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error.response);
          });
      });
    },
    getGenders({ commit: _commit }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get('/api/Catalog/gender')
          .then((result: any) => resolve(result.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getIdentificationTypes({ commit: _commit }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get('/api/Catalog/identificationType')
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAvailability({ commit: _commit }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get('/api/Catalog/availability')
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getAvailabilityTimes({ commit: _commit }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get('/api/Catalog/availabilityTime')
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getDays({ commit: _commit }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get('/api/Catalog/day')
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getLifts({ commit: _commit }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get('/api/Catalog/lift')
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getLanguages({ commit: _commit }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get('/api/Catalog/language')
          .then((response: any) => resolve(response.data))
          .catch((error: any) => { reject(error.response); });
      });
    },
    getJobPositions({ commit }: any): Promise<JobPosition[]> {
      return new Promise((resolve, reject) => {
        http.get('/api/Catalog/jobPosition')
          .then((response: any) => {
            commit('updateJobPositions', response.data);
            resolve(response.data);
          })
          .catch((error: any) => {
            reject(error);
          });
      });
    },
    getReasonCancellationRequest({ commit: _commit }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get('/api/Catalog/reasonCancellationRequest')
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getSkills({ commit: _commit }: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get('api/Catalog/skills')
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getCompanyIndustry(): Promise<Industry[]> {
      return new Promise((resolve, reject) => {
        http.get("/api/Catalog/industry")
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getCompanyStatus(): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get("/api/Catalog/companyStatus")
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      })
    },
    downloadInvoicePdf(_context: any, { invoiceId }: { invoiceId: string }): Promise<Blob> {
      return new Promise((resolve, reject) => {
        http.get("/api/Invoice/" + invoiceId + "/Document/PDF", { responseType: 'blob' })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    downloadPayrollSubcontractor(_context: any, { weekEnding }: { weekEnding: string }): Promise<Blob> {
      return new Promise((resolve, reject) => {
        http.get('/api/PayrollSubcontractor/' + weekEnding + '/Document/EXCEL', { responseType: 'blob' })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    downloadWeeklyPayrollExcel(_context: any, { weekEnding }: { weekEnding: string }): Promise<Blob> {
      return new Promise((resolve, reject) => {
        http.get(`/api/WeeklyPayroll/${weekEnding}/Document/EXCEL`, { responseType: 'blob' })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    downloadWeeklyPayrollExcelByWeekEnding(_context: any, { date }: { date: string }): Promise<Blob> {
      return new Promise((resolve, reject) => {
        http.get(`/api/WeeklyPayroll/${date}/Document/EXCEL/ByWeekEnding`, { responseType: 'blob' })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    downloadWeeklyPayrollExcelByPaymentDate(_context: any, { date }: { date: string }): Promise<Blob> {
      return new Promise((resolve, reject) => {
        http.get(`/api/WeeklyPayroll/${date}/Document/EXCEL/ByPaymentDate`, { responseType: 'blob' })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getLasVersion(): Promise<any> {
      return new Promise((resolve, reject) => {
        axios.get("/version.json")
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getRequestShift(_context: any, requestId: string): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get(`/api/Request/${requestId}/Shift`)
          .then((result: any) => resolve(result.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getQRCode(_context: any, text: string): Promise<Blob> {
      return new Promise((resolve, reject) => {
        http.get(`/api/QrCode/${text}`, { responseType: 'blob' })
          .then((result: any) => resolve(result.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getJobs(_context: any, filter: any): Promise<any> {
      return new Promise((resolve, reject) => {
        filter.countries = ['USA', 'CA'];
        http.get(`/api/WebSite/jobs`, {
          params: { ...filter }
        }).then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    getLandingJobPositions(): Promise<any> {
      return new Promise((resolve, reject) => {
        axios.get("/data/job-positions.json")
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      });
    },
    sendForm(_context: any, contact: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.post('/api/WebSite/contact', contact)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response));
      })
    },
    createCandidate(_context: any, formData: FormData): Promise<any> {
      return new Promise((resolve, reject) => {
        http.post('/api/website/candidate', formData, {
          headers: { 'Content-Type': 'multipart/form-data' }
        })
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response))
      });
    },
    getTaxCategories(): Promise<any> {
      return new Promise((resolve, reject) => {
        http.get('/api/catalog/tax-categories')
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response))
      });
    },
    addCity(_context: any, city: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.post(`/api/Location/city`, city)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response))
      });
    },
    addIndustry(_context: any, industry: any): Promise<any> {
      return new Promise((resolve, reject) => {
        http.post('/api/catalog/industry', industry)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response))
      });
    },
    addProvinceSetting(_context: any, provinceSetting: { provinceId: string; settings: any }): Promise<any> {
      return new Promise((resolve, reject) => {
        http.post(`/api/Location/province/${provinceSetting.provinceId}/settings`, provinceSetting.settings)
          .then((response: any) => resolve(response.data))
          .catch((error: any) => reject(error.response))
      });
    }
  }
};

export default catalogModule;

import { Route } from 'vue-router';
import store from "@/store";
import { getCompanyStatus, getIndustries } from "@/api/catalogApi";

export const loadAgencyCompaniesResolver = async (to: Route, from: Route, next: (...args: any[]) => void) => {
    (to.meta as Record<string, any>)['companyStatuses'] = await getCompanyStatus();
    next();
}

export const loadAgencyRequestToUpdateResolver = async (to: Route, from: Route, next: (...args: any[]) => void) => {
    (to.meta as Record<string, any>)['companyJobPositions'] = await store.dispatch("agency/getAgencyCompanyJobPositions", to.params.companyProfileId);
    (to.meta as Record<string, any>)['companyLocations'] = await store.dispatch("agency/getCompanyLocation", to.params.companyProfileId);
    (to.meta as Record<string, any>)['agencyPersonnel'] = await store.dispatch("agency/getAgencyPersonnel");
    (to.meta as Record<string, any>)['agencyRequest'] = await store.dispatch("agency/getAgencyRequest", to.params.requestId);
    (to.meta as Record<string, any>)['companyUsers'] = await store.dispatch("agency/getCompanyUsers", to.params.companyProfileId);
    next();
}

export const loadCompanyToUpdateResolver = async (to: Route, from: Route, next: (...args: any[]) => void) => {
    (to.meta as Record<string, any>)['companyStatuses'] = await getCompanyStatus();
    (to.meta as Record<string, any>)['industryList'] = await getIndustries();
    (to.meta as Record<string, any>)['company'] = await store.dispatch("agency/getCompany", to.params.companyProfileId);
    (to.meta as Record<string, any>)['agencyPersonnel'] = await store.dispatch("agency/getAgencyPersonnel");
    next();
}

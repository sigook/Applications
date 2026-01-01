import store from "@/store";

export const loadAgencyCompaniesResolver = async (to, from, next) => {
    to.meta['companyStatuses'] = await store.dispatch('getCompanyStatus');
    next();
}

export const loadAgencyRequestToUpdateResolver = async (to, from, next) => {
    to.meta['companyJobPositions'] = await store.dispatch("agency/getAgencyCompanyJobPositions", to.params.companyProfileId);
    to.meta['companyLocations'] = await store.dispatch("agency/getCompanyLocation", to.params.companyProfileId);
    to.meta['agencyPersonnel'] = await store.dispatch("agency/getAgencyPersonnel");
    to.meta['agencyRequest'] = await store.dispatch("agency/getAgencyRequest", to.params.requestId);
    to.meta['companyUsers'] = await store.dispatch("agency/getCompanyUsers", to.params.companyProfileId);
    next();
}

export const loadCompanyToUpdateResolver = async (to, from, next) => {
    to.meta['companyStatuses'] = await store.dispatch("getCompanyStatus");
    to.meta['industryList'] = await store.dispatch("getCompanyIndustry");
    to.meta['company'] = await store.dispatch("agency/getCompany", to.params.companyProfileId);
    to.meta['agencyPersonnel'] = await store.dispatch("agency/getAgencyPersonnel");
    next();
}
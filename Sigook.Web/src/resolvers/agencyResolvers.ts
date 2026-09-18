import type { RouteLocationNormalized, NavigationGuardNext } from 'vue-router';

type Route = RouteLocationNormalized;
import { getCompanyStatus, getIndustries } from "@/api/catalogApi";
import { getAgencyPersonnel } from "@/api/agencyApi";
import { getAgencyCompany } from "@/api/agencyCompanyApi";
import { getAgencyRequestLookup } from "@/api/agencyRequestApi";

export const loadAgencyCompaniesResolver = async (to: Route, from: Route, next: NavigationGuardNext) => {
    (to.meta as Record<string, unknown>)['companyStatuses'] = await getCompanyStatus();
    next();
}

export const loadAgencyRequestFormResolver = async (to: Route, from: Route, next: NavigationGuardNext) => {
    (to.meta as Record<string, unknown>)['requestLookup'] = await getAgencyRequestLookup(
        to.params.companyProfileId as string,
        to.params.requestId as string,
    );
    next();
}

export const loadCompanyToUpdateResolver = async (to: Route, from: Route, next: NavigationGuardNext) => {
    (to.meta as Record<string, unknown>)['companyStatuses'] = await getCompanyStatus();
    (to.meta as Record<string, unknown>)['industryList'] = await getIndustries();
    (to.meta as Record<string, unknown>)['company'] = await getAgencyCompany(to.params.companyProfileId as string);
    (to.meta as Record<string, unknown>)['agencyPersonnel'] = await getAgencyPersonnel();
    next();
}

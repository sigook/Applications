import type { RouteLocationNormalized, NavigationGuardNext } from 'vue-router';

type Route = RouteLocationNormalized;
import { getCompanyStatus, getIndustries } from "@/api/catalogApi";
import { getAgencyPersonnel } from "@/api/agencyApi";
import {
  getAgencyCompanyJobPositions,
  getAgencyCompanyLocation,
  getAgencyCompany,
  getCompanyUsers,
} from "@/api/agencyCompanyApi";
import { getAgencyRequest } from "@/api/agencyRequestApi";

export const loadAgencyCompaniesResolver = async (to: Route, from: Route, next: NavigationGuardNext) => {
    (to.meta as Record<string, unknown>)['companyStatuses'] = await getCompanyStatus();
    next();
}

export const loadAgencyRequestToUpdateResolver = async (to: Route, from: Route, next: NavigationGuardNext) => {
    (to.meta as Record<string, unknown>)['companyJobPositions'] = await getAgencyCompanyJobPositions(to.params.companyProfileId as string);
    (to.meta as Record<string, unknown>)['companyLocations'] = await getAgencyCompanyLocation(to.params.companyProfileId as string);
    (to.meta as Record<string, unknown>)['agencyPersonnel'] = await getAgencyPersonnel();
    (to.meta as Record<string, unknown>)['agencyRequest'] = await getAgencyRequest(to.params.requestId as string);
    (to.meta as Record<string, unknown>)['companyUsers'] = await getCompanyUsers(to.params.companyProfileId as string);
    next();
}

export const loadCompanyToUpdateResolver = async (to: Route, from: Route, next: NavigationGuardNext) => {
    (to.meta as Record<string, unknown>)['companyStatuses'] = await getCompanyStatus();
    (to.meta as Record<string, unknown>)['industryList'] = await getIndustries();
    (to.meta as Record<string, unknown>)['company'] = await getAgencyCompany(to.params.companyProfileId as string);
    (to.meta as Record<string, unknown>)['agencyPersonnel'] = await getAgencyPersonnel();
    next();
}

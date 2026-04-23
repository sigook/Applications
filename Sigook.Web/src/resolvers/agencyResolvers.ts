import type { RouteLocationNormalized } from 'vue-router';

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

export const loadAgencyCompaniesResolver = async (to: Route, from: Route, next: (...args: any[]) => void) => {
    (to.meta as Record<string, any>)['companyStatuses'] = await getCompanyStatus();
    next();
}

export const loadAgencyRequestToUpdateResolver = async (to: Route, from: Route, next: (...args: any[]) => void) => {
    (to.meta as Record<string, any>)['companyJobPositions'] = await getAgencyCompanyJobPositions(to.params.companyProfileId as string);
    (to.meta as Record<string, any>)['companyLocations'] = await getAgencyCompanyLocation(to.params.companyProfileId as string);
    (to.meta as Record<string, any>)['agencyPersonnel'] = await getAgencyPersonnel();
    (to.meta as Record<string, any>)['agencyRequest'] = await getAgencyRequest(to.params.requestId as string);
    (to.meta as Record<string, any>)['companyUsers'] = await getCompanyUsers(to.params.companyProfileId as string);
    next();
}

export const loadCompanyToUpdateResolver = async (to: Route, from: Route, next: (...args: any[]) => void) => {
    (to.meta as Record<string, any>)['companyStatuses'] = await getCompanyStatus();
    (to.meta as Record<string, any>)['industryList'] = await getIndustries();
    (to.meta as Record<string, any>)['company'] = await getAgencyCompany(to.params.companyProfileId as string);
    (to.meta as Record<string, any>)['agencyPersonnel'] = await getAgencyPersonnel();
    next();
}

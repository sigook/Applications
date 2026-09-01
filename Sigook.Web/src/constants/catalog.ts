import type { ResidencyStatus } from '@/types/common';

export const maximumHoursPerDay = Number(import.meta.env.VUE_APP_MAXIMUM_HOURS_DAY) || 12;

export const residencyList: ResidencyStatus[] = ['Citizen', 'Work Permit', 'Student', 'Permanent Resident'];

import type { WorkerProfileDetail } from '@/shared/worker-profile/types';

export function createWorkerProfileDetail(overrides: Partial<WorkerProfileDetail> = {}): WorkerProfileDetail {
  return {
    id: 'worker-profile-1',
    agencyId: 'agency-1',
    numberId: 1001,
    profileImage: null,
    firstName: 'John',
    middleName: null,
    lastName: 'Smith',
    secondLastName: null,
    birthDay: '1990-05-15T04:00:00.000Z',
    gender: null,
    socialInsurance: null,
    socialInsuranceExpire: false,
    dueDate: null,
    socialInsuranceFile: null,
    identificationNumber1: null,
    identificationNumber2: null,
    havePoliceCheckBackground: false,
    identificationType1File: null,
    identificationType2File: null,
    identificationType1: null,
    identificationType2: null,
    policeCheckBackGround: null,
    mobileNumber: null,
    phone: null,
    phoneExt: null,
    location: null,
    hasVehicle: false,
    licenses: [],
    certificates: [],
    otherDocuments: [],
    availabilities: [],
    availabilityTimes: [],
    availabilityDays: [],
    locationPreferences: [],
    lift: null,
    languages: [],
    skills: [],
    resume: null,
    haveAnyHealthProblem: false,
    healthProblem: null,
    otherHealthProblem: null,
    contactEmergencyName: null,
    contactEmergencyLastName: null,
    contactEmergencyPhone: null,
    jobExperiences: [],
    email: 'john.smith@example.com',
    approvedToWork: true,
    workerId: 'worker-1',
    isSubcontractor: false,
    isContractor: false,
    federalTaxCategory: null,
    provincialTaxCategory: null,
    cpp: null,
    ei: null,
    dnu: false,
    createdBy: null,
    punchCardId: null,
    externalId: null,
    wcCode: null,
    missingRequirements: [],
    ...overrides,
  };
}

export function formDataPayload(formData: FormData): unknown {
  const raw = formData.get('data');
  if (typeof raw !== 'string') throw new Error('FormData has no data entry');
  return JSON.parse(raw);
}

export function oversizedFile(name: string): File {
  const file = new File(['x'], name, { type: 'application/pdf' });
  Object.defineProperty(file, 'size', { value: 16 * 1024 * 1024 });
  return file;
}

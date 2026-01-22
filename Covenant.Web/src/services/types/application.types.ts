export interface CandidateFormData {
  fullName: string
  email: string
  phone: string
  countryId: string
  address: string
  status: ResidencyStatus | '' | null | undefined
  skills: string[]
  hasVehicle: boolean
  resume: File | null
  termsAccepted: boolean
}

export interface Country {
  id: string
  value: string  // "Canada", "United States"
  code: string   // "CA", "USA"
}

export const RESIDENCY_STATUS = [
  'Citizen',
  'Work Permit',
  'Student',
  'Permanent Resident'
] as const

export type ResidencyStatus = (typeof RESIDENCY_STATUS)[number]

// Backend model types (matching Covenant.Api models)
export interface PhoneNumberModel {
  id: string
  phoneNumber: string
}

export interface SkillModel {
  id: string | null
  skill: string
}

export interface BaseModel {
  id: string
  value: string
}

export interface CandidateCreateModel {
  name: string
  email: string
  address: string
  postalCode: string
  gender: BaseModel | null
  hasVehicle: boolean
  residencyStatus: string
  source: string
  dnu: boolean
  phoneNumbers: PhoneNumberModel[]
  skills: SkillModel[]
  fileName: string | null
}

export interface CandidateViewModel {
  fullName: string
  email: string
  phone: string
  skills: string[]
  status: string
  countryId: string
  address: string
  fileName: string | null
  hasVehicle: boolean
  requestId?: string
}


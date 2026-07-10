import { submitCandidate } from '@/api/websiteApi'

export const RESIDENCY_STATUS = [
  'Citizen',
  'Work Permit',
  'Student',
  'Permanent Resident',
] as const

export type ResidencyStatus = (typeof RESIDENCY_STATUS)[number]

export interface CandidateFormData {
  fullName: string
  email: string
  phone: string
  countryId: string
  address: string
  status: ResidencyStatus | '' | null | undefined
  sourceId: string | null
  skills: string[]
  hasVehicle: boolean
  resume: File | null
  termsAccepted: boolean
  captchaResponse: string
}

interface CandidateViewModel {
  fullName: string
  email: string
  phone: string
  skills: string[]
  status: string
  countryId: string
  address: string
  fileName: string | null
  hasVehicle: boolean
  sourceId: string | null
  requestId?: string
  captchaResponse: string
}

function generateGuidWithoutDashes(): string {
  if (typeof crypto !== 'undefined' && crypto.randomUUID) {
    return crypto.randomUUID().replace(/-/g, '')
  }
  const guid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
    const r = (Math.random() * 16) | 0
    const v = c === 'x' ? r : (r & 0x3) | 0x8
    return v.toString(16)
  })
  return guid.replace(/-/g, '')
}

function getFileExtension(name: string): string {
  const lastDot = name.lastIndexOf('.')
  return lastDot !== -1 ? name.substring(lastDot) : ''
}

function generateFileName(prefix: string, originalFileName: string): string {
  return `${prefix}_${generateGuidWithoutDashes()}${getFileExtension(originalFileName)}`
}

export function submitCandidateApplication(
  data: CandidateFormData,
  requestId?: string,
): Promise<void> {
  const formData = new FormData()
  const fileName = data.resume ? generateFileName('Resume', data.resume.name) : null

  const viewModel: CandidateViewModel = {
    fullName: data.fullName,
    email: data.email,
    phone: data.phone,
    skills: data.skills,
    status: data.status || '',
    countryId: data.countryId,
    address: data.address,
    fileName,
    hasVehicle: data.hasVehicle,
    sourceId: data.sourceId || null,
    requestId,
    captchaResponse: data.captchaResponse,
  }

  formData.append('data', JSON.stringify(viewModel))

  if (data.resume && fileName) {
    formData.append(fileName, data.resume, fileName)
  }

  return submitCandidate(formData)
}

import api from './api'
import type { CandidateFormData, CandidateViewModel } from './types/application.types'

// Helper functions to generate file names (following Sigook.Web pattern)
function generateGuidWithoutDashes(): string {
  if (typeof crypto !== 'undefined' && crypto.randomUUID) {
    return crypto.randomUUID().replace(/-/g, '')
  }

  const guid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
    const r = (Math.random() * 16) | 0
    const v = c === 'x' ? r : (r & 0x3) | 0x8
    return v.toString(16)
  })
  return guid.replace(/-/g, '')
}

function getFileExtension(filename: string): string {
  const lastDot = filename.lastIndexOf('.')
  return lastDot !== -1 ? filename.substring(lastDot) : ''
}

function generateFileName(prefix: string, originalFileName: string): string {
  const guid = generateGuidWithoutDashes()
  const extension = getFileExtension(originalFileName)
  return `${prefix}_${guid}${extension}`
}

export const applicationService = {
  async submitApplication(data: CandidateFormData, requestId?: string): Promise<void> {
    const formData = new FormData()

    // Generate file name if resume is present
    const fileName = data.resume ? generateFileName('Resume', data.resume.name) : null

    // Build CandidateViewModel
    const candidateViewModel: CandidateViewModel = {
      fullName: data.fullName,
      email: data.email,
      phone: data.phone,
      skills: data.skills,
      status: data.status || '',
      countryId: data.countryId,
      address: data.address,
      fileName: fileName,
      hasVehicle: data.hasVehicle,
      requestId: requestId
    }

    // Append data field with CandidateViewModel JSON (similar to multipartUploadMixin.js)
    formData.append('data', JSON.stringify(candidateViewModel))

    // Append resume file with generated filename as key (similar to multipartUploadMixin.js)
    if (data.resume && fileName) {
      formData.append(fileName, data.resume, fileName)
    }

    await api.post('/api/website/candidate', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    })
  },

  async submitRegisteredApplication(email: string, requestId?: string): Promise<void> {
    // For already registered users, use the same v2 endpoint with multipart/form-data
    const formData = new FormData()

    // Build minimal CandidateViewModel for registered users
    const candidateViewModel: Partial<CandidateViewModel> = {
      email: email,
      requestId: requestId
    }

    // Append data field with CandidateViewModel JSON
    formData.append('data', JSON.stringify(candidateViewModel))

    await api.post('/api/website/v2/candidate', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    })
  }
}

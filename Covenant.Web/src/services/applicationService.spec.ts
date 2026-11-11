import { describe, expect, it, vi } from 'vitest'
import api from './api'
import { applicationService } from './applicationService'
import type { CandidateFormData } from './types/application.types'

const form: CandidateFormData = {
  fullName: 'Jane Doe',
  email: 'jane@example.com',
  phone: '416 555-1234',
  countryId: 'ca',
  provinceId: 'on',
  cityId: 'toronto',
  address: '1 Main St',
  status: 'Citizen',
  sourceId: '',
  skills: ['Forklift'],
  hasVehicle: true,
  resume: null,
  termsAccepted: true,
}

function mockPost() {
  return vi.spyOn(api, 'post').mockResolvedValue({ data: null })
}

function postedData(post: ReturnType<typeof mockPost>): FormData {
  return post.mock.calls[0][1] as FormData
}

function postedModel<T>(post: ReturnType<typeof mockPost>): T {
  return JSON.parse(postedData(post).get('data') as string) as T
}

describe('applicationService', () => {
  it('posts the candidate without a file when there is no resume', async () => {
    const post = mockPost()

    await applicationService.submitApplication(form, 'req-1')

    expect(post).toHaveBeenCalledWith('/api/website/candidate', expect.any(FormData), {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
    expect(postedModel(post)).toEqual({
      fullName: 'Jane Doe',
      email: 'jane@example.com',
      phone: '416 555-1234',
      skills: ['Forklift'],
      status: 'Citizen',
      cityId: 'toronto',
      address: '1 Main St',
      fileName: null,
      hasVehicle: true,
      sourceId: null,
      requestId: 'req-1',
    })
    expect([...postedData(post).keys()]).toEqual(['data'])
  })

  it('attaches the resume under a generated file name that keeps the extension', async () => {
    const post = mockPost()
    const resume = new File(['cv'], 'my-cv.pdf', { type: 'application/pdf' })

    await applicationService.submitApplication({ ...form, status: null, resume })

    const model = postedModel<{ fileName: string; status: string }>(post)
    expect(model.fileName).toMatch(/^Resume_[0-9a-f]{32}\.pdf$/)
    expect(model.status).toBe('')
    expect(postedData(post).get(model.fileName)).toBeInstanceOf(File)
  })

  it('posts only the email and request for registered candidates', async () => {
    const post = mockPost()

    await applicationService.submitRegisteredApplication('jane@example.com', 'req-2')

    expect(postedModel(post)).toEqual({ email: 'jane@example.com', requestId: 'req-2' })
  })
})

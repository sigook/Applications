import { beforeEach, describe, expect, it, vi } from 'vitest'
import { flushPromises, type VueWrapper } from '@vue/test-utils'
import { VFileInput } from 'vuetify/components'
import { mountWithPlugins } from '@/testing/mountWithPlugins'
import { applicationService } from '@/services/applicationService'
import { locationService } from '@/services/locationService'
import type { useApplicationForm } from '@/composables/useApplicationForm'
import type { Job } from '@/services/types/job.types'
import ApplyNowDialog from './ApplyNowDialog.vue'

const captured = vi.hoisted(() => ({ form: undefined as ReturnType<typeof useApplicationForm> | undefined }))

vi.mock('@/composables/useApplicationForm', async (importOriginal) => {
  const original = await importOriginal<typeof import('@/composables/useApplicationForm')>()
  return {
    useApplicationForm: () => {
      captured.form = original.useApplicationForm()
      return captured.form
    },
  }
})

const job: Job = {
  id: '1',
  requestId: 'req-1',
  numberId: '100',
  title: 'Forklift Operator',
  salary: '$20.00',
  location: 'Toronto',
  type: 'Temporary',
  description: '',
  requirements: '',
  responsibilities: '',
  shift: '',
  createdAt: '2026-11-01',
  createdBy: null,
  agencyId: null,
  companyType: null,
}

const sources = [
  { id: 's-cov', value: 'Covenant' },
  { id: 's-li', value: 'Linkedin' },
]

function form() {
  if (!captured.form) throw new Error('form not created')
  return captured.form
}

async function settle() {
  for (let i = 0; i < 3; i++) {
    await flushPromises()
    await new Promise((resolve) => setTimeout(resolve, 0))
  }
}

async function mountDialog(selectedJob: Job | null = null) {
  const wrapper = mountWithPlugins(ApplyNowDialog, { props: { modelValue: true, selectedJob } })
  await settle()
  return wrapper
}

function button(text: string) {
  const found = Array.from(document.querySelectorAll('button')).find((b) => b.textContent.trim() === text)
  if (!found) throw new Error(`button ${text} not found`)
  return found
}

async function click(text: string) {
  button(text).click()
  await settle()
}

async function fillStep1() {
  const f = form()
  f.setFieldValue('fullName', 'Jane Doe')
  f.setFieldValue('email', 'jane@example.com')
  f.setFieldValue('phone', '416 555-1234')
  f.setFieldValue('countryId', 'ca')
  await settle()
  f.setFieldValue('provinceId', 'on')
  await settle()
  f.setFieldValue('cityId', 'tor')
  f.setFieldValue('address', '1 Main St')
  await settle()
}

function body() {
  return document.body.textContent
}

describe('ApplyNowDialog', () => {
  beforeEach(() => {
    vi.spyOn(locationService, 'getCountries').mockResolvedValue([{ id: 'ca', value: 'Canada', code: 'CA' }])
    vi.spyOn(locationService, 'getProvinces').mockResolvedValue([{ id: 'on', value: 'Ontario', code: 'ON' }])
    vi.spyOn(locationService, 'getCities').mockResolvedValue([{ id: 'tor', value: 'Toronto' }])
    vi.spyOn(locationService, 'getSkills').mockResolvedValue(['Welding'])
    vi.spyOn(locationService, 'getSources').mockResolvedValue(sources)
  })

  it('registers without a job and defaults the source to Covenant', async () => {
    await mountDialog()

    expect(body()).toContain('Register with Us')
    expect(body()).not.toContain('Already Registered')
    expect(form().values.sourceId).toBe('s-cov')
  })

  it('applies to a job with both tabs', async () => {
    await mountDialog(job)

    expect(body()).toContain('Apply for Forklift Operator')
    expect(body()).toContain('Already Registered')
  })

  it('keeps working when the catalogs fail', async () => {
    const error = vi.spyOn(console, 'error').mockImplementation(() => undefined)
    vi.mocked(locationService.getCountries).mockRejectedValue(new Error('x'))
    vi.mocked(locationService.getSkills).mockRejectedValue(new Error('x'))
    vi.mocked(locationService.getSources).mockRejectedValue(new Error('x'))
    vi.mocked(locationService.getProvinces).mockRejectedValue(new Error('x'))
    vi.mocked(locationService.getCities).mockRejectedValue(new Error('x'))
    await mountDialog()

    form().setFieldValue('countryId', 'ca')
    await settle()
    form().setFieldValue('provinceId', 'on')
    await settle()

    expect(error).toHaveBeenCalledTimes(5)
    expect(form().values.sourceId).toBeNull()
  })

  it('keeps the source when Covenant is not in the catalog', async () => {
    vi.mocked(locationService.getSources).mockResolvedValue([sources[1]])
    await mountDialog()

    expect(form().values.sourceId).toBeNull()
  })

  it('reloads the provinces and cities in cascade', async () => {
    await mountDialog()
    await fillStep1()

    expect(locationService.getProvinces).toHaveBeenCalledWith('ca')
    expect(locationService.getCities).toHaveBeenCalledWith('on')

    form().setFieldValue('countryId', '')
    await settle()

    expect(form().values.provinceId).toBe('')
    expect(form().values.cityId).toBe('')
  })

  it('walks the three steps and submits the application', async () => {
    let sent: { fullName: string; skills: string[] } | undefined
    const submit = vi.spyOn(applicationService, 'submitApplication').mockImplementation((data) => {
      sent = { fullName: data.fullName, skills: [...data.skills] }
      return Promise.resolve()
    })
    const wrapper = await mountDialog(job)

    await click('Next')
    expect(form().currentStep.value).toBe(1)

    await fillStep1()
    await click('Next')
    expect(form().currentStep.value).toBe(2)

    form().setFieldValue('status', 'Citizen')
    form().setFieldValue('skills', ['Welding'])
    await click('Next')
    expect(form().currentStep.value).toBe(3)
    expect(body()).toContain('1 Main St, Toronto, ON')
    expect(body()).toContain('Covenant')

    await click('Submit Application')
    expect(submit).not.toHaveBeenCalled()

    form().setFieldValue('termsAccepted', true)
    await click('Submit Application')

    expect(submit).toHaveBeenCalledWith(expect.anything(), 'req-1')
    expect(sent).toEqual({ fullName: 'Jane Doe', skills: ['Welding'] })
    expect(wrapper.emitted('application-submitted')).toHaveLength(1)
    expect(form().values.fullName).toBe('')
  })

  it('goes back a step', async () => {
    await mountDialog()
    await fillStep1()
    await click('Next')

    await click('Back')

    expect(form().currentStep.value).toBe(1)
  })

  it('requires a resume for Linkedin applicants', async () => {
    const submit = vi.spyOn(applicationService, 'submitApplication').mockResolvedValue()
    await mountDialog()
    await fillStep1()
    await click('Next')
    form().setFieldValue('status', 'Citizen')
    form().setFieldValue('sourceId', 's-li')
    await settle()

    await click('Next')
    expect(form().currentStep.value).toBe(2)
    expect(body()).toContain('Resume is required when you apply via Linkedin')

    form().currentStep.value = 3
    form().setFieldValue('termsAccepted', true)
    await settle()
    await click('Submit Application')

    expect(submit).not.toHaveBeenCalled()
    expect(form().currentStep.value).toBe(2)
  })

  it('stores the chosen resume', async () => {
    const wrapper = await mountDialog()
    await fillStep1()
    await click('Next')
    const input = wrapper.findComponent(VFileInput)
    const file = new File(['cv'], 'cv.pdf')

    input.vm.$emit('update:modelValue', [file])
    expect(form().values.resume?.name).toBe('cv.pdf')

    form().setFieldValue('resume', null)
    input.vm.$emit('update:modelValue', file)
    expect(form().values.resume?.name).toBe('cv.pdf')

    input.vm.$emit('update:modelValue', null)
    expect(form().values.resume).toBeNull()
  })

  it('shows an error when the submission fails', async () => {
    vi.spyOn(console, 'error').mockImplementation(() => undefined)
    vi.spyOn(applicationService, 'submitApplication').mockRejectedValue(new Error('boom'))
    const wrapper = await mountDialog()
    await fillStep1()
    await click('Next')
    form().setFieldValue('status', 'Citizen')
    await click('Next')
    form().setFieldValue('termsAccepted', true)

    await click('Submit Application')

    expect(body()).toContain('Failed to submit application. Please try again.')
    expect(wrapper.emitted('application-submitted')).toBeUndefined()
  })

  it('formats the phone as the user types', async () => {
    const wrapper = await mountDialog()
    const input = document.querySelector<HTMLInputElement>('input[type="tel"]')
    if (!input) throw new Error('phone input missing')

    input.value = '4165551234'
    input.dispatchEvent(new Event('input', { bubbles: true }))
    await settle()
    input.dispatchEvent(new Event('input', { bubbles: true }))
    await settle()

    expect(form().values.phone).toBe(input.value)

    await wrapper.setProps({ modelValue: false })
    await settle()
  })

  describe('already registered', () => {
    async function openRegistered() {
      const wrapper = await mountDialog(job)
      await click('Already Registered')
      const input = document.querySelector<HTMLInputElement>('input[type="email"]')
      if (!input) throw new Error('email input missing')
      return { wrapper, input }
    }

    async function type(input: HTMLInputElement, value: string) {
      input.value = value
      input.dispatchEvent(new Event('input'))
      await settle()
    }

    it('validates the email and clears the error while typing', async () => {
      const submit = vi.spyOn(applicationService, 'submitRegisteredApplication')
      const { input } = await openRegistered()

      await type(input, 'not-an-email')
      await click('Submit Application')
      expect(body()).toContain('Invalid email format')

      await type(input, 'jane@')
      expect(body()).not.toContain('Invalid email format')
      expect(submit).not.toHaveBeenCalled()
    })

    it('submits the registered email for the job', async () => {
      const submit = vi.spyOn(applicationService, 'submitRegisteredApplication').mockResolvedValue()
      const { wrapper, input } = await openRegistered()

      await type(input, 'jane@example.com')
      await click('Submit Application')

      expect(submit).toHaveBeenCalledWith('jane@example.com', 'req-1')
      expect(wrapper.emitted('application-submitted')).toHaveLength(1)
    })

    it('shows an error when the registered submission fails', async () => {
      vi.spyOn(console, 'error').mockImplementation(() => undefined)
      vi.spyOn(applicationService, 'submitRegisteredApplication').mockRejectedValue(new Error('boom'))
      const { input } = await openRegistered()

      await type(input, 'jane@example.com')
      await click('Submit Application')

      expect(body()).toContain('Failed to submit application. Please try again.')
    })

    it('returns to the new applicant tab when reopened', async () => {
      const { wrapper } = await openRegistered()

      await wrapper.setProps({ modelValue: false })
      await settle()
      await wrapper.setProps({ modelValue: true })
      await settle()

      expect(body()).toContain('Personal Information')
      expect(form().currentStep.value).toBe(1)
    })
  })

  it('closes from the toolbar button', async () => {
    const wrapper = await mountDialog()
    const close = document.querySelector<HTMLButtonElement>('.v-toolbar button')
    if (!close) throw new Error('close button missing')

    close.click()
    await settle()

    expect(wrapper.emitted('update:modelValue')).toEqual([[false]])
  })

  it('ignores the close while submitting', async () => {
    vi.spyOn(applicationService, 'submitApplication').mockReturnValue(new Promise(() => undefined))
    const wrapper: VueWrapper = await mountDialog()
    await fillStep1()
    await click('Next')
    form().setFieldValue('status', 'Citizen')
    await click('Next')
    form().setFieldValue('termsAccepted', true)
    await click('Submit Application')

    document.querySelector<HTMLButtonElement>('.v-toolbar button')?.click()
    await settle()

    expect(wrapper.emitted('update:modelValue')).toBeUndefined()
  })
})

import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'
import { defineComponent } from 'vue'
import { useApplicationForm } from './useApplicationForm'

function setup() {
  let form: ReturnType<typeof useApplicationForm> | undefined
  mount(
    defineComponent({
      setup() {
        form = useApplicationForm()
        return () => null
      },
    }),
  )
  if (!form) throw new Error('form not created')
  return form
}

function fillStep1(form: ReturnType<typeof useApplicationForm>) {
  form.setFieldValue('fullName', 'Jane Doe')
  form.setFieldValue('email', 'jane@example.com')
  form.setFieldValue('phone', '416 555-1234')
  form.setFieldValue('countryId', 'ca')
  form.setFieldValue('provinceId', 'on')
  form.setFieldValue('cityId', 'toronto')
  form.setFieldValue('address', '1 Main St')
}

describe('useApplicationForm', () => {
  it('starts on step 1 with empty values', () => {
    const form = setup()

    expect(form.currentStep.value).toBe(1)
    expect(form.values.fullName).toBe('')
    expect(form.values.skills).toEqual([])
    expect(form.values.termsAccepted).toBe(false)
  })

  it('reports every missing field of step 1', async () => {
    const form = setup()

    expect(await form.validateCurrentStep()).toBe(false)
    expect(form.errors.value).toMatchObject({
      fullName: 'Full name is required',
      email: 'Email is required',
      cityId: 'City is required',
    })
  })

  it('rejects a phone that does not follow the mask', async () => {
    const form = setup()
    fillStep1(form)
    form.setFieldValue('phone', '4165551234')

    expect(await form.validateCurrentStep()).toBe(false)
    expect(form.errors.value.phone).toBe('Phone format must be: ### ###-####')
  })

  it('passes step 1 once it is complete and clears the errors', async () => {
    const form = setup()
    fillStep1(form)

    expect(await form.validateCurrentStep()).toBe(true)
    expect(form.errors.value).toEqual({})
  })

  it('requires the immigration status on step 2', async () => {
    const form = setup()
    form.goToStep(2)

    expect(await form.validateCurrentStep()).toBe(false)
    expect(form.errors.value.status).toBe('Immigration status is required')

    form.setFieldValue('status', 'Citizen')
    expect(await form.validateCurrentStep()).toBe(true)
  })

  it('requires accepting the terms on step 3', async () => {
    const form = setup()
    form.goToStep(3)

    expect(await form.validateCurrentStep()).toBe(false)
    expect(form.errors.value.termsAccepted).toBe('You must accept the terms and conditions')

    form.setFieldValue('termsAccepted', true)
    expect(await form.validateCurrentStep()).toBe(true)
  })
})

import { beforeEach, describe, expect, it, vi } from 'vitest'
import { flushPromises, type VueWrapper } from '@vue/test-utils'
import { settle } from '@/testing/settle'
import { mountWithPlugins } from '@/testing/mountWithPlugins'
import { contactService } from '@/services/contactService'
import { locationService } from '@/services/locationService'
import { useToast } from '@/composables/useToast'
import ContactForm from './ContactForm.vue'

const countries = [
  { id: 'us', value: 'United States', code: 'USA' },
  { id: 'ca', value: 'Canada', code: 'CA' },
]
const provinces = [{ id: 'on', value: 'Ontario', code: 'ON' }]

async function mountForm(props: Partial<InstanceType<typeof ContactForm>['$props']> = {}) {
  const wrapper = mountWithPlugins(ContactForm, { props: { subject: 'Contact', title: 'Website', ...props } })
  await flushPromises()
  return wrapper
}

async function fill(wrapper: VueWrapper) {
  await wrapper.find('input[placeholder="Name"]').setValue('Jane Doe')
  await wrapper.find('input[type="email"]').setValue('jane@example.com')
  const phone = wrapper.find('input[type="tel"]')
  ;(phone.element as HTMLInputElement).value = ''
  await phone.setValue('4165551234')
  await wrapper.find('input[placeholder="Company"]').setValue('Acme')
  await wrapper.find('select').setValue('Ontario')
  await wrapper.find('textarea').setValue('Hello')
}

function verifyCaptcha(wrapper: VueWrapper) {
  wrapper.findComponent({ name: 'Vue3Recaptcha2' }).vm.$emit('verify', 'captcha-token')
}

async function submit(wrapper: VueWrapper) {
  await wrapper.find('form').trigger('submit')
  await settle()
}

describe('ContactForm', () => {
  beforeEach(() => {
    vi.spyOn(locationService, 'getCountries').mockResolvedValue(countries)
    vi.spyOn(locationService, 'getProvinces').mockResolvedValue(provinces)
    useToast().toastState.value.show = false
  })

  it('loads the Canadian provinces into the location select', async () => {
    const wrapper = await mountForm()

    expect(locationService.getProvinces).toHaveBeenCalledWith('ca')
    expect(wrapper.findAll('option').map((o) => o.text())).toEqual(['Select a province', 'Ontario'])
  })

  it('leaves the select empty when Canada is not in the catalog', async () => {
    vi.mocked(locationService.getCountries).mockResolvedValue([countries[0]])
    const wrapper = await mountForm()

    expect(locationService.getProvinces).not.toHaveBeenCalled()
    expect(wrapper.findAll('option')).toHaveLength(1)
  })

  it('survives a catalog failure', async () => {
    vi.spyOn(console, 'error').mockImplementation(() => undefined)
    vi.mocked(locationService.getCountries).mockRejectedValue(new Error('down'))
    const wrapper = await mountForm()

    expect(wrapper.findAll('option')).toHaveLength(1)
  })

  it('shows the required errors on an empty submit', async () => {
    const send = vi.spyOn(contactService, 'sendContactEmail')
    const wrapper = await mountForm()

    await submit(wrapper)

    const errors = wrapper.findAll('.contact-form__error').map((e) => e.text())
    expect(errors).toEqual(expect.arrayContaining(['Name is required', 'Email is required', 'Location is required']))
    expect(send).not.toHaveBeenCalled()
  })

  it('asks for the captcha before sending', async () => {
    const send = vi.spyOn(contactService, 'sendContactEmail')
    const wrapper = await mountForm()
    await fill(wrapper)

    await submit(wrapper)

    expect(wrapper.find('.error-text').text()).toBe('Please verify you are human.')
    expect(send).not.toHaveBeenCalled()
  })

  it('sends the form with the static title and resets it', async () => {
    const send = vi.spyOn(contactService, 'sendContactEmail').mockResolvedValue()
    const wrapper = await mountForm()
    await fill(wrapper)
    verifyCaptcha(wrapper)

    await submit(wrapper)

    expect(send).toHaveBeenCalledWith({
      title: 'Website',
      name: 'Jane Doe',
      email: 'jane@example.com',
      phone: '416 555-1234',
      company: 'Acme',
      location: 'Ontario',
      message: 'Hello',
      subject: 'Contact',
      captchaResponse: 'captcha-token',
      emailSetting: 4,
    })
    expect(useToast().toastState.value).toMatchObject({ show: true, color: 'success' })
    expect((wrapper.find('input[placeholder="Name"]').element as HTMLInputElement).value).toBe('')
    expect(wrapper.find('.recaptcha-stub').exists()).toBe(false)
    await vi.waitFor(() => expect(wrapper.find('.recaptcha-stub').exists()).toBe(true))
  })

  it('builds a dynamic title from the name and suffix', async () => {
    const send = vi.spyOn(contactService, 'sendContactEmail').mockResolvedValue()
    const wrapper = await mountForm({ useDynamicTitle: true, titleSuffix: 'wants to partner' })
    await fill(wrapper)
    verifyCaptcha(wrapper)

    await submit(wrapper)

    expect(send).toHaveBeenCalledWith(expect.objectContaining({ title: 'Jane Doe wants to partner' }))
  })

  it('shows an error toast when sending fails', async () => {
    vi.spyOn(console, 'error').mockImplementation(() => undefined)
    vi.spyOn(contactService, 'sendContactEmail').mockRejectedValue(new Error('boom'))
    const wrapper = await mountForm()
    await fill(wrapper)
    verifyCaptcha(wrapper)

    await submit(wrapper)

    expect(useToast().toastState.value).toMatchObject({ show: true, color: 'error' })
    expect((wrapper.find('input[placeholder="Name"]').element as HTMLInputElement).value).toBe('Jane Doe')
  })

  it('drops the token when the captcha expires', async () => {
    vi.spyOn(console, 'warn').mockImplementation(() => undefined)
    vi.spyOn(console, 'error').mockImplementation(() => undefined)
    const send = vi.spyOn(contactService, 'sendContactEmail')
    const wrapper = await mountForm()
    await fill(wrapper)
    verifyCaptcha(wrapper)
    const captcha = wrapper.findComponent({ name: 'Vue3Recaptcha2' })
    captcha.vm.$emit('fail')
    captcha.vm.$emit('expire')

    await submit(wrapper)

    expect(send).not.toHaveBeenCalled()
    expect(wrapper.find('.error-text').exists()).toBe(true)
  })

  it('clears the fields with the reset button', async () => {
    const wrapper = await mountForm()
    await fill(wrapper)

    await wrapper.find('.contact-form__reset').trigger('click')

    expect((wrapper.find('input[type="tel"]').element as HTMLInputElement).value).toBe('')
    expect((wrapper.find('input[placeholder="Company"]').element as HTMLInputElement).value).toBe('')
  })
})

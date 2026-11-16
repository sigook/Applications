import { describe, expect, it, vi } from 'vitest'
import api from './api'
import { contactService } from './contactService'
import type { ContactDto } from './types/contact.types'

describe('contactService', () => {
  it('posts the contact form to the website endpoint', async () => {
    const post = vi.spyOn(api, 'post').mockResolvedValue({ data: null })
    const dto: ContactDto = {
      title: 'Hi',
      name: 'Jane',
      email: 'jane@example.com',
      phone: '416 555-1234',
      company: 'Acme',
      location: 'Ontario',
      message: '',
      subject: 'Contact',
      captchaResponse: 'token',
      emailSetting: 4,
    }

    await contactService.sendContactEmail(dto)

    expect(post).toHaveBeenCalledWith('/api/website/contact', dto)
  })
})

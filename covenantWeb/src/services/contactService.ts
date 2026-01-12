/**
 * Contact service for handling contact form submissions
 */

import api from './api'
import type { ContactDto } from './types/contact.types'

export const contactService = {
  /**
   * Send contact form email
   * @param contactData - Contact form data with reCAPTCHA token
   * @returns Promise that resolves when email is sent
   */
  async sendContactEmail(contactData: ContactDto): Promise<void> {
    await api.post('/api/website/contact', contactData)
  }
}

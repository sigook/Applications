/**
 * Contact form data types for Covenant API integration
 */

/**
 * Contact DTO matching backend ContactDto model
 * Used for API communication with /api/website/contact endpoint
 */
export interface ContactDto {
  title: string
  name: string
  email: string
  phone: string
  message: string
  subject: string
  captchaResponse: string
  emailSetting: number
}

/**
 * Form data structure used within Vue components
 */
export interface ContactFormData {
  name: string
  email: string
  phone: string
  message: string
}

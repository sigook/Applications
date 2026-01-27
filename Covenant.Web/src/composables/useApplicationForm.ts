import { ref, watch } from 'vue'
import { useForm } from 'vee-validate'
import * as yup from 'yup'
import type { CandidateFormData } from '@/services/types/application.types'

export function useApplicationForm() {
  const currentStep = ref(1)

  // Complete validation schema (all fields)
  const completeSchema = yup.object({
    fullName: yup.string().required('Full name is required').max(60, 'Full name must be at most 60 characters'),
    email: yup
      .string()
      .required('Email is required')
      .email('Invalid email format')
      .min(6, 'Email must be at least 6 characters')
      .max(100, 'Email must be at most 100 characters'),
    phone: yup
      .string()
      .required('Phone is required')
      .matches(/^\d{3} \d{3}-\d{4}$/, 'Phone format must be: ### ###-####'),
    countryId: yup.string().required('Country is required'),
    address: yup.string().required('City/Address is required').max(100, 'Address must be at most 100 characters'),
    status: yup.string().required('Immigration status is required'),
    skills: yup.array().of(yup.string().max(20, 'Each skill must be at most 20 characters')),
    hasVehicle: yup.boolean().required(),
    resume: yup.mixed().nullable(),
    termsAccepted: yup.boolean().oneOf([true], 'You must accept the terms and conditions')
  })

  // VeeValidate setup with complete schema
  const { values, errors, defineField, resetForm, setFieldValue, setErrors } = useForm<CandidateFormData>({
    validationSchema: completeSchema,
    initialValues: {
      fullName: '',
      email: '',
      phone: '',
      countryId: '',
      address: '',
      status: undefined,
      skills: [],
      hasVehicle: false,
      resume: null,
      termsAccepted: false
    } as any,
    validateOnMount: false
  })

  // Define fields for v-model binding with aggressive validation mode
  const [fullName, fullNameProps] = defineField('fullName', { validateOnModelUpdate: false })
  const [email, emailProps] = defineField('email', { validateOnModelUpdate: false })
  const [phone, phoneProps] = defineField('phone', { validateOnModelUpdate: false })
  const [countryId, countryIdProps] = defineField('countryId', { validateOnModelUpdate: false })
  const [address, addressProps] = defineField('address', { validateOnModelUpdate: false })
  const [status, statusProps] = defineField('status', { validateOnModelUpdate: false })
  const [skills, skillsProps] = defineField('skills', { validateOnModelUpdate: false })
  const [hasVehicle, hasVehicleProps] = defineField('hasVehicle', { validateOnModelUpdate: false })
  const [resume, resumeProps] = defineField('resume', { validateOnModelUpdate: false })
  const [termsAccepted, termsAcceptedProps] = defineField('termsAccepted', { validateOnModelUpdate: false })

  // Clear errors when step changes
  watch(currentStep, () => {
    setErrors({})
  })

  async function validateCurrentStep() {
    // Create step-specific schemas
    const step1Schema = yup.object({
      fullName: yup.string().required('Full name is required').max(60, 'Full name must be at most 60 characters'),
      email: yup
        .string()
        .required('Email is required')
        .email('Invalid email format')
        .min(6, 'Email must be at least 6 characters')
        .max(100, 'Email must be at most 100 characters'),
      phone: yup
        .string()
        .required('Phone is required')
        .matches(/^\d{3} \d{3}-\d{4}$/, 'Phone format must be: ### ###-####'),
      countryId: yup.string().required('Country is required'),
      address: yup.string().required('City/Address is required').max(100, 'Address must be at most 100 characters')
    })

    const step2Schema = yup.object({
      status: yup.string().required('Immigration status is required'),
      skills: yup.array().of(yup.string().max(20, 'Each skill must be at most 20 characters')),
      hasVehicle: yup.boolean().required()
    })

    const step3Schema = yup.object({
      resume: yup.mixed().nullable(),
      termsAccepted: yup.boolean().oneOf([true], 'You must accept the terms and conditions')
    })

    // Select the appropriate schema for current step
    let schema: any
    let valuesToValidate: any = {}

    if (currentStep.value === 1) {
      schema = step1Schema
      valuesToValidate = {
        fullName: values.fullName,
        email: values.email,
        phone: values.phone,
        countryId: values.countryId,
        address: values.address
      }
    } else if (currentStep.value === 2) {
      schema = step2Schema
      valuesToValidate = {
        status: values.status,
        skills: values.skills,
        hasVehicle: values.hasVehicle
      }
    } else if (currentStep.value === 3) {
      schema = step3Schema
      valuesToValidate = {
        resume: values.resume,
        termsAccepted: values.termsAccepted
      }
    }

    try {
      // Validate only the current step values
      await schema.validate(valuesToValidate, { abortEarly: false })
      // Clear errors for this step if validation passes
      setErrors({})
      return true
    } catch (err: any) {
      // Set errors for this step if validation fails
      const validationErrors: Record<string, string> = {}
      if (err.inner) {
        err.inner.forEach((error: any) => {
          if (error.path) {
            validationErrors[error.path] = error.message
          }
        })
      }
      setErrors(validationErrors)
      return false
    }
  }

  function goToStep(step: number) {
    currentStep.value = step
  }

  return {
    currentStep,
    values,
    errors,
    fields: {
      fullName,
      fullNameProps,
      email,
      emailProps,
      phone,
      phoneProps,
      countryId,
      countryIdProps,
      address,
      addressProps,
      status,
      statusProps,
      skills,
      skillsProps,
      hasVehicle,
      hasVehicleProps,
      resume,
      resumeProps,
      termsAccepted,
      termsAcceptedProps
    },
    validateCurrentStep,
    resetForm,
    setFieldValue,
    goToStep
  }
}

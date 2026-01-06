import axios, { AxiosError, type AxiosInstance } from 'axios'
import qs from 'qs'

const apiBaseURL = import.meta.env.VITE_API_BASE_URL

if (!apiBaseURL) {
  throw new Error(
    'VITE_API_BASE_URL is not defined. Please check your environment configuration.'
  )
}

const api: AxiosInstance = axios.create({
  baseURL: apiBaseURL,
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
  paramsSerializer: (params) => qs.stringify(params, { encodeValuesOnly: true }),
})

// Request interceptor
api.interceptors.request.use(
  (config) => {
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Response interceptor for error handling
api.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    let errorMessage = 'An unexpected error occurred'

    if (error.response) {
      // Server responded with error status
      switch (error.response.status) {
        case 400:
          errorMessage = 'Invalid request. Please check your input.'
          break
        case 404:
          errorMessage = 'Resource not found.'
          break
        case 500:
          errorMessage = 'Server error. Please try again later.'
          break
        default:
          errorMessage = `Request failed with status ${error.response.status}`
      }
    } else if (error.request) {
      // Request was made but no response received
      errorMessage = 'Network error. Please check your internet connection.'
    } else {
      // Something else happened
      errorMessage = error.message || errorMessage
    }

    // Attach user-friendly message to error
    error.message = errorMessage
    return Promise.reject(error)
  }
)

export default api

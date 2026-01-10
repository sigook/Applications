/**
 * Toast notification composable using Vuetify snackbar
 */

import { ref } from 'vue'

interface ToastState {
  show: boolean
  message: string
  color: string
  timeout: number
}

const toastState = ref<ToastState>({
  show: false,
  message: '',
  color: 'success',
  timeout: 4000
})

export function useToast() {
  /**
   * Show a success toast notification
   * @param message - Success message to display
   */
  const showSuccess = (message: string) => {
    toastState.value = {
      show: true,
      message,
      color: 'success',
      timeout: 4000
    }
  }

  /**
   * Show an error toast notification
   * @param message - Error message to display
   */
  const showError = (message: string) => {
    toastState.value = {
      show: true,
      message,
      color: 'error',
      timeout: 5000
    }
  }

  /**
   * Show an info toast notification
   * @param message - Info message to display
   */
  const showInfo = (message: string) => {
    toastState.value = {
      show: true,
      message,
      color: 'info',
      timeout: 4000
    }
  }

  return {
    toastState,
    showSuccess,
    showError,
    showInfo
  }
}

import { describe, expect, it } from 'vitest'
import { useToast } from './useToast'

describe('useToast', () => {
  it('shows a success toast for 4 seconds', () => {
    const { toastState, showSuccess } = useToast()
    showSuccess('Saved')
    expect(toastState.value).toEqual({ show: true, message: 'Saved', color: 'success', timeout: 4000 })
  })

  it('shows an error toast for 5 seconds', () => {
    const { toastState, showError } = useToast()
    showError('Failed')
    expect(toastState.value).toEqual({ show: true, message: 'Failed', color: 'error', timeout: 5000 })
  })

  it('shows an info toast for 4 seconds', () => {
    const { toastState, showInfo } = useToast()
    showInfo('Heads up')
    expect(toastState.value).toEqual({ show: true, message: 'Heads up', color: 'info', timeout: 4000 })
  })

  it('shares the same state across callers', () => {
    useToast().showInfo('Shared')
    expect(useToast().toastState.value.message).toBe('Shared')
  })
})

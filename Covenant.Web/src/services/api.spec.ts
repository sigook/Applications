import { AxiosError, AxiosHeaders, type InternalAxiosRequestConfig } from 'axios'
import { afterEach, describe, expect, it } from 'vitest'
import api from './api'

const originalAdapter = api.defaults.adapter

function failWith(build: (config: InternalAxiosRequestConfig) => AxiosError) {
  api.defaults.adapter = (config) => Promise.reject(build(config))
}

function statusError(status: number) {
  failWith(
    (config) =>
      new AxiosError('raw', 'ERR_BAD_RESPONSE', config, {}, {
        status,
        statusText: '',
        data: null,
        headers: {},
        config: { ...config, headers: new AxiosHeaders() },
      }),
  )
}

async function messageOf(): Promise<string> {
  try {
    await api.get('/anything')
  } catch (error) {
    if (error instanceof Error) return error.message
  }
  return 'resolved'
}

describe('api', () => {
  afterEach(() => {
    api.defaults.adapter = originalAdapter
  })

  it.each([
    [400, 'Invalid request. Please check your input.'],
    [404, 'Resource not found.'],
    [500, 'Server error. Please try again later.'],
    [403, 'Request failed with status 403'],
  ])('maps HTTP %i to a friendly message', async (status, message) => {
    statusError(status)
    expect(await messageOf()).toBe(message)
  })

  it('reports a network error when no response arrives', async () => {
    failWith((config) => new AxiosError('raw', 'ERR_NETWORK', config, {}))
    expect(await messageOf()).toBe('Network error. Please check your internet connection.')
  })

  it('keeps the original message when the request never left', async () => {
    failWith((config) => new AxiosError('Timeout', 'ECONNABORTED', config))
    expect(await messageOf()).toBe('Timeout')
  })
})

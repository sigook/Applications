import { flushPromises } from '@vue/test-utils'

const VEE_VALIDATE_DEBOUNCE_MS = 5

export async function settle() {
  for (let i = 0; i < 3; i++) {
    await flushPromises()
    await new Promise((resolve) => setTimeout(resolve, VEE_VALIDATE_DEBOUNCE_MS * 2))
  }
}

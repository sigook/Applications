import type { VNode } from 'vue'

declare global {
  namespace JSX {
    type Element = VNode
    interface IntrinsicElements {
      [elem: string]: any
    }
  }
}

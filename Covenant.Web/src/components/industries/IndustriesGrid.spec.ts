import { describe, expect, it, vi } from 'vitest'
import gsap from 'gsap'
import { mountWithPlugins } from '@/testing/mountWithPlugins'
import IndustriesGrid from './IndustriesGrid.vue'

describe('IndustriesGrid', () => {
  it('flips one industry at a time', async () => {
    const tween = gsap.to({}, { duration: 0 })
    const to = vi.spyOn(gsap, 'to').mockReturnValue(tween)
    const wrapper = mountWithPlugins(IndustriesGrid)
    const inners = wrapper.findAll('.ind-card__inner')
    const buttons = wrapper.findAll('.ind-card__icon')

    await buttons[0].trigger('click')
    expect(inners[0].classes()).toContain('ind-card__inner--flipped')
    expect(to).toHaveBeenLastCalledWith(inners[0].element, expect.objectContaining({ rotateY: 180 }))

    await buttons[1].trigger('click')
    expect(inners[0].classes()).not.toContain('ind-card__inner--flipped')
    expect(inners[1].classes()).toContain('ind-card__inner--flipped')
    expect(to).toHaveBeenCalledWith(inners[0].element, expect.objectContaining({ rotateY: 0 }))

    await wrapper.findAll('.ind-card__icon-wrapper')[1].trigger('click')
    expect(wrapper.findAll('.ind-card__inner--flipped')).toHaveLength(0)
  })
})

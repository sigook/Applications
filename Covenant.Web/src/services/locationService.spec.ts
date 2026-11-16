import { describe, expect, it, vi } from 'vitest'
import api from './api'
import { locationService } from './locationService'

describe('locationService', () => {
  it.each([
    ['getCountries', () => locationService.getCountries(), '/api/Location/country'],
    ['getProvinces', () => locationService.getProvinces('ca'), '/api/Location/province/ca'],
    ['getCities', () => locationService.getCities('on'), '/api/Location/city/on'],
    ['getSources', () => locationService.getSources(), '/api/Catalog/source'],
  ])('%s reads %s', async (_name, call, url) => {
    const get = vi.spyOn(api, 'get').mockResolvedValue({ data: [{ id: '1', value: 'x' }] })

    expect(await call()).toEqual([{ id: '1', value: 'x' }])
    expect(get).toHaveBeenCalledWith(url)
  })

  it('getSkills returns the skill names', async () => {
    vi.spyOn(api, 'get').mockResolvedValue({ data: [{ skill: 'Welding' }, { skill: 'Forklift' }] })

    expect(await locationService.getSkills()).toEqual(['Welding', 'Forklift'])
  })
})

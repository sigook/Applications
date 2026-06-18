import api from './api'
import type { BaseModel, Country, Province } from './types/application.types'

export interface Skill {
  skill: string
}

export const locationService = {
  async getCountries(): Promise<Country[]> {
    const response = await api.get<Country[]>('/api/Location/country')
    return response.data
  },

  async getProvinces(countryId: string): Promise<Province[]> {
    const response = await api.get<Province[]>(`/api/Location/province/${countryId}`)
    return response.data
  },

  async getSkills(): Promise<string[]> {
    const response = await api.get<Skill[]>('/api/Catalog/skills')
    return response.data.map((s) => s.skill)
  },

  async getSources(): Promise<BaseModel[]> {
    const response = await api.get<BaseModel[]>('/api/Catalog/source')
    return response.data
  }
}

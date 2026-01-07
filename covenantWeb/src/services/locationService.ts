import api from './api'
import type { Country } from './types/application.types'

export interface Skill {
  skill: string
}

export const locationService = {
  async getCountries(): Promise<Country[]> {
    const response = await api.get<Country[]>('/api/Location/country')
    return response.data
  },

  async getSkills(): Promise<string[]> {
    const response = await api.get<Skill[]>('/api/Catalog/skills')
    return response.data.map((s) => s.skill)
  }
}

export interface Job {
  id: string
  requestId: string
  numberId: string
  title: string
  salary: string
  location: string
  type: string
  description: string
  requirements: string
  responsibilities: string
  shift: string
  createdAt: string
  createdBy: string | null
  agencyId: string | null
  companyType: string | null
}

export interface JobFilters {
  countries?: string[]
  jobId?: string
  jobTitle?: string
  location?: string
}

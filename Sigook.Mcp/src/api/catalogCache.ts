import type { AgencyRequestListModel, CatalogItem, CityModel, ProvinceModel } from '../models/apiModels.js';
import { fold, isBlank, same, similarity } from './lookupRules.js';
import type { SigookApi } from './sigookApi.js';

export interface CityAlternative {
  id: string;
  value: string;
  score: number;
}

export interface CityResolution {
  city?: CityModel;
  province?: ProvinceModel;
  error?: string;
  provinces?: ProvinceModel[];
  alternatives?: CityAlternative[];
}

const pickProvince = ({ id, value, code }: ProvinceModel): ProvinceModel => ({ id, value, code });

function cached<K, T>(cache: Map<K, Promise<T>>, key: K, factory: () => Promise<T>): Promise<T> {
  const existing = cache.get(key);
  if (existing) return existing;
  const created = factory();
  cache.set(key, created);
  created.catch(() => cache.delete(key));
  return created;
}

export class CatalogCache {
  private readonly sourcesCache = new Map<string, Promise<CatalogItem[]>>();
  private readonly provincesCache = new Map<string, Promise<ProvinceModel[]>>();
  private readonly citiesCache = new Map<string, Promise<CityModel[]>>();
  private readonly requestsCache = new Map<number, Promise<AgencyRequestListModel | undefined>>();

  constructor(private readonly api: SigookApi) {}

  sources = () => cached(this.sourcesCache, '', () => this.api.getSources());

  provinces = (countryCode: string) => cached(this.provincesCache, countryCode, () => this.api.getProvinces(countryCode));

  cities = (provinceId: string) => cached(this.citiesCache, provinceId, () => this.api.getCities(provinceId));

  request = (numberId: number) => cached(this.requestsCache, numberId, async () => {
    const page = await this.api.getRequests(numberId, 5);
    return page.items.find(request => request.numberId === numberId);
  });

  async sourceId(name: string | null | undefined): Promise<string | undefined> {
    const all = await this.sources();
    const match = isBlank(name) ? undefined : all.find(source => same(source.value, name));
    return (match ?? all.find(source => same(source.value, 'Other')))?.id;
  }

  async resolveCity(city: string, provinceOrState: string, countryCode: string): Promise<CityResolution> {
    const upper = countryCode.trim().toUpperCase();
    const code = upper === 'US' ? 'USA' : upper;
    const all = (await this.provinces(code)).map(pickProvince);
    const province = all.find(item => same(item.code, provinceOrState) || same(item.value, provinceOrState));
    if (!province) return { error: `Province/state '${provinceOrState}' not found in ${code}`, provinces: all };
    const list = await this.cities(province.id);
    const exact = list.find(item => same(item.value, city));
    if (exact) return { city: { id: exact.id, value: exact.value, code: exact.code, province }, province };
    const needle = fold(city);
    const alternatives = list
      .map(item => ({ id: item.id, value: item.value, score: similarity(fold(item.value), needle) }))
      .sort((a, b) => b.score - a.score)
      .slice(0, 5);
    return { province, error: `City '${city}' not found in ${province.value}`, alternatives };
  }
}

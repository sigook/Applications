import type { CatalogCache } from '../api/catalogCache.js';
import type { ExportCache } from '../api/exportCache.js';
import type { SigookApi } from '../api/sigookApi.js';
import type { SigookSession } from '../auth/sigookSession.js';
import type { ImportInstructions } from '../instructions/importInstructions.js';

export interface Services {
  session: SigookSession;
  api: SigookApi;
  catalog: CatalogCache;
  exports: ExportCache;
  instructions: ImportInstructions;
}

export const jsonResult = (value: unknown) => ({
  content: [{ type: 'text' as const, text: JSON.stringify(value) }]
});

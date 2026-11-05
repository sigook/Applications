import { readFileSync } from 'node:fs';
import { fileURLToPath } from 'node:url';

const root = new URL('../../', import.meta.url);

export const instructionsDirectory = fileURLToPath(new URL('instructions/', root));

export const packageVersion: string = (JSON.parse(readFileSync(new URL('package.json', root), 'utf8')) as { version: string }).version;

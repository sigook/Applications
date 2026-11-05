import { createHash } from 'node:crypto';
import { copyFileSync, existsSync, mkdirSync, readdirSync, statSync, writeFileSync } from 'node:fs';
import { basename, dirname, extname, join, resolve } from 'node:path';
import AdmZip from 'adm-zip';
import { dataDirectory } from '../config/environment.js';

const isZip = (file: string): boolean => file.toLowerCase().endsWith('.zip');

function targetFor(source: string, zips: string[]): string {
  const fingerprint = [...zips].sort().map(zip => {
    const stats = statSync(zip);
    return `${zip}:${stats.size}:${stats.mtimeMs}`;
  }).join('|');
  const hash = createHash('sha256').update(source + fingerprint).digest('hex').slice(0, 16).toUpperCase();
  const target = join(dataDirectory(), 'exports', hash);
  mkdirSync(target, { recursive: true });
  return target;
}

function extractIfMissing(zip: string, target: string): void {
  const marker = join(target, `.${basename(zip, extname(zip))}.extracted`);
  if (existsSync(marker)) return;
  new AdmZip(zip).extractAllTo(target, true);
  writeFileSync(marker, new Date().toISOString());
}

export class ExportCache {
  materialize(path: string): string {
    const fullPath = resolve(path);
    if (!existsSync(fullPath)) throw new Error(`Path not found: ${fullPath}`);
    if (statSync(fullPath).isDirectory()) {
      const files = readdirSync(fullPath, { withFileTypes: true }).filter(entry => entry.isFile()).map(entry => join(fullPath, entry.name));
      const zips = files.filter(isZip);
      if (zips.length === 0) return fullPath;
      const target = targetFor(fullPath, zips);
      for (const zip of zips) extractIfMissing(zip, target);
      for (const file of files.filter(item => !isZip(item))) {
        const destination = join(target, basename(file));
        if (!existsSync(destination)) copyFileSync(file, destination);
      }
      return target;
    }
    if (isZip(fullPath)) {
      const target = targetFor(fullPath, [fullPath]);
      extractIfMissing(fullPath, target);
      return target;
    }
    return dirname(fullPath);
  }
}

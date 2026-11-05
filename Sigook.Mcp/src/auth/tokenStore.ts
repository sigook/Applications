import { createCipheriv, createDecipheriv, randomBytes } from 'node:crypto';
import { existsSync, mkdirSync, readFileSync, rmSync, writeFileSync } from 'node:fs';
import { dirname, join } from 'node:path';
import { Entry } from '@napi-rs/keyring';
import { dataDirectory } from '../config/environment.js';

export interface StoredTokens {
  accessToken: string;
  refreshToken?: string | null;
  expiresAt: string;
  environment: string;
}

interface EncryptedEnvelope {
  iv: string;
  tag: string;
  data: string;
}

interface PlainEnvelope {
  plain: StoredTokens;
}

function readOrCreateKey(): Buffer | null {
  try {
    const entry = new Entry('Sigook.Mcp', 'token-key');
    const existing = entry.getPassword();
    if (existing) return Buffer.from(existing, 'base64');
    const key = randomBytes(32);
    entry.setPassword(key.toString('base64'));
    return key;
  } catch (error) {
    if (process.platform === 'win32') throw error;
    return null;
  }
}

export class TokenStore {
  private readonly path: string;

  constructor(environmentName: string) {
    this.path = join(dataDirectory(), 'tokens', `${environmentName}.bin`);
  }

  load(): StoredTokens | null {
    if (!existsSync(this.path)) return null;
    try {
      const envelope = JSON.parse(readFileSync(this.path, 'utf8')) as EncryptedEnvelope | PlainEnvelope;
      if ('plain' in envelope) return envelope.plain;
      const key = readOrCreateKey();
      if (!key) return null;
      const decipher = createDecipheriv('aes-256-gcm', key, Buffer.from(envelope.iv, 'base64'));
      decipher.setAuthTag(Buffer.from(envelope.tag, 'base64'));
      const json = Buffer.concat([decipher.update(Buffer.from(envelope.data, 'base64')), decipher.final()]).toString('utf8');
      return JSON.parse(json) as StoredTokens;
    } catch {
      return null;
    }
  }

  save(tokens: StoredTokens): void {
    mkdirSync(dirname(this.path), { recursive: true });
    writeFileSync(this.path, JSON.stringify(this.seal(tokens)), { mode: 0o600 });
  }

  clear(): void {
    rmSync(this.path, { force: true });
  }

  private seal(tokens: StoredTokens): EncryptedEnvelope | PlainEnvelope {
    const key = readOrCreateKey();
    if (!key) return { plain: tokens };
    const iv = randomBytes(12);
    const cipher = createCipheriv('aes-256-gcm', key, iv);
    const data = Buffer.concat([cipher.update(JSON.stringify(tokens), 'utf8'), cipher.final()]);
    return { iv: iv.toString('base64'), tag: cipher.getAuthTag().toString('base64'), data: data.toString('base64') };
  }
}

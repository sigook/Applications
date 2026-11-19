import assert from 'node:assert/strict';
import { test } from 'node:test';
import type { WorkerDocumentsDetail, WorkerFileModel, WorkerIdentificationModel, WorkerLicenseModel, WorkerSinModel } from '../models/apiModels.js';
import { buildWorkerDocumentPayload, workerDocumentDescription, type WorkerDocumentInput } from './workerDocumentTools.js';

const TYPE_ID = '11111111-1111-1111-1111-111111111111';
const NEW_FILE: WorkerFileModel = { fileName: 'New_abc.pdf', description: 'New' };

const worker = (overrides: Partial<WorkerDocumentsDetail> = {}): WorkerDocumentsDetail => ({
  id: 'w1',
  firstName: 'Ana',
  lastName: 'Diaz',
  socialInsuranceExpire: false,
  havePoliceCheckBackground: false,
  licenses: [],
  certificates: [],
  otherDocuments: [],
  ...overrides
});

const input = (overrides: Partial<WorkerDocumentInput>): WorkerDocumentInput => ({ kind: 'other', replace: false, ...overrides });

function expectOk(detail: WorkerDocumentsDetail, request: WorkerDocumentInput) {
  const payload = buildWorkerDocumentPayload(detail, request, NEW_FILE);
  assert.ok(payload.ok, payload.ok ? '' : payload.error);
  return payload;
}

function expectError(detail: WorkerDocumentsDetail, request: WorkerDocumentInput, fragment: string) {
  const payload = buildWorkerDocumentPayload(detail, request, NEW_FILE);
  assert.equal(payload.ok, false);
  assert.match(payload.ok ? '' : payload.error, new RegExp(fragment));
}

test('description strips invalid characters, caps length and falls back by kind', () => {
  assert.equal(workerDocumentDescription('certificate', 'WHMIS (2026)!'), 'WHMIS 2026');
  assert.equal(workerDocumentDescription('license', 'Driver License Class G Ontario'), 'Driver License Class');
  assert.equal(workerDocumentDescription('sin', '  '), 'SIN');
  assert.equal(workerDocumentDescription('other', null), 'Document');
});

test('identification goes to the first empty slot and keeps the other slot, police check and resume', () => {
  const detail = worker({
    identificationNumber1: 'P123',
    identificationType1: { id: 'type-1', value: 'Passport' },
    identificationType1File: { fileName: 'Id1.pdf', description: 'Passport' },
    havePoliceCheckBackground: true,
    policeCheckBackGround: { fileName: 'Police.pdf', description: 'Police' },
    resume: { fileName: 'Resume.pdf', description: 'Resume' }
  });
  const payload = expectOk(detail, input({ kind: 'identification', identificationTypeId: TYPE_ID, number: 'D999' }));
  assert.equal(payload.section, 'Documents');
  const model = payload.model as WorkerIdentificationModel;
  assert.deepEqual(model.identificationType1File, { fileName: 'Id1.pdf', description: 'Passport' });
  assert.equal(model.identificationNumber1, 'P123');
  assert.deepEqual(model.identificationType1, { id: 'type-1' });
  assert.deepEqual(model.identificationType2File, NEW_FILE);
  assert.deepEqual(model.identificationType2, { id: TYPE_ID });
  assert.equal(model.identificationNumber2, 'D999');
  assert.equal(model.havePoliceCheckBackground, true);
  assert.deepEqual(model.policeCheckBackGround, { fileName: 'Police.pdf', description: 'Police' });
  assert.deepEqual(model.resume, { fileName: 'Resume.pdf', description: 'Resume' });
  assert.equal(payload.replaced, undefined);
});

test('identification requires a type when the slot has none', () => {
  expectError(worker(), input({ kind: 'identification' }), 'identificationTypeId');
});

test('identification refuses an occupied slot unless replace is set', () => {
  const detail = worker({
    identificationNumber1: 'P123',
    identificationType1: { id: 'type-1', value: 'Passport' },
    identificationType1File: { fileName: 'Id1.pdf', description: 'Passport' }
  });
  expectError(detail, input({ kind: 'identification', slot: 1 }), 'replace=true');
  const payload = expectOk(detail, input({ kind: 'identification', slot: 1, replace: true }));
  const model = payload.model as WorkerIdentificationModel;
  assert.deepEqual(model.identificationType1, { id: 'type-1' });
  assert.equal(model.identificationNumber1, 'P123');
  assert.deepEqual(payload.replaced, { fileName: 'Id1.pdf', description: 'Passport' });
});

test('identification with both slots full asks for slot and replace', () => {
  const detail = worker({
    identificationType1File: { fileName: 'Id1.pdf' },
    identificationType2File: { fileName: 'Id2.pdf' }
  });
  expectError(detail, input({ kind: 'identification', identificationTypeId: TYPE_ID }), 'Both identification slots');
});

test('sin keeps the current number and refuses an existing file unless replace is set', () => {
  const detail = worker({ socialInsurance: '123456789', socialInsuranceFile: { fileName: 'Sin.pdf', description: 'SIN' } });
  expectError(detail, input({ kind: 'sin' }), 'replace=true');
  const payload = expectOk(detail, input({ kind: 'sin', replace: true }));
  const model = payload.model as WorkerSinModel;
  assert.equal(payload.section, 'SinInformation');
  assert.equal(model.socialInsurance, '123456789');
  assert.deepEqual(model.socialInsuranceFile, NEW_FILE);
  assert.deepEqual(payload.replaced, { fileName: 'Sin.pdf', description: 'SIN' });
});

test('sin without a number fails', () => {
  expectError(worker(), input({ kind: 'sin' }), 'no SIN number');
});

test('sin that expires needs a due date', () => {
  expectError(worker(), input({ kind: 'sin', number: '123456789', sinExpires: true }), 'dueDate');
});

test('license resends the existing licenses with their data plus the new one', () => {
  const detail = worker({
    licenses: [{ license: { fileName: 'Lic1.pdf', description: 'Forklift' }, number: 'F1', issued: '2025-01-01T00:00:00', expires: '2027-01-01T00:00:00' }]
  });
  const payload = expectOk(detail, input({ kind: 'license', number: 'G2', expires: '2030-05-01' }));
  const model = payload.model as WorkerLicenseModel[];
  assert.equal(payload.section, 'Licenses');
  assert.deepEqual(model, [
    { license: { fileName: 'Lic1.pdf', description: 'Forklift' }, number: 'F1', issued: '2025-01-01T00:00:00', expires: '2027-01-01T00:00:00' },
    { license: NEW_FILE, number: 'G2', issued: undefined, expires: '2030-05-01' }
  ]);
});

test('certificate resends the existing certificates plus the new one', () => {
  const detail = worker({ certificates: [{ fileName: 'Cert1.pdf', description: 'WHMIS' }] });
  const payload = expectOk(detail, input({ kind: 'certificate' }));
  assert.equal(payload.section, 'Certificates');
  assert.deepEqual(payload.model, [{ fileName: 'Cert1.pdf', description: 'WHMIS' }, NEW_FILE]);
});

test('other document posts only the new file', () => {
  const payload = expectOk(worker({ otherDocuments: [{ fileName: 'Old.pdf' }] }), input({ kind: 'other' }));
  assert.equal(payload.section, 'OtherDocument');
  assert.deepEqual(payload.model, NEW_FILE);
});

test('resume refuses an existing resume unless replace is set', () => {
  const detail = worker({ resume: { fileName: 'Resume.pdf', description: 'Resume' } });
  expectError(detail, input({ kind: 'resume' }), 'replace=true');
  const payload = expectOk(detail, input({ kind: 'resume', replace: true }));
  assert.equal(payload.section, 'Resume');
  assert.deepEqual(payload.model, NEW_FILE);
  assert.deepEqual(payload.replaced, { fileName: 'Resume.pdf', description: 'Resume' });
});

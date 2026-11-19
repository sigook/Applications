import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import Clients from '@/modules/agency/recruiting/clients/pages/Clients.vue';
import { bulkAgencyCompanies, deleteAgencyCompany, getAgencyCompanies, getCompanyDeletionCheck } from '@/modules/agency/recruiting/clients/api';
import { getSalesCompanies } from '@/modules/agency/sales/api';
import { downloadAgencyReport } from '@/shared/api/reportApi';
import { getCompanyStatus } from '@/shared/api/catalogApi';
import { createAgencyCompanyNote, deleteAgencyCompanyNote, getAgencyCompanyNotes } from '@/modules/agency/shared/notes/api';
import { useAgencyStore } from '@/modules/agency/store';
import { useAppStore } from '@/app/stores/app';
import { CompanyStatus } from '@/shared/constants/enums';
import { downloadFile } from '@/shared/utils/downloadFile';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyCompanyListItem } from '@/modules/agency/recruiting/clients/types';
import type { NotesCreatePayload, NotesDeletePayload, NotesFetchPayload } from '@/modules/agency/shared/notes/types';

const { route, push, securityStore, stub } = vi.hoisted(() => ({
  route: { path: '/recruiting/companies', params: {}, query: {} },
  push: vi.fn(),
  securityStore: { userRoles: [] as string[] },
  stub: (name: string, props: string[], emits: string[] = []) => ({ name, props, emits, template: `<div class="${name}-stub" />` }),
}));

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => route,
  useRouter: () => ({ push }),
}));
vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => securityStore }));
vi.mock('@/modules/agency/recruiting/clients/api', () => ({
  getAgencyCompanies: vi.fn(),
  bulkAgencyCompanies: vi.fn(),
  getCompanyDeletionCheck: vi.fn(),
  deleteAgencyCompany: vi.fn(),
}));
vi.mock('@/modules/agency/sales/api', () => ({ getSalesCompanies: vi.fn() }));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/api/catalogApi', () => ({ getCompanyStatus: vi.fn() }));
vi.mock('@/modules/agency/shared/notes/api', () => ({
  getAgencyCompanyNotes: vi.fn(),
  createAgencyCompanyNote: vi.fn(),
  deleteAgencyCompanyNote: vi.fn(),
}));
vi.mock('@/shared/utils/downloadFile', () => ({ downloadFile: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertConfirm: vi.fn(), showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/modules/agency/shared/notes/NotesPopover.vue', () => ({
  default: stub('NotesPopover', ['canCreate', 'userId', 'notesCount', 'onGet', 'onCreate', 'onDelete'], ['update:count']),
}));
vi.mock('@/modules/agency/recruiting/candidates/components/BulkData.vue', () => ({
  default: stub('BulkData', ['uploadFn', 'errorFileName', 'title', 'fileLabel'], ['close']),
}));
vi.mock('@/modules/agency/shared/AgencyFilter.vue', () => ({ default: stub('AgencyFilter', ['modelValue'], ['update:modelValue', 'change']) }));

const statuses = [
  { id: CompanyStatus.Lead, value: 'Lead' },
  { id: CompanyStatus.Client, value: 'Client' },
];

type ClientRow = AgencyCompanyListItem;

const acme: ClientRow = {
  id: 'c1',
  agencyId: 'agency-1',
  fullName: 'Acme',
  email: 'ops@acme.com',
  phone: '416 555-0100',
  contactName: 'Jane Doe',
  contactRole: 'HR',
  website: 'https://acme.com',
  industry: 'Logistics',
  salesRepresentative: 'Rita Sales',
  createdAt: '2026-03-10T15:00:00Z',
  createdBy: 'admin@sigook.com',
  updatedAt: '2026-04-10T15:00:00Z',
  updatedBy: 'ops@sigook.com',
  notesCount: 2,
  companyStatus: CompanyStatus.Client,
  locations: ['Toronto', 'Ottawa', 'Hamilton'],
};
const globex: ClientRow = {
  id: 'c2',
  agencyId: 'agency-1',
  fullName: 'Globex',
  email: '',
  phone: '',
  contactName: '',
  contactRole: '',
  website: '',
  industry: 'Retail',
  salesRepresentative: '',
  createdAt: '2026-03-11T15:00:00Z',
  createdBy: '',
  updatedAt: null,
  updatedBy: '',
  notesCount: 0,
  companyStatus: CompanyStatus.Lead,
  locations: [],
};
const legacy: ClientRow = { ...globex, id: 'c3', fullName: 'Legacy', companyStatus: CompanyStatus.Client };

let pinia: Pinia;

async function setup(rows: ClientRow[] = [acme, globex, legacy]) {

  vi.mocked(getAgencyCompanies).mockResolvedValue({ items: rows, totalItems: 42, pageIndex: 1, totalPages: 2 });
  vi.mocked(getSalesCompanies).mockResolvedValue({ items: rows, totalItems: 42, pageIndex: 1, totalPages: 2 });
  const wrapper = mountWithPlugins(Clients, { global: { plugins: [pinia] } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function row(wrapper: Wrapper, index: number) {
  return wrapper.findAll('tbody tr')[index];
}

function cell(wrapper: Wrapper, rowIndex: number, columnIndex: number) {
  return row(wrapper, rowIndex).findAll('td')[columnIndex].text().replace(/\s+/g, ' ');
}

function lastFilter() {
  return vi.mocked(getAgencyCompanies).mock.lastCall?.[0];
}

function listItems() {
  return Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item[role="listitem"]'));
}

async function openRowActions(wrapper: Wrapper, index: number) {
  const before = listItems();
  await row(wrapper, index).find('td:last-child .dropdown').trigger('click');
  await flushPromises();
  return listItems().filter((i) => !before.includes(i)).map((i) => i.textContent.trim());
}

async function openToolbarActions(wrapper: Wrapper) {
  await wrapper.find('.sigook-grid-toolbar .dropdown-trigger').trigger('click');
  await flushPromises();
}

function menuItems() {
  return listItems().map((i) => i.textContent.trim());
}

function menuItem(text: string) {
  const item = Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).find((i) => i.textContent.trim() === text);
  if (!item) throw new Error(`${text} not found`);
  return item;
}

beforeEach(() => {
  pinia = createPinia();
  setActivePinia(pinia);
  route.path = '/recruiting/companies';
  securityStore.userRoles = ['recruiting'];
  vi.mocked(getCompanyStatus).mockResolvedValue(statuses);
});

afterEach(() => {
  vi.useRealTimers();
});

describe('Clients', () => {
  it('loads the recruiting clients newest first and shows the total', async () => {
    const wrapper = await setup();

    expect(lastFilter()).toMatchObject({ sortBy: 3, isDescending: true, pageIndex: 1 });
    expect(getSalesCompanies).not.toHaveBeenCalled();
    expect(wrapper.find('.page-header-title').text()).toBe('Clients 42');
        expect(useAgencyStore().agencyCompanyProfileFilter).toMatchObject({ sortBy: 3, isDescending: true });
  });

  it('loads the sales clients in the sales view', async () => {
    route.path = '/sales/companies';
    await setup();

    expect(getSalesCompanies).toHaveBeenCalledTimes(1);
    expect(getAgencyCompanies).not.toHaveBeenCalled();
  });

  it('restores the saved filter', async () => {
    useAgencyStore().updateAgencyCompanyProfileFilter({
      sortBy: 0,
      isDescending: false,
      businessInfo: 'acme',
      companyStatuses: [CompanyStatus.Client],
      createdAtFrom: '2026-03-01T05:00:00.000Z',
      createdAtTo: '2026-03-31T04:00:00.000Z',
    });
    const wrapper = await setup();

    expect(lastFilter()).toMatchObject({ sortBy: 0, businessInfo: 'acme', companyStatuses: [CompanyStatus.Client] });
    expect(wrapper.findComponent({ name: 'BTaginput' }).props('modelValue')).toEqual([statuses[1]]);
    expect(wrapper.findAllComponents({ name: 'BDatepicker' })[0].props('modelValue')).toEqual([
      new Date('2026-03-01T05:00:00.000Z'),
      new Date('2026-03-31T04:00:00.000Z'),
    ]);
  });

  it('shows the client rows', async () => {
    const wrapper = await setup();

    expect(cell(wrapper, 0, 0)).toContain('Acme TorontoOttawaSee details...');
    expect(row(wrapper, 0).find('a').attributes('href')).toBe('/recruiting/companies/c1');
    expect(cell(wrapper, 0, 1)).toContain('Jane Doe');
    expect(cell(wrapper, 0, 1)).toContain('https://acme.com');
    expect(cell(wrapper, 0, 2)).toBe('Logistics');
    expect(cell(wrapper, 0, 3)).toBe('Rita Sales');
    expect(cell(wrapper, 0, 4)).toContain('admin@sigook.com');
    expect(cell(wrapper, 0, 5)).toContain('ops@sigook.com');
    expect(cell(wrapper, 0, 7)).toBe('Client');

    expect(cell(wrapper, 1, 1)).toBe('No contact name No phoneNo roleNo website');
    expect(cell(wrapper, 1, 4)).toContain('Sigook');
    expect(cell(wrapper, 1, 5)).toBe('No updates');
    expect(cell(wrapper, 1, 7)).toBe('Lead');
    expect(cell(wrapper, 2, 5)).toBe('Existing client');
  });

  it('wires the notes popover to the company notes api', async () => {
    vi.mocked(getAgencyCompanyNotes).mockResolvedValue({ items: [], totalItems: 0, pageIndex: 1, totalPages: 1 });
    vi.mocked(createAgencyCompanyNote).mockResolvedValue({ id: 'n1', note: 'Hi' });
    vi.mocked(deleteAgencyCompanyNote).mockResolvedValue();
    const wrapper = await setup();
    const notes = wrapper.findAllComponents({ name: 'NotesPopover' })[0];
    expect(notes.props()).toMatchObject({ canCreate: false, userId: 'c1', notesCount: 2 });

    const props = notes.props() as {
      onGet: (p: NotesFetchPayload) => Promise<unknown>;
      onCreate: (p: NotesCreatePayload) => Promise<unknown>;
      onDelete: (p: NotesDeletePayload) => Promise<unknown>;
    };
    await props.onGet({ userId: 'c1', pagination: { page: 1, size: 5 } });
    await props.onCreate({ userId: 'c1', model: { note: 'Hi' } });
    await props.onDelete({ userId: 'c1', id: 'n1' });

    expect(getAgencyCompanyNotes).toHaveBeenCalledWith('c1', { page: 1, size: 5 });
    expect(createAgencyCompanyNote).toHaveBeenCalledWith('c1', { note: 'Hi' });
    expect(deleteAgencyCompanyNote).toHaveBeenCalledWith('c1', 'n1');

    notes.vm.$emit('update:count', 5);
    await flushPromises();
    expect(wrapper.findAllComponents({ name: 'NotesPopover' })[0].props('notesCount')).toBe(5);
  });

  it('opens a client when clicking a regular cell, but not contact, notes or actions', async () => {
    const wrapper = await setup();
    await row(wrapper, 1).findAll('td')[2].trigger('click');
    expect(push).toHaveBeenCalledWith({ path: '/recruiting/companies/c2' });

    push.mockClear();
    await row(wrapper, 1).findAll('td')[1].trigger('click');
    await row(wrapper, 1).findAll('td')[6].trigger('click');
    await row(wrapper, 1).findAll('td')[8].trigger('click');
    expect(push).not.toHaveBeenCalled();
  });

  it('filters by agency, status, creation and update dates', async () => {
    const wrapper = await setup();
    wrapper.findComponent({ name: 'AgencyFilter' }).vm.$emit('update:modelValue', 'agency-2');
    wrapper.findComponent({ name: 'AgencyFilter' }).vm.$emit('change');
    await flushPromises();
    expect(lastFilter()).toMatchObject({ agencyIds: 'agency-2', pageIndex: 1 });

    wrapper.findComponent({ name: 'BTaginput' }).vm.$emit('update:modelValue', [statuses[0]]);
    await flushPromises();
    expect(lastFilter()).toMatchObject({ companyStatuses: [CompanyStatus.Lead] });

    const [created, updated] = wrapper.findAllComponents({ name: 'BDatepicker' });
    created.vm.$emit('update:modelValue', [new Date('2026-03-01T05:00:00Z'), new Date('2026-03-31T04:00:00Z')]);
    await flushPromises();
    expect(lastFilter()).toMatchObject({ createdAtFrom: '2026-03-01T05:00:00.000Z', createdAtTo: '2026-03-31T04:00:00.000Z' });

    created.vm.$emit('icon-right-click');
    await flushPromises();
    expect(lastFilter()).toMatchObject({ createdAtFrom: null, createdAtTo: null });

    updated.vm.$emit('update:modelValue', [new Date('2026-04-01T04:00:00Z'), new Date('2026-04-30T04:00:00Z')]);
    await flushPromises();
    expect(lastFilter()).toMatchObject({ updatedAtFrom: '2026-04-01T04:00:00.000Z', updatedAtTo: '2026-04-30T04:00:00.000Z' });

    updated.vm.$emit('icon-right-click');
    await flushPromises();
    expect(lastFilter()).toMatchObject({ updatedAtFrom: null, updatedAtTo: null });
  });

  it('shows the error when the statuses cannot be loaded', async () => {
    const error = new Error('catalog down');
    vi.mocked(getCompanyStatus).mockRejectedValue(error);
    await setup();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('hides some columns on mobile', async () => {
    useAppStore().showMobile();
    const wrapper = await setup();

    expect(wrapper.findAll('thead th').map((th) => th.text())).not.toContain('Industry');
  });

  it('offers create request for clients and delete only to super admins', async () => {
    const wrapper = await setup();
    expect(row(wrapper, 1).find('td:last-child .dropdown').exists()).toBe(false);
    expect(await openRowActions(wrapper, 0)).toEqual(['Create Request']);

    menuItem('Create Request').click();
    expect(push).toHaveBeenCalledWith({ path: '/recruiting/requests/create/c1' });

    securityStore.userRoles = ['superadmin'];
    const superWrapper = await setup();
    expect(await openRowActions(superWrapper, 1)).toEqual(['Delete']);
  });

  it('refuses to delete a client with related records', async () => {
    securityStore.userRoles = ['superadmin'];
    vi.mocked(getCompanyDeletionCheck).mockResolvedValue({
      id: 'c2',
      fullName: 'Globex',
      canDelete: false,
      blockers: [{ entity: 'Requests', count: 3 }, { entity: 'Invoices', count: 1 }],
    });
    const wrapper = await setup();
    await openRowActions(wrapper, 1);
    menuItem('Delete').click();
    await flushPromises();

    expect(getCompanyDeletionCheck).toHaveBeenCalledWith('c2');
    expect(showAlertError).toHaveBeenCalledWith('Globex cannot be deleted because it has related records (Requests: 3, Invoices: 1).');
    expect(showAlertConfirm).not.toHaveBeenCalled();
    expect(deleteAgencyCompany).not.toHaveBeenCalled();
  });

  it('deletes a client after confirming and reloads the grid', async () => {
    securityStore.userRoles = ['superadmin'];
    vi.mocked(getCompanyDeletionCheck).mockResolvedValue({ id: 'c2', fullName: '<i>Globex</i>', canDelete: true, blockers: [] });
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteAgencyCompany).mockResolvedValue();
    const wrapper = await setup();
    await openRowActions(wrapper, 1);
    menuItem('Delete').click();
    await flushPromises();

    expect(showAlertConfirm).toHaveBeenCalledWith(
      'Delete client',
      '&lt;i&gt;Globex&lt;/i&gt; and its users will be permanently deleted. This action cannot be undone.',
      'Delete',
    );
    expect(deleteAgencyCompany).toHaveBeenCalledExactlyOnceWith('c2');
    expect(showAlertSuccess).toHaveBeenCalledWith('<i>Globex</i> was deleted');
    expect(getAgencyCompanies).toHaveBeenCalledTimes(2);
  });

  it('does not delete when the confirmation is declined', async () => {
    securityStore.userRoles = ['superadmin'];
    vi.mocked(getCompanyDeletionCheck).mockResolvedValue({ id: 'c2', fullName: 'Globex', canDelete: true, blockers: [] });
    vi.mocked(showAlertConfirm).mockResolvedValue(false);
    const wrapper = await setup();
    await openRowActions(wrapper, 1);
    menuItem('Delete').click();
    await flushPromises();

    expect(deleteAgencyCompany).not.toHaveBeenCalled();
  });

  it('shows the error when the deletion fails', async () => {
    securityStore.userRoles = ['superadmin'];
    const error = new Error('delete failed');
    vi.mocked(getCompanyDeletionCheck).mockRejectedValue(error);
    const wrapper = await setup();
    await openRowActions(wrapper, 1);
    menuItem('Delete').click();
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(false);
  });

  it('opens the bulk upload with the companies uploader', async () => {
    const wrapper = await setup();
    await openToolbarActions(wrapper);
    expect(menuItems()).toEqual(['Bulk Data', 'Export']);

    menuItem('Bulk Data').click();
    await flushPromises();
    const bulk = wrapper.findComponent({ name: 'BulkData' });
    expect(bulk.props()).toEqual({ uploadFn: bulkAgencyCompanies, errorFileName: 'BulkCompaniesError', title: 'Bulk Companies', fileLabel: 'Companies File' });

    bulk.vm.$emit('close');
    await flushPromises();
    expect(wrapper.findComponent({ name: 'BModal' }).props('modelValue')).toBe(false);
  });

  it('exports with details for admins', async () => {
    securityStore.userRoles = ['admin'];
    vi.useFakeTimers({ now: new Date('2026-11-19T12:00:00'), toFake: ['Date'] });
    const file = new Blob(['xlsx']);
    vi.mocked(downloadAgencyReport).mockResolvedValue(file);
    const wrapper = await setup();
    await openToolbarActions(wrapper);
    menuItem('Export with details').click();
    await flushPromises();

    expect(downloadAgencyReport).toHaveBeenCalledWith('/api/agency/recruiting/clients/FileWithDetails', expect.objectContaining({ sortBy: 3 }));
    expect(downloadFile).toHaveBeenCalledWith(file, `Companies_Details_${new Date().toLocaleDateString()}`);
  });

  it('stops loading when the detailed export fails', async () => {
    securityStore.userRoles = ['admin'];
    vi.mocked(downloadAgencyReport).mockRejectedValue(new Error('export failed'));
    const wrapper = await setup();
    await openToolbarActions(wrapper);
    menuItem('Export with details').click();
    await flushPromises();

    expect(downloadFile).not.toHaveBeenCalled();
    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(false);
  });

  it('exports from the sales endpoint in the sales view', async () => {
    route.path = '/sales/companies';
    vi.mocked(downloadAgencyReport).mockResolvedValue(new Blob(['xlsx']));
    const wrapper = await setup();
    await openToolbarActions(wrapper);
    menuItem('Export').click();
    await flushPromises();

    expect(downloadAgencyReport).toHaveBeenCalledWith('/api/agency/sales/clients/File', expect.objectContaining({ sortBy: 3 }));
  });
});

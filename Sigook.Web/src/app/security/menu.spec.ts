import { describe, expect, it } from 'vitest';
import menu from '@/app/security/menu';
import roles from '@/app/security/roles';
import type { AgencyDetail } from '@/modules/agency/profile/types';

const regularAgency: AgencyDetail = { id: 'a1', fullName: 'Covenant', masterAgency: false };
const masterAgency: AgencyDetail = { id: 'a2', fullName: 'Sigook', masterAgency: true };

function labels(userRoles: string[], agency: AgencyDetail = regularAgency): (string | undefined)[] {
  return menu.getMenu(userRoles, agency).map((group) => group.label);
}

function links(userRoles: string[], agency: AgencyDetail = regularAgency): string[] {
  return menu.getMenu(userRoles, agency).flatMap((group) => group.items.map((item) => item.to));
}

describe('getMenu', () => {
  it.each([roles.superAdmin, roles.admin])('gives %s the accounting, recruiting and sales groups sorted by label', (role) => {
    expect(labels([role])).toEqual(['Accounting', 'Recruiting', 'Sales']);
  });

  it('shows the agencies link to admins of a master agency only', () => {
    expect(links([roles.admin], masterAgency)).toContain('/sales/agencies');
    expect(links([roles.admin], regularAgency)).not.toContain('/sales/agencies');
  });

  it('never shows the agencies link to sales users', () => {
    expect(menu.getMenu([roles.sales], masterAgency)).toEqual([
      {
        label: 'Sales',
        icon: 'cart-outline',
        items: [
          { to: '/sales/dashboard', icon: 'view-dashboard-outline', label: 'Dashboard' },
          { to: '/sales/companies', icon: 'domain', label: 'Clients' },
        ],
      },
    ]);
  });

  it('gives recruiters only the recruiting group', () => {
    expect(labels([roles.recruiting])).toEqual(['Recruiting']);
    expect(links([roles.recruiting])).toEqual([
      '/recruiting/requests',
      '/recruiting/weekly-board',
      '/recruiting/applicants',
      '/recruiting/runners',
      '/recruiting/candidates',
      '/recruiting/workers',
      '/recruiting/companies',
    ]);
  });

  it('combines the groups of several roles', () => {
    expect(labels([roles.sales, roles.recruiting])).toEqual(['Recruiting', 'Sales']);
  });

  it('gives company owners requests and accounting', () => {
    expect(links([roles.company])).toEqual(['/company-requests', '/company-invoices']);
  });

  it('gives company users only requests', () => {
    expect(links([roles.companyUser])).toEqual(['/company-requests']);
  });

  it('gives workers jobs and activity', () => {
    expect(links([roles.worker])).toEqual(['/worker-requests', '/worker-activity']);
  });

  it('puts unlabelled groups first when mixed with labelled ones', () => {
    expect(labels([roles.recruiting, roles.worker])).toEqual([undefined, 'Recruiting']);
  });

  it('returns nothing for unknown or missing roles', () => {
    expect(menu.getMenu(['guest'], regularAgency)).toEqual([]);
    expect(menu.getMenu([], regularAgency)).toEqual([]);
  });

  it('returns fresh groups each time', () => {
    const first = menu.salesMenu(masterAgency, true);
    const second = menu.salesMenu(masterAgency, true);
    expect(first.items).toHaveLength(3);
    expect(second.items).toHaveLength(3);
    expect(first).not.toBe(second);
  });
});

describe('getDefaultHomePageUrlBaseOnRoles', () => {
  it.each([
    [[roles.superAdmin], '/recruiting/requests'],
    [[roles.admin], '/recruiting/requests'],
    [[roles.recruiting], '/recruiting/weekly-board'],
    [[roles.sales], '/sales/requests'],
    [[roles.company], '/company-requests'],
    [[roles.companyUser], '/company-requests'],
    [[roles.worker], '/worker-requests'],
  ])('%s -> %s', (userRoles, url) => {
    expect(menu.getDefaultHomePageUrlBaseOnRoles(userRoles)).toBe(url);
  });

  it('uses the first known role', () => {
    expect(menu.getDefaultHomePageUrlBaseOnRoles(['guest', roles.sales, roles.admin])).toBe('/sales/requests');
  });

  it('sends users without a known role to unauthorized', () => {
    expect(menu.getDefaultHomePageUrlBaseOnRoles([])).toBe('/unauthorized');
    expect(menu.getDefaultHomePageUrlBaseOnRoles(['guest'])).toBe('/unauthorized');
  });
});

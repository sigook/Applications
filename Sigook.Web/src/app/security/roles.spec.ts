import { describe, expect, it } from 'vitest';
import roles, { adminAccess, agencyStaff, recruitingAccess, roleLabels, salesAccess } from '@/app/security/roles';

describe('roles', () => {
  it('uses the backend role claims', () => {
    expect(roles).toEqual({
      superAdmin: 'superadmin',
      admin: 'admin',
      recruiting: 'recruiting',
      sales: 'sales',
      company: 'company',
      companyUser: 'company.user',
      worker: 'worker',
    });
  });

  it('grants recruiting to admins and recruiters only', () => {
    expect(recruitingAccess).toEqual(['superadmin', 'admin', 'recruiting']);
  });

  it('grants sales to admins and sales only', () => {
    expect(salesAccess).toEqual(['superadmin', 'admin', 'sales']);
  });

  it('limits admin access to admin roles', () => {
    expect(adminAccess).toEqual(['superadmin', 'admin']);
  });

  it('treats every agency role as staff and excludes company and worker', () => {
    expect(agencyStaff).toEqual(['superadmin', 'admin', 'recruiting', 'sales']);
    expect(agencyStaff).not.toContain(roles.company);
    expect(agencyStaff).not.toContain(roles.companyUser);
    expect(agencyStaff).not.toContain(roles.worker);
  });

  it('labels only agency staff roles', () => {
    expect(roleLabels).toEqual({
      superadmin: 'Super Admin',
      admin: 'Admin',
      recruiting: 'Recruiting',
      sales: 'Sales',
    });
    expect(roleLabels[roles.worker]).toBeUndefined();
  });
});

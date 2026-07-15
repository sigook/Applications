const roles = {
  superAdmin: "superadmin",
  admin: "admin",
  recruiting: "recruiting",
  sales: "sales",
  company: "company",
  companyUser: "company.user",
  worker: "worker"
} as const;

// Mirrors Covenant.Common/Constants/CovenantConstants.cs -> Role. Keep both sides in sync.
export const roleGroups = {
  agencyAccess: [roles.superAdmin, roles.admin, roles.recruiting],
  agencyStaff: [roles.superAdmin, roles.admin, roles.recruiting, roles.sales],
  salesAccess: [roles.superAdmin, roles.admin, roles.sales],
  accounting: [roles.superAdmin, roles.admin]
} as const;

export function hasAnyRole(userRoles: string[], group: readonly string[]): boolean {
  return group.some((role) => userRoles.includes(role));
}

export default roles;

const roles = {
  superAdmin: "superadmin",
  admin: "admin",
  recruiting: "recruiting",
  sales: "sales",
  company: "company",
  companyUser: "company.user",
  worker: "worker"
} as const;

export const recruitingAccess: string[] = [roles.superAdmin, roles.admin, roles.recruiting];

export const agencyStaff: string[] = [...recruitingAccess, roles.sales];

export const salesAccess: string[] = [roles.superAdmin, roles.admin, roles.sales];

export const accountingAccess: string[] = [roles.superAdmin, roles.admin];

export const administrationAccess: string[] = [roles.superAdmin, roles.admin];

export const roleLabels: Record<string, string> = {
  [roles.superAdmin]: "Super Admin",
  [roles.admin]: "Admin",
  [roles.recruiting]: "Recruiting",
  [roles.sales]: "Sales",
};

export default roles;

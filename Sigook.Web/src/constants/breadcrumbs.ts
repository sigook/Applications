import type { PageBreadcrumb } from '@/types/common';

export const accountingCrumbs: PageBreadcrumb[] = [{ label: 'Accounting' }];

export const invoicesCrumbs: PageBreadcrumb[] = [
  ...accountingCrumbs,
  { label: 'Invoices', to: '/accounting/invoices' },
];

export const payStubsCrumbs: PageBreadcrumb[] = [
  ...accountingCrumbs,
  { label: 'PayStubs', to: '/accounting/paystubs' },
];

export const agenciesCrumbs: PageBreadcrumb[] = [
  { label: 'Sales' },
  { label: 'Agencies', to: '/sales/agencies' },
];

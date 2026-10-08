import type { AgencyTypeOption } from '@/modules/agency/profile/types';

export const appGlobals = {
  $statusOpen: 'Open',
  $statusFilled: 'Filled',
  $statusCancelled: 'Cancelled',

  $statusDisplayOpen: 'Open',
  $statusDisplayFilled: 'Filled',
  $statusDisplayCancelled: 'Cancelled',

  $statusApply: 'Applied',
  $statusDecline: 'Declined',
  $statusReject: 'Rejected',
  $statusBook: 'Booked',
  $statusInQueue: 'InQueue',

  $regexAddress: /^[-.# a-zA-Z0-9]+$/,

  $longTerm: 'LongTerm',

  $agencyTypeMaster: 1,

  $agencyTypes: [
    { value: 2, label: 'Regular' },
    { value: 3, label: 'Business Partner' },
  ] as AgencyTypeOption[],
};

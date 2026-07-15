import roles, { hasAnyRole, roleGroups } from "@/security/roles";
import { useBillingAdmin } from "@/composables/useBillingAdmin";
import type { AgencyDetail } from "@/types/agency";

interface MenuLink {
  to: string;
  icon?: string;
  label: string;
  external?: boolean;
}

interface MenuGroup {
  label?: string;
  icon?: string;
  items: MenuLink[];
}

export default {
  getMenu(userRoles: string[], agency: AgencyDetail): MenuGroup[] {
    const result: MenuGroup[] = [];
    if (hasAnyRole(userRoles, roleGroups.agencyAccess)) {
      result.push(this.recruitingMenu());
    }
    if (hasAnyRole(userRoles, roleGroups.salesAccess)) {
      result.push(this.salesMenu(agency));
    }
    if (hasAnyRole(userRoles, roleGroups.accounting)) {
      result.push(this.agencyBillingMenu(agency));
    }
    if (userRoles.includes(roles.company)) {
      result.push(...this.companyMenu());
    }
    if (userRoles.includes(roles.companyUser)) {
      result.push(...this.companyUserMenu());
    }
    if (userRoles.includes(roles.worker)) {
      result.push(...this.workerMenu());
    }
    result.sort((a, b) => (a.label ?? "").localeCompare(b.label ?? ""));
    return result;
  },
  recruitingMenu(): MenuGroup {
    const recruiting: MenuGroup = {
      label: "Recruiting",
      icon: "account-search",
      items: [
        {
          to: "/recruiting/requests",
          icon: "calendar-month",
          label: "Requests",
        },
        {
          to: "/recruiting/weekly-board",
          icon: "view-week",
          label: "Weekly Board",
        },
        {
          to: "/recruiting/attendance-review",
          icon: "clipboard-check-outline",
          label: "Attendance Review",
        },
        {
          to: "/recruiting/candidates",
          icon: "account-hard-hat",
          label: "Candidates",
        },
        {
          to: "/recruiting/workers",
          icon: "badge-account-outline",
          label: "Workers",
        },
        {
          to: "/recruiting/companies",
          icon: "domain",
          label: "Clients",
        },
      ],
    };
    return recruiting;
  },
  salesMenu(agency: AgencyDetail): MenuGroup {
    const sales: MenuGroup = {
      label: "Sales",
      icon: "cart-outline",
      items: [
        {
          to: "/sales/dashboard",
          icon: "view-dashboard-outline",
          label: "Dashboard",
        },
        {
          to: "/sales/companies",
          icon: "domain",
          label: "Clients",
        },
      ],
    };
    if (agency.masterAgency) {
      sales.items.push({
        to: "/sales/agencies",
        icon: "handshake-outline",
        label: "Agencies",
      });
    }
    return sales;
  },
  agencyBillingMenu(agency: AgencyDetail): MenuGroup {
    const accounting: MenuGroup = {
      label: "Accounting",
      icon: "finance",
      items: [
        {
          to: "/accounting/invoices",
          icon: "invoice-list-outline",
          label: "Invoices",
        },
        {
          to: "/accounting/reports",
          icon: "file-chart-outline",
          label: "Reports",
        },
      ],
    };
    if (!agency.usaAgency) {
      accounting.items.push({
        to: "/accounting/paystubs",
        icon: "cash-multiple",
        label: "Pay Stubs",
      });
    }
    return accounting;
  },
  companyMenu(): MenuGroup[] {
    return [
      {
        items: [
          {
            to: "/company-requests",
            icon: "calendar-month",
            label: "Staff Requests",
          },
          {
            to: "/company-invoices",
            icon: "finance",
            label: "Accounting",
          },
        ],
      },
    ];
  },
  companyUserMenu(): MenuGroup[] {
    return [
      {
        items: [
          {
            to: "/company-requests",
            icon: "calendar-month",
            label: "Staff Requests",
          },
        ],
      },
    ];
  },
  workerMenu(): MenuGroup[] {
    return [
      {
        items: [
          {
            to: "/worker-requests",
            icon: "calendar-month",
            label: "Jobs available for you",
          },
          {
            to: "/worker-history",
            icon: "history",
            label: "History",
          },
        ],
      },
    ];
  },
  getDefaultHomePageUrlBaseOnRoles(userRoles: string[]): string {
    const { isPayrollManager } = useBillingAdmin();
    if (hasAnyRole(userRoles, roleGroups.agencyAccess)) {
      return isPayrollManager.value ? "/recruiting/requests" : "/recruiting/weekly-board";
    }
    if (userRoles.includes(roles.sales)) {
      return "/sales/dashboard";
    }
    if (hasAnyRole(userRoles, [roles.company, roles.companyUser])) {
      return "/company-requests";
    }
    if (userRoles.includes(roles.worker)) {
      return "/worker-requests";
    }
    return "/unauthorized";
  },
};

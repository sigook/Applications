import roles from "@/security/roles";

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
  getMenu(userRoles: string[], agency: any): MenuGroup[] {
    const result: MenuGroup[] = [];
    for (let i = 0; i < userRoles.length; i++) {
      switch (userRoles[i]) {
        case roles.agencyPersonnel:
          result.push(...this.agencyMenu(agency));
          if (
            userRoles.some((ur: string) => ur === roles.payroll || ur === roles.admin)
          ) {
            result.push(this.agencyBillingMenu(agency));
          }
          break;
        case roles.company:
          result.push(...this.companyMenu());
          break;
        case roles.companyUser:
          result.push(...this.companyUserMenu());
          break;
        case roles.worker:
          result.push(...this.workerMenu());
          break;
      }
    }
    result.sort((a, b) => (a.label ?? "").localeCompare(b.label ?? ""));
    return result;
  },
  agencyMenu(agency: any): MenuGroup[] {
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
    const sales: MenuGroup = {
      label: "Sales",
      icon: "cart-outline",
      items: [
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
        icon: "warehouse",
        label: "Agencies",
      });
    }
    return [recruiting, sales];
  },
  agencyBillingMenu(agency: any): MenuGroup {
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
    for (let i = 0; i < userRoles.length; i++) {
      switch (userRoles[i]) {
        case roles.agencyPersonnel:
        case roles.agency:
          return "/recruiting/requests";
        case roles.company:
        case roles.companyUser:
          return "/company-requests";
        case roles.worker:
          return "/worker-requests";
      }
    }
    return "/unauthorized";
  },
};

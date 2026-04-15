import roles from "@/security/roles";

interface MenuItem {
  to: string;
  icon?: string;
  label: string;
  items?: { to: string; label: string }[];
}

export default {
  getMenu(userRoles: string[], agency: any): MenuItem[] {
    const result: MenuItem[] = [];
    for (let i = 0; i < userRoles.length; i++) {
      switch (userRoles[i]) {
        case roles.agencyPersonnel:
          result.push(...this.agencyMenu(agency));
          if (
            userRoles.some((ur: string) => ur === roles.payroll || ur === roles.admin)
          ) {
            result.push(...this.agencyBillingMenu(agency));
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
    return result;
  },
  agencyMenu(agency: any): MenuItem[] {
    const menus: MenuItem[] = [
      {
        to: "/agency-requests",
        icon: "calendar-month",
        label: "Orders",
      },
      {
        to: "/agency-candidates",
        icon: "account-group",
        label: "Candidates",
      },
      {
        to: "/agency-workers",
        icon: "badge-account-outline",
        label: "Workers",
      },
      {
        to: "/agency-companies",
        icon: "domain",
        label: "Clients",
      },
    ];
    if (agency.masterAgency) {
      menus.push({
        to: "/agency-agencies",
        icon: "office-building",
        label: "Agencies",
      });
    }
    return menus;
  },
  agencyBillingMenu(agency: any): MenuItem[] {
    const root: MenuItem = {
      to: "/accounting",
      icon: "finance",
      label: "Accounting",
      items: [],
    };
    root.items?.push(
      {
        to: "/invoices",
        label: "Invoices",
      },
      {
        to: "/reports",
        label: "Reports",
      }
    );
    const menus: MenuItem[] = [root];
    if (!agency.usaAgency) {
      root.items?.push({
        to: "/paystubs",
        label: "Pay Stubs",
      });
    }
    return menus;
  },
  companyMenu(): MenuItem[] {
    return [
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
    ];
  },
  companyUserMenu(): MenuItem[] {
    return [
      {
        to: "/company-requests",
        icon: "calendar-month",
        label: "Staff Requests",
      },
    ];
  },
  workerMenu(): MenuItem[] {
    return [
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
    ];
  },
  getDefaultHomePageUrlBaseOnRoles(userRoles: string[]): string {
    for (let i = 0; i < userRoles.length; i++) {
      switch (userRoles[i]) {
        case roles.agencyPersonnel:
        case roles.agency:
          return "/agency-requests";
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

import roles from "@/security/roles";

export default {
  getMenu(userRoles, agency) {
    let result = [];
    for (let i = 0; i < userRoles.length; i++) {
      switch (userRoles[i]) {
        case roles.agencyPersonnel:
          result.push(...this.agencyMenu(agency));
          if (
            userRoles.some((ur) => ur === roles.payroll || ur === roles.admin)
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
  agencyMenu(agency) {
    const menus = [
      {
        to: "/agency-requests",
        icon: "calendar-month",
        label: "MenuRequestsAgency",
      },
      {
        to: "/agency-candidates",
        icon: "account-group",
        label: "MenuCandidatesAgency",
      },
      {
        to: "/agency-workers",
        icon: "badge-account-outline",
        label: "MenuWorkersAgency",
      },
      {
        to: "/agency-companies",
        icon: "domain",
        label: "MenuCompanies",
      },
    ];
    if (agency.masterAgency) {
      menus.push({
        to: "/agency-agencies",
        icon: "office-building",
        label: "MenuAgencies",
      });
    }
    return menus;
  },
  agencyBillingMenu(agency) {
    const root = {
      to: "/accounting",
      icon: "finance",
      label: "MenuBilling",
      items: [],
    };
    root.items.push(
      {
        to: "/invoices",
        label: "Invoices",
      },
      {
        to: "/reports",
        label: "Reports",
      }
    );
    const menus = [root];
    if (!agency.usaAgency) {
      root.items.push({
        to: "/paystubs",
        label: "Paystubs",
      });
    }
    return menus;
  },
  companyMenu() {
    return [
      {
        to: "/company-requests",
        icon: "calendar-month",
        label: "MenuCompaniesRequests",
      },
      {
        to: "/company-invoices",
        icon: "finance",
        label: "MenuBilling",
      },
    ];
  },
  companyUserMenu() {
    return [
      {
        to: "/company-requests",
        icon: "calendar-month",
        label: "MenuCompaniesRequests",
      },
    ];
  },
  workerMenu() {
    return [
      {
        to: "/worker-requests",
        icon: "calendar-month",
        label: "MenuJobs",
      },
      {
        to: "/worker-history",
        icon: "history",
        label: "MenuHistory",
      },
      {
        to: "/worker-payroll",
        icon: "cash-multiple",
        label: "MenuPayRoll",
      },
    ];
  },
  getDefaultHomePageUrlBaseOnRoles(userRoles) {
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

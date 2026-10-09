export interface CompanyContactPersonModel {
  id?: string;
  companyProfileId?: string;
  title: string;
  firstName: string;
  middleName: string;
  lastName: string;
  position: string;
  mobileNumber: string;
  officeNumber: string;
  officeNumberExt: number | null;
  email: string;
}

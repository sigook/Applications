export interface CompanyContactPersonModel {
  id?: string;
  companyProfileId?: string;
  title: string;
  firstName: string;
  middleName: string | null;
  lastName: string;
  position: string;
  mobileNumber: string;
  officeNumber: string;
  officeNumberExt: number | null;
  email: string;
}

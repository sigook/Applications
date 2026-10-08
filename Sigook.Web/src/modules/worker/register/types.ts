export interface WorkerFileObjects {
  profileImage: File | null;
  identificationType1: File | null;
  identificationType2: File | null;
  licenses: File[];
  certificates: File[];
  resume: File | null;
  otherDocuments: File[];
}

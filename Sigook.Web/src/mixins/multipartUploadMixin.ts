import compress from './compressFiles';

interface FileObjects {
  profileImage?: File | null;
  identificationType1?: File | null;
  identificationType2?: File | null;
  licenses?: File[];
  certificates?: File[];
  resume?: File | null;
  otherDocuments?: File[];
}

interface GeneratedFileNames {
  profileImage: string | null;
  identificationType1: string | null;
  identificationType2: string | null;
  licenses: string[];
  certificates: string[];
  resume: string | null;
  otherDocuments: string[];
}

export default {
  methods: {
    generateGuidWithoutDashes(): string {
      if (typeof crypto !== 'undefined' && crypto.randomUUID) {
        return crypto.randomUUID().replace(/-/g, '');
      }

      const guid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        const r = Math.random() * 16 | 0;
        const v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
      });
      return guid.replace(/-/g, '');
    },

    getFileExtension(filename: string): string {
      const lastDot = filename.lastIndexOf('.');
      return lastDot !== -1 ? filename.substring(lastDot) : '';
    },

    generateFileName(prefix: string, originalFileName: string): string {
      const guid = (this as any).generateGuidWithoutDashes();
      const extension = (this as any).getFileExtension(originalFileName);
      return `${prefix}_${guid}${extension}`;
    },

    async createMultipartFormData(worker: any, fileObjects: FileObjects): Promise<FormData> {
      const vm = this as any;
      const formData = new FormData();

      const generatedFileNames: GeneratedFileNames = {
        profileImage: null,
        identificationType1: null,
        identificationType2: null,
        licenses: [],
        certificates: [],
        resume: null,
        otherDocuments: []
      };

      if (fileObjects.profileImage) {
        generatedFileNames.profileImage = vm.generateFileName('ProfileImage', fileObjects.profileImage.name);
      }

      if (fileObjects.identificationType1) {
        generatedFileNames.identificationType1 = vm.generateFileName('Document', fileObjects.identificationType1.name);
      }

      if (fileObjects.identificationType2) {
        generatedFileNames.identificationType2 = vm.generateFileName('Document', fileObjects.identificationType2.name);
      }

      if (fileObjects.licenses && fileObjects.licenses.length > 0) {
        generatedFileNames.licenses = fileObjects.licenses.map((file: File) =>
          vm.generateFileName('License', file.name)
        );
      }

      if (fileObjects.certificates && fileObjects.certificates.length > 0) {
        generatedFileNames.certificates = fileObjects.certificates.map((file: File) =>
          vm.generateFileName('Certificate', file.name)
        );
      }

      if (fileObjects.resume) {
        generatedFileNames.resume = vm.generateFileName('Resume', fileObjects.resume.name);
      }

      if (fileObjects.otherDocuments && fileObjects.otherDocuments.length > 0) {
        generatedFileNames.otherDocuments = fileObjects.otherDocuments.map((file: File) =>
          vm.generateFileName('OtherDocument', file.name)
        );
      }

      const workerData: Record<string, any> = {
        firstName: worker.firstName,
        lastName: worker.lastName,
        birthDay: worker.birthDay,
        gender: worker.gender,
        email: worker.email,
        password: worker.password,
        confirmPassword: worker.confirmPassword,
        location: worker.location,
        mobileNumber: worker.mobileNumber,
        availabilities: worker.availabilities,
        availabilityTimes: worker.availabilityTimes,
        availabilityDays: worker.availabilityDays,
        skills: worker.skills,
        languages: worker.languages,
        lift: worker.lift,
        hasVehicle: worker.hasVehicle,
        agreeTermsAndConditions: worker.agreeTermsAndConditions,
        identificationType1: worker.identificationType1,
        identificationNumber1: worker.identificationNumber1,
        identificationType2: worker.identificationType2,
        identificationNumber2: worker.identificationNumber2,
      };

      if (generatedFileNames.profileImage) {
        workerData.profileImage = {
          fileName: generatedFileNames.profileImage
        };
      }

      if (generatedFileNames.identificationType1) {
        workerData.identificationType1File = {
          fileName: generatedFileNames.identificationType1,
          description: worker.identificationType1File?.description || ""
        };
      }

      if (generatedFileNames.identificationType2) {
        workerData.identificationType2File = {
          fileName: generatedFileNames.identificationType2,
          description: worker.identificationType2File?.description || ""
        };
      }

      if (worker.licenses && worker.licenses.length > 0) {
        workerData.licenses = worker.licenses.map((l: any, index: number) => ({
          license: {
            fileName: generatedFileNames.licenses[index],
            description: l.license.description
          },
          number: l.number,
          issued: l.issued,
          expires: l.expires
        }));
      }

      if (worker.certificates && worker.certificates.length > 0) {
        workerData.certificates = worker.certificates.map((c: any, index: number) => ({
          fileName: generatedFileNames.certificates[index],
          description: c.description
        }));
      }

      if (worker.resume) {
        workerData.resume = {
          fileName: generatedFileNames.resume
        };
      }

      if (worker.otherDocuments && worker.otherDocuments.length > 0) {
        workerData.otherDocuments = worker.otherDocuments.map((d: any, index: number) => ({
          fileName: generatedFileNames.otherDocuments[index],
          description: d.description
        }));
      }

      formData.append('data', JSON.stringify(workerData));

      if (fileObjects.profileImage && generatedFileNames.profileImage) {
        try {
          const compressedImage = await vm.compressFile(fileObjects.profileImage);
          formData.append(generatedFileNames.profileImage, compressedImage, generatedFileNames.profileImage);
        } catch (error) {
          formData.append(generatedFileNames.profileImage, fileObjects.profileImage, generatedFileNames.profileImage);
        }
      }

      if (fileObjects.identificationType1 && generatedFileNames.identificationType1) {
        formData.append(generatedFileNames.identificationType1, fileObjects.identificationType1, generatedFileNames.identificationType1);
      }

      if (fileObjects.identificationType2 && generatedFileNames.identificationType2) {
        formData.append(generatedFileNames.identificationType2, fileObjects.identificationType2, generatedFileNames.identificationType2);
      }

      if (fileObjects.licenses && fileObjects.licenses.length > 0) {
        fileObjects.licenses.forEach((file: File, index: number) => {
          formData.append(generatedFileNames.licenses[index], file, generatedFileNames.licenses[index]);
        });
      }

      if (fileObjects.certificates && fileObjects.certificates.length > 0) {
        fileObjects.certificates.forEach((file: File, index: number) => {
          formData.append(generatedFileNames.certificates[index], file, generatedFileNames.certificates[index]);
        });
      }

      if (fileObjects.resume && generatedFileNames.resume) {
        formData.append(generatedFileNames.resume, fileObjects.resume, generatedFileNames.resume);
      }

      if (fileObjects.otherDocuments && fileObjects.otherDocuments.length > 0) {
        fileObjects.otherDocuments.forEach((file: File, index: number) => {
          formData.append(generatedFileNames.otherDocuments[index], file, generatedFileNames.otherDocuments[index]);
        });
      }

      return formData;
    }
  },
  mixins: [compress]
};

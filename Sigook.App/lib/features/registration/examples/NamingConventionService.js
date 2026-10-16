import compress from './compressFiles';

export default {
  methods: {
    generateGuidWithoutDashes() {
      if (typeof crypto !== 'undefined' && crypto.randomUUID) {
        return crypto.randomUUID().replace(/-/g, '');
      }

      const guid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        const r = Math.random() * 16 | 0;
        const v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
      });
      return guid.replace(/-/g, '');
    },

    getFileExtension(filename) {
      const lastDot = filename.lastIndexOf('.');
      return lastDot !== -1 ? filename.substring(lastDot) : '';
    },

    generateFileName(prefix, originalFileName) {
      const guid = this.generateGuidWithoutDashes();
      const extension = this.getFileExtension(originalFileName);
      return `${prefix}_${guid}${extension}`;
    },

    async createMultipartFormData(worker, fileObjects) {
      const formData = new FormData();

      const generatedFileNames = {
        profileImage: null,
        identificationType1: null,
        identificationType2: null,
        licenses: [],
        certificates: [],
        resume: null,
        otherDocuments: []
      };

      if (fileObjects.profileImage) {
        generatedFileNames.profileImage = this.generateFileName('ProfileImage', fileObjects.profileImage.name);
      }

      if (fileObjects.identificationType1) {
        generatedFileNames.identificationType1 = this.generateFileName('Document', fileObjects.identificationType1.name);
      }

      if (fileObjects.identificationType2) {
        generatedFileNames.identificationType2 = this.generateFileName('Document', fileObjects.identificationType2.name);
      }

      if (fileObjects.licenses && fileObjects.licenses.length > 0) {
        generatedFileNames.licenses = fileObjects.licenses.map(file =>
          this.generateFileName('License', file.name)
        );
      }

      if (fileObjects.certificates && fileObjects.certificates.length > 0) {
        generatedFileNames.certificates = fileObjects.certificates.map(file =>
          this.generateFileName('Certificate', file.name)
        );
      }

      if (fileObjects.resume) {
        generatedFileNames.resume = this.generateFileName('Resume', fileObjects.resume.name);
      }

      if (fileObjects.otherDocuments && fileObjects.otherDocuments.length > 0) {
        generatedFileNames.otherDocuments = fileObjects.otherDocuments.map(file =>
          this.generateFileName('OtherDocument', file.name)
        );
      }

      const workerData = {
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
        workerData.licenses = worker.licenses.map((l, index) => ({
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
        workerData.certificates = worker.certificates.map((c, index) => ({
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
        workerData.otherDocuments = worker.otherDocuments.map((d, index) => ({
          fileName: generatedFileNames.otherDocuments[index],
          description: d.description
        }));
      }

      formData.append('data', JSON.stringify(workerData));

      if (fileObjects.profileImage) {
        try {
          const compressedImage = await this.compressFile(fileObjects.profileImage);
          formData.append(generatedFileNames.profileImage, compressedImage, generatedFileNames.profileImage);
        } catch (error) {
          formData.append(generatedFileNames.profileImage, fileObjects.profileImage, generatedFileNames.profileImage);
        }
      }

      if (fileObjects.identificationType1) {
        formData.append(generatedFileNames.identificationType1, fileObjects.identificationType1, generatedFileNames.identificationType1);
      }

      if (fileObjects.identificationType2) {
        formData.append(generatedFileNames.identificationType2, fileObjects.identificationType2, generatedFileNames.identificationType2);
      }

      if (fileObjects.licenses && fileObjects.licenses.length > 0) {
        fileObjects.licenses.forEach((file, index) => {
          formData.append(generatedFileNames.licenses[index], file, generatedFileNames.licenses[index]);
        });
      }

      if (fileObjects.certificates && fileObjects.certificates.length > 0) {
        fileObjects.certificates.forEach((file, index) => {
          formData.append(generatedFileNames.certificates[index], file, generatedFileNames.certificates[index]);
        });
      }

      if (fileObjects.resume) {
        formData.append(generatedFileNames.resume, fileObjects.resume, generatedFileNames.resume);
      }

      if (fileObjects.otherDocuments && fileObjects.otherDocuments.length > 0) {
        fileObjects.otherDocuments.forEach((file, index) => {
          formData.append(generatedFileNames.otherDocuments[index], file, generatedFileNames.otherDocuments[index]);
        });
      }

      return formData;
    }
  },
  mixins: [compress]
};
 
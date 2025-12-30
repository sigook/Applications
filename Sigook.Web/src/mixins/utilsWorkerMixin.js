export default {
    data(){
        return {
            experiences: [],
            licenses: [],
            certificates: [],
            resume: null,
            filteredSkills : [],
            filteredLanguages : []
        }
    },
    methods: {
        addTag (newTag) {
            const tag = {
                id: null,
                value: newTag
            };
            this.filteredSkills.push(tag);
        },
        validateExperience(index){
            this.$validator.validateAll()
                .then(results => {
                    if(!results){
                        this.errors.items.forEach(item => {
                            let iName = item.field.includes(index);
                            if(iName){
                                return false;
                            }
                        });
                    } else {
                        return true;
                    }
                });
            let current = this.$validator.fields.filter(x => x.name.includes(index));
            return new Promise((resolve) => {
                Promise.all(current.map(field => this.$validator.validate(field)))
                    .then((validatedFields) => {
                        let isValidForm = true;
                        validatedFields.forEach(item => {
                            if(item.valid === false){
                                isValidForm = false;
                            }
                        });
                        resolve(isValidForm);
                    });
            });
        },
        addExperience(){
            this.experiences.push({
                id: null,
                companyName: '',
                title: '',
                startDate: null,
                endDate: null,
                currentJob: false,
                description: ''
            });
        },
        addLicense(file){
            this.licenses.push({
                id: null,
                fileName: file.fileName
            });
        },
        addCertificate(file){
            this.certificates.push({
                id: null,
                fileName: file.fileName
            });
        },
        deleteLicense(index){
            this.licenses.splice(index, 1);
        },
        deleteCertificate(index){
            this.certificates.splice(index, 1);
        },
        verifyAllCurrentJob(index){
            this.experiences.forEach((item, indexItem) => {
                if(index !== indexItem){
                    item.currentJob = false;
                }
            });
        },
        disableEndDate(index){
            this.experiences.forEach((item, indexItem) => {
                if(index === indexItem){
                    if(item.currentJob){
                        item.endDate = null;
                    }
                }
            });
        },
        removeExperience(index){
            this.experiences.splice(index, 1);
        },
        addResume(file){
            this.resume = {
                id: null,
                fileName: file.fileName
            };
        },
        deleteResume(){
            this.resume = null;
        }
    },
    async created(){
        this.filteredSkills = await this.$store.dispatch('getSkills');
        this.filteredLanguages = await this.$store.dispatch('getLanguages');
    }
}; 
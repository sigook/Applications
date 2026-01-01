<template>
    <div class="form-section form-100">
        <div>
            <div class="form-50 sm-form-100">
                <label class="fz-1 fw-400 sign-required">{{$t('WorkerCompany')}}  </label>
                <b-field :type="errors.has('company') ? 'is-danger' : ''" :label="$t('Company')"
                          :message="errors.has('company') ? errors.first('company') : ''">
                    <b-input type="text"
                             class="input-border input-block"
                             v-model="workExperience.company"
                             :name="'company'"
                             v-validate="'required|max:50|min:2'" />
                </b-field>
            </div>
            <div class="form-50 sm-form-100">
                <label class="fz-1 fw-400 sign-required">{{$t('WorkerSupervisor')}} </label>
                <b-field :type="errors.has('supervisor') ? 'is-danger' : ''" :label="$t('Supervisor')"
                          :message="errors.has('supervisor') ? errors.first('supervisor') : ''">
                    <b-input type="text"
                             class="input-border input-block"
                             v-model="workExperience.supervisor"
                             :name="'supervisor'"
                             v-validate="'required|max:60|min:2'" />
                </b-field>
            </div>

            <div class="form-100">
                <label class="fz-1 fw-400 sign-required">{{$t('WorkerDuties')}}</label>
                <b-field :type="errors.has('duties') ? 'is-danger' : ''" :label="$t('Duties')"
                          :message="errors.has('duties') ? errors.first('duties') : ''">
                    <textarea
                            class="textarea-border textarea-almost100"
                            v-model="workExperience.duties"
                            :name="'duties'"
                            v-validate="'required|max:5000'" />
                </b-field>
            </div>

            <div class="form-50 sm-form-100">
                <label class="fz-1 fw-400 sign-required">{{$t('WorkerStartDate')}}
                </label>

                <b-field :type="errors.has('start date') ? 'is-danger' : ''" :label="$t('StartDate')"
                          :message="errors.has('start date') ? errors.first('start date') : ''">
                    <b-datepicker class="input-block"
                                  v-model="workExperience.startDate"
                                  :name="'start date'"
                                  v-validate="'required'"
                                  :max-date="disableStartDate"
                                  position="is-top-right">
                    </b-datepicker>
                </b-field>
            </div>

            <div class="form-25 sm-form-100">
                <label class="fz-1 fw-400">{{ $t('CurrentJob') }}</label>
                <div class="contain-radio">
                    <label><input type="radio" :name="'currentJob'" :value="true" v-model="workExperience.isCurrentJobPosition"/>{{ $t("Yes")}}</label>
                    <label><input type="radio" :name="'currentJob'" :value="false" v-model="workExperience.isCurrentJobPosition"/>{{ $t("No")}}</label>
                </div>
            </div>

            <div class="form-25 sm-form-100" v-if="!workExperience.isCurrentJobPosition">
                <label class="fz-1 fw-400 sign-required">{{$t('WorkerEndDate')}}</label>
                <b-field :type="errors.has('end date') ? 'is-danger' : ''" :label="$t('EndDate')"
                          :message="errors.has('end date') ? errors.first('end date') : ''">
                    <b-datepicker class="input-block"
                                  v-model="workExperience.endDate"
                                  :name="'end date'"
                                  v-validate="'required'"
                                  :max-date="disableStartDate"
                                  :min-date="workExperience.startDate"
                                  position="is-top-right">
                    </b-datepicker>
                </b-field>
            </div>

            <br>
            <button class="background-btn md-btn primary-button btn-radius margin-top-15"
                    @click="validateAll()" type="button">SAVE</button>
        </div>
    </div>
</template>

<script>
    import toast from '../../mixins/toastMixin';
    export default {
        props: ['workerId', 'data'],
        data() {
            return {
                disableStartDate: null,
                workExperience: {
                    company: "",
                    supervisor: "",
                    duties: "",
                    startDate: null,
                    endDate: null,
                    isCurrentJobPosition: true
                }
            }
        },
        mixins: [toast],
        methods: {
            validateAll(){
                this.$validator.validateAll().then((isValid) => {
                    if (isValid) {
                        this.editWorkerWorkExperience();
                        return;
                    }
                    this.showAlertError(this.$t('PleaseVerifyThatTheFieldsAreCorrect'));
                });
            },
            editWorkerWorkExperience(){
                this.$store.dispatch("worker/editWorkerWorkExperience", {profileId: this.workerId, id: this.data.id, model: this.workExperience})
                    .then(() => {
                        this.$emit("updateExperience", true);
                    })
                    .catch(error => {
                        this.showAlertError(error);
                    })
            },
            updateData(){
                this.workExperience = Object.assign({}, this.data);
                //this.workExperience = this.data;
                this.workExperience.startDate = new Date(this.data.startDate);
                this.workExperience.endDate = this.data.endDate ? new Date(this.data.endDate) : null;
            }
        },
        created(){
            this.$store.dispatch('getCurrentDate').then(response => {
                this.disableStartDate = response;
            });
            this.updateData();
        },
    }
</script>
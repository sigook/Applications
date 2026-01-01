<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <form @submit.prevent="createCandidate" class="p-3">
      <b-tabs v-model="activeTab" position="is-centered">
        <b-tab-item label="New Applicant">
          <b-steps animated mobile-mode="compact">
            <b-step-item step="1" label="Basic">
              <div class="container-flex">
                <div class="col-12 col-padding">
                  <b-field :type="errors.has('fullName') ? 'is-danger' : ''" label="Full Name"
                    :message="errors.has('fullName') ? errors.first('fullName') : ''">
                    <b-input v-model="candidate.fullName" name="fullName" has-counter maxLength="50"
                      v-validate="{ required: isNewApplicantTab, max: 50 }"></b-input>
                  </b-field>
                </div>
                <div class="col-12 col-padding">
                  <b-field :type="errors.has('email') ? 'is-danger' : ''" label="Email"
                    :message="errors.has('email') ? errors.first('email') : ''">
                    <b-input type="email" v-model="candidate.email" has-counter name="email" maxLength="50"
                      v-validate="{ email: true, required: isNewApplicantTab, max: 50 }"></b-input>
                  </b-field>
                </div>
                <div class="col-12 col-padding">
                  <phone-input model="Phone" :required="isNewApplicantTab" :defaultValue="candidate.phone"
                    @formattedPhone="(phone) => candidate.phone = phone"></phone-input>
                </div>
                <div class="col-12 col-padding">
                  <b-field :type="errors.has('country') ? 'is-danger' : ''" label="Country"
                    :message="errors.has('country') ? errors.first('country') : ''">
                    <b-select v-model="candidate.countryId" expanded name="country"
                      v-validate="{ required: isNewApplicantTab }">
                      <option v-for="country in countries" :key="country.id" :value="country.id">{{ country.value }}
                      </option>
                    </b-select>
                  </b-field>
                </div>
                <div class="col-12 col-padding">
                  <b-field :type="errors.has('address') ? 'is-danger' : ''" label="City"
                    :message="errors.has('address') ? errors.first('address') : ''">
                    <b-input v-model="candidate.address" name="address" maxLength="50" has-counter
                      v-validate="{ required: isNewApplicantTab, max: 50 }"></b-input>
                  </b-field>
                </div>
              </div>
            </b-step-item>

            <b-step-item step="2" label="Additionals">
              <div class="container-flex">
                <div class="col-12 col-padding">
                  <b-field :type="errors.has('status') ? 'is-danger' : ''" label="Status"
                    :message="errors.has('status') ? errors.first('status') : ''">
                    <b-select v-model="candidate.status" name="status" expanded
                      v-validate="{ required: isNewApplicantTab }">
                      <option value="Citizen">Citizen</option>
                      <option value="Work Permit">Work Permit</option>
                      <option value="Student">Student</option>
                      <option value="Permanent Resident">Permanent Resident</option>
                    </b-select>
                  </b-field>
                </div>
                <div class="col-12 col-padding">
                  <b-field label="Resume">
                    <b-field class="file is-primary" :class="{ 'has-name': !!file }">
                      <b-upload v-model="file" class="file-label" accept=".pdf,.doc,.docx" @input="uploadResume"
                        rounded>
                        <span class="file-cta">
                          <b-icon class="file-icon" icon="upload"></b-icon>
                          <span class="file-label">{{ file ? file.name : "Click to upload" }}</span>
                        </span>
                      </b-upload>
                    </b-field>
                  </b-field>
                </div>
                <div class="col-12 col-padding">
                  <b-field :type="errors.has('skills') ? 'is-danger' : ''"
                    label="Roles of Interest (you may type more that one)"
                    :message="errors.has('skills') ? errors.first('skills') : ''">
                    <b-taginput v-model="candidate.skills" open-on-focus icon="label" :maxlength="50" ellipsis
                      placeholder="Select or Add Skill" allow-new name="skills">
                    </b-taginput>
                  </b-field>
                </div>

                <div class="col-12 col-padding">
                  <b-field label="Transportation">
                    <b-switch v-model="candidate.hasVehicle" :true-value="true" :false-value="false">
                      {{ candidate.hasVehicle ? "Own" : "Public" }}
                    </b-switch>
                  </b-field>
                </div>

                <div class="col-12 col-padding">
                  <b-field :type="errors.has('termsAndConditions') ? 'is-danger' : ''"
                    :message="errors.has('termsAndConditions') ? errors.first('termsAndConditions') : ''">
                    <b-checkbox v-model="termnsAndConditions" v-validate="{ required: isNewApplicantTab }"
                      name="termsAndConditions" class="mt-3">
                      I agree
                      <router-link to="/terms-and-conditions" target="_blank">Terms and Conditions</router-link>
                      &
                      <router-link to="/privacy-policy" target="_blank">Privacy Policy</router-link>
                    </b-checkbox>
                  </b-field>
                </div>
              </div>
            </b-step-item>
          </b-steps>
        </b-tab-item>
        <b-tab-item label="Already Registered">
          <div class="container-flex">
            <div class="col-12 col-padding">
              <b-field :type="errors.has('emailRegistered') ? 'is-danger' : ''" label="Email"
                :message="errors.has('emailRegistered') ? errors.first('emailRegistered') : ''">
                <b-input type="email" v-model="candidate.email" maxlength="50" has-counter name="emailRegistered"
                  v-validate="{ email: true, required: !isNewApplicantTab, max: 50 }" ></b-input>
              </b-field>
            </div>
          </div>
        </b-tab-item>
      </b-tabs>

      <div class="container-flex">
        <div class="col-12 col-padding">
          <b-field position="is-right">
            <b-button type="is-primary is-light" native-type="button" class="mr-1" @click="close()">Close</b-button>
            <b-button type="is-primary" native-type="submit"
              :disabled="errors.items.length > 0 || !termnsAndConditions">Save Changes</b-button>
          </b-field>
        </div>
      </div>
    </form>
  </div>
</template>

<script>
import updateMixin from "@/mixins/uploadFiles";

export default {
  props: ['jobToApply'],
  mixins: [updateMixin],
  components: {
    phoneInput: () => import("@/components/PhoneInput")
  },
  data() {
    return {
      activeTab: 0,
      isLoading: false,
      countries: [],
      skill: null,
      skillsSelected: [],
      alreadyRegistered: false,
      file: null,
      candidate: {},
      termnsAndConditions: false
    }
  },
  async created() {
    this.countries = await this.$store.dispatch("getCountries");
  },
  methods: {
    async createCandidate() {
      const result = await this.$validator.validateAll();
      if (result) {
        this.isLoading = true;
        if (this.jobToApply) {
          this.candidate.requestId = this.jobToApply.requestId;
        }
        await this.$store.dispatch('createCandidate', this.candidate);
        this.$emit('candidateCreated');
        this.isLoading = false;
      }
    },
    async close() {
      if (this.candidate.resume) {
        await this.deleteFile(this.candidate.fileName);
      }
      this.$emit('candidateCreated');
    },
    async uploadResume(file) {
      this.isLoading = true;
      const response = await this.uploadFile(file, 'document', 'Resume_');
      this.candidate.fileName = response;
      this.isLoading = false;
    }
  },
  computed: {
    isNewApplicantTab() {
      return this.activeTab === 0;
    }
  },
  watch: {
    activeTab(newValue) {
      this.termnsAndConditions = !this.isNewApplicantTab;
    }
  }
}

</script>
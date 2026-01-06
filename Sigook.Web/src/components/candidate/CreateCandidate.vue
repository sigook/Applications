<template>
  <div class="p-3 white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <form @submit.prevent="validateForm">
      <b-steps animated mobile-mode="compact">
        <b-step-item step="1" label="Basic">
          <div class="container-flex">
            <div class="col-12">
              <b-field label="Full Name" :type="errors.has('full name') ? 'is-danger' : ''"
                :message="errors.has('full name') ? errors.first('full name') : ''">
                <b-input type="text" v-model="candidate.name" name="full name" v-validate="'required|max:60|min:2'" />
              </b-field>
            </div>
            <div class="col-12">
              <b-field :type="errors.has('email') ? 'is-danger' : ''" label="Email"
                :message="errors.has('email') ? errors.first('email') : ''">
                <b-input type="email" v-model="candidate.email" name="email"
                  v-validate="'required|email|max:50|min:6'" />
              </b-field>
            </div>
            <div class="col-12">
              <phone-input ref="phoneComponent" :required="true" model="Phone" :defaultValue="phoneNumber"
                @formattedPhone="(phone) => phoneNumber = phone"></phone-input>
            </div>
            <div class="col-12">
              <b-field :type="errors.has('address') ? 'is-danger' : ''" label="Address"
                :message="errors.has('address') ? errors.first('address') : ''">
                <b-input type="text" v-model="candidate.address" name="address" v-validate="'required|max:100|min:2'" />
              </b-field>
            </div>
            <div class="col-12 mt-5">
              <b-field class="file is-primary" :class="{ 'has-name': !!file }">
                <b-upload v-model="file" class="file-label" accept=".pdf,.doc,.docx" @input="uploadResume" rounded>
                  <span class="file-cta">
                    <b-icon class="file-icon" icon="upload"></b-icon>
                    <span class="file-label">{{ file ? file.name : "Click to upload" }}</span>
                  </span>
                </b-upload>
              </b-field>
            </div>
          </div>
        </b-step-item>
        <b-step-item step="2" label="Additionals">
          <div class="col-12">
            <b-field label="Skills (Press enter to add)" :type="errors.has('skills') ? 'is-danger' : ''">
              <b-taginput v-model="candidate.skills" :maxlength="20" open-on-focus icon="label" placeholder="Add Skills" allow-new>
              </b-taginput>
            </b-field>
          </div>
          <div class="col-12">
            <b-field label="Status">
              <b-select v-model="candidate.residencyStatus" expanded placeholder="Select a residency status">
                <option v-for="(item, index) in residencyList" :key="'residency' + index" :value="item">{{ item }}
                </option>
              </b-select>
            </b-field>
          </div>
          <div class="col-12">
            <b-field label="Source">
              <b-select v-model="candidate.source" expanded placeholder="Select a source">
                <option v-for="(item, index) in sourceList" :value="item" :key="'sourceItem' + index">{{ item }}
                </option>
              </b-select>
            </b-field>
          </div>
          <div class="col-12">
            <b-field label="Gender">
              <b-select v-model="candidate.gender" expanded placeholder="Select a gender">
                <option v-for="item in genders" v-bind:key="item.id" :value="item">{{ item.value }}</option>
              </b-select>
            </b-field>
          </div>
          <div class="col-12">
            <b-field label="Has Vehicle">
              <b-switch v-model="candidate.hasVehicle" :true-value="true" :false-value="false">
                {{ candidate.hasVehicle ? $t("Yes") : $t("No") }}
              </b-switch>
            </b-field>
          </div>
          <div class="col-12 mt-5">
            <b-button type="is-primary" native-type="submit">{{ $t('Create') }}</b-button>
          </div>
        </b-step-item>
      </b-steps>
    </form>
  </div>
</template>
<script>
import updateMixin from "@/mixins/uploadFiles";

export default {
  data() {
    return {
      isLoading: false,
      showMoreInformation: false,
      genders: [],
      skills: [],
      skill: "",
      phoneNumber: "",
      candidate: {
        name: null,
        email: null,
        phoneNumbers: [],
        skills: [],
        source: null,
        gender: null,
        hasVehicle: false,
        address: null,
        postalCode: null,
        residencyStatus: null,
      },
      file: null
    }
  },
  mixins: [updateMixin],
  components: {
    phoneInput: () => import("@/components/PhoneInput")
  },
  methods: {
    async validateForm() {
      const mainFormValid = await this.$validator.validateAll();
      const phoneValid = await this.$refs.phoneComponent.validatePhone();
      if (mainFormValid && phoneValid) {
        this.createAgencyCandidate();
        return;
      } else {
        this.showAlertError(this.$t("PleaseVerifyThatTheFieldsAreCorrect"));
      }
    },
    createAgencyCandidate() {
      this.isLoading = true;
      this.candidate.skills = this.candidate.skills.map(s => ({ skill: s }));
      if (this.phoneNumber != null) {
        this.candidate.phoneNumbers = [{ phoneNumber: this.phoneNumber }]
      }
      this.$store.dispatch('agency/createAgencyCandidate', this.candidate)
        .then(() => {
          this.isLoading = false;
          this.showAlertSuccess("Created")
          this.$emit('onClose', true);
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error)
        })
    },
    async uploadResume(file) {
      this.isLoading = true;
      const response = await this.uploadFile(file, 'document', 'Resume_');
      this.candidate.fileName = response;
      this.isLoading = false;
    }
  },
  async created() {
    this.genders = await this.$store.dispatch('getGenders')
  },
  computed: {
    residencyList() {
      return this.$store.state.catalog.residencyList
    },
    sourceList() {
      return this.$store.state.catalog.sourceList
    }
  }
}
</script>
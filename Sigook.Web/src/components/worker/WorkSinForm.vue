<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12">
        <b-field :label="$t('WorkerSocialInsuranceNumber')"
          :type="errors.has('social insurance #') ? 'is-danger' : ''"
          :message="errors.has('social insurance #') ? errors.first('social insurance #') : ''">
          <b-input type="text" v-model="sin.socialInsurance" name="social insurance #"
            v-validate="'required|max:15|min:9'">
          </b-input>
        </b-field>
      </div>
      <div class="col-12">
        <b-field :label="$t('WorkerSocialInsuranceFile')">
          <div class="input-file-edited input-block" v-if="sin.socialInsuranceFile && sin.socialInsuranceFile.fileName">
            <span>
              {{ sin.socialInsuranceFile.fileName | filename }}
            </span>
            <button v-if="sin.socialInsuranceFile" @click="deleteWorkerSin()" class="button cross-button"
              type="button" />
          </div>
          <upload-file v-if="!sin.socialInsuranceFile || sin.socialInsuranceFile.fileName === ''"
            class="mtop5 inline-100" @fileSelected="file => updateSocialInsurance(file)" :format="'document'"
            :name="'SIN-SSN_'" :required="false" />
        </b-field>
      </div>
      <div class="col-12">
        <b-field :label="$t('Expire')" class="has-text-weight-normal">
          <b-switch v-model="sin.socialInsuranceExpire" :true-value="true" :false-value="false">
            {{ sin.socialInsuranceExpire ? $t("Yes") : $t("No") }}
          </b-switch>
        </b-field>
      </div>
      <div class="col-12" v-if="sin.socialInsuranceExpire === true">
        <b-field :label="$t('WorkerDueDate')" :type="errors.has('due date') ? 'is-danger' : ''"
          :message="errors.has('due date') ? errors.first('due date') : ''">
          <b-datepicker v-model="sin.dueDate" name="due date" v-validate="'required'" append-to-body position="is-top-right">
          </b-datepicker>
        </b-field>
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="validateAll()">
          {{ $t("Save") }}
        </b-button>
      </div>
    </div>
  </div>
</template>

<script>
import updateMixin from "../../mixins/uploadFiles";

export default {
  props: ['data'],
  mixins: [updateMixin],
  data() {
    return {
      isLoading: false,
      sin: {
        socialInsurance: "",
        socialInsuranceExpire: false,
        dueDate: null,
        socialInsuranceFile: {
          fileName: "",
          description: ""
        }
      }
    }
  },
  components: {
    socialInsuranceEdit: () => import("./WorkSinForm"),
    UploadFile: () => import("../UploadFiles")
  },
  methods: {
    updateSocialInsurance(file) {
      if (this.sin.socialInsuranceFile) {
        this.sin.socialInsuranceFile.fileName = file
      } else {
        this.sin.socialInsuranceFile = {
          fileName: file,
          description: ""
        }
      }
    },
    validateAll() {
      this.$validator.validateAll().then((isValid) => {
        if (isValid) {
          this.createWorkerSin();
          return;
        }
        this.showAlertError(this.$t('PleaseVerifyThatTheFieldsAreCorrect'));
      });
    },
    createWorkerSin() {
      this.isLoading = true;
      this.$store.dispatch('worker/createWorkerSin', { profileId: this.data.id, model: this.sin })
        .then(() => {
          this.isLoading = false;
          this.$emit('closeModal', true);
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    },
    deleteWorkerSin() {
      this.deleteFile(this.sin.socialInsuranceFile.fileName)
        .then(() => this.sin.socialInsuranceFile = null);
    }
  },
  created() {
    if (this.data.socialInsurance !== null && this.data.socialInsurance !== "") {
      this.sin = Object.assign({}, this.data);
      this.sin.dueDate = this.data.dueDate ? new Date(this.sin.dueDate) : null;
    }
  }
}
</script>
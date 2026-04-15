<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="errors.has('company') ? 'is-danger' : ''" :label="'Company'"
          :message="errors.has('company') ? errors.first('company') : ''">
          <b-input type="text" v-model="workExperience.company" :name="'company'"
            v-validate="'required|max:50|min:2'" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="errors.has('supervisor') ? 'is-danger' : ''" :label="'Supervisor'"
          :message="errors.has('supervisor') ? errors.first('supervisor') : ''">
          <b-input type="text" v-model="workExperience.supervisor" :name="'supervisor'"
            v-validate="'required|max:60|min:2'" />
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-field :type="errors.has('duties') ? 'is-danger' : ''" :label="'Duties'"
          :message="errors.has('duties') ? errors.first('duties') : ''">
          <b-input type="textarea" v-model="workExperience.duties" :name="'duties'" v-validate="'required|max:5000'" />
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-field :label="'Current Job'">
          <b-switch v-model="workExperience.isCurrentJobPosition" :name="'isCurrentJobPosition'" :true-value="true"
            :false-value="false">
            {{ workExperience.isCurrentJobPosition ? 'Yes' : 'No' }}
          </b-switch>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="errors.has('startDate') ? 'is-danger' : ''" :label="'Start date'"
          :message="errors.has('startDate') ? errors.first('startDate') : ''">
          <b-datepicker v-model="workExperience.startDate" :name="'startDate'" v-validate="'required'"
            :max-date="disableStartDate" append-to-body position="is-top-right">
          </b-datepicker>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding" v-if="!workExperience.isCurrentJobPosition">
        <b-field :type="errors.has('endDate') ? 'is-danger' : ''" :label="'End date'"
          :message="errors.has('endDate') ? errors.first('endDate') : ''">
          <b-datepicker v-model="workExperience.endDate" :name="'endDate'" v-validate="'required'"
            :max-date="disableStartDate" :min-date="workExperience.startDate" append-to-body position="is-top-right">
          </b-datepicker>
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-button type="is-primary" @click="validateAll">
          {{ "Save" }}
        </b-button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { mapStores } from 'pinia';
import { useAppStore } from '@/stores/app';
import { showAlertError } from "@/utils/toast";
import { createWorkerWorkExperience, editWorkerWorkExperience } from '@/api/workerApi';
export default {
  props: ['workerId', 'data'],
  data() {
    return {
      isLoading: false,
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
  computed: {
    ...mapStores(useAppStore),
  },
  methods: {
    validateAll() {
      this.$validator.validateAll().then(async (isValid) => {
        if (isValid) {
          if (this.data) {
            this.editWorkerWorkExperience()
          } else {
            this.createWorkerWorkExperience();
          }
          return;
        }
        showAlertError('Please make sure all required fields are filled out correctly');
      });
    },
    createWorkerWorkExperience() {
      this.isLoading = true;
      createWorkerWorkExperience(this.workerId, this.workExperience)
        .then(() => {
          this.isLoading = false;
          this.$emit("updateExperience");
        })
        .catch(error => {
          this.isLoading = false;
          showAlertError(error);
        })
    },
    editWorkerWorkExperience() {
      this.isLoading = true;
      editWorkerWorkExperience(this.workerId, this.data.id, this.workExperience)
        .then(() => {
          this.isLoading = false;
          this.$emit("updateExperience");
        })
        .catch(error => {
          this.isLoading = false;
          showAlertError(error);
        })
    },
    updateData() {
      this.workExperience = Object.assign({}, this.data);
      this.workExperience.startDate = new Date(this.data.startDate);
      this.workExperience.endDate = this.data.endDate ? new Date(this.data.endDate) : null;
    }
  },
  created() {
    this.appStore.getCurrentDate().then(response => {
      this.disableStartDate = response;
    });
    if (this.data) {
      this.updateData();
    }
  }
}
</script>

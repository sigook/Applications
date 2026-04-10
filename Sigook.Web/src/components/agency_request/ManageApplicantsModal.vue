<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="p-3">
      <div class="container-flex">
        <div class="col-12 col-padding">
          <b-field label="Worker/Candidate" :type="errors.has('applicant') ? 'is-danger' : ''"
            :message="errors.has('applicant') ? errors.first('applicant') : 'Type at least 3 characters to search'">
            <b-autocomplete v-model="searchText" :data="applicants" placeholder="Search by name, email or ID..."
              name="applicant" append-to-body :loading="isLoadingList" @typing="onSearchInput" @select="selectApplicant"
              :custom-formatter="(option) => `#${option.numberId} | ${option.name} | ${option.email || 'No Email' } | ${option.type}`">
              <template slot-scope="props">
                <div class="d-flex justify-content-between align-items-center">
                  <div>
                    <strong>#{{ props.option.numberId }}</strong>
                    <span class="ml-2">{{ props.option.name }}</span>
                    <small class="ml-2 color-gray-light" v-if="props.option.email">{{ props.option.email }}</small>
                  </div>
                  <span class="tag-sm-gray ml-2">{{ props.option.type }}</span>
                </div>
              </template>
            </b-autocomplete>
          </b-field>
        </div>
        <div class="col-12 mt-5">
          <b-button type="is-primary" @click="saveApplicant">Add Applicant</b-button>
        </div>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import { searchAgencyRequestApplicants } from "@/api/agencyRequestApi";
export default {
  data() {
    return {
      isLoading: false,
      isLoadingList: false,
      requestId: this.$route.params.id,
      searchText: '',
      applicants: [],
      model: {
        workerProfileId: null,
        candidateId: null,
        comments: null
      }
    }
  },
  methods: {
    onSearchInput(text) {
      if (text.length >= 3) {
        this.searchApplicants(text);
      } else {
        this.applicants = [];
      }
    },
    searchApplicants(text) {
      this.isLoadingList = true;
      searchAgencyRequestApplicants(this.requestId, text)
        .then(response => {
          this.isLoadingList = false;
          this.applicants = response;
        })
        .catch(error => {
          this.isLoadingList = false;
          this.showAlertError(error);
        });
    },
    selectApplicant(item) {
      if (!item) return;
      this.model = {
        workerProfileId: item.workerProfileId || null,
        candidateId: item.candidateId || null
      };
    },
    saveApplicant() {
      this.$emit("updateApplicants", { model: this.model });
    }
  }
}
</script>

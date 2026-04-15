<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12 col-padding">
        <b-field label="OrderID, Position">
          <b-autocomplete :data="rows" placeholder="OrderID, Position" :loading="isLoadingList"
            :custom-formatter="(option) => `${option.numberId} | ${option.jobTitle} | ${option.companyFullName}`"
            @typing="onInputEntered" @select="(option) => optionSelected = option" append-to-body>
            <template v-slot="props">
              <small>
                {{ props.option.numberId }} |
                {{ props.option.jobTitle }} |
                {{ props.option.companyFullName }}
              </small>
            </template>
          </b-autocomplete>
        </b-field>
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="saveRequestApplicant">Save</b-button>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import { showAlertError } from "@/utils/toast";
import { getAllAgencyRequests, postAgencyRequestApplicant } from "@/api/agencyRequestApi";
export default {
  props: ['candidateId'],
  data() {
    return {
      isLoading: false,
      isLoadingList: false,
      rows: [],
      serverParams: {
        statuses: [0, 1, 4]
      },
      optionSelected: null
    }
  },
  methods: {
    onInputEntered(value) {
      this.serverParams.filter = value;
      this.loadRequests();
    },
    loadRequests() {
      this.isLoadingList = true;
      getAllAgencyRequests(this.serverParams)
        .then((response) => {
          this.isLoadingList = false;
          this.rows = response;
        })
        .catch(error => {
          this.isLoadingList = false;
          showAlertError(error);
        })
    },
    saveRequestApplicant() {
      this.isLoading = true;
      postAgencyRequestApplicant(this.optionSelected.id, { candidateId: this.candidateId })
        .then(() => {
          this.isLoading = false;
          this.$emit('onSelectRequest');
        }).catch((error) => {
          this.isLoading = false;
          showAlertError(error)
        })
    }
  }
}
</script>
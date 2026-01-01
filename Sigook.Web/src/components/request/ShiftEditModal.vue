<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <shift-form v-if="shift" :current-shift="shift" :is-update="true" @updateModel="(val) => shift = val" />
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="updateAgencyRequestShift(shift)">Save</b-button>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  data() {
    return {
      isLoading: false,
      shift: null,
      requestId: this.$route.params.id
    }
  },
  components: {
    ShiftForm: () => import("../../components/request/ShiftsForm")
  },
  created() {
    this.getRequestShift();
  },
  methods: {
    getRequestShift() {
      this.isLoading = true;
      this.$store.dispatch('getRequestShift', this.requestId)
        .then(response => {
          this.isLoading = false;
          console.log(response);
          this.shift = response;
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    },
    updateAgencyRequestShift(model) {
      console.log(model);
      this.isLoading = true;
      this.$store.dispatch('agency/updateAgencyRequestShift', { requestId: this.requestId, model })
        .then(response => {
          this.isLoading = false;
          this.$emit("onUpdateShift", response.displayShift)
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error)
        })
    }
  }
}
</script>
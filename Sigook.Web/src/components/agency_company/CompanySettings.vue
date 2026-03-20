<template>
  <div class="mt-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <b-checkbox class="col-2" v-model="localCompany.requiresPermissionToSeeOrders" @input="updateRequiresPermissionToSee">
        Requires permission to see orders?
      </b-checkbox>
      <b-checkbox class="col-2" v-model="localCompany.paidHolidays" @input="updatePaidHolidays">
        Paid Holidays?
      </b-checkbox>
      <span class="line-gray"></span>
      <div class="col-4">
        <b-field label="Overtime Starts After">
          <b-input v-model="localCompany.overtimeStartsAfter"></b-input>
          <b-button type="is-primary is-light" @click="updateOvertime">Save</b-button>
        </b-field>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
export default {
  props: ["company"],
  data() {
    return {
      isLoading: false,
      localCompany: JSON.parse(JSON.stringify(this.company))
    }
  },
  watch: {
    company: {
      handler(newVal) {
        this.localCompany = JSON.parse(JSON.stringify(newVal));
      },
      deep: true
    }
  },
  methods: {
    update(action) {
      this.isLoading = true;
      this.$emit('update:company', this.localCompany);
      this.$store.dispatch(`agency/${action}`, { companyId: this.localCompany.id, settings: this.localCompany })
        .then(() => this.isLoading = false)
        .catch((error) => {
          this.showAlertError(error);
          this.isLoading = false;
        });
    },
    updateRequiresPermissionToSee() {
      this.update("updatePermissionToSeeOrders");
    },
    updatePaidHolidays() {
      this.update("updatePaidHolidays");
    },
    updateOvertime() {
      this.update("updateOvertime");
    }
  }
}
</script>
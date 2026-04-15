<template>
  <div class="white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <div class="section-top-title container-flex mb-1">
      <h2 class="fz1 pt-3 col-8 col-md-6 col-sm-12">
        <ul class="tabs-basic tabs-home d-inline-block">
          <li class="active">
            {{ "Orders" }}
            <span class="fw-100 fz-1">({{ totalItems }})</span>
          </li>
          <li>
            <router-link to="/board">
              Weekly Board
            </router-link>
          </li>
        </ul>
      </h2>
    </div>
    <div>
      <table-requests @onDataLoading="(value) => isLoading = value" v-model:total-items="totalItems" />
    </div>
  </div>
</template>

<script lang="ts">

import { defineAsyncComponent } from 'vue';
export default {
  data() {
    return {
      isLoading: true,
      totalItems: 0,
    };
  },
  components: {
    TableRequests: defineAsyncComponent(() => import("../../components/agency_request/TableRequests.vue")),
  }
};
</script>


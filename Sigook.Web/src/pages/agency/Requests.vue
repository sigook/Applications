<template>
  <div class="white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <div class="section-top-title container-flex mb-5">
      <h2 class="fz1 pt-3 col-6 col-md-5 col-sm-7">
        Orders
        <span class="fw-100 fz-1">
          ({{ totalItems }})
        </span>
      </h2>
    </div>
    <div>
      <table-requests @onDataLoading="(value) => isLoading = value" v-model:total-items="totalItems" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import TableRequests from '@/components/agency_request/TableRequests.vue';

const isLoading = ref(true);
const totalItems = ref(0);
</script>

<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="section-top-title columns is-multiline mb-5">
      <h2 class="fz1 pt-3 column is-7-mobile is-5">
        Requests
        <span class="has-text-weight-light fz-1">
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

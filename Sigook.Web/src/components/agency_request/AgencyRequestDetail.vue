<template>
  <div class="columns is-multiline detail-split">
    <section class="column is-9 detail-split-main">
      <request-detail :request="request" @update:request="emit('update:request', $event)" @refreshRequest="emit('refreshRequest')" />
    </section>
    <aside class="column is-3 detail-split-aside">
      <notes :canEdit="request.canEdit" />
      <location :jobLocation="request.jobLocation" />
      <requested-by class="mt-4" :canEdit="request.canEdit" :requestId="request.id"
        :companyProfileId="request.companyProfileId" />
      <report-to class="mt-3" :canEdit="request.canEdit" :requestId="request.id" :companyProfileId="request.companyProfileId" />
      <div class="mt-3">
        <h3 class="has-text-weight-bold">Created by</h3>
        <span class="is-inline-block valign-middle">
          {{ emailName(request.createdBy) }} {{ dateFromNow(request.createdAt) }}
        </span>
      </div>
    </aside>
  </div>
</template>
<script setup lang="ts">
import { emailName, dateFromNow } from '@/utils/filters';
import RequestDetail from '../request/RequestDetail.vue';
import Location from '../request/RequestLocation.vue';
import Notes from '../agency_request/RequestNotes.vue';
import RequestedBy from './RequestedBy.vue';
import ReportTo from './ReportTo.vue';

defineProps<{ request: any }>();
const emit = defineEmits<{
  (e: 'update:request', value: any): void;
  (e: 'refreshRequest'): void;
}>();
</script>

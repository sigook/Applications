<template>
  <div class="container-flex">
    <section class="col-sm-12 col-md-9">
      <request-detail :request="request" @update:request="$emit('update:request', $event)" @refreshRequest="$emit('refreshRequest')" />
    </section>
    <aside class="col-sm-12 col-md-3">
      <notes :canEdit="request.canEdit" />
      <location :jobLocation="request.jobLocation" />
      <requested-by class="mt-4" :canEdit="request.canEdit" :requestId="request.id"
        :companyId="request.companyProfileId" />
      <report-to class="mt-3" :canEdit="request.canEdit" :requestId="request.id" :companyId="request.companyProfileId" />
      <div class="mt-3">
        <h3 class="fw-700">Created by</h3>
        <span class="d-inline-block valign-middle">
          {{ emailName(request.createdBy) }} {{ dateFromNow(request.createdAt) }}
        </span>
      </div>
    </aside>
  </div>
</template>
<script lang="ts">
import { defineAsyncComponent } from 'vue';
import { emailName, dateFromNow } from '@/utils/filters';

export default {
  props: ["request"],
  methods: {
    emailName,
    dateFromNow,
  },
  components: {
    RequestDetail: defineAsyncComponent(() => import("../request/RequestDetail.vue")),
    Location: defineAsyncComponent(() => import("../request/RequestLocation.vue")),
    Notes: defineAsyncComponent(() => import("../agency_request/RequestNotes.vue")),
    RequestedBy: defineAsyncComponent(() => import("./RequestedBy.vue")),
    ReportTo: defineAsyncComponent(() => import("./ReportTo.vue"))
  }
}
</script>
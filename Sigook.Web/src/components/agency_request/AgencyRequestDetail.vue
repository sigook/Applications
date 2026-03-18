<template>
  <div class="container-flex">
    <section class="col-sm-12 col-md-9">
      <request-detail :request.sync="request" @refreshRequest="$emit('refreshRequest')" />
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
          {{ request.createdBy | emailName }} {{ request.createdAt | dateFromNow }}
        </span>
      </div>
    </aside>
  </div>
</template>
<script lang="ts">
export default {
  props: ["request"],
  components: {
    RequestDetail: () => import("../request/RequestDetail.vue"),
    Location: () => import("../request/RequestLocation.vue"),
    Notes: () => import("../agency_request/RequestNotes.vue"),
    RequestedBy: () => import("./RequestedBy.vue"),
    ReportTo: () => import("./ReportTo.vue")
  }
}
</script>
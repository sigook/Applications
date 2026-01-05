<template>
  <div class="container-flex">
    <section class="col-sm-12 col-md-9">
      <request-detail :request="request" />
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
<script>
export default {
  props: ["request"],
  components: {
    RequestDetail: () => import("../request/RequestDetail"),
    Location: () => import("../request/RequestLocation"),
    Notes: () => import("../agency_request/RequestNotes"),
    RequestedBy: () => import("./RequestedBy"),
    ReportTo: () => import("./ReportTo")
  }
}
</script>
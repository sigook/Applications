<template>
  <div class="worker-detail-job white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>

    <section class="wrapper-request-top" v-if="request">
      <div>
        <img v-if="request.agencyLogo" :src="request.agencyLogo" />
        <h2 class="capitalize fz1 fw-700">{{ request.jobTitle }}</h2>
      </div>

      <div>
        <div v-if="request.status && request.status !== 'None'" class="capitailized fw-700 is-inline-block"
          :class="request.status">
          {{ $t(request.status) }}
        </div>
      </div>
    </section>

    <b-tabs v-model="currentTab" @input="changeTab" v-if="request">
      <b-tab-item label="Summary Request" value="Summary Request">
        <div v-if="visitedTabs.includes('Summary Request')" class="container-flex">
          <section class="col-md-9 col-sm-12 section-left">
            <request-detail :request="request" />
          </section>
          <aside class="col-md-3 col-sm-12 section-right">
            <location :jobLocation="request.jobLocation" />
          </aside>
        </div>
      </b-tab-item>
      <b-tab-item label="Punch Card" value="Punch Card">
        <punch-card v-if="visitedTabs.includes('Punch Card') && timesheet" :requestId="request.id" :timesheet="timesheet" />
      </b-tab-item>
      <b-tab-item label="Time Sheet" value="Time Sheet">
        <time-sheet v-if="visitedTabs.includes('Time Sheet')" :data="timesheet" />
      </b-tab-item>
    </b-tabs>
  </div>
</template>

<script>

export default {
  components: {
    RequestDetail: () => import("../../components/worker/RequestDetail"),
    Location: () => import("../../components/request/RequestLocation"),
    PunchCard: () => import("./PunchCard"),
    TimeSheet: () => import("./TimeSheet"),
  },
  data() {
    return {
      isLoading: true,
      request: {},
      currentTab: "Summary Request",
      visitedTabs: ["Summary Request"],
      timesheet: {},
    };
  },
  methods: {
    changeTab(tab) {
      if (!this.visitedTabs.includes(tab)) {
        this.visitedTabs.push(tab);
      }
    },
    getWorkerRequest() {
      this.$store
        .dispatch("worker/getWorkerRequest", this.$route.params.id)
        .then((response) => {
          this.isLoading = false;
          this.request = response;
          this.getTimeSheet();
        })
        .catch(() => {
          this.isLoading = false;
        });
    },
    getTimeSheet() {
      this.$store
        .dispatch("worker/workerGetTimeSheet", this.request.id)
        .then((response) => {
          this.isLoading = false;
          this.timesheet = response;
        })
        .catch(() => {
          this.isLoading = false;
        });
    },
  },
  created() {
    this.getWorkerRequest();
  },
};
</script>

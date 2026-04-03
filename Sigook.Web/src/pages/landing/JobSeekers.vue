<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <sub-menu solutionType="jobSeekers"></sub-menu>
    <section class="container-fluid">
      <div class="looking-for-worker-banner p-3 px-sm-0 py-sm-3 color-white">
        <div class="pb-2 pb-sm-0">
          <h5>Looking for an entry-level job, or to improve your career?</h5>
          <h5 class="font-weight-bold pb-2">SIGOOK<label class="superscript">®</label> is for you.</h5>
          <hr class="w-10 col-2 b-color-white ml-0 my-1" />
          <h6>✔ Anywhere in the USA</h6>
          <h6>✔ Receive the right job offer for you</h6>
          <h6>✔ Get hired on the spot</h6>
          <h6>✔ Review your timesheets in real-time data</h6>
          <h6>✔ Rate your employers</h6>
          <h6>✔ Count on a human team of recruiters ready to assist and guide you along the way</h6>
        </div>
        <sigook-video></sigook-video>
      </div>
    </section>
    <section class="container-fluid" id="jobsContainer">
      <div class="d-flex justify-content-center align-items-center color-white search-job">
        <div class="p-2 hide-on-mobile">
          <strong class="color-white">SEARCH JOBS</strong>
        </div>
        <div class="p-2">
          <job-search @onSearch="searchJob"></job-search>
        </div>
      </div>
    </section>
    <section class="d-none d-lg-block">
      <div class="container py-4">
        <div class="row job-list py-3">
          <div class="col-4 h-100 overflow-hidden job-list-side">
            <div>
              <p>Select Job</p>
              <hr />
            </div>
            <div class="overflow-auto">
              <p v-if="jobs.length === 0">No jobs available at the moment</p>
              <div v-else>
                <div v-for="job in jobs" :key="job.numberId">
                  <div class="job p-2" :class="{ 'job-selected': jobSelected && job.numberId === jobSelected.numberId }"
                    @click="selectJob(job)">
                    <h5><strong>{{ job.title }} - {{ job.numberId }}</strong></h5>
                    <div class="d-flex justify-content-between">
                      <h6 class="m-0">{{ job.location }}</h6>
                      <h6 class="color-blue-light m-0">{{ job.salary }}</h6>
                    </div>
                  </div>
                  <hr />
                </div>
              </div>

            </div>
          </div>
          <div class="color-blue-light col-8 h-100 align-items-center overflow-auto">
            <div v-if="!jobSelected" class="overflow-auto">
              <h6>by Sigook<label class="superscript">®</label></h6>
              <h4><strong id="jobTitle">No jobs available at the moment</strong></h4>
            </div>
            <div v-else>
              <h6>by Sigook<label class="superscript">®</label></h6>
              <div class="d-flex justify-content-between align-items-center">
                <h4 class="m-0"><strong>{{ jobSelected.title }} - {{ jobSelected.numberId }}</strong></h4>
                <button class="button-rounded bg-blue-light color-white" @click="applyNow()">APPLY NOW</button>
              </div>
              <div class="d-flex justify-content-start color-grey-light mt-2">
                <h6>{{ jobSelected.location }}</h6>
                <h6 class="ml-4">{{ jobSelected.salary }}</h6>
                <h6 class="ml-4">{{ jobSelected.type }}</h6>
                <h6 class="ml-4">{{ jobSelected.shift }}</h6>
              </div>
              <hr />
              <h4>Description</h4>
              <article class="color-grey-dark" v-html="jobSelected.description"></article>
              <br />
              <h4>Responsibilities</h4>
              <article class="color-grey-dark" v-html="jobSelected.responsibilities"></article>
              <br />
              <h4>Requirements</h4>
              <article class="color-grey-dark" v-html="jobSelected.requirements"></article>
              <hr />
              <button class="button-rounded bg-blue-light color-white mb-2" @click="applyNow()">APPLY NOW</button>
            </div>
          </div>
        </div>
      </div>
    </section>
    <section class="container-fluid p-4 d-block d-lg-none">
      <div class="border p-3 rounded-lg">
        <div class="accordion" id="accordionExample">
          <p v-if="jobs.length === 0">No jobs available at the moment</p>
          <div v-else>
            <div class="card" v-for="job in jobs" :key="job.numberId">
              <div class="card-header" :id="`heading-${job.numberId}`">
                <h2 class="mb-0">
                  <button class="btn btn-block text-left collapsed color-blue-light bold" type="button"
                    data-toggle="collapse" :data-target="`#collapse-${job.numberId}`" aria-expanded="false"
                    :aria-controls="`collapse-${job.numberId}`">
                    <div class="d-flex justify-content-between align-items-center">
                      <h4 class="m-1">{{ job.title }} - {{ job.numberId }}</h4>
                      <h6 class="m-1">{{ job.location }}</h6>
                      <h6 class="m-1 text-right">{{ job.salary }}</h6>
                    </div>
                  </button>
                </h2>
              </div>
              <div :id="`collapse-${job.numberId}`" class="collapse" :aria-labelledby="`heading-${job.numberId}`"
                data-parent="#accordionExample">
                <div class="card-body">
                  <div class="d-flex justify-content-between align-items-center">
                    <h6 class="m-0">by Sigook<label class="superscript">®</label></h6>
                    <button class="button-rounded bg-blue-light color-white" @click="applyNow()">APPLY NOW</button>
                  </div>
                  <div class="d-flex color-grey-light mt-2">
                    <h6>{{ job.location }}</h6>
                    <h6>{{ job.salary }}</h6>
                    <div>
                      <h6>{{ job.type }}</h6>
                      <h6>{{ job.shift }}</h6>
                    </div>
                  </div>
                  <hr />
                  <h4>Description</h4>
                  <article class="color-grey-dark" v-html="job.description"></article>
                  <h4>Responsibilities</h4>
                  <article class="color-grey-dark" v-html="job.responsibilities"></article>
                  <h4>Requirements</h4>
                  <article class="color-grey-dark" v-html="job.requirements"></article>
                  <hr />
                  <button class="button-rounded bg-blue-light color-white mb-2" @click="applyNow()">APPLY NOW</button>
                </div>
              </div>
            </div>
          </div>

        </div>
      </div>
    </section>
    <b-modal v-model="showApplyNowModal" @close="showApplyNowModal = false" :can-cancel="true" width="500px">
      <apply-now :jobToApply="jobToApply" @candidateCreated="showApplyNowModal = false"></apply-now>
    </b-modal>
  </div>
</template>


<script lang="ts">
import { getJobs } from "@/api/websiteApi";

export default {
  data() {
    return {
      isLoading: false,
      jobs: [],
      jobSelected: null,
      showApplyNowModal: false,
      jobToApply: null
    }
  },
  components: {
    SubMenu: () => import("@/components/landing/SubMenu.vue"),
    SigookVideo: () => import("@/components/landing/SigookVideo.vue"),
    JobSearch: () => import("@/components/landing/JobSearch.vue"),
    ApplyNow: () => import("@/components/landing/ApplyNow.vue"),
  },
  async created() {
    this.isLoading = true;
    this.jobs = await getJobs(this.$route.query);

    const jobIdFromQuery = this.$route.query.jobId;
    if (jobIdFromQuery) {
      const targetJob = this.jobs.find(j => String(j.numberId) === String(jobIdFromQuery));
      this.selectJob(targetJob || this.jobs[0]);
    } else {
      this.selectJob(this.jobs[0]);
    }

    this.isLoading = false;

    this.$nextTick(() => {
      const VueScrollTo = require('vue-scrollto');
      if (jobIdFromQuery) {
        VueScrollTo.scrollTo("#jobsContainer", 500, { offset: -20 });
      } else {
        VueScrollTo.scrollTo("#jobsContainer");
      }
    });
  },
  methods: {
    selectJob(job) {
      if (job) {
        this.jobSelected = job;
        this.jobToApply = job;
        this.$router.replace({
          query: { jobId: job.numberId }
        });
      }
    },
    async searchJob(jobSearch) {
      this.isLoading = true;
      this.jobs = await getJobs(jobSearch);
      this.isLoading = false;
    },
    applyNow() {
      this.showApplyNowModal = true;
    }
  }
}

</script>

<style lang="scss">
@import "../../assets/scss/mixins";
@import "../../assets/scss/variables";

.looking-for-worker-banner {
  background: url("../../assets/images/job_seekers_banner.png") left bottom no-repeat;
  background-size: cover;
  align-items: center;
  justify-content: center;
  display: grid;
  column-gap: 15px;
  @include desktop {
    grid-template-columns: 40% 40%;
  }

  @include tablet {
    grid-template-columns: 44% 44%;
  }
}

.job-list {
  border: 1.3px solid $grey-dark;
  border-radius: 15px;
  height: 67vh;
}

.job-list-side {
  display: grid;
  grid-template-rows: auto 1fr;
}

.search-job {
  background-color: $blue-light;
  column-gap: 5%;

  @include smaller-than-notebook {
    column-gap: 3%;
  }

  @include only-mobile {
    flex-direction: column;
  }

  >div:first-of-type {
    font-size: x-large;

    @include smaller-than-notebook {
      font-size: large;
    }
  }
}

.w-100-only-mobile {
  @include only-mobile {
    width: 100% !important;
  }
}

.w-95 {
  width: 95% !important;
}

.h-80 {
  height: 80% !important;
}

.sigook-download-app-reach-goals {
  color: $grey-font;
  padding: 1rem;
  text-align: center;

  @include tablet {
    background: url("/assets/images/app_mobile_workers_bg.png") no-repeat center;
    background-size: cover;
    color: $white;
  }

  .sigook-download-app-reach-goals-grid {
    display: grid;
    grid-gap: 1rem;
    font-size: 2rem !important;
    font-weight: bold;
    text-align: center;

    @include tablet {
      grid-template-columns: 45% 25% 25% 5%;
      padding: 1rem;

      .sigook-download-app-label {
        margin-top: 0.6em;
        font-size: 2rem !important;
        grid-column: 2 / 4;
      }

      .sigook-download-app-store {
        width: 90%;
        margin: auto;
        grid-column: 2;
      }

      .sigook-download-app-google-play {
        width: 90%;
        margin: auto;
        grid-column: 3;
      }
    }

    @include only-notebook {
      grid-row-gap: 2em;
    }

    @include desktop {
      grid-template-columns: 45% 22% 22% 11%;
      margin-top: 3rem;
      margin-bottom: 3rem;

      .sigook-download-app-label {
        font-size: 3rem !important;
        margin-top: 0;
      }

      .sigook-download-app-store {
        width: 70%;
      }

      .sigook-download-app-google-play {
        width: 70%;
      }
    }
  }
}

.job-selected {
  background-color: whitesmoke;
}

.job:hover {
  cursor: pointer;
  background-color: whitesmoke;
}

article ul {
  list-style: inside;
}
</style>
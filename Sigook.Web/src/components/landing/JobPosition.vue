<template>
  <div>
    <sub-menu :solutionType="solutionType"></sub-menu>
    <section class="container-fluid">
      <div class="p-2 p-sm-5 sigook-positions-banner" :style="{ background: `url(${bannerImage})` }">
        <div class="p-4 p-sm-5 caption"
          :class="{ 'bg-blue-light-opacity': solutionType === 'jobSeekers', 'bg-red-light-opacity': solutionType === 'business' }">
          <label class="caption-title m-0">{{ currentPosition.title }}</label>
          <hr class="caption-line" />
          <p class="py-3 py-sm-4">{{ currentPosition.shortDescription }}</p>
          <div class="mt-3 pb-3 text-right">
            <router-link v-if="solutionType === 'jobSeekers'" class="button-rounded bg-grey-light color-white mt-2"
                        to="/jobSeekers">SEARCH JOBS</router-link>
            <a v-else class="button-rounded bg-grey-light color-white cursor-pointer mt-2"
                      v-scroll-to="'#needStaffSection'">REQUEST PERSONNEL</a>
          </div>
        </div>
      </div>
    </section>
    <section class="container-fluid py-5">
      <div class="sigook-positions-container">
        <div class="sigook-position-card"
          :class="{ 'bg-blue-light': solutionType === 'jobSeekers', 'bg-red-light': solutionType === 'business' }"
          v-for="position in currentPosition.positions" :key="position.title">
          <div class="position-relative">
            <img :src="getPositionImage(position.image)" class="w-100" />
            <img v-if="position.comingSoon" src="@/assets/images/comingsoon.png" class="coming-soon-image" />
          </div>
          <div class="color-white">
            <div class="ml-3">
              <label class="position-card-title">{{ position.title }}</label>
              <label>
                {{ position.description }}
              </label>
            </div>
            <div class="mt-3 mb-3 right">
              <router-link v-if="solutionType === 'jobSeekers'" to="/jobSeekers"
                class="button-rounded bg-grey-light color-white">SEE OPEN POSITIONS</router-link>
              <a v-else class="button-rounded bg-grey-light color-white cursor-pointer"
                v-scroll-to="'#needStaffSection'">REQUEST PERSONEL</a>
            </div>
          </div>
        </div>
      </div>
    </section>
    <slot name="content"></slot>
    <section class="conntainer">
      <div class="py-4 p-lg-5 my-5 text-center">
        <h2
          :class="{ 'color-blue-light': solutionType === 'jobSeekers', 'color-red-light': solutionType === 'business' }">
          Download our app <span class="font-weight-bold">and reach your goals now</span>
        </h2>
        <div class="d-flex flex-column flex-sm-row mt-4 justify-content-center align-items-center">
          <a class="p-0 border-0 bg-transparent" href="https://apps.apple.com/us/app/sigook/id1446736193" target="_blank">
            <img src="@/assets/images/appstore.png" alt="appstore button" />
          </a>
          <a class="p-0 border-0 ml-sm-3 bg-transparent"
            href="https://play.google.com/store/apps/details?id=com.sigook.sigook" target="_blank">
            <img src="@/assets/images/googleplay.png" alt="playstore button" />
          </a>
        </div>
      </div>
    </section>
  </div>
</template>
<script>
export default {
  props: ['solutionType'],
  data() {
    return {
      currentPosition: {}
    }
  },
  components: {
    SubMenu: () => import("@/components/landing/SubMenu")
  },
  async created() {
    if (this.$route.params.position) {
      await this.loadInfo(this.$route.params.position);
    }
  },
  methods: {
    async loadInfo(positionType) {
      const positions = await this.$store.dispatch('getLandingJobPositions');
      const position = positions[this.solutionType].find(p => p.id === positionType);
      if (position) {
        this.currentPosition = position;
      }
    },
    getPositionImage(imageName) {
      return require(`@/assets/images/positions/${this.solutionType}/${imageName}`);
    },
    scrollToRequestStaffForm() {

    },
  },
  computed: {
    bannerImage() {
      if (this.currentPosition.backgroundImage) {
        return require(`@/assets/images/banners/${this.currentPosition.backgroundImage}`);
      } else {
        return ''
      }
    }
  },
  watch: {
    '$route.params.position': async function (value) {
      await this.loadInfo(value);
    }
  }
}
</script>
<style lang="scss">
@import "../../assets/scss/mixins";
@import "../../assets/scss/variables";

.sigook-positions-banner {
  background-position: right bottom !important;
  background-repeat: no-repeat !important;
  background-size: cover !important;
  display: grid;
  color: $white;
  grid-template-columns: 10% 45% 35%;

  @include only-mobile {
    grid-template-columns: 0;
    background-position: left;
  }

  .caption-title {
    @include smaller-than-desktop {
      font-size: 3rem;
    }

    @include notebook {
      font-size: 4.5rem;
    }

    @include only-mobile {
      font-size: 2.2rem;
    }

    font-weight: 900;
  }

  .caption-line {
    margin: 0;
    border: $white 1px solid;
    border-radius: 2px;
    width: 30%;
  }

  .caption {
    @include tablet {
      grid-column: 2/4;
    }

    @include smaller-than-notebook {
      grid-column: 2/4;
    }

    @include notebook {
      grid-column: 1/3;
    }

    @include desktop {
      grid-column: 2/3;
    }

    border-radius: 18px;
  }
}

.sigook-positions-container {
  display: grid;
  grid-gap: 1.5em;
  grid-template-columns: auto;

  @include tablet {
    grid-template-columns: 1fr 1fr;
  }

  @include notebook {
    margin-left: 4rem !important;
    margin-right: 4rem !important;
  }

  @include superdesktop {
    grid-template-columns: auto auto auto;
  }
}

.sigook-position-card {
  display: grid;
  align-items: center;
  grid-gap: 1em;
  padding: 1em;

  @include desktop {
    grid-template-columns: 1fr 2fr;
  }

  .position-card-title {
    font-size: 1.3rem;
    font-weight: 600;
  }

  .coming-soon-image {
    position: absolute;
    width: 100%;
    left: 0;
  }
}


.position-placeholder-bg {
  background: url(../../assets/images/banners/worker_skill_jobs.jpg) no-repeat;
  background-size: cover;
}
</style>
<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <sub-menu solutionType="business"></sub-menu>
    <section class="container-fluid">
      <div class="request-personnel-banner p-3 px-sm-0 py-sm-5 color-white">
        <div class="pb-4 pb-sm-0">
          <h4>Join SIGOOK<label class="superscript">®</label> today and get access to our self-serve hiring, and work
            management benefits.</h4>
          <hr class="w-10 col-2 b-color-white ml-0" />
          <h4>✔ Temporary, permanent, temp-to-hire, or contract workers at your request</h4>
          <h4>✔ Receive the most qualified candidates from our pool</h4>
          <h4 class="pb-3">✔ Monitor staff productivity and manage your day-to-day workload Start enjoying the new
            experience in staffing!</h4>
          <h4>Start enjoying the new experience in staffing!</h4>
        </div>
        <sigook-video></sigook-video>
      </div>
    </section>
    <section class="px-3 py-5 px-sm-5">
      <div class="p-lg-4 d-lg-flex">
        <div class="mb-5 mb-lg-0 flex-lg-grow-1">
          <div class="d-flex flex-column mb-2">
            <label class="font-weight-bold font-x-large color-grey mr-2">
              DO YOU PREFER <span class="color-red-light">TO CONNECT</span>?
            </label>
            <h4>Send us a message here</h4>
          </div>
          <need-staff cssClass="businesses-staff-form" @onLoading="(value) => isLoading = value"></need-staff>
        </div>
        <div class="flex-lg-fill ml-lg-3">
          <div class="accordion accordion-flush" id="accordionNeedStaff">
            <div class="accordion-item">
              <div class="accordion-header" id="headingOne">
                <button
                  class="btn fw-bold color-grey btn-block text-left collapsed d-flex align-items-center column-gap-1 w-100"
                  type="button" data-bs-toggle="collapse" data-bs-target="#collapseOne" aria-expanded="true"
                  aria-controls="collapseOne">
                  <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" fill="currentColor"
                    class="bi bi-plus-circle-fill mr-2" viewBox="0 0 16 16">
                    <path
                      d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0M8.5 4.5a.5.5 0 0 0-1 0v3h-3a.5.5 0 0 0 0 1h3v3a.5.5 0 0 0 1 0v-3h3a.5.5 0 0 0 0-1h-3z" />
                  </svg>
                  WHO WE ARE
                </button>
              </div>
              <div id="collapseOne" class="collapse show" aria-labelledby="headingOne" data-parent="#accordionNeedStaff">
                <div class="accordion-body">
                  We do what we know best, find the top talents! We have an extensive experience in the Employment
                  Industry
                  with over 10 years serving clients in Canada as COVENANT GROUP Staffing and Recruiting. Ask us for
                  temps,
                  permanent, term to perm, contractual, direct hiring, and more anywhere in the USA, and let us do the
                  work
                  for you.
                </div>
              </div>
            </div>
            <div class="accordion-item">
              <div class="accordion-header" id="headingTwo">
                <button
                  class="btn fw-bold color-grey btn-block text-left collapsed d-flex align-items-center column-gap-1 w-100"
                  type="button" data-bs-toggle="collapse" data-bs-target="#collapseTwo" aria-expanded="false"
                  aria-controls="collapseTwo">
                  <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" fill="currentColor"
                    class="bi bi-plus-circle-fill mr-2" viewBox="0 0 16 16">
                    <path
                      d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0M8.5 4.5a.5.5 0 0 0-1 0v3h-3a.5.5 0 0 0 0 1h3v3a.5.5 0 0 0 1 0v-3h3a.5.5 0 0 0 0-1h-3z" />
                  </svg>
                  SIGOOK<label class="superscript">®</label>APP
                </button>
              </div>
              <div id="collapseTwo" class="collapse" aria-labelledby="headingTwo" data-parent="#accordionNeedStaff">
                <div class="accordion-body">
                  SIGOOK<label class="superscript">®</label> App is a self-serve platform created in-house where you can
                  hire qualified candidates with a few
                  clicks. Download the SIGOOK<label class="superscript">®</label> app, register with us, and start
                  enjoying the new experience in Staffing.
                </div>
              </div>
            </div>
            <div class="accordion-item">
              <div class="accordion-header" id="headingThree">
                <button
                  class="btn fw-bold color-grey btn-block text-left collapsed d-flex align-items-center column-gap-1 w-100"
                  type="button" data-bs-toggle="collapse" data-bs-target="#collapseThree" aria-expanded="false"
                  aria-controls="collapseThree">
                  <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" fill="currentColor"
                    class="bi bi-plus-circle-fill mr-2" viewBox="0 0 16 16">
                    <path
                      d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0M8.5 4.5a.5.5 0 0 0-1 0v3h-3a.5.5 0 0 0 0 1h3v3a.5.5 0 0 0 1 0v-3h3a.5.5 0 0 0 0-1h-3z" />
                  </svg>
                  SIGOOK<label class="superscript">®</label> SOLUTIONS
                </button>
              </div>
              <div id="collapseThree" class="collapse" aria-labelledby="headingThree" data-parent="#accordionNeedStaff">
                <div class="accordion-body">
                  Our team of professional experts will support you from the beginning of your search to the end, making
                  your experience with us enjoyable and successful.
                  From prospecting to hiring, we will follow up the process with effective solutions. Our top-notch
                  technology is our ally to solve any questions and requirements you may have.
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
    <section class="color-white">
      <div class="app-section-grid app-businesses-section-bg py-5 py-sm-0 px-5">
        <div class="ml-lg-5 px-lg-5">
          <div class="py-5 mt-5 hide-on-mobile"></div>
          <label class="app-section-font">Download our app</label>
          <label class="app-section-font font-weight-bold">and reach your goals now</label>
          <div class="app-section-buttons-grid pt-2 pt-sm-4">
            <a class="p-0 border-0 bg-transparent" href="https://apps.apple.com/us/app/sigook/id1446736193"
              target="_blank">
              <img src="@/assets/images/appstore.png" class="w-100" alt="appstore button" />
            </a>
            <a class="p-0 border-0 ml-3 bg-transparent"
              href="https://play.google.com/store/apps/details?id=com.sigook.sigook" target="_blank">
              <img src="@/assets/images/googleplay.png" class="w-100" alt="playstore button" />
            </a>
          </div>
        </div>
        <img v-lazy="require('@/assets/images/app_mobile_businesses.png')" class="w-100 grid-item-start hide-on-mobile"
          alt="app mobile" />
      </div>
    </section>
  </div>
</template>

<script>

export default {
  components: {
    SigookVideo: () => import("@/components/landing/SigookVideo"),
    SubMenu: () => import("@/components/landing/SubMenu"),
    NeedStaff: () => import("@/components/landing/NeedStaff")
  },
  data() {
    return {
      isLoading: false
    }
  }
}

</script>

<style lang="scss">
@import "../../assets/scss/mixins";

.request-personnel-banner {
  background: url("../../assets/images/for_businesses_bg.png") left bottom no-repeat;
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

.app-businesses-section-bg {
  background: url('../../assets/images/app_mobile_businesses_bg.png') no-repeat bottom center;
  background-size: contain;

  @include only-mobile {
    background-size: cover;
  }
}
</style>
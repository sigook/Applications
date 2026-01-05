<template>
  <div>
    <section class="container-fluid">
      <div class="px-lg-3">
        <div id="homeMainBanner" class="carousel slide color-white" data-ride="carousel">
          <ol class="carousel-indicators">
            <li data-target="#homeMainBanner" data-slide-to="0" class="active"></li>
            <li data-target="#homeMainBanner" data-slide-to="1"></li>
          </ol>
          <div class="carousel-inner main-banner-bg">
            <div class="carousel-item active px-sm-5">
              <div class="carousel-item-grid pt-2 px-3 pt-sm-0 px-sm-5 pr-lg-0">
                <div class="align-self-center pb-5 pt-4 pt-lg-5 px-lg-5">
                  <div class="px-lg-4 py-lg-5">
                    <div class="ml-lg-4 text-center text-lg-start">
                      <h3>Our Professional team turns your employment needs into the best quality talent.</h3>
                      <h3 class="font-weight-bold">Working for your Success is our Success!</h3>
                      <hr class="b-color-white visible-on-desktop" />
                      <div class="visible-on-desktop">
                        <h6>We are SIGOOK<label class="superscript">®</label>. Whether you are looking to hire Temporary
                          or Permanent Staff or searching for the next big step in your career.
                          SIGOOK<label class="superscript">®</label> will help you and go that extra mile with you.</h6>
                      </div>
                    </div>
                    <div class="d-flex justify-content-center justify-content-lg-around mt-5">
                      <router-link to="/jobSeekers" class="button-rounded bg-white color-black">
                        START WORKING
                      </router-link>
                      <router-link to="/business" class="button-rounded bg-grey-light color-white ml-4">
                        FIND STAFF
                      </router-link>
                    </div>
                  </div>
                </div>
                <img class="d-block grid-item-start w-100 align-self-end pt-5 mt-4 mt-lg-0 carousel-item-image"
                  src="@/assets/images/main_banner/image.png" alt="First slide">
              </div>
            </div>
            <div class="carousel-item px-sm-5">
              <div class="carousel-item-grid pt-2 px-3 pt-sm-0 px-sm-5 pr-lg-0">
                <div class="align-self-center pb-5 pt-4 pt-lg-5 px-lg-5">
                  <div class="px-lg-4 py-lg-5">
                    <div class="ml-lg-4 text-center text-lg-start">
                      <h3>SIGOOK<label class="superscript">®</label> cares about your business. We have the talent that
                        fits your direct hiring needs.</h3>
                      <h3 class="font-weight-bold">Working for your Success is our Success!</h3>
                      <hr class="b-color-white visible-on-desktop" />
                      <div class="visible-on-desktop">
                        <h6>We are SIGOOK<label class="superscript">®</label>. Whether you are looking to hire Temporary
                          or Permanent Staff or searching for the next big step in your career.
                          SIGOOK<label class="superscript">®</label> will help you and go that extra mile with you.</h6>
                      </div>
                    </div>
                    <div class="d-flex justify-content-center justify-content-lg-around mt-5">
                      <router-link to="/jobSeekers" class="button-rounded bg-white color-black">
                        START WORKING
                      </router-link>
                      <router-link to="/business" class="button-rounded bg-grey-light color-white ml-4">
                        FIND STAFF
                      </router-link>
                    </div>
                  </div>
                </div>
                <img class="d-block grid-item-start w-100 align-self-end pt-5 mt-4 mt-lg-0 carousel-item-image"
                  src="@/assets/images/main_banner/image2.png" alt="First slide">
              </div>
            </div>
          </div>
          <a class="carousel-control-prev main-banner-arrow left" data-target="#homeMainBanner" role="button"
            data-slide="prev">
            <img src="@/assets/images/main_banner/left_arrow.png" />
          </a>
          <a class="carousel-control-next main-banner-arrow right" data-target="#homeMainBanner" role="button"
            data-slide="next">
            <img src="@/assets/images/main_banner/right_arrow.png" />
          </a>
        </div>
      </div>
    </section>
    <section class="container-fluid color-white">
      <div class="grid-home-looking-section px-lg-3">
        <div class="start-working-section p-5 h-100">
          <div class="py-sm-5 px-sm-2 px-lg-4">
            <label>JOB SEEKERS AREA</label>
            <h3 class="font-weight-bold">LOOKING FOR A JOB?</h3>
            <hr class="col-1 d-inline-flex b-color-white" />
            <h5 class="mb-5">Browse for a job, register with us, and be part of our growing and successful community of
              satisfied workers.</h5>
            <job-search class="pt-1"></job-search>
          </div>
        </div>
        <div class="looking-for-worker-section p-5 h-100">
          <div class="py-sm-5 px-sm-2 px-lg-4">
            <label>BUSINESS AREA</label>
            <h3 class="font-weight-bold">LOOKING FOR EMPLOYEES?</h3>
            <hr class="col-1 d-inline-flex b-color-white" />
            <h5>Temporary - Permanent - Temp to Perm - Part Time - Peak - Direct Hiring</h5>
            <div class="d-lg-flex pt-md-3">
              <div class="ml-sm-4 ml-lg-4 d-flex pt-4 pt-md-4">
                <router-link to="/business" class="button-rounded bg-grey-light color-white">
                  FIND STAFF
                </router-link>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
    <section class="py-5">
      <div class="pt-5 py-sm-5">
        <div class="d-flex flex-column text-center mx-3 pb-3">
          <h2 class="font-weight-bold">WHAT THEY THINK OF US</h2>
          <h6 class="">Every day more companies and workers are beyond satisfied staffing together with SIGOOK<label
              class="superscript">®</label></h6>
        </div>
        <div id="theyThinkCarousel" class="carousel slide" data-ride="carousel">
          <div class="carousel-inner">
            <div class="carousel-item px-3 active">
              <img v-lazy="require('@/assets/images/home_carousel/banner_testimonials_1.png')" class="d-block w-100" alt="Slide 1">
            </div>
            <div class="carousel-item px-3">
              <img v-lazy="require('@/assets/images/home_carousel/banner_testimonials_2.png')" class="d-block w-100" alt="Slide 2">
            </div>
            <div class="carousel-item px-3">
              <img v-lazy="require('@/assets/images/home_carousel/banner_testimonials_3.png')" class="d-block w-100" alt="Slide 3">
            </div>
          </div>
          <a class="carousel-control-prev" href="#theyThinkCarousel" role="button" data-slide="prev">
            <span class="carousel-control-prev-icon" aria-hidden="true"></span>
            <span class="sr-only">Previous</span>
          </a>
          <a class="carousel-control-next" href="#theyThinkCarousel" role="button" data-slide="next">
            <span class="carousel-control-next-icon" aria-hidden="true"></span>
            <span class="sr-only">Next</span>
          </a>
        </div>
        <div class="mx-sm-5 px-sm-4">
          <div class="mx-4 p-2 p-sm-5 bg-white text-center shadow">
            <div id="carouselTestimonial" class="carousel slide" data-ride="carousel">
              <div class="carousel-inner font-italic">
                <div class="carousel-item active">
                  <p class="mx-2 mx-sm-5 mt-4 color-grey">
                    "Great Agency, their staff is friendly, caring and goes above and beyond the call of their duty. I
                    strongly recommend them to anyone that is seeking employment".
                  </p>
                  <p class="color-red-light">General Labor Worker</p>
                </div>
                <div class="carousel-item">
                  <p class="mx-2 mx-sm-5 mt-4 color-grey">
                    "Excellent company, serious offers, and good advice".
                  </p>
                  <p class="color-red-light">Worker in IT</p>
                </div>
                <div class="carousel-item">
                  <p class="mx-2 mx-sm-5 mt-4 color-grey">
                    “Great Agency!! Helped me a lot to find an excellent position!!!".
                  </p>
                  <p class="color-red-light">Worker in a Clothing Manufacturer</p>
                </div>
              </div>
              <a class="carousel-control-prev" href="#carouselTestimonial" role="button" data-slide="prev">
                <span class="carousel-control-prev-icon" aria-hidden="true"></span>
                <span class="sr-only">Previous</span>
              </a>
              <a class="carousel-control-next" href="#carouselTestimonial" role="button" data-slide="next">
                <span class="carousel-control-next-icon" aria-hidden="true"></span>
                <span class="sr-only">Next</span>
              </a>
            </div>
          </div>
        </div>
      </div>
    </section>
    <section class="p-3 p-sm-5">
      <div class="grid-home-video py-sm-5">
        <div class="p-4 p-lg-5 text-area-home-video">
          <div></div>
          <div class="p-lg-5">
            <h2 class="m-0">
              Enjoy a new hiring experience! Life is easier with us.
            </h2>
          </div>
        </div>
        <div class="w-100 grid-item-start">
          <sigook-video></sigook-video>
        </div>
      </div>
    </section>
    <section class="color-white pb-sm-5 mb-sm-5">
      <div class="app-section-grid app-section-bg py-5 py-sm-0 px-5">
        <div class="ml-lg-5 px-lg-5">
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
        <img v-lazy="require('@/assets/images/app_mobile.png')" class="w-100 grid-item-start hide-on-mobile" alt="app mobile" />
      </div>
    </section>
  </div>
</template>

<script>
export default {
  components: {
    SigookVideo: () => import("@/components/landing/SigookVideo"),
    JobSearch: () => import("@/components/landing/JobSearch")
  },
}

</script>

<style lang="scss">
@import "../../assets/scss/mixins";

.main-banner-bg {
  background: url("../../assets/images/main_banner/background.png") left no-repeat;
  background-size: cover;
}

.carousel-item-grid {
  display: grid;

  @include desktop {
    grid-template-columns: 50% 50%;
    align-items: center;
  }
}

.carousel-item-image {
  @include smaller-than-desktop {
    border-bottom: 3px solid $white;
  }
}

.grid-home-looking-section {
  display: grid;

  @include tablet {
    grid-template-columns: 50% 50%;
    align-items: center;
  }
}

.grid-home-video {
  display: grid;
  grid-gap: 1.2em;
  justify-content: center;

  @include desktop {
    grid-template-columns: 42% 42%;
    align-items: center;
  }

  @include tablet {
    padding-left: 5em;
    padding-right: 5em;
  }
}

.text-area-home-video {
  display: grid;
  grid-template-rows: 1fr auto;
  height: 100%;
  color: $white;
  background: url("../../assets/images/home_video_section_image.png") no-repeat center;
  background-size: cover;
  border-radius: 10px;
  min-height: 34rem;

  @include only-mobile {
    background-image: url("../../assets/images/home_video_mobile_section_image.png") !important;
  }
}

c-carousel-control {
  cursor: pointer;
}
</style>
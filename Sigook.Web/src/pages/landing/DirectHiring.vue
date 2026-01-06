<template>
  <div>
    <section class="container-fluid">

      <div class="px-lg-3">

        <div id="directHiringMainBanner" class="carousel slide color-white" data-bs-ride="carousel">

          <ol class="carousel-indicators">
            <li data-target="#directHiringMainBanner" data-slide-to="0" class="active bg-dark border"></li>
            <li data-target="#directHiringMainBanner" data-slide-to="1" class="active bg-dark border"></li>
            <li data-target="#directHiringMainBanner" data-slide-to="2" class="active bg-dark border"></li>
          </ol>

          <div class="carousel-inner color-black">
            <!--CAROUSEL ITEM TEMPLATE-->
            <div class="carousel-item active">

              <div class="direct-hiring-banner-section-bg img1"></div>

              <div class="container py-5 text-sm-center">
                At all times, especially when the challenges are greater,
                SIGOOK<label class="superscript">®</label> Work Factory is always ready to work shoulder to shoulder with
                you,
                searching for the human resources you need.
                If you are tired of trying multiple recruitment firms without finding the right talent,
                or disappointed to hire someone who was not what you expected,
                then ask about our services, and we will find the people that meet your requirements.
              </div>

            </div>

            <div class="carousel-item">

              <div class="direct-hiring-banner-section-bg img2"></div>

              <div class="container pt-4 pb-5 text-sm-center">
                <div><strong>Practices - Legal & Compliance</strong></div>
                Our clients can count on SIGOOK<label class="superscript">®</label> Work Factory to help their businesses
                evolve in an ever-shifting legal landscape.
                Our recruiting team specialists have experience in placing countless attorneys and other legal
                professionals in prominent law firms and corporate legal departments nationwide.
                Align with elite talent by first aligning yourself with SIGOOK<label class="superscript">®</label>.
              </div>

            </div>

            <div class="carousel-item">

              <div class="direct-hiring-banner-section-bg img3"></div>

              <div class="container pt-4 pb-5 text-sm-center">
                As trusted business partners to our clients and dependable agents to our candidates,
                we believe that developing long-lasting relationships is the only way to conduct business.
                Our core business focus falls within, Engineering, IT, Power and Energy, Aerospace/Defense, Biotechnology,
                and Telecommunications market segments among others.
                SIGOOK<label class="superscript">®</label> strives to continually provide our clients with high-level
                customer service through our flexible,
                tailored and scalable solutions, designed to fit the client’s individual needs and business objectives.
              </div>

            </div>
          </div>

          <a class="carousel-control-prev main-banner-arrow left" data-target="#directHiringMainBanner" role="button"
            data-slide="prev">
            <img src="@/assets/images/main_banner/left_arrow.png" />
          </a>

          <a class="carousel-control-next main-banner-arrow right" data-target="#directHiringMainBanner" role="button"
            data-slide="next">
            <img src="@/assets/images/main_banner/right_arrow.png" />
          </a>

        </div>

      </div>

    </section>
    <section class="container-fluid color-white py-2">
      <div class="grid-home-looking-section">
        <div class="start-working-section p-5 h-100">
          <div class="py-sm-5 px-sm-2 px-lg-4">
            <label>JOB SEEKERS AREA</label>
            <h3 class="font-weight-bold">LOOKING FOR A JOB?</h3>
            <hr class="col-1 d-inline-flex b-color-white" />
            <h5 class="mb-4">
              Browse for a job, register with us, and begin to be part of our growing and successful satisfied workers’
              community.
            </h5>
            <job-search class="pt-4"></job-search>
          </div>
        </div>
        <div class="looking-for-worker-section p-5 h-100">
          <div class="py-sm-5 px-sm-2 px-lg-4">
            <label>BUSINESS AREA</label>
            <h3 class="font-weight-bold">LOOKING FOR EMPLOYEES?</h3>
            <hr class="col-1 d-inline-flex b-color-white" />
            <h5>Temporary - Permanent - Temp to Perm - Part Time - Peak - Direct Hiring</h5>
            <div class="d-lg-flex">
              <div class="ml-sm-4 ml-lg-4 d-flex">
                <router-link to="/business" class="button-rounded bg-grey-light color-white">
                  SEARCH PERSONNEL
                </router-link>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
    <section class="color-white pt-5 mt-5">
      <div class="app-section-grid direct-hiring-section-bg py-5 py-sm-0 px-5">
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
    JobSearch: () => import("@/components/landing/JobSearch")
  }
}

</script>


<style lang="scss">
@import "../../assets/scss/mixins";
@import "../../assets/scss/variables";

.direct-hiring-banner-section-bg {
  background-size: cover !important;
  height: 600px;

  &.img1 {
    background: url('../../assets/images/banners/direct_hiring_banner_bg.jpg') top center no-repeat;
  }

  &.img2 {
    background: url('../../assets/images/banners/direct_hiring_banner_bg2.jpg') top right no-repeat;
  }

  &.img3 {
    background: url('../../assets/images/banners/direct_hiring_banner_bg3.jpg') top right no-repeat;
  }
}

.direct-hiring-section-bg {
  background: url('../../assets/images/app_mobile_home_bg.png') no-repeat bottom center;
  background-size: contain;

  @include only-mobile {
    background-size: cover;
  }
}
</style>
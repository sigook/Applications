<template>
  <div>
    <section class="container-fluid">
      <div class="about-banner-section-bg color-grey">
        <div class="about-banner-image-grid p-4 py-sm-0 pt-md-4 pt-lg-5">
          <img src="@/assets/images/about_banner_img.png" class="w-100 align-self-end" />
          <div class="pt-4 py-sm-4">
            <h5 class="fw-medium">
              We are SIGOOK<label class="superscript">®</label> Work Factory, a staffing agency in Florida to serve the
              staffing needs of Palm Beach, Miami
              Dade, and Broward counties in South Florida,
              plus with our APP and our relationship with several career sources partners, we extend our services to other
              areas of Florida and the USA.
              We are working tirelessly for the success of our clients and job seekers by transforming the Staffing and
              Hiring at everyone's hands.
              SIGOOK<label class="superscript">®</label> combines experience in staffing business with top-notch
              technology to reach a wider community of job
              seekers and connect them with employers.
            </h5>
            <br />
            <h2 class="font-weight-bold">Download SIGOOK<label class="superscript">®</label> APP</h2>
            <div class="app-section-buttons-grid">
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
        </div>
      </div>
      <div class="px-5 py-4 bg-gray-light">
        <div class="about-banner-features-grid px-lg-5">
          <div>
            <h6><b-icon size="small" icon="check-circle" class="mr-1"></b-icon>Temporal or Permanent</h6>
            <h6><b-icon size="small" icon="check-circle" class="mr-1"></b-icon>Temp-To-Hire</h6>
            <h6><b-icon size="small" icon="check-circle" class="mr-1"></b-icon>Direct Hire</h6>
            <h6><b-icon size="small" icon="check-circle" class="mr-1"></b-icon>Contract</h6>
          </div>
          <div>
            <h6><b-icon size="small" icon="check-circle" class="mr-1"></b-icon>Light Industry</h6>
            <h6><b-icon size="small" icon="check-circle" class="mr-1"></b-icon>Clerical</h6>
            <h6><b-icon size="small" icon="check-circle" class="mr-1"></b-icon>Skilled Trades</h6>
            <h6><b-icon size="small" icon="check-circle" class="mr-1"></b-icon>IT roles</h6>
            <h6>... and more</h6>
          </div>
          <div>
            <h6><b-icon size="small" icon="check-circle" class="mr-1"></b-icon>Administrative & Clerical</h6>
            <h6><b-icon size="small" icon="check-circle" class="mr-1"></b-icon>Customer Service</h6>
            <h6><b-icon size="small" icon="check-circle" class="mr-1"></b-icon>Call Center</h6>
            <h6><b-icon size="small" icon="check-circle" class="mr-1"></b-icon>Warehouse/Supply Chain & logistic</h6>
            <h6><b-icon size="small" icon="check-circle" class="mr-1"></b-icon>Industrial and Manufacturing</h6>
            <h6><b-icon size="small" icon="check-circle" class="mr-1"></b-icon>Technology & IT</h6>
          </div>
        </div>
      </div>
    </section>
    <section class="container-fluid py-5">
      <div class="row p-2 p-sm-3 p-lg-4 text-justify text-sm-center">
        <div class="col-md-10 offset-md-1 col-lg-8 offset-lg-2 col-xl-6 offset-xl-3">
          <h2 class="color-grey font-weight-bold pb-2">
            Enjoy the new experience in <span class="color-red-light">Staffing!</span>
          </h2>
        </div>
      </div>
    </section>
    <section class="color-white">
      <div class="p-3 p-sm-4 p-lg-5 about-looking-section-bg">
        <div class="grid-home-looking-section">
          <div class="start-working-section p-5 h-100">
            <div class="py-sm-5 px-sm-2 px-lg-4">
              <label>JOB SEEKERS AREA</label>
              <h3 class="font-weight-bold">LOOKING FOR A JOB?</h3>
              <hr class="col-1 d-inline-flex b-color-white" />
              <h5 class="mb-4">Browse for a job, register with us, and begin to be part of our growing and successful
                community of satisfied workers.</h5>
              <job-search></job-search>
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

.about-banner-section-bg {
  background: url('../../assets/images/about_banner_bg.png') top center no-repeat;
  background-size: cover;
}

.about-looking-section-bg {
  background: url('../../assets/images/app_mobile_home_bg.png') top center no-repeat;
  background-size: cover;
}

/*#region BANNER */

.about-banner-image-grid {
  display: grid;
  align-items: center;
  justify-content: center;

  @include notebook {
    grid-template-columns: 30% 40%;
  }

  @include only-notebook {
    grid-gap: 4rem;
  }

  @include tablet {
    grid-gap: 6rem;
    grid-template-columns: 25% 40%;
  }

  @include desktop {
    grid-gap: 8rem;
  }
}

.about-banner-features-grid {
  display: grid;
  grid-gap: 2rem;

  @include tablet {
    justify-content: center;
    align-items: center;
    grid-template-columns: 30% 30% 30%;
  }
}
</style>
<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <job-position solutionType="business">
      <template v-slot:content>
        <section class="pt-5" id="needStaffSection">
          <div class="need-job-section">
            <div class="need-job-background sigook-red-section-bg">
            </div>
            <div class="p-4 p-sm-5 need-job-form-container">
              <img src="@/assets/images/need_staff_red.png" />
              <need-staff cssClass="mt-4 need-job-form" @onLoading="(value) => isLoading = value"></need-staff>
            </div>
          </div>
        </section>
      </template>
    </job-position>
  </div>
</template>


<script>

export default {
  components: {
    JobPosition: () => import("@/components/landing/JobPosition"),
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
@import "../../assets/scss/variables";

.sigook-red-section-bg {
  background: url('../../assets/images/sigook_red_section.jpg');
}

.need-job-section {
  display: grid;
  grid-template-columns: auto auto auto;

  .need-job-background {
    background-repeat: no-repeat;
    background-position: bottom;
    background-size: cover;
    grid-column: 1/4;
    grid-row: 1;
    margin-top: 7rem;
  }

  .need-job-form-container {
    grid-column: 2/3;
    grid-row: 1;
    margin-bottom: 7rem;
    background-color: $white;
    border-radius: 8px;
    box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.15),
      0 6px 15px 0 rgba(0, 0, 0, 0.15);

    @include only-mobile {
      margin-left: 1em;
      margin-right: 1em;

      img {
        width: 100%;
      }
    }

    .need-job-form {
      display: grid;
      grid-gap: 1em;

      @include tablet {
        grid-template-columns: auto auto auto;
        grid-template-rows: auto auto auto;
      }

      .need-job-text-area {
        resize: none;

        @include tablet {
          grid-column: 1/4;
        }
      }
    }
  }
}
</style>
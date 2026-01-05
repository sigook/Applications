<template>
  <div class="apply-container">
    <b-loading v-model="isLoading"></b-loading>
    <p class="alert-warning-red text-center" v-if="errorMessage" v-html="errorMessage"></p>
    <p class="alert-success text-center" v-if="successMessage">
      {{ successMessage }}
    </p>
    <div>
      <button v-on:click="redirectToHome" type="button" class="background-btn create-btn primary-button btn-radius">
        OK
      </button>
    </div>
  </div>
</template>

<script>
import toastMixin from "@/mixins/toastMixin";

export default {
  name: "WorkerApply",
  mixins: [toastMixin],
  data() {
    return {
      isLoading: false,
      errorMessage: null,
      successMessage: null,
      defaultSuccessMessage:
        "Thank you, one of our recruiters will contact you soon.",
    };
  },
  methods: {
    apply() {
      let workerId = this.$route.query.w;
      let requestId = this.$route.query.r;
      if (!workerId || !requestId) {
        this.redirectToHome();
        return;
      }

      const key = `${workerId}${requestId}`;
      let alreadyApplied = window.sessionStorage.getItem(key);
      if (alreadyApplied) {
        this.successMessage = this.defaultSuccessMessage;
        return;
      }

      this.isLoading = true;
      this.$store.dispatch("worker/workerRequestApply", {
          workerId,
          requestId,
          model: {},
        })
        .then(() => {
          this.isLoading = false;
          this.successMessage = this.defaultSuccessMessage;
          window.sessionStorage.setItem(key, "1");
        })
        .catch((error) => {
          this.isLoading = false;
          this.errorMessage = this.getErrorMessage(error);
          window.sessionStorage.setItem(key, "1");
        });
    },
    redirectToHome() {
      window.location.href = "/";
    },
  },
  created() {
    this.apply();
  },
};
</script>

<style scoped>
.apply-container {
  width: 100%;
  height: 100vh;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
}
</style>

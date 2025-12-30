<template>
  <div class="unsubscribe-container">
    <b-loading v-model="isLoading"></b-loading>
    <h3 class="text-center">Would you like to unsubscribe from these emails?</h3>
    <p class="alert-warning-red text-center" v-if="errorMessage" v-html="errorMessage">
    <div>
      <button v-on:click="redirectToHome" type="button" class="background-btn create-btn btn-radius">Cancel
      </button>
      <button v-on:click="unsubscribe" type="button" class="background-btn create-btn red-button btn-radius">Yes
      </button>
    </div>
  </div>
</template>

<script>
export default {
  name: "EmailPreferences",
  data() {
    return {
      isLoading: false,
      errorMessage: null
    }
  },
  methods: {
    unsubscribe() {

      let typeOfUser = this.$route.query.u;
      let userId = this.$route.query.id;
      let subscriptionType = this.$route.query.t;

      if (!typeOfUser || !userId || !subscriptionType) {
        this.redirectToHome();
        return;
      }

      const key = `${typeOfUser}${userId}${subscriptionType}`;
      let alreadyUnsubscribe = window.sessionStorage.getItem(key)
      if (alreadyUnsubscribe) {
        this.redirectToHome();
        return;
      }

      this.isLoading = true;
      this.$store.dispatch('shared/unsubscribe', {userId: userId, typeId: subscriptionType})
          .then(() => {
            this.isLoading = false;
            window.sessionStorage.setItem(key, '1');
            this.redirectToHome();
          })
          .catch(error => {
            this.isLoading = false;
            this.errorMessage = this.getErrorMessage(error);
            window.sessionStorage.setItem(key, '1');
          });
    },
    redirectToHome() {
      window.location.href = "/";
    }
  }
}
</script>

<style scoped>
.unsubscribe-container {
  width: 100%;
  height: 100vh;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
}
</style>
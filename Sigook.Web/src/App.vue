<template>
  <div id="app">
    <div class="message-version" v-if="isANewVersion">
      New version available!
      <button @click="updateAppVersion">
        Please click here to get the latest version.
      </button>
    </div>
    <div v-if="isCallback">
      <router-view />
    </div>
    <div v-else-if="isLogged">
      <nav-bar-logged></nav-bar-logged>
      <div class="container-fluid">
        <div class="white-container">
          <router-view />
        </div>
      </div>
    </div>
    <div v-else>
      <Header></Header>
      <router-view />
      <Footer></Footer>
    </div>
  </div>
</template>

<script>
import Oidc from "oidc-client";

export default {
  name: "app",
  data() {
    return {
      isMobile: false,
      isANewVersion: false,
      userManager: new Oidc.UserManager(),
      isLogged: false
    };
  },
  components: {
    NavBarLogged: () => import("@/components/NavBarLogged"),
    Header: () => import("@/components/landing/Header"),
    Footer: () => import("@/components/landing/Footer")
  },
  computed: {
    isCallback() {
      return this.$route.name === 'callback';
    }
  },
  async created() {
    if (
      navigator.userAgent.match(/Android/i) ||
      navigator.userAgent.match(/iPhone/i) ||
      navigator.userAgent.match(/iPod/i) ||
      navigator.userAgent.match(/BlackBerry/i)
    ) {
      this.isMobile = true;
      this.$store.commit("showMobile");
    }

    this.getAppVersion();
    let vm = this;
    let delay = 60 * 60 * 1000; // 1 hour in msec
    setInterval(
      function () {
        vm.getAppVersion();
      }.bind(this),
      delay
    );
    await this.setIsLogged(this.$route);
  },
  methods: {
    getAppVersion() {
      const item = localStorage.getItem("versionApp");
      this.$store.dispatch("getLasVersion")
        .then((response) => {
          if (item === "null" || item === null) {
            localStorage.setItem("versionApp", response.version);
          } else {
            if (response.version !== item) {
              this.isANewVersion = true;
            }
          }
        })
    },
    updateAppVersion() {
      localStorage.removeItem("versionApp");
      // Clear all caches and force reload
      if ('caches' in window) {
        caches.keys().then(names => {
          names.forEach(name => caches.delete(name));
        }).then(() => {
          window.location.reload(true);
        });
      } else {
        window.location.reload(true);
      }
    },
    async setIsLogged(route) {
      this.isLogged = await this.$store.dispatch("getUser") ? true : false;
      if (!route.meta.requiresAuth) {
        this.isLogged = false;
      }
    }
  },
  watch: {
    "$route": async function (value) {
      await this.setIsLogged(value);
    }
  }
};
</script>

<style lang="scss">
@import "assets/scss/variables";
@import "assets/fonts.css";
@import "assets/scss/master";

.no-menu {
  display: none;
}

.main-container {
  background-color: $gray-bg;
  min-height: calc(100vh - 63px);
  padding: 30px;
}

.white-container {
  background-color: white;
  padding: 30px;
  box-shadow: 6px 6px 10px #d6d6d6;
  position: relative;
  min-height: 600px;
}

.message-version {
  background-color: #13dde8;
  color: white;
  text-align: center;
  padding: 5px 10px;

  button {
    display: block;
    font-size: 14px;
    color: #fff;
    margin: 5px auto;
    border: 0;
    border-bottom: 1px solid white;
  }
}

@media (max-width: 767px) {
  .main-container {
    min-height: auto;
    padding: 0;
    background-color: #fff;
  }

  .white-container {
    padding: 15px;
    box-shadow: none;
    background-color: $gray-bg;
  }
}
</style>

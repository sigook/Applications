<template>
  <div class="container-fluid">
    <div class="p-2 d-none d-sm-flex flex-wrap align-items-center justify-content-between">
      <div class="d-flex folders">
        <router-link to="/jobSeekers" class="p-2 mr-1">
          <strong>FOR JOB SEEKERS</strong>
        </router-link>
        <router-link to="/business" class="p-2">
          <strong>FOR BUSINESS</strong>
        </router-link>
      </div>
      <button class="btn btn-primary px-4" @click="login">{{ loginButton }}</button>
    </div>
    <nav class="navbar navbar-expand-lg navbar-sigook">
      <div class="container-fluid">
        <router-link to="/home" class="d-flex p-2">
          <img src="@/assets/images/sigook_blue_logo.png" width="200" />
        </router-link>
        <button class="navbar-toggler menu-btn py-2" type="button" data-toggle="collapse" data-target="#sigookMainMenu"
          aria-controls="sigookMainMenu" aria-expanded="false" aria-label="Toggle navigation">
          <b-icon icon="menu" />
        </button>
        <div class="collapse navbar-collapse justify-content-end" id="sigookMainMenu">
          <ul class="nav">
            <li class="nav-item d-sm-none">
              <button class="nav-link" active-class="menu-active" @click="login">LOGIN</button>
            </li>
            <li class="nav-item">
              <router-link to="/home" class="nav-link" active-class="menu-active">HOME</router-link>
            </li>
            <li class="nav-item">
              <router-link to="/jobSeekers" class="nav-link" active-class="menu-active">FOR JOB SEEKERS</router-link>
            </li>
            <li class="nav-item">
              <router-link to="/business" class="nav-link" active-class="menu-active">FOR BUSINESS</router-link>
            </li>
            <li class="nav-item">
              <router-link to="/direct-hiring" class="nav-link" active-class="menu-active">DIRECT HIRING</router-link>
            </li>
            <li class="nav-item">
              <router-link to="/about-us" class="nav-link" active-class="menu-active">ABOUT US</router-link>
            </li>
            <li class="nav-item">
              <router-link to="/contact" class="nav-link" active-class="menu-active">CONTACT</router-link>
            </li>
          </ul>
        </div>
      </div>
    </nav>
  </div>
</template>


<script>

export default {
  methods: {
    async login() {
      await this.$store.dispatch('signIn');
    }
  },
  computed: {
    hiddeHeader() {
      return this.$route.query.hiddeHeader === "true";
    },
    loginButton() {
      if (this.$store.state.security.user) {
        return 'Go to Portal'
      } else {
        return 'Login'
      }
    }
  }
}

</script>

<style lang="scss">
@import "../../assets/scss/mixins";
@import "../../assets/scss/variables";

.folders {
  >a {
    border-radius: 20px 20px 0 0;
    width: 200px;
    color: $white !important;
    text-align: center;
    text-decoration: none !important;
  }

  >a:first-of-type {
    background-color: $blue-light !important;
  }

  >a:nth-of-type(2) {
    background-color: $red-light !important;
  }
}

.navbar-sigook {
  align-items: center !important;

  @include only-mobile {
    flex-direction: column;
    align-items: flex-start;
  }

  >a {
    display: flex;

    @include only-mobile {
      width: 100%;
    }

    >img {
      @include only-mobile {
        width: 310px;
      }
    }
  }

  nav {
    margin-left: auto !important;

    @include only-mobile {
      margin-left: unset;
    }

    .nav-item {
      a {
        font-weight: bold;
        color: #343434;
        text-decoration: none;

        &:hover {
          color: #7d7d7d;
        }
      }
    }
  }
}

.menu-btn {
  border: 1px solid gray;
}

.menu-active {
  color: #7d7d7d !important;
}
</style>
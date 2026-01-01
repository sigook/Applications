<template>
  <section class="worker-comments" id="comments">
    <b-loading v-model="isLoading"></b-loading>

    <div class="button-right" v-if="!onlyView">
      <h3>{{ $t('WorkerCommentsAndQualification') }}</h3>
      <button class="outline-btn md-btn orange-button btn-radius" @click="alertComment">{{ $t('WorkerAddComment') }}
        +</button>
    </div>
    <div>
      <div class="comment" v-for="comment in data.items" v-bind:key="comment.id">
        <img :src="comment.logo ? comment.logo : require('../assets/images/icon_agency.svg')">
        <div>
          <!--<h4>King Company</h4>-->
          <p class="fisrt-letter">{{ comment.comment }}</p>
          <div class="content-rating">
            <span class="icon-rating" v-for="star in comment.rate" v-bind:key="star">
              <span style="display: none;">{{ star }}</span>
            </span>
          </div>
          <span class="line-gray"></span>
        </div>
      </div>
    </div>
    <pagination :total-pages="data.totalPages" :index-page="data.pageIndex" :size-page="this.sizeComments"
      @changePage="(index) => changePage(index)">
    </pagination>




    <!-- custom modal -->
    <transition name="modal">
      <div v-if="modalValidation" class="vue-modal">
        <div class="modal-mask">
          <div class="modal-wrapper">
            <div class="modal-container small-container modal-light">
              <button @click="modalValidation = false" class="cross-icon">{{ $t('Close') }}</button>
              <!-- <cancel-list @sendReason="(reason) => cancelRequest(reason)"></cancel-list>-->
              <dialog-comment @createComment="(data) => getComment(data)"></dialog-comment>
            </div>
          </div>
        </div>
      </div>
    </transition>
    <!-- end custom modal -->



  </section>
</template>

<script>
import roles from "@/security/roles";

export default {
  data() {
    return {
      isLoading: false,
      currentPage: 1,
      commentary: {
        comment: "",
        rate: 0
      },
      modalValidation: false
    }
  },
  props: ['userId', 'data', "sizeComments", "onlyView"],
  components: {
    DialogComment: () => import("./DialogWorkerComment"),
    Pagination: () => import("./Paginator")
  },
  methods: {
    alertComment() {
      this.modalValidation = true;

    },
    getComment(data) {
      this.modalValidation = false;
      if (data.comment && data.rating) {
        this.commentary.comment = data.comment;
        this.commentary.rate = data.rating;
        this.comment();
      } else {
        this.showAlertError(this.$t("AllFieldsAreRequired"))
      }
    },
    comment() {
      const userRoles = this.$store.state.security.userRoles;
      for (let i = 0; i < userRoles.length; i++) {
        switch (userRoles[i]) {
          case roles.agency:
          case roles.agencyPersonnel:
            this.sendComment('agency/agencyCommentWorker');
            return;
          case roles.company:
          case roles.companyUser:
            this.sendComment('company/companyCommentWorker');
            return;
        }
      }
    },
    sendComment(url) {
      this.isLoading = true;
      this.$store.dispatch(url, { id: this.userId, comment: this.commentary })
        .then(() => {
          this.$emit('newComment', true);
          this.isLoading = false;
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    changePage(page) {
      //this.currentPage = page;
      this.$emit('changePage', page);
    }
  }
}
</script>

<style lang="scss">
p.fisrt-letter::first-letter {
  text-transform: uppercase;
}

.swal2-show {
  -webkit-animation: showSweetAlert .4s !important;
  animation: showSweetAlert .4s !important;
}

.swal2-hide {
  -webkit-animation: hideSweetAlert 0s !important;
  animation: hideSweetAlert 0s !important;
}

@keyframes showSweetAlert {
  from {
    transform: scale(.5);
    opacity: 0;
  }

  to {
    transform: scale(1);
    opacity: 1;
  }
}

@keyframes hideSweetAlert {
  to {
    opacity: 0;
  }

  from {
    opacity: 1;
  }
}
</style>
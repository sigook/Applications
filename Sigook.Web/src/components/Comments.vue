<template>
  <section class="worker-comments" id="comments">
    <b-loading v-model="isLoading"></b-loading>

    <div class="d-flex align-items-center justify-content-between" v-if="!onlyView">
      <h3>{{ 'Comments & Qualification' }}</h3>
      <button class="outline-btn md-btn orange-button btn-radius" @click="alertComment">{{ 'Add Comment' }}
        +</button>
    </div>
    <div>
      <div class="comment" v-for="comment in data.items" v-bind:key="comment.id">
        <img :src="comment.logo ? comment.logo : iconAgency">
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
    <pagination :total-pages="data.totalPages" :index-page="data.pageIndex" :size-page="sizeComments"
      @changePage="(index) => changePage(index)">
    </pagination>




    <!-- custom modal -->
    <transition name="modal">
      <div v-if="modalValidation" class="vue-modal">
        <div class="modal-mask">
          <div class="modal-wrapper">
            <div class="modal-container small-container modal-light">
              <button @click="modalValidation = false" class="cross-icon">{{ 'Close' }}</button>
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

<script setup lang="ts">
import { ref } from 'vue';
import { useSecurityStore } from '@/stores/security';
import { showAlertError } from "@/utils/toast";
import roles from "@/security/roles";
import { agencyCommentWorker } from "@/api/agencyWorkerApi";
import { companyCommentWorker } from "@/api/companyApi";
import iconAgency from '@/assets/images/icon_agency.svg';
import DialogComment from "./DialogWorkerComment.vue";
import Pagination from "./Paginator.vue";

const props = defineProps<{
  userId?: any;
  data?: any;
  sizeComments?: number;
  onlyView?: boolean;
}>();

const emit = defineEmits<{
  (e: 'newComment', v: boolean): void;
  (e: 'changePage', page: number): void;
}>();

const securityStore = useSecurityStore();

const isLoading = ref(false);
const commentary = ref({ comment: '', rate: 0 });
const modalValidation = ref(false);

function alertComment() {
  modalValidation.value = true;
}

function getComment(data: { comment: string; rating: number }) {
  modalValidation.value = false;
  if (data.comment && data.rating) {
    commentary.value.comment = data.comment;
    commentary.value.rate = data.rating;
    comment();
  } else {
    showAlertError("All fields are required");
  }
}

function comment() {
  const userRoles = securityStore.userRoles;
  for (let i = 0; i < userRoles.length; i++) {
    switch (userRoles[i]) {
      case roles.agency:
      case roles.agencyPersonnel:
        sendComment(agencyCommentWorker);
        return;
      case roles.company:
      case roles.companyUser:
        sendComment(companyCommentWorker);
        return;
    }
  }
}

function sendComment(commentFn: (userId: any, c: any) => Promise<any>) {
  isLoading.value = true;
  commentFn(props.userId, commentary.value)
    .then(() => {
      emit('newComment', true);
      isLoading.value = false;
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function changePage(page: number) {
  emit('changePage', page);
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

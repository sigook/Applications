<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <section class="worker-comments">
      <comments v-if="comments" :user-id="this.worker.workerId" :data="comments" :size-comments="this.commentSize"
        only-view="true" @newComment="() => updateComments()" @changePage="page => changePageComments(page)"></comments>
    </section>
  </div>
</template>


<script lang="ts">
import { defineAsyncComponent } from 'vue';
import { getCommentsWorker } from '@/api/workerApi';

export default {
  props: ['worker'],
  data() {
    return {
      isLoading: true,
      commentSize: 10,
      commentPageIndex: 1,
      comments: []
    }
  },
  components: {
    Comments: defineAsyncComponent(() => import("../../components/Comments.vue"))
  },
  methods: {
    updateComments() {
      this.isLoading = true;
      getCommentsWorker({ workerId: this.worker.workerId, size: this.commentSize, pageIndex: this.commentPageIndex })
        .then((data) => {
          this.comments = data;
          this.isLoading = false;
        });

    },
    changePageComments(page) {
      this.commentPageIndex = page;
      this.updateComments();
    }
  },
  created() {
    this.updateComments();
  }
}

</script>

<style lang="scss">
@import '../../assets/scss/detail-worker';
</style>
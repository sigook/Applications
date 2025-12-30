<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <section class="worker-comments">
      <comments v-if="comments" :user-id="this.worker.workerId" :data="comments" :size-comments="this.commentSize"
        only-view="true" @newComment="() => updateComments()" @changePage="page => changePageComments(page)"></comments>
    </section>
  </div>
</template>


<script>
export default {
  props: ['worker'],
  data() {
    return {
      isLoading: true,
      commentSize: 10,
      commentPageIndex: 1
    }
  },
  components: {
    Comments: () => import("../../components/Comments")
  },
  methods: {
    updateComments() {
      this.isLoading = true;
      this.$store.dispatch('worker/getCommentsWorker', { workerId: this.worker.workerId, size: this.commentSize, pageIndex: this.commentPageIndex })
        .then(() => {
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
  },
  computed: {
    comments() {
      return this.$store.state.worker.workerComments;
    }
  }

}

</script>

<style lang="scss">
@import '../../assets/scss/detail-worker';
</style>
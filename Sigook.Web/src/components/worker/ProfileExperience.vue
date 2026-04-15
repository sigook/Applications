<template>
  <form>
    <b-loading v-model="isLoading"></b-loading>
    <div class="tab-actions">
      <b-button type="is-primary" icon-right="plus" @click="modalWorkExperience = true">
        {{ "Add Experience" }}
      </b-button>
    </div>
    <div>
      <work-experience-detail
        v-for="(item, index) in worker.jobExperiences"
        :key="'workExp' + index"
        :item="item"
        :workerId="worker.id"
        @getWorker="() => updateExperience()" />

      <b-modal v-model="modalWorkExperience" width="800px">
        <work-experience :workerId="worker.id" @updateExperience="() => updateExperience()" />
      </b-modal>
    </div>

  </form>
</template>

<script lang="ts">
import { defineAsyncComponent } from 'vue';
import { mapStores } from 'pinia';
import { useAppStore } from '@/stores/app';
export default {
  props: ['worker'],
  data() {
    return {
      disableStartDate: null,
      modalWorkExperience: false,
      isLoading: false
    }
  },
  computed: {
    ...mapStores(useAppStore),
  },
  methods: {
    disableEndDate(index) {
      let disabledDates = this.appStore.currentDate;

      if (this.worker.jobExperiences[index].startDate) {
        disabledDates = this.worker.jobExperiences[index].startDate
      }

      return disabledDates;
    },
    updateExperience() {
      this.modalWorkExperience = false;
      this.$emit('updateProfile');
    }
  },
  components: {
    workExperience: defineAsyncComponent(() => import("./WorkExperienceForm.vue")),
    workExperienceDetail: defineAsyncComponent(() => import("../../components/worker/WorkExperienceDetail.vue"))
  },
  created() {
    this.appStore.getCurrentDate().then(response => {
      this.disableStartDate = response;
    })
  }
}
</script>

<style scoped>
.tab-actions {
  display: flex;
  justify-content: flex-end;
  margin-bottom: 12px;
}
</style>

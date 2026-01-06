<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12">
        <b-field :label="$t('Availabilities')" class="has-text-weight-normal">
          <b-checkbox v-for="item in availabilities" :key="item.id" v-model="worker.availabilities" :native-value="item"
            class="mb-2">
            {{ item.value }}
          </b-checkbox>
        </b-field>
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="createWorkerAvailabilities()">
          {{ $t("Save") }}
        </b-button>
      </div>
    </div>
  </div>
</template>
<script>
import toastMixin from "../../mixins/toastMixin";
export default {
  props: ['data'],
  data() {
    return {
      isLoading: false,
      availabilities: [],
      worker: {
        availabilities: []
      }
    }
  },
  mixins: [toastMixin],
  methods: {
    createWorkerAvailabilities() {
      this.isLoading = true;
      this.$store.dispatch('worker/createWorkerAvailabilities', { profileId: this.data.id, model: this.worker.availabilities })
        .then(() => {
          this.isLoading = false;
          this.$emit('closeModal', true);
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    }
  },
  async created() {
    this.availabilities = await this.$store.dispatch('getAvailability');
    if (this.data != null) {
      this.worker.availabilities = this.data.availabilities;
    }
  }
}
</script>
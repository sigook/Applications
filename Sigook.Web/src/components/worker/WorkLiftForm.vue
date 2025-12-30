<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12">
        <b-field :label="$t('WorkerYouCanLift')" class="has-text-weight-normal"
          :type="errors.has('lift') ? 'is-danger' : ''">
          <b-select v-model="worker.lift.id" placeholder="Select option" expanded 
            name="lift" v-validate="'required'">
            <option v-for="item in lifts" :value="item.id" v-bind:key="item.id">
              {{ item.value }}
            </option>
          </b-select>
        </b-field>
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="createWorkerOther()">
          {{ $t("Save") }}
        </b-button>
      </div>
    </div>
  </div>
</template>
<script>

export default {
  props: ['data'],
  data() {
    return {
      isLoading: false,
      lifts: [],
      worker: {
        lift: {}
      }
    }
  },
  methods: {
    createWorkerOther() {
      this.$validator.validateAll().then((isValid) => {
        if (isValid) {
          this.isLoading = true;
          this.$store.dispatch('worker/createWorkerOther', { profileId: this.data.id, model: this.worker })
            .then(() => {
              this.isLoading = false;
              this.$emit('closeModal', true);
            })
            .catch(error => {
              this.isLoading = false;
              this.showAlertError(error);
            })
        } else {
          this.showAlertError(this.$t('PleaseVerifyThatTheFieldsAreCorrect'));
        }
      });
    }
  },
  async created() {
    this.lifts = await this.$store.dispatch('getLifts');
    if (this.data != null) {
      this.worker.lift = Object.assign({}, this.data.lift);
    }
  }
}
</script>
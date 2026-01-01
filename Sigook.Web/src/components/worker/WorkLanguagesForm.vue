<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12">
        <b-field label="Languages">
          <b-taginput v-model="worker.languages" autocomplete :data="filteredLanguages" open-on-focus field="value"
            icon="label" placeholder="Select Languages" @typing="getFilteredLanguages" append-to-body>
          </b-taginput>
        </b-field>
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="createWorkerLanguages()">
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
      languages: [],
      filteredLanguages: [],
      worker: {
        languages: []
      }
    }
  },
  methods: {
    createWorkerLanguages() {
      this.isLoading = true;
      this.$store.dispatch('worker/createWorkerLanguages', { profileId: this.data.id, model: this.worker.languages })
        .then(() => {
          this.isLoading = false;
          this.$emit('closeModal', true);
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    },
    getFilteredLanguages(text) {
      this.filteredLanguages = this.languages.filter((option) => 
        option.value.toLowerCase().includes(text.toLowerCase())
      );
    }
  },
  async created() {
    this.languages = await this.$store.dispatch('getLanguages');
    this.filteredLanguages = this.languages;
    if (this.data != null) {
      this.worker.languages = this.data.languages;
    }
  }
}
</script>
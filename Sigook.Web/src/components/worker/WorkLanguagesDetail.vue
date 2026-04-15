<template>
  <div class="hover-transform">
    <div class="detail-worker-profile">
      <span class="width-30">{{ "Languages" }}</span>
      <span class="width-70 items">
        <b-taglist>
          <b-tag v-for="item in worker.languages" :key="'languages' + item.value" type="is-info is-light" size="is-medium" rounded>{{ item.value }}</b-tag>
        </b-taglist>
      </span>
      <b-button type="is-info" outlined rounded icon-right="pencil" @click="modalLanguages = true"></b-button>
    </div>
    <b-modal v-model="modalLanguages" width="500px">
      <languages-edit :data="worker" @closeModal="() => closeModalEdit()" />
    </b-modal>
  </div>
</template>
<script lang="ts">
import { defineAsyncComponent } from 'vue';
export default {
  props: ['worker'],
  data() {
    return {
      modalLanguages: false
    }
  },
  methods: {
    closeModalEdit() {
      this.$emit('updateProfile', true);
      this.modalLanguages = false
    }
  },
  components: {
    languagesEdit: defineAsyncComponent(() => import("./WorkLanguagesForm.vue"))
  }
}
</script>
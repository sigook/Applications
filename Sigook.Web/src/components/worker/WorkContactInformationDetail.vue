<template>
  <section>
    <div class="button-right">
      <h3 class="section-title">{{ "Contact information" }}</h3>
      <b-button type="is-info" outlined rounded icon-right="pencil"
        @click="modalContactInformation = true"></b-button>
    </div>
    <div class="worker-documents">
      <div v-if="worker.location">
        <span>{{ 'Address' }}</span>
        <span>
          <p class="fw-200 margin-0 capitalize">
            {{ worker.location.address }}
            {{ worker.location.city.value }}, {{ worker.location.city.province.code }}
            {{ worker.location.postalCode }}
          </p>
        </span>
      </div>
      <div>
        <span>{{ 'Mobile Number' }}</span>
        <span>
          <p class="fw-200 margin-0">{{ worker.mobileNumber }}</p>
        </span>
      </div>
      <div v-if="worker.phone">
        <span>{{ 'Phone' }}</span>
        <span>
          <p class="fw-200 margin-0">{{ worker.phone }}
            <span v-if="worker.phoneExt" class="fw-200">
              <b>{{ 'Ext.' }}:</b>
              {{ worker.phoneExt }}
            </span>
          </p>
        </span>
      </div>
    </div>
    <b-modal v-model="modalContactInformation" width="800px">
      <contact-information-edit :data="worker" @closeModal="() => closeModalEdit()" />
    </b-modal>
  </section>
</template>

<script lang="ts">
import { defineAsyncComponent } from 'vue';
export default {
  props: ['worker'],
  data() {
    return {
      modalContactInformation: false
    }
  },
  methods: {
    closeModalEdit() {
      this.$emit('updateProfile', true);
      this.modalContactInformation = false
    }
  },
  components: {
    contactInformationEdit: defineAsyncComponent(() => import("./WorkContactInformationForm.vue"))
  }
}
</script>
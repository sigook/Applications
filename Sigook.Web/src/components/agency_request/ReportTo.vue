<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="is-flex is-flex-wrap-wrap is-justify-content-space-between">
      <h3 class="has-text-weight-bold">Report to</h3>
      <b-button v-if="canEdit" type="is-primary" size="is-small" outlined rounded @click="showModal = true">Add</b-button>
    </div>
    <div>
      <ul class="p-1">
        <li v-for="(item) in contacts" :key="item.id"
          class="content-flex-between is-align-items-center mb-0 hover-actions fz-14">
          <span class="is-inline-block valign-middle">{{ item.firstName }} {{ item.lastName }}</span>
          <b-button v-if="canEdit" type="is-ghost" size="is-small" icon-left="close"
            class="contact-remove actions" :aria-label="`Remove ${item.firstName} ${item.lastName}`"
            @click="removeReportTo(item)" />
        </li>
      </ul>
    </div>
    <b-modal custom-content-class="card" v-model="showModal" width="600px">
      <contact-list :companyProfileId="companyProfileId" :activeUsers="contacts"
        @removeContact="removeReportTo" @selectContact="addReportTo" />
    </b-modal>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/utils/toast";
import { postAgencyRequestReportTo, deleteAgencyRequestReportTo } from "@/api/agencyRequestApi";
import ContactList from './ContactListModal.vue';
import type { AgencyCompanyContactPerson, AgencyRequestPersonItem } from "@/types/agency";

const props = defineProps<{ requestId: string; companyProfileId: string; canEdit?: boolean }>();
const contacts = defineModel<AgencyRequestPersonItem[]>({ required: true });

const showModal = ref(false);
const isLoading = ref(false);

function addReportTo(item: AgencyCompanyContactPerson) {
  isLoading.value = true;
  postAgencyRequestReportTo(props.requestId, item.id!)
    .then(() => {
      isLoading.value = false;
      contacts.value = [...contacts.value, { ...item, id: item.id! }];
      showModal.value = false;
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function removeReportTo(item: { id?: string }) {
  isLoading.value = true;
  deleteAgencyRequestReportTo(props.requestId, item.id!)
    .then(() => {
      isLoading.value = false;
      contacts.value = contacts.value.filter(x => x.id !== item.id);
      showModal.value = false;
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}
</script>

<style lang="scss" scoped>
@import '../../assets/scss/variables';

.button.contact-remove {
  width: 28px;
  height: 28px;
  padding: 0;
  color: $grey-font;

  &:hover,
  &:focus {
    background: $gray-bg;
    color: $danger-hover;
    text-decoration: none;
  }
}
</style>

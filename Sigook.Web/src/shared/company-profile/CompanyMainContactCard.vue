<template>
  <detail-card title="Main contact">
    <template v-if="contacts.length > 1" #actions>
      <b-button type="is-ghost" size="is-small" class="detail-link" @click="emit('showAll')">
        All {{ contacts.length }}
      </b-button>
    </template>
    <div v-if="main" class="detail-list-item">
      <span class="company-contact-name">{{ fullName(main) }}</span>
      <span v-if="main.position" class="detail-muted">{{ main.position }}</span>
      <a v-if="main.mobileNumber" :href="`tel:${main.mobileNumber}`">{{ main.mobileNumber }}</a>
      <a v-if="main.email" :href="`mailto:${main.email}`">{{ main.email }}</a>
    </div>
    <div v-else class="company-contact-empty">
      <span class="detail-empty">No contacts</span>
      <b-button type="is-ghost" size="is-small" class="detail-link" @click="emit('showAll')">Add contact</b-button>
    </div>
  </detail-card>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { showAlertError } from '@/shared/utils/toast';
import type { CompanyContactSummary } from '@/shared/company-profile/types';
import DetailCard from '@/shared/detail-page/DetailCard.vue';

const props = defineProps<{ fetchContacts: () => Promise<CompanyContactSummary[]> }>();
const emit = defineEmits<{
  (e: 'showAll'): void;
  (e: 'loaded', count: number): void;
}>();

const contacts = ref<CompanyContactSummary[]>([]);
const main = computed(() => contacts.value[0]);

function fullName(contact: CompanyContactSummary) {
  return [contact.firstName, contact.middleName, contact.lastName].filter(Boolean).join(' ');
}

props.fetchContacts()
  .then((response) => {
    contacts.value = response;
    emit('loaded', response.length);
  })
  .catch(showAlertError);
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';

.company-contact-name {
  font-weight: 600;
}

.detail-list-item a {
  color: $blue;
}

.company-contact-empty {
  display: flex;
  align-items: center;
  justify-content: space-between;
}
</style>

<template>
  <div class="p-3">
    <div class="columns is-multiline">
      <div class="column is-12">
        <b-field label="Numbers">
          <b-autocomplete ref="autoCompleteNumbers" v-model="selectedNumber" :data="numbers" :loading="isLoading"
            @typing="search" selectable-footer append-to-body @select-footer="onSelectFooter">
            <template #footer>
              <a><span> Add new... </span></a>
            </template>
            <template #empty>Skip Payroll Number not found</template>
          </b-autocomplete>
        </b-field>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { BAutocomplete } from 'buefy';
import { getSkipPayrollNumbers, addSkipPayrollNumber } from "@/modules/agency/accounting/paystubs/api";
import { getDialog } from '@/shared/utils/buefyProgrammatic';
import { useDebouncedSearch } from '@/shared/composables/useDebouncedSearch';

const { results: numbers, isSearching: isLoading, search } =
  useDebouncedSearch((term) => getSkipPayrollNumbers({ searchTerm: term }), { minLength: 1 });
const selectedNumber = ref<string | null>(null);
const autoCompleteNumbers = ref<InstanceType<typeof BAutocomplete> | null>(null);

function onSelectFooter() {
  getDialog().prompt({
    message: "Number",
    inputAttrs: {
      type: "text",
      placeholder: "Number",
      value: selectedNumber.value
    },
    closeOnConfirm: false,
    confirmText: 'Add',
    onConfirm: async (value, dialog) => {
      await addSkipPayrollNumber({ value: value });
      autoCompleteNumbers.value?.setSelected(value);
      dialog.close();
    }
  });
}
</script>

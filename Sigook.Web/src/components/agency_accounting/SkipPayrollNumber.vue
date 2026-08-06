<template>
  <div class="p-3">
    <div class="columns is-multiline">
      <div class="column is-12">
        <b-field label="Numbers">
          <b-autocomplete ref="autoCompleteNumbers" v-model="selectedNumber" :data="numbers" :loading="isLoading"
            @typing="onInputEntered" selectable-footer append-to-body @select-footer="onSelectFooter">
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
import { showAlertError } from "@/utils/toast";
import { getSkipPayrollNumbers, addSkipPayrollNumber } from "@/api/agencyPayStubApi";
import { getDialog } from '@/utils/buefyProgrammatic';

const isLoading = ref(false);
const numbers = ref<any[]>([]);
const selectedNumber = ref<any>(null);
const autoCompleteNumbers = ref<any>(null);

function onInputEntered(text: string) {
  getAllNumbers(text);
}

function getAllNumbers(text: string) {
  isLoading.value = true;
  getSkipPayrollNumbers({ searchTerm: text })
    .then(response => {
      isLoading.value = false;
      numbers.value = response;
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

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
    onConfirm: async (value: string, dialog: any) => {
      await addSkipPayrollNumber({ value: value });
      autoCompleteNumbers.value?.setSelected(value);
      dialog.close();
    }
  });
}
</script>

<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <b-message type="is-warning" has-icon>
      Please note that all workers in the order will be rejected.
    </b-message>
    <div class="columns is-multiline cancellation-list">
      <div class="item column is-12" v-for="item in cancellationList" v-bind:key="item.id"
        @click="reasonSelected = item" :class="{ 'selected': item === reasonSelected }">
        {{ item.value }}
      </div>
      <transition name="fadeHeight">
        <div v-if="reasonSelected" class="">
          <b-field label="Please indicate the reason" :type="formErrors.reason ? 'is-danger' : ''"
            :message="formErrors.reason || ''">
            <b-input type="textarea" v-model="reason" name="reason" />
          </b-field>
        </div>
      </transition>
      <div class="column is-12">
        <b-button type="is-primary" @click="validateInput">{{ "Send" }}</b-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import { getReasonCancellationRequest } from "@/api/catalogApi";

const emit = defineEmits<{ (e: 'sendReason', payload: { reasonId: any; otherMessage: string }): void }>();

const schema = yup.object({
  reason: yup.string().required('Reason is required'),
});

const form = useStickyForm<{ reason: string }>({
  schema,
  initialValues: { reason: '' },
});
const { reason } = form.fields;
const formErrors = form.errors;

const isLoading = ref(true);
const reasonSelected = ref<any>(null);
const cancellationList = ref<any[]>([]);

(async () => {
  cancellationList.value = await getReasonCancellationRequest();
  isLoading.value = false;
})();

function validateInput() {
  if (!reasonSelected.value) {
    showAlertError('Please select a cancellation reason');
    return;
  }
  form.markInteracted();
  form.handleSubmit((values) => {
    emit('sendReason', { reasonId: reasonSelected.value.id, otherMessage: values.reason });
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}
</script>
<style lang="scss" scoped>
.cancellation-list {

  .item {
    border-bottom: 1px solid #adadad;
    cursor: pointer;

    &:last-child {
      border-bottom: none;
    }

    &:hover {
      background-color: #f3f3f3;
    }

    &.selected {
      background: #d9d9d9;
    }
  }
}
</style>

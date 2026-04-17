<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <b-message type="is-warning" has-icon>
      Please note that all workers in the order will be rejected.
    </b-message>
    <div class="container-flex cancellation-list">
      <div class="item col-12 col-padding" v-for="item in cancellationList" v-bind:key="item.id"
        @click="reasonSelected = item" :class="{ 'selected': item === reasonSelected }">
        {{ item.value }}
      </div>
      <transition name="fadeHeight">
        <div v-if="reasonSelected" class="col-12 col-padding">
          <b-field label="Please indicate the reason" :type="formErrors.reason ? 'is-danger' : ''"
            :message="formErrors.reason || ''">
            <b-input type="textarea" v-model="reason" name="reason" />
          </b-field>
        </div>
      </transition>
      <div class="col-12 col-padding">
        <b-button type="is-primary" @click="validateInput">{{ "Send" }}</b-button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import { getReasonCancellationRequest } from "@/api/catalogApi";

const schema = yup.object({
  reason: yup.string().required('Reason is required'),
});

export default {
  setup() {
    const form = useStickyForm({
      schema,
      initialValues: { reason: '' },
    });
    return {
      ...form.fields,
      formErrors: form.errors,
      handleSubmit: form.handleSubmit,
      markInteracted: form.markInteracted,
    };
  },
  data() {
    return {
      isLoading: true,
      reasonSelected: null as any,
      cancellationList: [] as any[],
    }
  },
  async created() {
    this.cancellationList = await getReasonCancellationRequest();
    this.isLoading = false;
  },
  methods: {
    validateInput() {
      if (!this.reasonSelected) {
        showAlertError('Please select a cancellation reason');
        return;
      }
      this.markInteracted();
      this.handleSubmit((values) => {
        this.$emit('sendReason', { reasonId: this.reasonSelected.id, otherMessage: values.reason });
      }, () => {
        showAlertError('Please make sure all required fields are filled out correctly');
      })();
    }
  }
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

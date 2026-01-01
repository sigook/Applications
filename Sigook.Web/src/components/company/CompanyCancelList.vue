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
          <b-field label="Please indicate the reason" :type="errors.has('reason') ? 'is-danger' : ''"
            :message="errors.has('reason') ? errors.first('reason') : ''">
            <b-input type="textarea" v-model="reasonMessage" name="reason" v-validate="'required'" />
          </b-field>
        </div>
      </transition>
      <div class="col-12 col-padding">
        <b-button type="is-primary" @click="validateInput">{{ $t("Send") }}</b-button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      isLoading: true,
      reasonSelected: null,
      reasonMessage: null,
      cancellationList: []
    }
  },
  async created() {
    this.cancellationList = await this.$store.dispatch('getReasonCancellationRequest');
    this.isLoading = false;
  },
  methods: {
    validateInput() {
      this.$validator.validateAll().then((result) => {
        if (result) {
          this.$emit('sendReason', { reasonId: this.reasonSelected.id, otherMessage: this.reasonMessage });
        }
      });
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
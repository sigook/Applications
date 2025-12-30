<template>
  <div class="p-3">
    <div class="container-flex">
      <div class="col-12 col-padding">
        <b-field label="Comment" :type="errors.has('comment') ? 'is-danger' : ''"
          :message="errors.has('comment') ? errors.first('comment') : ''">
          <b-input type="textarea" v-model="comment" v-validate="'required'"></b-input>
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-button type="is-primary" @click="send">{{ $t("Send") }}</b-button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      comment: ''
    }
  },
  methods: {
    send() {
      this.$validator.validateAll().then((result) => {
        if (result) {
          this.$emit('sendAnotherWorker', this.comment);
        }
      });
    }
  }
}
</script>
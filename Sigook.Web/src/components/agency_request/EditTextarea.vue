<template>
  <div class="p-3">
    <div class="container-flex">
      <div class="col-12 col-padding">
        <b-field :label="title" :type="errors.has(title) ? 'is-danger' : ''"
          :message="errors.has(title) ? errors.first(title) : ''">
          <b-input type="textarea" :name="title" v-model="dataModel"
            v-validate="{
              required: true,
              max: 50000,
              min: minLength
            }"></b-input>
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-button type="is-primary" @click="onSave">Save</b-button>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  props: ['data', 'title', 'minLength'],
  data() {
    return {
      dataModel: this.data
    }
  },
  methods: {
    onSave() {
      this.$validator.validateAll().then((result) => {
        if (result) {
          this.$emit("updateContent", this.dataModel)
        }
      });
    }
  }
}
</script>
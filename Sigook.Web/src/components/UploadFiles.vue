<template>
  <div class="inline">
    <b-loading v-model="isLoading"></b-loading>
    <input type="file" class="input-70 input-border" @change="validateFile($event)"
      accept=".pdf,.doc,.docx,.xls,.xlsx,image/*" title="logo" :name="'file' + name + format" v-validate="{
        required: required,
        size: '15500',
        ext: ['pdf', 'jpeg', 'jpg', 'png', 'gif', 'doc', 'docx', 'xls', 'xlsx'],
      }" :class="{ 'is-danger': errors.has('file' + name + format) }" :disabled="disabled"
      :ref="'file' + name + format" :id="id" />
    <span v-show="errors.has('file' + name + format)" class="help is-danger no-margin">
      {{ errors.first("file" + name + format) }}
    </span>
  </div>
</template>
<script>
import updateMixin from "@/mixins/uploadFiles";

export default {
  props: ["name", "format", "required", "id", "disabled"],
  data() {
    return {
      pathFile: null,
      isLoading: false,
      selected: false,
    };
  },
  mixins: [updateMixin],
  methods: {
    onSelectFile(evt) {
      this.isLoading = true;
      this.$emit("onUpload", true);
      this.uploadFile(evt, this.format, this.name)
        .then((response) => {
          this.pathFile = response;
          this.cleanInput();
          this.$emit("finishUpload", true);
          this.$emit("fileSelected", this.pathFile);
          this.isLoading = false;
        })
        .catch((e) => {
          this.showAlertError(e);
          this.cleanInput();
          this.$emit("finishUpload", true);
          this.isLoading = false;
        });
    },
    cleanInput() {
      const input = this.$refs["file" + this.name + this.format];
      input.type = "text";
      input.type = "file";
    },
    validateFile(evt) {
      this.selected = true;
      this.$validator.validate("file" + this.name + this.format)
        .then((isValid) => {
          if (isValid) {
            this.onSelectFile(evt);
          }
        })
    },
  },
};
</script>

<style lang="scss" scoped>
.inline {
  display: inline-block;
  width: calc(100% - 32px);

  input {
    width: 100%;
  }
}
</style>

<template>
  <div class="inline">
    <b-loading v-model="isLoading"></b-loading>
    <input type="file" class="input-70 input-border" @change="validateFile($event)"
      accept=".pdf,.doc,.docx,.xls,.xlsx,image/*" title="logo" :name="'file' + name + format"
      :class="{ 'is-danger': errorMessage }" :disabled="disabled"
      :ref="'file' + name + format" :id="id" />
    <span v-show="errorMessage" class="help is-danger no-margin">
      {{ errorMessage }}
    </span>
  </div>
</template>
<script lang="ts">
import { showAlertError } from "@/utils/toast";
import { uploadFile } from "@/utils/fileUpload";

const ALLOWED_EXTENSIONS = ['pdf', 'jpeg', 'jpg', 'png', 'gif', 'doc', 'docx', 'xls', 'xlsx'];
const MAX_SIZE_KB = 15500;

export default {
  props: ["name", "format", "required", "id", "disabled"],
  emits: ["onUpload", "finishUpload", "fileSelected"],
  data() {
    return {
      pathFile: null as string | null,
      isLoading: false,
      selected: false,
      errorMessage: "" as string,
    };
  },
  methods: {
    onSelectFile(evt: Event) {
      this.isLoading = true;
      this.$emit("onUpload", true);
      uploadFile(evt, this.format, this.name)
        .then((response: string) => {
          this.pathFile = response;
          this.cleanInput();
          this.$emit("finishUpload", true);
          this.$emit("fileSelected", this.pathFile);
          this.isLoading = false;
        })
        .catch((e: any) => {
          showAlertError(e);
          this.cleanInput();
          this.$emit("finishUpload", true);
          this.isLoading = false;
        });
    },
    cleanInput() {
      const input = (this.$refs["file" + this.name + this.format] as HTMLInputElement);
      input.type = "text";
      input.type = "file";
    },
    validateFile(evt: Event) {
      this.selected = true;
      this.errorMessage = "";
      const target = evt.target as HTMLInputElement;
      const file = target.files && target.files[0];

      if (!file) {
        if (this.required) this.errorMessage = "File is required";
        return;
      }

      const ext = file.name.split('.').pop()?.toLowerCase() || '';
      if (!ALLOWED_EXTENSIONS.includes(ext)) {
        this.errorMessage = `File extension not allowed. Allowed: ${ALLOWED_EXTENSIONS.join(', ')}`;
        return;
      }

      const sizeKb = file.size / 1024;
      if (sizeKb > MAX_SIZE_KB) {
        this.errorMessage = `File size must be less than ${MAX_SIZE_KB} KB`;
        return;
      }

      this.onSelectFile(evt);
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

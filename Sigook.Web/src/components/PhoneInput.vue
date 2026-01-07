<template>
  <b-field :type="errors.has(model) ? 'is-danger' : ''"
    :message="errors.has(model) ? errors.first(model) : ''">
    <template #label>
      {{ model }} <span v-if="required" class="has-text-danger">*</span>
    </template>
    <b-input type="tel" title="phone" :name="model" v-validate="{ required: required, phoneCustom: '' }"
      @keyup="focusOut" @blur="focusOut" :disabled="disabled" :placeholder="placeholder" v-model="formattedPhoneValue"
      v-cleave="mask" />
  </b-field>
</template>
<script>
import phoneFormat from "@/mixins/phoneFormatMixin";
import phoneMaskMixin from "@/mixins/phoneMaskMixin"

export default {
  props: ["model", "defaultValue", "disabled", "required", "placeholder"],
  data() {
    let self = this;
    return {
      phoneValue: 0,
      preventNextIteration: false,
      formattedPhoneValue: self.defaultValue || ""
    };
  },
  methods: {
    focusOut: function (event) {
      this.formattedPhoneValue = event.target.value;
      if (!this.formattedPhoneValue) {
        this.$emit("formattedPhone", null);
      } else {
        this.$emit("formattedPhone", this.formattedPhoneValue);
      }
    },
    async validatePhone() {
      return await this.$validator.validateAll();
    },
  },
  mixins: [phoneFormat, phoneMaskMixin]
};
</script>
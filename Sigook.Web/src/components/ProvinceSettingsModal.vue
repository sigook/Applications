<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>

    <h3 class="title is-4 mb-4">Province Settings - {{ provinceName }}</h3>

    <div class="container-flex">
      <div class="col-12 col-padding">
        <b-field>
          <b-checkbox v-model="form.paidHolidays">
            Paid Holidays
          </b-checkbox>
        </b-field>
      </div>

      <div class="col-12 col-padding">
        <b-field label="Overtime Starts After" :type="errors.has('overtimeStartsAfter') ? 'is-danger' : ''"
          :message="errors.first('overtimeStartsAfter')">
          <b-input v-model="form.overtimeStartsAfter" v-validate="'decimal|min_value:0'"
            name="overtimeStartsAfter" type="number" step="1"></b-input>
        </b-field>
      </div>

      <div class="col-12 col-padding">
        <b-button type="is-primary" @click="saveSettings">
          Save
        </b-button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'ProvinceSettingsModal',
  props: ["provinceId", "provinceName", "currentSettings"],
  data() {
    return {
      isLoading: false,
      form: {
        paidHolidays: false,
        overtimeStartsAfter: null
      }
    };
  },
  created() {
    this.initializeForm();
  },
  methods: {
    initializeForm() {
      if (this.currentSettings) {
        this.form = Object.assign({}, this.currentSettings);
      }
    },
    async saveSettings() {
      const isValid = await this.$validator.validateAll();
      if (!isValid) {
        this.showAlertError('Please fill in all required fields correctly');
        return;
      }
      const settings = {
        paidHolidays: this.form.paidHolidays,
        overtimeStartsAfter: parseFloat(this.form.overtimeStartsAfter)
      };
      this.$emit('saved', settings);
    }
  }
};
</script>

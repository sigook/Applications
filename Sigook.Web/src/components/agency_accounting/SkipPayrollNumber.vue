<template>
  <div class="p-3">
    <div class="container-flex">
      <div class="col-12 col-padding">
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

<script>
export default {
  data() {
    return {
      isLoading: false,
      numbers: [],
      selectedNumber: null
    }
  },
  methods: {
    onInputEntered(text) {
      this.getAllNumbers(text);
    },
    getAllNumbers(text) {
      this.isLoading = true;
      this.$store.dispatch("agency/getSkipPayrollNumbers", { searchTerm: text })
        .then(response => {
          this.isLoading = false;
          this.numbers = response;
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    },
    onSelectFooter() {
      this.$buefy.dialog.prompt({
        message: "Number",
        inputAttrs: {
          type: "text",
          placeholder: "Number",
          value: this.selectedNumber
        },
        closeOnConfirm: false,
        confirmText: 'Add',
        onConfirm: async (value, dialog) => {
          await this.$store.dispatch("agency/addSkipPayrollNumber", { value: value });
          this.$refs.autoCompleteNumbers.setSelected(value);
          dialog.close();
        }
      });
    }
  }
}
</script>
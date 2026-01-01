<template>
  <div class="container-flex">
    <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
      <b-field :type="errors.has('name') ? 'is-danger' : ''" label="Name"
        :message="errors.has('name') ? errors.first('name') : ''">
        <b-input type="text" v-model="user.name" name="name" v-validate="'required|max:50|min:2'" />
      </b-field>
    </div>
    <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
      <b-field :type="errors.has('lastname') ? 'is-danger' : ''" label="Last Name"
        :message="errors.has('lastname') ? errors.first('lastname') : ''">
        <b-input type="text" v-model="user.lastname" name="lastname" v-validate="'required|max:50|min:2'" />
      </b-field>
    </div>
    <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
      <b-field :type="errors.has('position') ? 'is-danger' : ''" label="Position"
        :message="errors.has('position') ? errors.first('position') : ''">
        <b-input type="text" v-model="user.position" name="position" v-validate="'max:50|min:2'" />
      </b-field>
    </div>
    <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
      <phone-input :required="false" model="Mobile Number" :default-value="user.mobileNumber"
        @formattedPhone="(phone) => user.mobileNumber = phone"></phone-input>
    </div>
    <div class="col-12 mt-5">
      <b-button type="is-primary" @click="update">{{ $t("Save") }}</b-button>
    </div>
  </div>
</template>

<script>

import PhoneInput from "@/components/PhoneInput";

export default {
  name: "CompanyUserUpdate",
  props: ["user"],
  components: { PhoneInput },
  methods: {
    update() {
      this.$validator.validateAll().then(r => {
        if (!r) return;
        this.$store.dispatch("company/updateCompanyUser", { id: this.user.id, user: this.user })
          .then(() => {
            this.showAlertSuccess(this.$t("Updated"));
          })
      })
    }
  }
}
</script>
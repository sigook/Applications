<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>

    <div class="container-flex">
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-field :label="'Title'" :type="errors.has('title') ? 'is-danger' : ''"
          :message="errors.has('title') ? errors.first('title') : ''">
          <b-select v-model="contactPerson.title" :name="'title'" v-validate="'required'" expanded
            :placeholder="'Select'">
            <option :value="item" v-for="(item, index) in ['Mr','Mrs','Ms','Miss','Mx','Master','Madam']" :key="'companyContactPersons' + index">
              {{ item }}
            </option>
          </b-select>
        </b-field>
      </div>

      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :label="'Name'" :type="errors.has('name') ? 'is-danger' : ''"
          :message="errors.has('name') ? errors.first('name') : ''">
          <b-input v-model="contactPerson.firstName" :name="'name'" v-validate="'required|max:20|min:2'">
          </b-input>
        </b-field>
      </div>

      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :label="'Middle Name'" :type="errors.has('middlename') ? 'is-danger' : ''"
          :message="errors.has('middlename') ? errors.first('middlename') : ''">
          <b-input v-model="contactPerson.middleName" :name="'middlename'" v-validate="'max:20|min:1'">
          </b-input>
        </b-field>
      </div>

      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :label="'Last Name'" :type="errors.has('lastname') ? 'is-danger' : ''"
          :message="errors.has('lastname') ? errors.first('lastname') : ''">
          <b-input v-model="contactPerson.lastName" :name="'lastname'" v-validate="'required|max:20|min:2'">
          </b-input>
        </b-field>
      </div>

      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :label="'Position'" :type="errors.has('position') ? 'is-danger' : ''"
          :message="errors.has('position') ? errors.first('position') : ''">
          <b-input v-model="contactPerson.position" :name="'position'" v-validate="'required|max:100|min:2'">
          </b-input>
        </b-field>
      </div>

      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <phone-input :required="false" :model="'Office Number'" :defaultValue="contactPerson.officeNumber"
          @formattedPhone="(phone) => contactPerson.officeNumber = phone">
        </phone-input>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :label="'Ext.'" :type="errors.has('officeNumberExt') ? 'is-danger' : ''"
          :message="errors.has('officeNumberExt') ? errors.first('officeNumberExt') : ''">
          <b-input v-model="contactPerson.officeNumberExt" :name="'officeNumberExt'" v-validate="'max:8|min:1|numeric'">
          </b-input>
        </b-field>
      </div>

      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <phone-input :required="false" :model="'Mobile Number'" :defaultValue="contactPerson.mobileNumber"
          @formattedPhone="(phone) => contactPerson.mobileNumber = phone">
        </phone-input>
      </div>

      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :label="'Email'" :type="errors.has('email') ? 'is-danger' : ''"
          :message="errors.has('email') ? errors.first('email') : ''">
          <b-input type="email" v-model="contactPerson.email" :name="'email'"
            v-validate="'required|max:50|email|min:6'">
          </b-input>
        </b-field>
      </div>

      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-button type="is-primary" @click="validateForm">
          {{ currentContact ? 'Save' : 'Create' }}
        </b-button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { createAgencyCompanyContactPerson, updateAgencyCompanyContactPerson } from "@/api/agencyCompanyApi";
export default {
  props: ['currentContact', 'profileId'],
  data() {
    return {
      isLoading: false,
      contactPerson: {
        title: null,
        firstName: null,
        middleName: null,
        lastName: null,
        position: null,
        mobileNumber: null,
        officeNumber: null,
        officeNumberExt: null,
        email: null
      }
    }
  },
  components: {
    phoneInput: () => import("../PhoneInput.vue")
  },
  methods: {
    validateForm() {
      this.submitted = true;
      this.$validator.validateAll().then((result) => {
        if (result) {
          if (this.contactPerson.id) {
            this.updateContactPerson(this.contactPerson.id);
          } else {
            this.createContactPerson();
          }
          return;
        }
        showAlertError('Please make sure all required fields are filled out correctly');
      });
    },
    createContactPerson() {
      this.isLoading = true;
      createAgencyCompanyContactPerson(this.profileId, this.contactPerson)
        .then(() => {
          this.isLoading = false;
          showAlertSuccess('Created')
          this.$emit('updateContent');
        })
        .catch(error => {
          this.isLoading = false;
          showAlertError(error)
        })
    },
    updateContactPerson(id) {
      this.isLoading = true;
      updateAgencyCompanyContactPerson(this.profileId, id, this.contactPerson)
        .then(() => {
          this.isLoading = false;
          showAlertSuccess('Updated')
          this.$emit('updateContent');
        })
        .catch(error => {
          this.isLoading = false;
          showAlertError(error)
        })
    }
  },
  created() {
    if (this.currentContact && this.currentContact.id) this.contactPerson = Object.assign({}, this.currentContact);
  }
}
</script>
<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <b-field grouped position="is-right">
      <b-button type="is-ghost" icon-right="plus-circle" @click="showModal = true">Add</b-button>
    </b-field>
    <b-table :data="contactPeople" narrowed hoverable :mobile-cards="false" paginated
      pagination-rounded>
      <template v-slot:empty>
        <p class="container text-center">No records available</p>
      </template>
      <template>
        <b-table-column field="firstName" label="Full Name" v-slot="props">
          {{ props.row.firstName }} {{ props.row.middleName }} {{ props.row.lastName }}
        </b-table-column>
        <b-table-column field="position" label="Position" v-slot="props">
          {{ props.row.position }}
        </b-table-column>
        <b-table-column field="officeNumber" label="Phone Number" v-slot="props">
          <p>{{ props.row.mobileNumber }}</p>
          <p>
            <span>{{ props.row.officeNumber }}</span>
            <span v-if="props.row.officeNumberExt">Ext. {{ props.row.officeNumberExt }}</span>
          </p>
        </b-table-column>
        <b-table-column field="email" label="Email" v-slot="props">
          <p>{{ props.row.email }}</p>
        </b-table-column>
        <b-table-column field="actions" v-slot="props">
          <b-button type="is-danger" outlined rounded icon-right="delete"
            @click="removeLine(props.row.id)" />
        </b-table-column>
      </template>
    </b-table>
    <b-modal v-model="showModal">
      <div class="p-3">
        <div class="container-flex">
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Title" :type="errors.has('title') ? 'is-danger' : ''"
              :message="errors.has('title') ? errors.first('title') : ''">
              <b-select v-model="contact.title" v-validate="'required'" name="title" expanded>
                <option v-for="(item, index) in $t('TitleList')" :key="'title' + index" :value="item">
                  {{ item }}
                </option>
              </b-select>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="First Name" :type="errors.has('first name') ? 'is-danger' : ''"
              :message="errors.has('first name') ? errors.first('first name') : ''">
              <b-input v-model="contact.firstName" v-validate="'required|max:20|min:2'" name="first name" />
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Middle Name" :type="errors.has('middle name') ? 'is-danger' : ''"
              :message="errors.has('middle name') ? errors.first('middle name') : ''">
              <b-input v-model="contact.middleName" v-validate="'max:20|min:1'" name="middle name" />
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Last Name" :type="errors.has('last name') ? 'is-danger' : ''"
              :message="errors.has('last name') ? errors.first('last name') : ''">
              <b-input v-model="contact.lastName" v-validate="'required|max:20|min:2'" name="last name" />
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Email" :type="errors.has('email') ? 'is-danger' : ''"
              :message="errors.has('email') ? errors.first('email') : ''">
              <b-input v-model="contact.email" v-validate="'required|email'" name="email" />
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Position" :type="errors.has('position') ? 'is-danger' : ''"
              :message="errors.has('position') ? errors.first('position') : ''">
              <b-input v-model="contact.position" v-validate="'required|max:100|min:3'" name="position" />
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-4 col-padding">
            <phone-input ref="mobileComponent" :required="true" :defaultValue="contact.mobileNumber"
              model="Mobile Number" @formattedPhone="(phone) => contact.mobileNumber = phone" />
          </div>
          <div class="col-sm-12 col-md-6 col-lg-4 col-padding">
            <phone-input ref="officeComponent" :required="false" :defaultValue="contact.officeNumber"
              model="Office Number" @formattedPhone="(phone) => contact.officeNumber = phone" />
          </div>
          <div class="col-sm-12 col-md-6 col-lg-4 col-padding">
            <b-field label="Extension" :type="errors.has('officeNumberExt') ? 'is-danger' : ''"
              :message="errors.has('officeNumberExt') ? errors.first('officeNumberExt') : ''">
              <b-input v-model="contact.officeNumberExt" v-validate="'max:8|min:1|numeric'" name="officeNumberExt" />
            </b-field>
          </div>
        </div>
        <div class="col-12 col-padding">
          <b-button type="is-primary" @click="validateForm">SAVE</b-button>
        </div>
      </div>
    </b-modal>
  </div>
</template>

<script>

export default {
  props: ['companyData', 'isDisabled'],
  data() {
    return {
      isLoading: false,
      showModal: false,
      contactPeople: [],
      contact: {}
    }
  },
  components: {
    phoneInput: () => import("../../components/PhoneInput")
  },
  methods: {
    async getContactPersons() {
      this.contactPeople = await this.$store.dispatch('company/contactPeople');
    },
    async removeLine(id) {
      await this.$store.dispatch('company/deleteContactPerson', id);
      await this.getContactPersons();
    },
    async validateForm() {
      const phoneValid = await this.$refs.mobileComponent.validatePhone();
      const officeValid = await this.$refs.officeComponent.validatePhone();
      const mainFormValid = await this.$validator.validateAll();
      if (mainFormValid && phoneValid && officeValid) {
        this.isLoading = true;
        this.$store.dispatch('company/saveContactPerson', this.contact)
          .then(async (response) => {
            this.isLoading = false;
            await this.getContactPersons();
            this.showAlertSuccess('Profile updated');
          })
          .catch((error) => {
            this.isLoading = false;
            this.showAlertError(error.data);
          });
      }
    }
  },
  async created() {
    await this.getContactPersons();
  }
}
</script>
<template>
  <div class="p-3 white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <form @submit.prevent="submit">
      <b-steps animated mobile-mode="compact">
        <b-step-item step="1" label="Basic">
          <div class="container-flex">
            <div class="col-12">
              <b-field label="Full Name" :type="errors.name ? 'is-danger' : ''" :message="errors.name">
                <b-input type="text" v-model="name" />
              </b-field>
            </div>
            <div class="col-12">
              <b-field label="Email" :type="errors.email ? 'is-danger' : ''" :message="errors.email">
                <b-input type="email" v-model="email" />
              </b-field>
            </div>
            <div class="col-12">
              <phone-input model="Phone" :required="true" :defaultValue="phoneNumber"
                @formattedPhone="(phone) => phoneNumber = phone"></phone-input>
            </div>
            <div class="col-12">
              <b-field label="Address" :type="errors.address ? 'is-danger' : ''" :message="errors.address">
                <b-input type="text" v-model="address" />
              </b-field>
            </div>
            <div class="col-12 mt-5">
              <b-field class="file is-primary" :class="{ 'has-name': !!file }">
                <b-upload v-model="file" class="file-label" accept=".pdf,.doc,.docx" @update:modelValue="uploadResume" rounded>
                  <span class="file-cta">
                    <b-icon class="file-icon" icon="upload"></b-icon>
                    <span class="file-label">{{ file ? file.name : "Click to upload" }}</span>
                  </span>
                </b-upload>
              </b-field>
            </div>
          </div>
        </b-step-item>
        <b-step-item step="2" label="Additionals">
          <div class="col-12">
            <b-field label="Skills (Press enter to add)">
              <b-taginput v-model="candidate.skills" :maxlength="20" open-on-focus icon="label" placeholder="Add Skills" allow-new>
              </b-taginput>
            </b-field>
          </div>
          <div class="col-12">
            <b-field label="Status">
              <b-select v-model="candidate.residencyStatus" expanded placeholder="Select a residency status">
                <option v-for="(item, index) in residencyList" :key="'residency' + index" :value="item">{{ item }}
                </option>
              </b-select>
            </b-field>
          </div>
          <div class="col-12">
            <b-field label="Source">
              <b-select v-model="candidate.source" expanded placeholder="Select a source">
                <option v-for="(item, index) in sourceList" :value="item" :key="'sourceItem' + index">{{ item }}
                </option>
              </b-select>
            </b-field>
          </div>
          <div class="col-12">
            <b-field label="Gender">
              <b-select v-model="candidate.gender" expanded placeholder="Select a gender">
                <option v-for="item in genders" v-bind:key="item.id" :value="item">{{ item.value }}</option>
              </b-select>
            </b-field>
          </div>
          <div class="col-12">
            <b-field label="Has Vehicle">
              <b-switch v-model="candidate.hasVehicle" :true-value="true" :false-value="false">
                {{ candidate.hasVehicle ? "Yes" : "No" }}
              </b-switch>
            </b-field>
          </div>
          <div class="col-12 mt-5">
            <b-button type="is-primary" native-type="submit">Create</b-button>
          </div>
        </b-step-item>
      </b-steps>
    </form>
  </div>
</template>

<script lang="ts">
import { defineAsyncComponent } from 'vue';
import { useForm } from 'vee-validate';
import * as yup from 'yup';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { uploadFile } from "@/utils/fileUpload";
import { getGenders } from "@/api/catalogApi";
import { residencyList, sourceList } from "@/constants/catalog";
import { createAgencyCandidate } from "@/api/agencyCandidateApi";
import { phoneSchema } from "@/utils/validation";

export default {
  components: {
    phoneInput: defineAsyncComponent(() => import("@/components/PhoneInput.vue")),
  },
  setup() {
    const schema = yup.object({
      name: yup.string().required('Full name is required').min(2).max(60),
      email: yup.string().required('Email is required').email('Invalid email').min(6).max(50),
      address: yup.string().required('Address is required').min(2).max(100),
      Phone: phoneSchema(true),
    });

    const { handleSubmit, errors, defineField } = useForm({
      validationSchema: schema,
    });

    const [name] = defineField('name');
    const [email] = defineField('email');
    const [address] = defineField('address');

    return { name, email, address, errors, handleSubmit };
  },
  data() {
    return {
      isLoading: false,
      genders: [] as Array<{ id: number; value: string }>,
      phoneNumber: "",
      candidate: {
        phoneNumbers: [] as Array<{ phoneNumber: string }>,
        skills: [] as string[],
        source: null as string | null,
        gender: null as { id: number; value: string } | null,
        hasVehicle: false,
        residencyStatus: null as string | null,
        fileName: null as string | null,
      },
      file: null as File | null,
    };
  },
  methods: {
    submit() {
      (this as any).handleSubmit((values: any) => this.submitCandidate(values))();
    },
    submitCandidate(values: any) {
      this.isLoading = true;
      const payload: any = {
        name: values.name,
        email: values.email,
        address: values.address,
        skills: this.candidate.skills.map((s: string) => ({ skill: s })),
        phoneNumbers: this.phoneNumber ? [{ phoneNumber: this.phoneNumber }] : [],
        source: this.candidate.source,
        gender: this.candidate.gender,
        hasVehicle: this.candidate.hasVehicle,
        residencyStatus: this.candidate.residencyStatus,
        fileName: this.candidate.fileName,
      };
      createAgencyCandidate(payload)
        .then(() => {
          this.isLoading = false;
          showAlertSuccess("Created");
          this.$emit('onClose', true);
        })
        .catch((error: any) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    async uploadResume(file: File) {
      this.isLoading = true;
      const response = await uploadFile(file, 'document', 'Resume_');
      this.candidate.fileName = response;
      this.isLoading = false;
    },
  },
  async created() {
    this.genders = await getGenders();
  },
  computed: {
    residencyList() {
      return residencyList;
    },
    sourceList() {
      return sourceList;
    },
  },
};
</script>

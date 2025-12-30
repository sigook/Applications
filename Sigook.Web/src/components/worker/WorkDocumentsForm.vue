<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-6">
        <b-field :label="$t('WorkerIdentificationType')" 
          :type="errors.has('identificationType1') ? 'is-danger' : ''"
          :message="errors.has('identificationType1') ? errors.first('identificationType1') : ''">
          <b-select v-model="worker.identificationType1" name="identificationType1" v-validate="'required'" expanded>
            <option value="" disabled>{{ $t("Select") }}</option>
            <option v-for="(type, index) in identificationTypes" :value="type"
              :disabled="type === worker.identificationType2" v-bind:key="'identificationType1' + index">
              {{ type.value }}
            </option>
          </b-select>
        </b-field>
      </div>
      <div class="col-6">
        <b-field :label="$t('IdentificationNumber')" 
          :type="errors.has('identificationNumber1') ? 'is-danger' : ''"
          :message="errors.has('identificationNumber1') ? errors.first('identificationNumber1') : ''">
          <b-input type="text" v-model="worker.identificationNumber1" name="identificationNumber1" 
            v-validate="'max:15|min:5'" expanded>
          </b-input>
        </b-field>
      </div>
      <div class="col-12">
        <b-field :label="$t('WorkerIdentificationFile')">
          <div class="input-file-edited input-block"
            v-if="worker.identificationType1File && worker.identificationType1File.fileName">
            <span>
              {{ worker.identificationType1File.fileName | filename }}
            </span>
            <button v-if="worker.identificationType1File"
              @click="deleteWorkerDocumentsType1(worker.identificationType1File)" class="button cross-button"
              type="button" />
          </div>
          <upload-file v-else class="input-block inline-100"
            @fileSelected="(file) => (worker.identificationType1File = { fileName: file })" :format="'document'"
            :name="'Identification_'" :required="false" @onUpload="() => subscribe('file')"
            @finishUpload="() => unsubscribe()" />
        </b-field>
      </div>
      <div class="col-6">
        <b-field :label="$t('WorkerIdentificationType')" 
          :type="errors.has('identificationType2') ? 'is-danger' : ''"
          :message="errors.has('identificationType2') ? errors.first('identificationType2') : ''">
          <b-select v-model="worker.identificationType2" name="identificationType2" v-validate="'required'" expanded>
            <option value="" disabled>{{ $t("Select") }}</option>
            <option v-for="(type, index) in identificationTypes" :value="type"
              :disabled="type === worker.identificationType1" v-bind:key="'identificationType2' + index">
              {{ type.value }}
            </option>
          </b-select>
        </b-field>
      </div>
      <div class="col-6">
        <b-field :label="$t('IdentificationNumber')" 
          :type="errors.has('identificationNumber2') ? 'is-danger' : ''"
          :message="errors.has('identificationNumber2') ? errors.first('identificationNumber2') : ''">
          <b-input type="text" v-model="worker.identificationNumber2" name="identificationNumber2" 
            v-validate="'max:15|min:5'" expanded>
          </b-input>
        </b-field>
      </div>
      <div class="col-12">
        <b-field :label="$t('WorkerIdentificationFile')">
          <div class="input-file-edited input-block"
            v-if="worker.identificationType2File && worker.identificationType2File.fileName">
            <span>
              {{ worker.identificationType2File.fileName | filename }}
            </span>
            <button v-if="worker.identificationType2File"
              @click="deleteWorkerDocumentsType2(worker.identificationType2File)" class="button cross-button"
              type="button" />
          </div>
          <upload-file v-else :required="false" class="input-block inline-100" 
            @fileSelected="(file) => (worker.identificationType2File = { fileName: file })"
            :format="'document'" :name="'Identification_'" @onUpload="() => subscribe('file')"
            @finishUpload="() => unsubscribe()" />
        </b-field>
      </div>
      <div class="col-12">
        <b-field :label="$t('HasPoliceCheckBackground')">
          <b-switch v-model="worker.havePoliceCheckBackground" :true-value="true" :false-value="false">
            {{ worker.havePoliceCheckBackground ? $t("Yes") : $t("No") }}
          </b-switch>
        </b-field>
      </div>
      <div class="col-12" v-if="worker.havePoliceCheckBackground">
        <b-field :label="$t('WorkerPoliceCheckBackground')">
          <div class="input-file-edited input-block"
            v-if="worker.policeCheckBackGround && worker.policeCheckBackGround.fileName">
            <span>
              {{ worker.policeCheckBackGround.fileName | filename }}
            </span>
            <button v-if="worker.policeCheckBackGround" @click="worker.policeCheckBackGround = null"
              class="button cross-button" type="button" />
          </div>
          <upload-file v-else class="input-block inline-100"
            @fileSelected="(file) => (worker.policeCheckBackGround = { fileName: file })" :format="'document'"
            :name="'PoliceCheck_'" :required="false" @onUpload="() => subscribe('file')"
            @finishUpload="() => unsubscribe()" />
        </b-field>
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="validateAll()" :disabled="isLoadingFiles">
          {{ $t("Save") }}
        </b-button>
      </div>
    </div>
  </div>
</template>

<script>
import pubSub from "../../mixins/pubSub";
import updateMixin from "../../mixins/uploadFiles";
export default {
  props: ["data"],
  data() {
    return {
      isLoading: false,
      worker: {},
      identificationTypes: [],
    };
  },
  mixins: [pubSub, updateMixin],
  async created() {
    this.identificationTypes = await this.$store.dispatch("getIdentificationTypes");
    if (this.data != null) {
      this.worker = { ...this.data };
    }
  },
  components: {
    UploadFile: () => import("@/components/UploadFiles"),
  },
  methods: {
    validateAll() {
      this.$validator.validateAll().then((isValid) => {
        if (isValid) {
          this.createWorkerDocuments();
          return;
        }
        this.showAlertError(this.$t("PleaseVerifyThatTheFieldsAreCorrect"));
      });
    },
    createWorkerDocuments() {
      this.isLoading = true;
      this.$store.dispatch("worker/createWorkerDocuments", { profileId: this.worker.id, model: this.worker, })
        .then(() => {
          this.isLoading = false;
          this.$emit("closeModal", true);
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    deleteWorkerDocumentsType1(document) {
      this.deleteFile(document.fileName)
        .then(() => this.worker.identificationType1File = null);
    },
    deleteWorkerDocumentsType2(document) {
      this.deleteFile(document.fileName)
        .then(() => this.worker.identificationType2File = null);
    },
  }
}
</script>

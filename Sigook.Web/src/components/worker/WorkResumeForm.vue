<template>
    <div>
        <b-loading v-model="isLoading"></b-loading>
        <div class="form-section form-100">
            <div class="form-100m20 sm-form-100 form-100m15">
                <div class="fz-1 fw-400">{{ $t('Resume') }}
                    <span class="sign-required"></span>
                    <div class="input-file-edited  input-block" v-if="resume && resume.fileName">
                        <span>
                            Resume-File
                        </span>
                        <button v-if="resume" @click="deleteWorkerResume()" class="button cross-button" type="button" />
                    </div>
                    <upload-file v-else class="input-block inline-100" @fileSelected="file => resume = { fileName: file }"
                        :format="'document'" :name="'resume'" :required="false" @onUpload="() => subscribe('file')"
                        @finishUpload="() => unsubscribe()" />
                </div>
            </div>
            <br>
            <button class="background-btn md-btn primary-button btn-radius margin-top-15 uppercase" @click="validateAll()"
                type="button" :disabled="isLoadingFiles">{{ $t("Save") }}</button>
        </div>
    </div>
</template>

<script>
import toastMixin from "../../mixins/toastMixin";
import pubSub from '../../mixins/pubSub';
import updateMixin from "../../mixins/uploadFiles";
export default {
    props: ['data'],
    data() {
        return {
            isLoading: false,
            resume: {}
        }
    },
    mixins: [pubSub, toastMixin, updateMixin],
    created() {
        if (this.data != null) {
            this.resume = Object.assign({}, this.data.resume);
        }
    },
    components: {
        UploadFile: () => import("../../components/UploadFiles")
    },
    methods: {
        validateAll() {
            this.$validator.validateAll().then((isValid) => {
                if (isValid) {
                    this.createWorkerResume();
                    return;
                }
                this.showAlertError(this.$t('PleaseVerifyThatTheFieldsAreCorrect'));
            });
        },
        createWorkerResume() {
            this.isLoading = true;
            this.$store.dispatch('worker/createWorkerResume', { profileId: this.data.id, model: this.resume })
                .then(() => {
                    this.isLoading = false;
                    this.$emit('closeModal', true);
                })
                .catch(error => {
                    this.isLoading = false;
                    this.showAlertError(error);
                })
        },
        deleteWorkerResume() {
            this.deleteFile(this.resume.fileName)
                .then(() => this.resume = null);
        }
    }
}
</script>
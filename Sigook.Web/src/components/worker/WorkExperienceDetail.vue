<template>
    <div>
        <div class="button-right">
            <h4>{{item.company}}  | <span class="fw-200"> {{item.supervisor}} </span>
            </h4>
            <div class="actions text-right">
                <b-tooltip label="Edit"
                           type="is-dark"
                           position="is-top">
                    <button class="btn-icon-sm btn-icon-edit"
                            type="button"
                            @click="modalEdit = true">{{ $t("Edit")}}</button>
                </b-tooltip>

                <b-tooltip label="Delete"
                           type="is-dark"
                           position="is-top">
                    <button class="btn-icon-sm btn-icon-delete"
                            type="button"
                            @click="confirmDelete()">{{ $t("Delete")}}</button>
                </b-tooltip>
            </div>
        </div>

        <div>
            <p class="margin-0">
                <span>{{toDateMMYYYY(item.startDate)}} -
                    <span v-if="item.isCurrentJobPosition">{{$t('Present')}}</span>
                    <span v-else>{{toDateMMYYYY(item.endDate)}}</span>
                </span>
            </p>
            <p class="margin-0">{{item.duties}}</p>
        </div>

        <!-- custom modal -->
        <transition name="modal">
            <div v-if="modalEdit" class="vue-modal">
                <div class="modal-mask">
                    <div class="modal-wrapper">
                        <div class="modal-container  modal-light overflow-initial">
                            <span class="fz1 fw-700">Work Experience</span>
                            <button type="button" @click="modalEdit = false" class="cross-icon">{{ $t('Close') }}</button>
                            <work-experience-edit :workerId="workerId" :data="item" @updateExperience="() => updateExperience()"/>
                        </div>
                    </div>
                </div>
            </div>
        </transition>
        <!-- end custom modal -->

    </div>
</template>
<script>
import dayjs from "dayjs";
import toastMixin from "../../mixins/toastMixin";
export default {
    props: ['workerId', 'item'],
    data() {
        return {
            modalEdit: false
        }
    },
    mixins: [toastMixin],
    methods: {
        toDateMMYYYY(date){
            return date ? dayjs(date).format('MM/YYYY') : date;
        },
        confirmDelete(){
            let vm = this;
            this.showAlertConfirm('Are you sure', 'that you want to delete this item?')
            .then(response => {
                if (response){
                    vm.deleteWorkerWorkExperience();
                }
            })
        },
        deleteWorkerWorkExperience(){
            this.$store.dispatch('worker/deleteWorkerWorkExperience', {profileId: this.workerId, id: this.item.id})
            .then(() => {
                this.$emit("getWorker", true)
            })
            .catch(error => {
                this.showAlertError(error);
            })
        },
        updateExperience(){
            this.$emit("getWorker", true);
            this.modalEdit = false;
        }
    },
    components: {
        workExperienceEdit: () => import("../../components/worker/WorkExperienceEdit")
    }
}
</script>
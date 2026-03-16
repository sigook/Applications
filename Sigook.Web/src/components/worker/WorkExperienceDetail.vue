<template>
    <div class="experience-item">
        <div class="button-right">
            <h4>{{item.company}}  | <span class="fw-200"> {{item.supervisor}} </span>
            </h4>
            <div class="actions text-right">
                <b-button type="is-info" outlined rounded icon-right="pencil" class="mr-2"
                    @click="modalEdit = true"></b-button>
                <b-button type="is-danger" outlined rounded icon-right="delete"
                    @click="confirmDelete()"></b-button>
            </div>
        </div>

        <div class="experience-body">
            <p class="margin-0 date-range">
                <span>{{toDateMMYYYY(item.startDate)}} -
                    <span v-if="item.isCurrentJobPosition">{{$t('Present')}}</span>
                    <span v-else>{{toDateMMYYYY(item.endDate)}}</span>
                </span>
            </p>
            <p class="margin-0 duties-text">{{item.duties}}</p>
        </div>

        <b-modal v-model="modalEdit" width="800px">
            <work-experience-form :workerId="workerId" :data="item" @updateExperience="updateExperience" />
        </b-modal>

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
            this.showAlertConfirm('Are you sure', 'that you want to delete this item?')
            .then((response) => {
                if (response){
                    this.deleteWorkerWorkExperience();
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
        workExperienceForm: () => import("./WorkExperienceForm")
    }
}
</script>

<style scoped>
.experience-item {
  display: block !important;
  border: 1px solid #e8e8e8;
  border-left: 4px solid #ff9932;
  border-radius: 6px;
  padding: 14px 16px 12px;
  margin-bottom: 14px;
  background: #fff;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.05);
  transition: box-shadow 0.2s ease, border-left-color 0.2s ease;
}

.experience-item:hover {
  box-shadow: 0 4px 14px rgba(0, 0, 0, 0.1);
  border-left-color: #e07a10;
}

.experience-item h4 {
  font-size: 1rem;
  font-weight: 700;
  margin: 0;
  color: #363636;
}

.experience-item h4 .fw-200 {
  font-size: 0.9rem;
  font-weight: 400;
  color: #777;
}

.experience-body {
  margin-top: 10px;
  padding-top: 10px;
  border-top: 1px solid #f0f0f0;
}

.date-range {
  display: inline-block;
  background: #f5f5f5;
  border-radius: 20px;
  padding: 2px 10px;
  font-size: 0.82rem;
  color: #666;
  margin-bottom: 8px;
  width: auto !important;
}

.duties-text {
  font-size: 0.9rem;
  color: #555;
  line-height: 1.5;
  width: 100% !important;
}
</style>

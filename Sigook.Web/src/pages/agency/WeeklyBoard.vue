<template>
    <div class="body-top-on-pagination white-container-mobile">
        <b-loading v-model="isLoading"></b-loading>
        <h2 v-if="data" class="fz1 pt-3 pb-3 col-8 col-md-6 col-sm-12">
            <ul class="tabs-basic tabs-home d-inline-block">
                <li>
                    <router-link to="/agency-requests">
                        Orders
                    </router-link>
                </li>
                <li class="active">
                    Weekly Board
                    <span class="fw-100 fz-1">({{data.totalItems}})</span>
                </li>
            </ul>
        </h2>
        <div v-if="data" class="scroll-desktop">
            <table class="bordered-cells main-table">
                <col width="7%">
                <col width="10%">
                <col width="19%">
                <col width="19%">
                <col width="5%">
                <col width="15%">
                <col width="12%">
                <col width="10%">
                <col width="6%">
                <col width="3%">
                <col width="3%">
                <td></td>
                <thead>
                    <tr>
                        <td>Order Id</td>
                        <td class="min-120">Start date</td>
                        <td>Client</td>
                        <td>Role</td>
                        <td>Rate</td>
                        <td>Worker</td>
                        <td>Phone Number</td>
                        <td>Recruiter</td>
                        <td></td>
                        <td></td>
                    </tr>
                </thead>
                <tbody v-for="(item, index) in data.items" :key="item.id">
                    <tr v-if="addBreak(index)">
                        <td colspan="10" style="border-left: 1px solid white; border-right: 1px solid white;">
                            <div class="fz-14 fw-700 color-primary pt-3">Week {{item.weekStartWorking | dateMonth}}</div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <router-link :to="'/agency-request/' + item.requestId">
                                {{item.numberId}}
                            </router-link>
                        </td>
                        <td>{{item.startWorking | dateMonth}}</td>
                        <td>
                            <router-link :to="'/agency-companies/company/' + item.companyProfileId">
                                {{item.companyFullName}}
                            </router-link>
                            <span class="fz-2 d-block">
                                {{item.location}}
                                <span v-if="item.entrance"> - {{item.entrance}}</span>
                            </span>
                        </td>
                        <td>
                            <router-link :to="'/agency-request/' + item.requestId">
                                {{item.jobTitle}}
                            </router-link>
                            <agency-shift class="pl-0 fz-2 d-block"
                                          :requestId="item.requestId"
                                          :displayShift="item.displayShift" />
                            <span class="fz-2 d-block">{{item.durationTerm | splitCapital}}</span>
                        </td>
                        <td>{{item.workerRate | currency}}</td>
                        <td>
                            <router-link :to="'/agency-workers/worker/' + item.workerProfileId"
                                         :class="workerColor(item)">
                                {{item.firstName}} {{item.lastName}}
                            </router-link>
                            <div class="pl-0 pt-0 line-height-1">
                                <span class="fz-2" v-if="item.socialInsurance">SIN {{item.socialInsurance}}</span>
                                <span v-if="item.socialInsuranceExpire" class="fz-2"> | {{item.dueDate | dateMonth}}</span>
                            </div>
                            <span class="fz-1 d-block" v-if="item.rejectComments">
                                <span class="orange-dot"></span>
                                {{item.rejectComments}}</span>
                        </td>
                        <td>{{item.mobileNumber}}</td>
                        <td>{{item.displayRecruiters | breakWord}}</td>
                        <td>
                            <div class="capitalize is-inline-block v-middle w-100 text-right">

                                <b-tooltip :label="$t(item.requestStatus)" type="is-dark">
                                    <div class="dot-status" :class="'status-' + item.requestStatus.toLowerCase()"></div>
                                </b-tooltip>

                            </div>
                        </td>
                        <td @mouseleave="hideNotes(index)">
                            <div class="p-0">
                                <div class="relative d-inline-block" @click.stop="showNotesClick(index)">
                                    <button type="button"
                                            class="btn-icon-sm btn-icon-notes margin-0 bg-transparent"
                                            style="position: relative; top: -4px; width: 24px;">
                                        NOTES
                                    </button>
                                    <span v-if="item.notesCount" class="notes-notification">
                                        {{item.notesCount < 10 ? item.notesCount : '+'}}
                                    </span>
                                </div>
                                <div v-if="item.showNotes" class="notes-tooltip">
                                    <modal-notes :request-id="item.requestId"
                                                 :user-id="item.id"
                                                 :on-get="getNotes"
                                                 :on-create="createNote"
                                                 :on-delete="deleteNote"
                                                 :on-update="updateNote"
                                                 @onUpdateNote="(val) => item.notesCount = val.size">
                                    </modal-notes>
                                </div>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
            <pagination :total-pages="data.totalPages"
                        :index-page="data.pageIndex"
                        :size-page="this.size"
                        @changePage="(index) => getAgencyRequestBoard(index)">
            </pagination>
        </div>
    </div>
</template>
<script>
import toastMixin from "@/mixins/toastMixin";
import dayjs from "dayjs";
export default {
    data() {
        return {
            size: 30,
            currentPage: 1,
            data: null,
            isLoading: false,
            momentFormat: 'YYYY-MM-DD',
            getNotes: "agency/getAgencyRequestWorkerNote",
            createNote: "agency/createAgencyRequestWorkerNote",
            deleteNote: "agency/deleteAgencyRequestWorkerNote",
            updateNote: "agency/updateAgencyRequestWorkerNote",
        }
    },
    mixins: [toastMixin],
    components: {
        Pagination: () => import("../../components/Paginator"),
        ModalNotes: () => import("../../components/notes/ModalNotes"),
        AgencyShift: () => import("../../components/agency_request/AgencyShiftDetail")
    },
    methods: {
        getAgencyRequestBoard(index){
            this.isLoading = true;
            this.$store.dispatch('agency/getAgencyRequestBoard', {pagination: {page: index, size: this.size}})
            .then(response => {
                this.data = response;
                this.isLoading = false;
            })
            .catch(error => {
                this.showAlertError(error);
                this.isLoading = false;
            })
        },
        addBreak(index){
            if (index === 0){
                return true;
            } else if (dayjs(this.data.items[index].weekStartWorking).format(this.momentFormat) === dayjs(this.data.items[index-1].weekStartWorking).format(this.momentFormat)){
                return false;
            }
            return true;
        },
        workerColor(worker){
            if (worker.workerRequestStatus === this.$statusReject){
                return 'Rejected'
            } else if (worker.isSubcontractor) {
                return 'Blue'
            }
        },
        showNotesClick(index) {
            if (!this.data.items[index].showNotes) {
                this.data.items[index].mouseOver = true;
            }
            this.$set(this.data.items[index], 'showNotes', true);
        },
        hideNotes(index) {
            if (this.data.items[index].showNotes) {
                this.data.items[index].mouseOver = false;
                this.data.items[index].showNotes = false;
            }
        },
    },
    created() {
        this.getAgencyRequestBoard(this.currentPage);
    }
}
</script>
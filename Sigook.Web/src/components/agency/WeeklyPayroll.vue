<template>
    <div>

        <b-loading v-model="isLoading"></b-loading>

        <div class="scroll-desktop scroll" v-if="dataPayroll && dataPayroll.items.length > 0">
            <table class="table-report-simple">
                <col width="30%">
                <col width="20%">
                <col width="20%">
                <col width="20%">
                <col width="10%">
                <thead>
                <th><div class="custom-switch">
                    <button :class="{'active': byWeekEnding}" @click="getWeeklyPayrollByWeekEnding(currentPage)">
                        {{$t("WeekEnding")}}
                    </button>
                    <button :class="{'active': !byWeekEnding}" @click="getWeeklyPayrollByPaymentDate(currentPage)">
                        Payment Date
                    </button>
                </div>
                </th>
                <th>{{ $t("PayStubs")}}</th>
                <th>{{ $t("TotalPaid")}}</th>
                <th class="text-center"></th>
                </thead>
                <tbody>
                <tr v-for="item in dataPayroll.items" :key="'weeklypayroll' + item.weekEnding">
                    <td>{{item.weekEnding | date}}</td>
                    <td>{{item.numberOfPayStubs}}</td>
                    <td>{{item.totalNet | currency}}</td>
                    <td class="table-options">
                        <b-tooltip label="EXCEL"
                                   type="is-dark"
                                   position="is-bottom">
                            <img src="../../assets/images/excel.png" alt="download" @click="downloadWeeklyPayroll(item)">
                        </b-tooltip>
                    </td>
                </tr>
                </tbody>
            </table>
        </div>
        <div v-else>
            <div class="empty">
                {{ $t("WeDidNotFindAnythingToShowHere")}}
            </div>
        </div>
        <pagination v-if="dataPayroll"
                    :total-pages="dataPayroll.totalPages"
                    :index-page="dataPayroll.pageIndex"
                    :size-page="this.size"
                    @changePage="(index) => getWeeklyPayroll(index)">
        </pagination>
    </div>
</template>

<script>
import toast from '../../mixins/toastMixin'
import download from '../../mixins/downloadFileMixin';
export default {
    data(){
        return {
            currentPage: 1,
            size: 15,
            dataPayroll: null,
            isLoading: false,
            byWeekEnding: true
        }
    },
    methods: {
        getWeeklyPayrollByWeekEnding(index){
            this.byWeekEnding = true;
            this.isLoading = true;
            this.$store.dispatch("agency/getWeeklyPayrollByWeekEnding", {pagination:{ size: this.size, page: index }})
                .then(response => {
                    this.dataPayroll = response;
                    this.isLoading = false;
                })
                .catch(error => {
                    this.isLoading = false;
                    this.showAlertError(error || "Error");
                })
        },
        getWeeklyPayrollByPaymentDate(index){
            this.byWeekEnding = false;
            this.isLoading = true;
            this.$store.dispatch("agency/getWeeklyPayrollByPaymentDate", {pagination:{ size: this.size, page: index }})
                .then(response => {
                    this.dataPayroll = response;
                    this.isLoading = false;
                })
                .catch(error => {
                    this.isLoading = false;
                    this.showAlertError(error || "Error");
                })
        },
        getWeeklyPayroll(index){
            if(this.byWeekEnding){
                this.getWeeklyPayrollByWeekEnding(index)
            } else {
                this.getWeeklyPayrollByPaymentDate(index)
            }
        },
        downloadWeeklyPayroll(item){
            if(this.byWeekEnding){
                this.downloadWeeklyPayrollExcelByWeekEnding(item)
            } else {
                this.downloadWeeklyPayrollExcelByPaymentDate(item)
            }
        },
        downloadWeeklyPayrollExcelByWeekEnding(item){
            this.isLoading = true;
            this.$store.dispatch('downloadWeeklyPayrollExcelByWeekEnding', {date:item.displayWeekEnding})
                .then(response => {
                    this.isLoading = false;
                    this.downloadFile(response, `Week_Ending_${item.weekEnding}_`);
                })
                .catch(error => {
                    this.isLoading = false;
                    this.showAlertError(error || "Error");
                })
        },
        downloadWeeklyPayrollExcelByPaymentDate(item){
            this.isLoading = true;
            this.$store.dispatch('downloadWeeklyPayrollExcelByPaymentDate', {date:item.displayWeekEnding})
                .then(response => {
                    this.isLoading = false;
                    this.downloadFile(response, `Payment_Date_${item.weekEnding}_`);
                })
                .catch(error => {
                    this.isLoading = false;
                    this.showAlertError(error || "Error");
                })
        }
    },
    created(){
        this.getWeeklyPayroll(this.currentPage);
    },
    components: {
        Pagination: () => import("../../components/Paginator")
    },
    mixins: [
        toast,
        download
    ]
}
</script>
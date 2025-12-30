<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <calendar :highlights="data" :workerId="workerId" :requestId="requestId" :startDate="request.startAt"
      :status="request.status" :worker="worker" @onMonthChange="(args) => onMonthChange(args.startDate, args.endDate)">
      <template v-slot:punch-input="slotProps">
        <div v-if="slotProps.item.id !== null" class="mt-1">
          <div class="container-flex">
            <div class="col-12 col-padding">
              <div v-if="slotProps.item.clockIn">
                <b-tag type="is-info is-light">
                  <strong>Clock in:</strong> {{ slotProps.item.clockIn | dateHHmm }}
                  <template v-if="slotProps.item.clockOut">
                    <strong> to </strong>{{ slotProps.item.clockOut | dateHHmm }}
                  </template>
                </b-tag>
              </div>
              <b-tag type="is-success is-light">
                <strong>Hours:</strong> {{ slotProps.item.totalHours | hour }}
              </b-tag>
              <div v-if="slotProps.item.missingHours">
                <b-tag type="is-success is-light">
                  <strong>Missing Hours:</strong> {{ slotProps.item.missingHours | hour }}
                </b-tag>
              </div>
              <div v-if="slotProps.item.totalHoursApproved">
                <b-tag type="is-success is-light">
                  <strong>Hours Approved:</strong> {{ slotProps.item.totalHoursApproved | hour }}
                </b-tag>
              </div>
              <div class="d-flex gap-2 justify-content-center align-items-center">
                <b-tooltip label="Detail" type="is-dark">
                  <b-button type="is-ghost" @click="openDetail(slotProps.item)" icon-right="eye"></b-button>
                </b-tooltip>
                <b-tooltip label="Edit" type="is-dark">
                  <b-button type="is-ghost" icon-right="pencil" @click="editPunchCard(slotProps.item)">
                  </b-button>
                </b-tooltip>
                <b-tooltip label="Approve" type="is-dark" v-if="!slotProps.item.totalHoursApproved">
                  <b-button type="is-ghost" icon-right="check"
                    @click="timeSheetFastApprove(slotProps.item, requestId, workerId)">
                  </b-button>
                </b-tooltip>
                <div class="d-flex" v-if="slotProps.item.id && !slotProps.item.canUpdate">
                  <b-tooltip :triggers="['click']" :auto-close="['outside', 'escape']" type="is-dark" size="is-medium"
                    position="is-top" multilined>
                    <template slot="content">
                      <div>
                        <p v-if="currentTimeSheetUsage.invoiceNumber"><b>Invoice:</b>
                          {{ currentTimeSheetUsage.invoiceNumber }}
                        </p>
                        <p v-if="currentTimeSheetUsage.payStubNumber"><b>PayStub:</b>
                          {{ currentTimeSheetUsage.payStubNumber }}
                        </p>
                        <p v-if="!currentTimeSheetUsage.invoiceNumber && !currentTimeSheetUsage.payStubNumber"> . </p>
                      </div>
                    </template>
                    <b-button type="is-ghost" @click="getTimesheetUsages(slotProps.item)"
                      icon-right="paperclip"></b-button>
                  </b-tooltip>
                </div>
                <b-tooltip label="Delete" type="is-dark" position="is-bottom"
                  v-if="slotProps.item.id && slotProps.item.canUpdate">
                  <b-button icon-right="delete" type="is-ghost"
                    @click="deleteWorkerTimSheet(slotProps.item)"></b-button>
                </b-tooltip>
              </div>
            </div>
          </div>
        </div>
        <div class="mt-2" v-else>
          <div class="container-flex">
            <div class="col-12 col-padding">
              <b-field :type="errors.has('item' + slotProps.indexDay) ? 'is-danger' : ''"
                :message="errors.has('item' + slotProps.indexDay) ? errors.first('item' + slotProps.indexDay) : ''">
                <b-numberinput v-model="slotProps.item.totalHoursApproved" placeholder="Hours"
                  :disabled="!!slotProps.item.id" step="0.01" :name="'item' + slotProps.indexDay"
                  v-validate="{ 'max_value': maximumDailyHours, 'min_value': 0, decimal: 2 }" title="Approved hours"
                  :controls="false">
                </b-numberinput>
                <b-button type="is-ghost" @click="validatePost(slotProps.item)" v-if="!slotProps.item.id"
                  icon-right="check"></b-button>
              </b-field>
            </div>
          </div>
        </div>
      </template>
    </calendar>

    <!-- Modal para punch card -->
    <b-modal v-model="showModalPunchCard">
      <time-sheet-modal v-if="editableDay" :worker="{ workerId: workerId }" :editable-day="editableDay"
        @updateData="updateCell" />
    </b-modal>

    <!-- Modal para detalle -->
    <b-modal v-model="showDetailPunchCard" width="500px">
      <time-sheet-detail v-if="editableDay" :editable-day="editableDay" />
    </b-modal>
  </div>
</template>
<script>
import timeSheetReportMixin from "@/mixins/agencyTimeSheetReportMixin";
import dayjs from "dayjs";
import duration from 'dayjs/plugin/duration';

dayjs.extend(duration);

export default {
  props: ["workerId", "requestId", "request", "worker"],
  data() {
    return {
      startDate: '',
      endDate: '',
      data: [],
      isLoading: false,
      editableDay: false,
      currentTimeSheetUsage: {
        invoiceNumber: null,
        payStubNumber: null
      },
      showModalPunchCard: false,
      showDetailPunchCard: false
    }
  },
  mixins: [timeSheetReportMixin],
  computed: {
    maximumDailyHours() {
      return this.$store.state.catalog.maximumHoursPerDay ? this.$store.state.catalog.maximumHoursPerDay : 12;
    }
  },
  methods: {
    getAgencyWorkerTimeSheetByDate() {
      this.isLoading = true;
      this.$store.dispatch("agency/getAgencyWorkerTimeSheetByDate", { requestId: this.requestId, workerId: this.workerId, date: { startDate: this.startDate, endDate: this.endDate } })
        .then(response => {
          this.isLoading = false;
          this.data = response;
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error)
        })
    },
    onMonthChange(startDate, endDate) {
      this.startDate = startDate;
      this.endDate = endDate;
      this.getAgencyWorkerTimeSheetByDate()
    },
    updateCell() {
      this.getAgencyWorkerTimeSheetByDate();
      this.showModalPunchCard = false;
    },
    validatePost(model) {
      this.$validator.validateAll().then((result) => {
        if (result) {
          this.reportWorkerTimSheet(model);
          return;
        }
        this.showAlertError(this.$t('PleaseVerifyThatTheFieldsAreCorrect'));
      });
    },
    reportWorkerTimSheet(item) {
      let model = {
        hours: this.changeDecimalToHour(item.totalHoursApproved),
        timeIn: this.todayTimeZero(item.day),
        missingHours: item.missingHours,
        missingHoursOvertime: item.missingHoursOvertime,
        missingRateWorker: item.missingRateWorker,
        missingRateAgency: item.missingRateAgency,
        deductionsOthers: item.deductionsOthers
      };
      this.isLoading = true;
      this.$store.dispatch("agency/postAgencyWorkerTimeSheet", { requestId: this.requestId, workerId: this.workerId, model: model })
        .then(() => {
          this.isLoading = false;
          this.getAgencyWorkerTimeSheetByDate();
        }).catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    },
    deleteWorkerTimSheet(item) {
      this.isLoading = true;
      this.$store.dispatch("agency/deleteWorkerTimeSheet", { requestId: this.requestId, workerId: this.workerId, id: item.id })
        .then(() => {
          this.isLoading = false;
          item.id = null;
          item.totalHoursApproved = null;
          item.timeIn = null;
          this.getAgencyWorkerTimeSheetByDate();
        }).catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    },
    changeDecimalToHour(time) {
      return dayjs().startOf('day').add(time, 'hours').format('HH:mm:ss');
    },
    todayTimeZero(time) {
      let date = new Date(time);
      date.setHours(0);
      date.setMinutes(0);
      date.setSeconds(0);
      return date
    },
    getTimesheetUsages(item) {
      this.isLoading = true;
      this.$store.dispatch('agency/getTimesheetUsages', { requestId: this.requestId, workerId: this.workerId, id: item.id })
        .then(response => {
          this.isLoading = false;
          this.currentTimeSheetUsage = {
            invoiceNumber: response.invoiceNumber,
            payStubNumber: response.payStubNumber
          }
        }).catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    },
    openDetail(item) {
      this.editableDay = item;
      this.showDetailPunchCard = true;
    },
    editPunchCard(day) {
      this.editableDay = null;
      /*
      * Update object to show Modal
      * */
      this.editableDay = Object.assign({}, day);
      this.editableDay.timeInApproved = new Date(day.timeInApproved);
      this.editableDay.timeOutApproved = new Date(day.timeOutApproved);
      this.editableDay.missinghoursToDate = this.stringToDate(this.editableDay.missingHours)
      this.editableDay.missingHoursOvertimeToDate = this.stringToDate(this.editableDay.missingHoursOvertime)
      this.editableDay.hoursApprovedToDate = this.startTime;
      this.editableDay.missingRateWorker = day.missingRateWorker;
      this.editableDay.missingRateAgency = day.missingRateAgency
      /*
      * Change hours to format date
      */
      let newTime = new Date();
      let duration = dayjs.duration(day.totalHoursApproved, 'hours');
      newTime.setHours(duration.hours());
      let min = duration.minutes();
      newTime.setMinutes(min);
      let sec = duration.seconds();
      newTime.setSeconds(sec);
      this.editableDay.hoursApprovedToDate = newTime;

      /*
      * Show Modal
      * */
      this.showModalPunchCard = true;
    },
    stringToDate(value) {
      if (value) {
        let tmp = value.split(":");
        let date = new Date();
        date.setHours(parseInt(tmp[0]));
        date.setMinutes(parseInt(tmp[1]));
        date.setSeconds(0);
        return date;
      }
      return this.startTime;
    },
  },
  components: {
    Calendar: () => import("../calendar/CalendarPunchCard"),
    TimeSheetModal: () => import("../../components/agency_request/AgencyRequestTimeSheetModal"),
    TimeSheetDetail: () => import("../../components/agency_request/AgencyRequestTimeSheetDetail"),
  }
}
</script>
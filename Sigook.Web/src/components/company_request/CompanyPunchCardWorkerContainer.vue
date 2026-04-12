<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <calendar :highlights="data" :workerId="workerId" :requestId="requestId" :startDate="request.startAt"
      :status="request.status" :worker="worker" @onMonthChange="(args) => onMonthChange(args.startDate, args.endDate)">
      <template v-slot:punch-input="slotProps">
        <div v-if="slotProps.item.id !== null" class="mt-2">
          <div class="container-flex">
            <div class="col-8 col-padding">
              <div v-if="slotProps.item.clockIn" class="mb-2">
                <b-tag type="is-info is-light">
                  <strong>Clock in:</strong> {{ dateHHmm(slotProps.item.clockIn) }}
                  <template v-if="slotProps.item.clockOut">
                    <strong> to </strong>{{ dateHHmm(slotProps.item.clockOut) }}
                  </template>
                </b-tag>
              </div>
              <div v-if="slotProps.item.totalHours">
                <b-tag type="is-success is-light">
                  <strong>Hours:</strong> {{ hour(slotProps.item.totalHours) }}
                </b-tag>
              </div>
              <div v-if="slotProps.item.totalHoursApproved">
                <b-tag type="is-success is-light">
                  <strong>Hours Approved:</strong> {{ hour(slotProps.item.totalHoursApproved) }}
                </b-tag>
              </div>
              <div class="d-flex gap-2 justify-content-center align-items-center">
                <b-tooltip label="Detail" type="is-dark">
                  <b-button v-if="slotProps.item.id && !slotProps.item.canUpdate" type="is-ghost"
                    @click="openDetail(slotProps.item)" icon-right="eye"></b-button>
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
                <b-tooltip label="Delete" type="is-dark" position="is-bottom"
                  v-if="slotProps.item.id && slotProps.item.canUpdate">
                  <b-button icon-right="delete" type="is-ghost"
                    @click="confirmDelete(slotProps.item)"></b-button>
                </b-tooltip>
              </div>
            </div>
          </div>
        </div>
        <div class="mt-2" v-else>
          <div class="container-flex">
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
          <div v-if="isToday(slotProps.item.day) && (!slotProps.item.totalHoursApproved)" class="mt-2">
            <b-button type="is-primary" @click="showClockIn = true" expanded>
              Clock in
            </b-button>
          </div>
        </div>
      </template>
    </calendar>

    <!-- Modal para punch card -->
    <b-modal v-model="showModalPunchCard">
      <time-sheet-modal v-if="editableDay" :requestId="requestId" :worker="{ workerId: workerId }"
        v-model:editable-day="editableDay" @updateData="updateCell" />
    </b-modal>

    <!-- Modal para detalle -->
    <b-modal v-model="showDetailPunchCard" width="500px">
      <time-sheet-detail v-if="editableDay" :editable-day="editableDay" />
    </b-modal>

    <!-- Clock in modal -->
    <b-modal v-model="showClockIn" width="400px">
      <div class="p-3">
        <b-field label="At what time did the worker clock in?">
          <b-timepicker v-model="clockInTime" hour-format="24" :max-time="maxClockInTime" append-to-body>
          </b-timepicker>
        </b-field>
        <b-button type="is-primary" @click="onClockIn">Save</b-button>
      </div>
    </b-modal>
  </div>
</template>

<script lang="ts">
import { dateHHmm, hour } from '@/utils/filters';
import dayjs from "dayjs";
import duration from 'dayjs/plugin/duration';
import { buildTimeSheetApproveModel } from "@/utils/timeSheetApprove";
import { maximumHoursPerDay } from "@/constants/catalog";
import {
  getCompanyWorkerTimeSheetByDate,
  postCompanyWorkerTimeSheet,
  deleteCompanyWorkerTimeSheet,
  companyTimeSheetClockIn,
  updateCompanyRequestWorkerTimeSheet,
} from '@/api/companyApi';

dayjs.extend(duration);

export default {
  props: ["workerId", "requestId", "request", "worker"],
  data() {
    const emptyTime = dayjs().hour(0).minute(0).second(0).millisecond(0).toDate();
    return {
      startDate: '',
      endDate: '',
      data: [],
      isLoading: false,
      editableDay: null,
      showClockIn: false,
      clockInTime: emptyTime,
      maxClockInTime: new Date(),
      showModalPunchCard: false,
      showDetailPunchCard: false
    }
  },
  computed: {
    maximumDailyHours() {
      return maximumHoursPerDay;
    }
  },
  methods: {
    dateHHmm,
    hour,
    timeSheetFastApprove(item, requestId, workerId) {
      this.isLoading = true;
      const model = buildTimeSheetApproveModel(item);
      updateCompanyRequestWorkerTimeSheet(requestId, workerId, item.id, model)
        .then(() => {
          this.updateCell();
        })
        .catch((error) => {
          this.showAlertError(error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    getAgencyWorkerTimeSheetByDate() {
      this.isLoading = true;
      getCompanyWorkerTimeSheetByDate(this.requestId, this.workerId, { startDate: this.startDate, endDate: this.endDate })
        .then(response => {
          this.isLoading = false;
          this.data = response
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
    isToday(date) {
      return dayjs(date).format('YYYY-MM-DD') === dayjs().format('YYYY-MM-DD')
    },
    onClockIn() {
      if (!this.clockInTime) return;
      this.isLoading = true;
      let model = { "clockIn": dayjs(this.clockInTime).format("HH:mm:ss") }
      companyTimeSheetClockIn(this.requestId, this.workerId, model)
        .then(() => {
          this.isLoading = false;
          this.showClockIn = false;
          this.getAgencyWorkerTimeSheetByDate();
          this.clockInTime = dayjs().hour(0).minute(0).second(0).millisecond(0).toDate();
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error)
        })
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
      this.isLoading = true;
      let model = {
        hours: this.changeDecimalToHour(item.totalHoursApproved),
        timeIn: this.todayTimeZero(item.day),
        missingHours: item.missingHours,
        missingHoursOvertime: item.missingHoursOvertime,
        missingRateWorker: item.missingRateWorker,
        missingRateAgency: item.missingRateAgency,
        deductionsOthers: item.deductionsOthers,
        bonusOrOthers: item.bonusOrOthers,
        deductionsOthersDescription: item.deductionsOthersDescription,
        bonusOrOthersDescription: item.bonusOrOthersDescription,
        comments: item.comments
      };
      postCompanyWorkerTimeSheet(this.requestId, this.workerId, model)
        .then(() => {
          this.isLoading = false;
          this.getAgencyWorkerTimeSheetByDate();
        }).catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    },
    confirmDelete(item) {
      this.showAlertConfirm('Are you sure', 'You want to delete this item?')
        .then(response => {
          if (response) {
            this.deleteCompanyWorkerTimeSheet(item);
          }
        })
    },
    deleteCompanyWorkerTimeSheet(item) {
      this.isLoading = true;
      deleteCompanyWorkerTimeSheet(this.requestId, this.workerId, item.id)
        .then(() => {
          this.isLoading = false;
          item.id = null;
          item.totalHoursApproved = null;
          item.timeIn = null;
          this.getAgencyWorkerTimeSheetByDate();
        })
        .catch(error => {
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
    }
  },
  components: {
    Calendar: () => import("../calendar/CalendarPunchCard.vue"),
    TimeSheetModal: () => import("../../components/company_request/CompanyRequestTimeSheetModal.vue"),
    TimeSheetDetail: () => import("../../components/company_request/CompanyRequestTimeSheetDetail.vue")
  }
}
</script>
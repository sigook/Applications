<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <Calendar :highlights="data" :workerId="workerId" :requestId="requestId" :startDate="request.startAt"
      :status="request.status" :worker="worker" @onMonthChange="(args) => onMonthChange(args.startDate, args.endDate)">
      <template v-slot:punch-input="slotProps">
        <div v-if="slotProps.item.id !== null" class="mt-1">
          <div class="container-flex">
            <div class="col-12 col-padding">
              <div v-if="slotProps.item.clockIn">
                <b-tag type="is-info is-light">
                  <strong>Clock in:</strong> {{ dateHHmm(slotProps.item.clockIn) }}
                  <template v-if="slotProps.item.clockOut">
                    <strong> to </strong>{{ dateHHmm(slotProps.item.clockOut) }}
                  </template>
                </b-tag>
              </div>
              <b-tag type="is-success is-light">
                <strong>Hours:</strong> {{ hour(slotProps.item.totalHours) }}
              </b-tag>
              <div v-if="slotProps.item.missingHours">
                <b-tag type="is-success is-light">
                  <strong>Missing Hours:</strong> {{ hour(slotProps.item.missingHours) }}
                </b-tag>
              </div>
              <div v-if="slotProps.item.totalHoursApproved">
                <b-tag type="is-success is-light">
                  <strong>Hours Approved:</strong> {{ hour(slotProps.item.totalHoursApproved) }}
                </b-tag>
              </div>
              <div class="d-flex gap-2 justify-content-center align-items-center">
                <b-tooltip label="Detail" type="is-dark" append-to-body>
                  <b-button type="is-ghost" @click="openDetail(slotProps.item)" icon-right="eye"></b-button>
                </b-tooltip>
                <b-tooltip label="Edit" type="is-dark" append-to-body>
                  <b-button type="is-ghost" icon-right="pencil" @click="editPunchCard(slotProps.item)">
                  </b-button>
                </b-tooltip>
                <b-tooltip label="Approve" type="is-dark" v-if="!slotProps.item.totalHoursApproved" append-to-body>
                  <b-button type="is-ghost" icon-right="check"
                    @click="timeSheetFastApprove(slotProps.item, requestId, workerId)">
                  </b-button>
                </b-tooltip>
                <div class="d-flex" v-if="slotProps.item.id && !slotProps.item.canUpdate">
                  <b-tooltip :triggers="['click']" :auto-close="['outside', 'escape']" type="is-dark" size="is-medium"
                    position="is-top" multilined append-to-body>
                    <template #content>
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
                    <b-button type="is-ghost" @click="loadTimeSheetUsages(slotProps.item)"
                      icon-right="paperclip"></b-button>
                  </b-tooltip>
                </div>
                <b-tooltip label="Delete" type="is-dark" position="is-bottom"
                  v-if="slotProps.item.id && slotProps.item.canUpdate" append-to-body>
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
              <b-field :type="itemErrors[slotProps.index] ? 'is-danger' : ''"
                :message="itemErrors[slotProps.index] || ''">
                <b-numberinput v-model="slotProps.item.totalHoursApproved" placeholder="Hours"
                  :disabled="!!slotProps.item.id" step="0.01" :name="'item' + slotProps.index"
                  title="Approved hours" :controls="false">
                </b-numberinput>
                <b-button type="is-ghost" @click="validatePost(slotProps.item, slotProps.index)" v-if="!slotProps.item.id"
                  icon-right="check"></b-button>
              </b-field>
            </div>
          </div>
        </div>
      </template>
    </Calendar>

    <!-- Modal para punch card -->
    <b-modal v-model="showModalPunchCard">
      <TimeSheetModal v-if="editableDay" :worker="{ workerId: workerId }" v-model:editable-day="editableDay"
        @updateData="updateCell" />
    </b-modal>

    <!-- Modal para detalle -->
    <b-modal v-model="showDetailPunchCard" width="500px">
      <TimeSheetDetail v-if="editableDay" :editable-day="editableDay" />
    </b-modal>
  </div>
</template>
<script setup lang="ts">
import { ref, reactive, computed } from 'vue';
import { showAlertError } from "@/utils/toast";
import { dateHHmm, hour } from '@/utils/filters';
import { buildTimeSheetApproveModel } from "@/utils/timeSheetApprove";
import { maximumHoursPerDay } from "@/constants/catalog";
import dayjs from "dayjs";
import duration from 'dayjs/plugin/duration';
import {
  getAgencyWorkerTimeSheetByDate,
  postAgencyWorkerTimeSheet,
  deleteAgencyWorkerTimeSheet,
  getAgencyTimeSheetUsages,
  updateAgencyWorkerTimeSheet
} from "@/api/agencyTimeSheetApi";
import Calendar from '../calendar/CalendarPunchCard.vue';
import TimeSheetModal from '../../components/agency_request/AgencyRequestTimeSheetModal.vue';
import TimeSheetDetail from '../../components/agency_request/AgencyRequestTimeSheetDetail.vue';

dayjs.extend(duration);

const props = defineProps<{ workerId: any; requestId: any; request: any; worker: any }>();

const startDate = ref('');
const endDate = ref('');
const data = ref<any[]>([]);
const isLoading = ref(false);
const editableDay = ref<any>(false);
const currentTimeSheetUsage = reactive<{ invoiceNumber: any; payStubNumber: any }>({
  invoiceNumber: null,
  payStubNumber: null
});
const showModalPunchCard = ref(false);
const showDetailPunchCard = ref(false);
const itemErrors = ref<Record<string, string>>({});

const maximumDailyHours = computed(() => maximumHoursPerDay);

function timeSheetFastApprove(item: any, requestId: any, workerId: any) {
  isLoading.value = true;
  const model = buildTimeSheetApproveModel(item);
  updateAgencyWorkerTimeSheet(requestId, workerId, item.id, model)
    .then(() => {
      updateCell();
    })
    .catch((error) => {
      showAlertError(error);
    })
    .finally(() => {
      isLoading.value = false;
    });
}

function loadTimeSheets() {
  isLoading.value = true;
  getAgencyWorkerTimeSheetByDate(props.requestId, props.workerId, { startDate: startDate.value, endDate: endDate.value })
    .then(response => {
      isLoading.value = false;
      data.value = response;
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function onMonthChange(sDate: string, eDate: string) {
  startDate.value = sDate;
  endDate.value = eDate;
  loadTimeSheets();
}

function updateCell() {
  loadTimeSheets();
  showModalPunchCard.value = false;
}

function validatePost(model: any, indexDay: any) {
  const key = String(indexDay);
  const raw = model.totalHoursApproved;
  const value = Number(raw);
  if (raw == null || raw === '' || isNaN(value) || value < 0 || value > maximumDailyHours.value) {
    itemErrors.value = { ...itemErrors.value, [key]: `Hours must be between 0 and ${maximumDailyHours.value}` };
    showAlertError('Please make sure all required fields are filled out correctly');
    return;
  }
  if (!/^\d+(\.\d{1,2})?$/.test(String(raw))) {
    itemErrors.value = { ...itemErrors.value, [key]: 'Max 2 decimal places' };
    showAlertError('Please make sure all required fields are filled out correctly');
    return;
  }
  const next = { ...itemErrors.value };
  delete next[key];
  itemErrors.value = next;
  reportWorkerTimSheet(model);
}

function reportWorkerTimSheet(item: any) {
  const model = {
    hours: changeDecimalToHour(item.totalHoursApproved),
    timeIn: todayTimeZero(item.day),
    missingHours: item.missingHours,
    missingHoursOvertime: item.missingHoursOvertime,
    missingRateWorker: item.missingRateWorker,
    missingRateAgency: item.missingRateAgency,
    deductionsOthers: item.deductionsOthers
  };
  isLoading.value = true;
  postAgencyWorkerTimeSheet(props.requestId, props.workerId, model)
    .then(() => {
      isLoading.value = false;
      loadTimeSheets();
    }).catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function deleteWorkerTimSheet(item: any) {
  isLoading.value = true;
  deleteAgencyWorkerTimeSheet(props.requestId, props.workerId, item.id)
    .then(() => {
      isLoading.value = false;
      item.id = null;
      item.totalHoursApproved = null;
      item.timeIn = null;
      loadTimeSheets();
    }).catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function changeDecimalToHour(time: any) {
  return dayjs().startOf('day').add(time, 'hours').format('HH:mm:ss');
}

function todayTimeZero(time: any) {
  const date = new Date(time);
  date.setHours(0);
  date.setMinutes(0);
  date.setSeconds(0);
  return date;
}

function loadTimeSheetUsages(item: any) {
  isLoading.value = true;
  getAgencyTimeSheetUsages(props.requestId, props.workerId, item.id)
    .then((response) => {
      isLoading.value = false;
      currentTimeSheetUsage.invoiceNumber = response.invoiceNumber;
      currentTimeSheetUsage.payStubNumber = response.payStubNumber;
    }).catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function openDetail(item: any) {
  editableDay.value = item;
  showDetailPunchCard.value = true;
}

function editPunchCard(day: any) {
  editableDay.value = null;
  /*
  * Update object to show Modal
  * */
  editableDay.value = Object.assign({}, day);
  editableDay.value.timeInApproved = new Date(day.timeInApproved);
  editableDay.value.timeOutApproved = new Date(day.timeOutApproved);
  editableDay.value.missinghoursToDate = stringToDate(editableDay.value.missingHours);
  editableDay.value.missingHoursOvertimeToDate = stringToDate(editableDay.value.missingHoursOvertime);
  editableDay.value.hoursApprovedToDate = null;
  editableDay.value.missingRateWorker = day.missingRateWorker;
  editableDay.value.missingRateAgency = day.missingRateAgency;
  /*
  * Change hours to format date
  */
  const newTime = new Date();
  const dur = dayjs.duration(day.totalHoursApproved, 'hours');
  newTime.setHours(dur.hours());
  const min = dur.minutes();
  newTime.setMinutes(min);
  const sec = dur.seconds();
  newTime.setSeconds(sec);
  editableDay.value.hoursApprovedToDate = newTime;

  /*
  * Show Modal
  * */
  showModalPunchCard.value = true;
}

function stringToDate(value: any) {
  if (value) {
    const tmp = value.split(":");
    const date = new Date();
    date.setHours(parseInt(tmp[0]));
    date.setMinutes(parseInt(tmp[1]));
    date.setSeconds(0);
    return date;
  }
  return null;
}

defineExpose({ updateCell });
</script>

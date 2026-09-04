<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <PunchCardAgenda v-if="isTouch" :highlights="data" :start-date="request.startAt" :worker="worker"
      :errors="itemErrors" @month-change="(args: any) => onMonthChange(args.startDate, args.endDate)"
      @view="openDetail" @edit="editPunchCard" @approve="(day: any) => timeSheetFastApprove(day, requestId, workerProfileId)"
      @post-hours="(day: any) => validatePost(day, dayKeyOf(day))" @clock-in="showClockIn = true" />
    <Calendar v-else :highlights="data" :workerProfileId="workerProfileId" :requestId="requestId" :startDate="request.startAt"
      :status="request.status" :worker="worker" @onMonthChange="(args: any) => onMonthChange(args.startDate, args.endDate)">
      <template v-slot:punch-input="slotProps">
        <div v-if="slotProps.item.id !== null" class="mt-2">
          <div class="columns is-multiline">
            <div class="column is-8-mobile is-8">
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
              <div class="is-flex is-gap-1 is-justify-content-center is-align-items-center">
                <b-tooltip label="Detail" type="is-dark" append-to-body>
                  <b-button v-if="slotProps.item.id && !slotProps.item.canUpdate" type="is-ghost"
                    @click="openDetail(slotProps.item)" icon-right="eye"></b-button>
                </b-tooltip>
                <b-tooltip label="Edit" type="is-dark" append-to-body>
                  <b-button type="is-ghost" icon-right="pencil" @click="editPunchCard(slotProps.item)">
                  </b-button>
                </b-tooltip>
                <b-tooltip label="Approve" type="is-dark" v-if="!slotProps.item.totalHoursApproved" append-to-body>
                  <b-button type="is-ghost" icon-right="check"
                    @click="timeSheetFastApprove(slotProps.item, requestId, workerProfileId)">
                  </b-button>
                </b-tooltip>
                <b-tooltip label="Delete" type="is-dark" position="is-bottom"
                  v-if="slotProps.item.id && slotProps.item.canUpdate" append-to-body>
                  <b-button icon-right="delete" type="is-ghost"
                    @click="confirmDelete(slotProps.item)"></b-button>
                </b-tooltip>
              </div>
            </div>
          </div>
        </div>
        <div class="mt-2" v-else>
          <div class="is-flex is-flex-wrap-wrap">
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
          <div v-if="isToday(slotProps.item.day) && (!slotProps.item.totalHoursApproved)" class="mt-2">
            <b-button type="is-primary" @click="showClockIn = true" expanded>
              Clock in
            </b-button>
          </div>
        </div>
      </template>
    </Calendar>

    <!-- Modal para punch card -->
    <b-modal custom-content-class="card" v-model="showModalPunchCard">
      <TimeSheetModal v-if="editableDay" :requestId="requestId" :worker="{ workerProfileId: workerProfileId }"
        v-model:editable-day="editableDay" @updateData="updateCell" />
    </b-modal>

    <!-- Modal para detalle -->
    <b-modal custom-content-class="card" v-model="showDetailPunchCard" width="500px">
      <TimeSheetDetail v-if="editableDay" :editable-day="editableDay" />
    </b-modal>

    <TimeSheetDaySheet v-model="showDaySheet" :editable-day="editableDay" @save="onSheetSave" @post="onSheetPost"
      @delete="onSheetDelete" />

    <SheetPanel v-model="showDetailSheet" :title="detailSheetTitle">
      <TimeSheetDetail v-if="editableDay" :editable-day="editableDay" />
    </SheetPanel>

    <!-- Clock in modal -->
    <b-modal custom-content-class="card" v-model="showClockIn" width="400px">
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

<script setup lang="ts">
import { ref, computed } from 'vue';
import { showAlertConfirm, showAlertError, showAlertSuccess } from "@/utils/toast";
import { dateHHmm, hour } from '@/utils/filters';
import dayjs from "dayjs";
import duration from 'dayjs/plugin/duration';
import Calendar from "../calendar/CalendarPunchCard.vue";
import PunchCardAgenda from "../../components/company_request/PunchCardAgenda.vue";
import TimeSheetDaySheet from "../../components/company_request/TimeSheetDaySheet.vue";
import SheetPanel from "../../components/responsive/SheetPanel.vue";
import TimeSheetModal from "../../components/company_request/CompanyRequestTimeSheetModal.vue";
import TimeSheetDetail from "../../components/company_request/CompanyRequestTimeSheetDetail.vue";
import { useBreakpoint } from '@/composables/useBreakpoint';
import type { PunchCardDay, TimeSheetModel } from '@/types/company';
import { buildTimeSheetApproveModel } from "@/utils/timeSheetApprove";
import { maximumHoursPerDay } from "@/constants/catalog";
import {
  getCompanyWorkerTimeSheetByDate,
  postCompanyWorkerTimeSheet,
  deleteCompanyWorkerTimeSheet as deleteCompanyWorkerTimeSheetApi,
  companyTimeSheetClockIn,
  updateCompanyRequestWorkerTimeSheet
} from '@/api/companyApi';

dayjs.extend(duration);

const props = defineProps<{ workerProfileId: any; requestId: any; request: any; worker: any }>();

const { isTouch } = useBreakpoint();

const emptyTime = dayjs().hour(0).minute(0).second(0).millisecond(0).toDate();
const startDate = ref('');
const endDate = ref('');
const data = ref<any[]>([]);
const isLoading = ref(false);
const editableDay = ref<any>(null);
const showClockIn = ref(false);
const clockInTime = ref<Date>(emptyTime);
const maxClockInTime = ref<Date>(new Date());
const showModalPunchCard = ref(false);
const showDetailPunchCard = ref(false);
const showDaySheet = ref(false);
const showDetailSheet = ref(false);
const itemErrors = ref<Record<string, string>>({});
const startTime = ref<Date | null>(null);

const detailSheetTitle = computed(() =>
  editableDay.value ? dayjs(editableDay.value.day).format('dddd, MMMM D') : '',
);

function dayKeyOf(day: PunchCardDay) {
  return dayjs(day.day).format('YYYY-MM-DD');
}

function onSheetSave(model: TimeSheetModel) {
  const item = editableDay.value;
  if (!item?.id) return;
  isLoading.value = true;
  updateCompanyRequestWorkerTimeSheet(props.requestId, props.workerProfileId, item.id, model)
    .then(() => {
      isLoading.value = false;
      showAlertSuccess('Updated');
      showDaySheet.value = false;
      getAgencyWorkerTimeSheetByDate();
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function onSheetPost(hours: number) {
  const item = editableDay.value;
  if (!item) return;
  showDaySheet.value = false;
  reportWorkerTimSheet({ ...item, totalHoursApproved: hours });
}

function onSheetDelete() {
  const item = editableDay.value;
  if (!item) return;
  showAlertConfirm('Are you sure', 'You want to delete this item?')
    .then(response => {
      if (response) {
        showDaySheet.value = false;
        doDelete(item);
      }
    });
}

const maximumDailyHours = computed(() => maximumHoursPerDay);

function timeSheetFastApprove(item: any, requestId: any, workerProfileId: any) {
  isLoading.value = true;
  const model = buildTimeSheetApproveModel(item);
  updateCompanyRequestWorkerTimeSheet(requestId, workerProfileId, item.id, model)
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

function getAgencyWorkerTimeSheetByDate() {
  isLoading.value = true;
  getCompanyWorkerTimeSheetByDate(props.requestId, props.workerProfileId, { startDate: startDate.value, endDate: endDate.value })
    .then((response: any) => {
      isLoading.value = false;
      data.value = response;
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function onMonthChange(sd: string, ed: string) {
  startDate.value = sd;
  endDate.value = ed;
  getAgencyWorkerTimeSheetByDate();
}

function updateCell() {
  getAgencyWorkerTimeSheetByDate();
  showModalPunchCard.value = false;
  showDaySheet.value = false;
}

function isToday(date: any) {
  return dayjs(date).format('YYYY-MM-DD') === dayjs().format('YYYY-MM-DD');
}

function onClockIn() {
  if (!clockInTime.value) return;
  isLoading.value = true;
  const model = { "clockIn": dayjs(clockInTime.value).format("HH:mm:ss") };
  companyTimeSheetClockIn(props.requestId, props.workerProfileId, model)
    .then(() => {
      isLoading.value = false;
      showClockIn.value = false;
      getAgencyWorkerTimeSheetByDate();
      clockInTime.value = dayjs().hour(0).minute(0).second(0).millisecond(0).toDate();
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function validatePost(model: any, index: any) {
  const key = String(index);
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
  isLoading.value = true;
  const model = {
    hours: changeDecimalToHour(item.totalHoursApproved),
    timeIn: todayTimeZero(item.day),
    missingHours: item.missingHours,
    missingHoursOvertime: item.missingHoursOvertime,
    missingRateWorker: item.missingRateWorker,
    missingRateAgency: item.missingRateAgency,
    deductionsOthers: item.deductionsOthers,
    bonusOrOthers: item.bonusOrOthers,
    deductionsOthersDescription: item.deductionsOthersDescription,
    bonusOrOthersDescription: item.bonusOrOthersDescription,
    comments: item.comments,
  };
  postCompanyWorkerTimeSheet(props.requestId, props.workerProfileId, model)
    .then(() => {
      isLoading.value = false;
      getAgencyWorkerTimeSheetByDate();
    }).catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function confirmDelete(item: any) {
  showAlertConfirm('Are you sure', 'You want to delete this item?')
    .then(response => {
      if (response) {
        doDelete(item);
      }
    });
}

function doDelete(item: any) {
  isLoading.value = true;
  deleteCompanyWorkerTimeSheetApi(props.requestId, props.workerProfileId, item.id)
    .then(() => {
      isLoading.value = false;
      item.id = null;
      item.totalHoursApproved = null;
      item.timeIn = null;
      getAgencyWorkerTimeSheetByDate();
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function changeDecimalToHour(time: number) {
  return dayjs().startOf('day').add(time, 'hours').format('HH:mm:ss');
}

function todayTimeZero(time: any) {
  return dayjs(time).hour(0).minute(0).second(0).format('YYYY-MM-DDTHH:mm:ss');
}

function openDetail(item: any) {
  editableDay.value = item;
  if (isTouch.value) {
    showDetailSheet.value = true;
  } else {
    showDetailPunchCard.value = true;
  }
}

function editPunchCard(day: any) {
  editableDay.value = null;
  editableDay.value = Object.assign({}, day);
  editableDay.value.timeInApproved = new Date(day.timeInApproved);
  editableDay.value.timeOutApproved = new Date(day.timeOutApproved);
  editableDay.value.missinghoursToDate = stringToDate(editableDay.value.missingHours);
  editableDay.value.missingHoursOvertimeToDate = stringToDate(editableDay.value.missingHoursOvertime);
  editableDay.value.hoursApprovedToDate = startTime.value;
  editableDay.value.missingRateWorker = day.missingRateWorker;
  editableDay.value.missingRateAgency = day.missingRateAgency;

  const newTime = new Date();
  const dur = dayjs.duration(day.totalHoursApproved, 'hours');
  newTime.setHours(dur.hours());
  const min = dur.minutes();
  newTime.setMinutes(min);
  const sec = dur.seconds();
  newTime.setSeconds(sec);
  editableDay.value.hoursApprovedToDate = newTime;

  if (isTouch.value) {
    showDaySheet.value = true;
  } else {
    showModalPunchCard.value = true;
  }
}

function stringToDate(value: string) {
  if (value) {
    const tmp = value.split(":");
    const d = new Date();
    d.setHours(parseInt(tmp[0]));
    d.setMinutes(parseInt(tmp[1]));
    d.setSeconds(0);
    return d;
  }
  return startTime.value;
}
</script>

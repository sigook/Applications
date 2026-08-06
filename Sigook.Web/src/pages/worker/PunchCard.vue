<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <DataEntryTerms></DataEntryTerms>
    <section class="punch-card">
      <div class="left-60">
        <b-datepicker inline v-model="dateSelected" :events="highlights" indicators="dots"></b-datepicker>
      </div>
      <div class="right-40">
        <h3 class="fz2 has-text-weight-normal">{{ "Punch card" }}</h3>
        <table class="table-report-hours">
          <tr>
            <td>Day:</td>
            <td>{{ date(dateSelected) }}
            </td>
          </tr>
          <tr v-if="timesheet.items && todayData">
            <td>Clock In:</td>
            <td>{{ time(todayData.clockIn) }}</td>
          </tr>

          <tr v-if="todayData && todayData.clockOut !== null">
            <td>Clock out:</td>
            <td>{{ time(todayData.clockOut) }}</td>
          </tr>
        </table>
        <div>
          <div class="current-hour">
            <span>{{ currentHour }}</span>
          </div>
          <div class="has-text-centered" v-if="canBeRegistered">
            <b-button v-if="isEntryTime" type="is-white" outlined rounded @click="registerEntryHour">START SHIFT</b-button>
            <b-button v-else type="is-white" outlined rounded @click="registerDepartureHour">END SHIFT</b-button>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onUnmounted } from 'vue';
import { useAppStore } from '@/stores/app';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/utils/toast';
import dayjs from 'dayjs';
import { workerRegisterTime, getClockType as getClockTypeApi } from '@/api/workerApi';
import { ClockType } from '@/constants/enums';
import { date, time } from '@/utils/filters';
import DataEntryTerms from '../../components/DataEntryTerms.vue';

const props = defineProps<{
  requestId?: any;
  timesheet?: any;
}>();

const emit = defineEmits<{
  (e: 'refreshTimeSheet'): void;
}>();

const appStore = useAppStore();

const isLoading = ref(false);
const dateSelected = ref<Date | null>(null);
const currentHour = ref<string | null>(null);
const isEntryTime = ref(false);
const position = ref<GeolocationPosition | null>(null);
const canBeRegistered = ref(false);

const highlights = computed(() => {
  if (props.timesheet && props.timesheet.items && props.timesheet.items.length > 0) {
    return props.timesheet.items.map((i: any) => new Date(i.day));
  }
  return [];
});

const todayData = computed(() => {
  if (!props.timesheet || !props.timesheet.items) return null;
  const today = props.timesheet.items.find((item: any) => {
    return dayjs(item.day).format('DD-MM-YYYY') === dayjs(dateSelected.value).format('DD-MM-YYYY');
  });
  return today || null;
});

function getTimeFromNow() {
  currentHour.value = dayjs().format('HH:mm:ss');
}

async function getClockType() {
  return await getClockTypeApi(props.requestId, dayjs(dateSelected.value).format('YYYY-MM-DD'));
}

function registerHour() {
  if (!position.value) return;
  isLoading.value = true;
  return workerRegisterTime(props.requestId, position.value.coords.latitude, position.value.coords.longitude)
    .then(async () => {
      emit('refreshTimeSheet');
      const clockType = await getClockType();
      isEntryTime.value = clockType === ClockType.ClockIn;
      canBeRegistered.value = clockType !== ClockType.None;
      isLoading.value = false;
      if (isEntryTime.value) {
        showAlertSuccess('Enjoy your shift!');
      } else {
        showAlertSuccess('Thanks for your job!');
      }
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError((error as { data?: unknown }).data);
    });
}

function registerEntryHour() {
  if (position.value) {
    showAlertConfirm('Starting Job').then((response: boolean) => {
      if (response) registerHour();
    });
  } else {
    showAlertError('Please enable to know your location in the browser');
  }
}

function registerDepartureHour() {
  if (position.value) {
    showAlertConfirm('Ending Work', '').then((response: boolean) => {
      if (response) registerHour();
    });
  } else {
    showAlertError('Please enable to know your location in the browser');
  }
}

getTimeFromNow();
const timerId = setInterval(getTimeFromNow, 1000);

(async () => {
  dateSelected.value = await appStore.getCurrentDate();
  navigator.geolocation.watchPosition(
    (p) => (position.value = p),
    () => (position.value = null)
  );
})();

onUnmounted(() => {
  clearInterval(timerId);
});

watch(dateSelected, async (value) => {
  if (value) {
    isLoading.value = true;
    const clockType = await getClockType();
    isEntryTime.value = clockType === 1;
    canBeRegistered.value = clockType !== 0;
    isLoading.value = false;
  }
});
</script>

<style lang="scss">
@import '../../assets/scss/variables';

.punch-card {
  max-width: 700px;
  margin: 40px auto;
  display: flex;

  h3 {
    margin: 10px 0 10px;
  }

  .left-60 {
    flex-basis: 60%;
    padding-right: 15px;
  }

  .right-40 {
    flex-basis: 40%;
    background-color: $primary;
    padding: 15px;
    color: white;
  }

  table {
    border: 0;

    tr td {
      padding: 0 5px 5px;
    }
  }

  .current-hour {
    text-align: center;
    border-bottom: 1px solid rgba(255, 255, 255, .5);
    padding-bottom: 4px;
    margin-top: 30px;
    margin-bottom: 20px;
    letter-spacing: 4px;
    font-size: 2.35em;
  }
}

@media(max-width: 767px) {
  .punch-card {
    display: block;
    margin: 10px 0 0;

    .left-60 {
      padding: 0;

    }

    .right-40 {
      display: block;
      width: 100%;
    }
  }
}
</style>

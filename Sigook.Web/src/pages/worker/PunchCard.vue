<template>
  <div class="white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <data-entry-terms></data-entry-terms>
    <section class="punch-card">
      <div class="left-60">
        <b-datepicker inline v-model="dateSelected" :events="highlights" indicators="dots"></b-datepicker>
      </div>
      <div class="right-40">
        <h3 class="fz2 fw-400">{{ $t("WorkerPunchCard") }}</h3>
        <table class="table-report-hours">
          <tr>
            <td>Day:</td>
            <td>{{ dateSelected | date }}
            </td>
          </tr>
          <tr v-if="timesheet.items && todayData">
            <td>Clock In:</td>
            <td>{{ todayData.clockIn | time }}</td>
          </tr>

          <tr v-if="todayData && todayData.clockOut !== null">
            <td>Clock out:</td>
            <td>{{ todayData.clockOut | time }}</td>
          </tr>
        </table>
        <div>
          <div class="current-hour">
            <span>{{ currentHour }}</span>
          </div>
          <div class="text-center" v-if="canBeRegistered">
            <button class="btn outline-btn white-button md-btn btn-radius" @click="registerEntryHour"
              v-if="isEntryTime">{{ $t("AddEntryTime") }}</button>
            <button v-else class="btn outline-btn white-button md-btn btn-radius" @click="registerDepartureHour">
              {{ $t("AddDepartureTime") }}
            </button>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script>
import dayjs from "dayjs";

export default {
  props: ['requestId', 'timesheet'],
  data() {
    return {
      isLoading: false,
      dateSelected: null,
      currentHour: null,
      isEntryTime: false,
      position: null,
      canBeRegistered: false
    }
  },
  methods: {
    getTimeFromNow() {
      this.currentHour = dayjs(this.created).format('HH:mm:ss');
    },
    registerHour() {
      this.isLoading = true;
      return this.$store.dispatch('worker/workerRegisterTime', {
        requestId: this.requestId,
        latitude: this.position.coords.latitude,
        longitude: this.position.coords.longitude
      }).then(async () => {
        this.$parent.getTimeSheet();
        const clockType = await this.getClockType();
        this.isEntryTime = clockType === 1;
        this.canBeRegistered = clockType !== 0;
        this.isLoading = false;
        this.isEntryTime ? this.showAlertSuccess(this.$t("EnjoyYourShift")) : this.showAlertSuccess(this.$t("ThanksForYourJob"));
      }).catch(error => {
        this.isLoading = false;
        this.showAlertError(error.data);
      });
    },
    registerEntryHour() {
      if (this.position) {
        this.showAlertConfirm(this.$t("StartingJob"))
          .then(response => {
            if (response) {
              this.registerHour();
            }
          })
      } else {
        this.showAlertError('Please enable to know your location in the browser');
      }
    },
    registerDepartureHour() {
      if (this.position) {
        this.showAlertConfirm(this.$t("EndingWork"), "")
        .then(response => {
          if (response) {
            this.registerHour();
          }
        });
      } else {
        this.showAlertError('Please enable to know your location in the browser');
      }
    },
    async getClockType() {
      return (await this.$store.dispatch('worker/getClockType', {
        requestId: this.requestId,
        date: dayjs(this.dateSelected).format('YYYY-MM-DD')
      }));
    }
  },
  async created() {
    this.getTimeFromNow();
    setInterval(this.getTimeFromNow, 1000);
    this.dateSelected = await this.$store.dispatch('getCurrentDate');
    navigator.geolocation.watchPosition((position) => this.position = position, () => this.position = null);
  },
  destroyed() {
    clearInterval(this.getTimeFromNow)
  },
  components: {
    DataEntryTerms: () => import("../../components/DataEntryTerms.vue")
  },
  computed: {
    highlights() {
      if (this.timesheet && this.timesheet.items.length > 0) {
        return this.timesheet.items.map(i => new Date(i.day));
      }
    },
    todayData() {
      let today = this.timesheet.items.find(item => {
        return dayjs(item.day).format('DD-MM-YYYY') === dayjs(this.dateSelected).format('DD-MM-YYYY');
      });
      if (today) {
        return today;
      } else {
        return null;
      }
    }
  },
  watch: {
    dateSelected: async function (value) {
      if (value) {
        this.isLoading = true;
        const clockType = await this.getClockType();
        this.isEntryTime = clockType === 1;
        this.canBeRegistered = clockType !== 0;
        this.isLoading = false;
      }
    }
  }
}
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

#app .worked-day .vdp-datepicker__calendar .cell.highlighted {

  &:after {
    width: 12px;
    height: 12px;
    top: auto;
    bottom: 3px;
    left: auto;
    right: 6px;
    z-index: 1;
    background: rgba(210, 210, 210, .5) url("../../assets/images/checked.png") no-repeat 50%;
    background-size: calc(100% - 4px);
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
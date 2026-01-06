<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="text-center main-title">{{ workerName }}</h2>
    <div class="submenu-modal-tabs">
      <ul>
        <li @click="punchCard = true" :class="punchCard == true ? 'active' : ''">
          {{ $t("PunchCard") }}
        </li>
        <li @click="showTimeSheet()" :class="punchCard == false ? 'active' : ''">
          {{ $t("TimeSheet") }}
        </li>
      </ul>
    </div>
    <transition name="fadeHeight">
      <div class="container-flex" v-if="punchCard">
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="errors.has('newDate') ? 'is-danger' : ''" label="Date"
            :message="errors.has('newDate') ? errors.first('newDate') : ''">
            <b-datepicker v-model="newDate.date" indicators="bars" append-to-body>
            </b-datepicker>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :label="$t('DailyHours')" :type="errors.has('newDateHour') ? 'is-danger' : ''"
            :message="errors.has('newDateHour') ? errors.first('newDateHour') : ''">
            <b-timepicker placeholder="Select a time..." v-model="newDate.hours" name="newDateHour"
              v-validate="'required'" :disabled="!newDate.date" hour-format="24">
            </b-timepicker>
          </b-field>
        </div>
        <div class="col-12 col-padding">
          <b-switch v-model="showOptions">
            {{ showOptions ? 'Hide Options' : 'Show Options' }}
          </b-switch>
        </div>
        <div class="container-flex" v-if="showOptions">
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field :label="$t('MissingHours')">
              <b-timepicker v-model="newDate.missingHours" hour-format="24" :max-time="maximumMissing">
              </b-timepicker>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field :label="$t('MissingHours') + ' ' + $t('Overtime')">
              <b-timepicker v-model="newDate.missingHoursOvertime" hour-format="24" :max-time="maximumMissing">
              </b-timepicker>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Missing Worker Rate" :type="errors.has('deductionsW') ? 'is-danger' : ''"
              :message="errors.has('deductionsW') ? errors.first('deductionsW') : ''">
              <b-numberinput v-model="newDate.missingRateWorker" step="0.01" name="deductionsW"
                v-validate="'max_value:100|min_value:0'" controls-alignment="right">
              </b-numberinput>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Missing Agency Rate" :type="errors.has('deductionsC') ? 'is-danger' : ''"
              :message="errors.has('deductionsC') ? errors.first('deductionsC') : ''">
              <b-numberinput v-model="newDate.missingRateAgency" step="0.01" name="deductionsC"
                v-validate="'max_value:100|min_value:0'" controls-alignment="right">
              </b-numberinput>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Deductions" :type="errors.has('deductions') ? 'is-danger' : ''"
              :message="errors.has('deductions') ? errors.first('deductions') : ''">
              <b-numberinput v-model="newDate.deductionsOthers" step="0.01" name="deductions"
                v-validate="'max_value:1000|min_value:0'" controls-alignment="right">
              </b-numberinput>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Deductions Description" :type="errors.has('deductionsDes') ? 'is-danger' : ''"
              :message="errors.has('deductionsDes') ? errors.first('deductionsDes') : ''">
              <b-input v-model="newDate.deductionsOthersDescription" type="text" name="deductionsDes"
                v-validate="'max:1000'">
              </b-input>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Bonus or others" :type="errors.has('bonus') ? 'is-danger' : ''"
              :message="errors.has('bonus') ? errors.first('bonus') : ''">
              <b-numberinput v-model="newDate.bonusOrOthers" step="0.01" name="bonus"
                v-validate="'max_value:1000|min_value:0'" controls-alignment="right">
              </b-numberinput>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Bonus or others Description" :type="errors.has('bonusDes') ? 'is-danger' : ''"
              :message="errors.has('bonusDes') ? errors.first('bonusDes') : ''">
              <b-input v-model="newDate.bonusOrOthersDescription" type="text" name="bonusDes" v-validate="'max:1000'">
              </b-input>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Reimbursements" :type="errors.has('reimbursements') ? 'is-danger' : ''"
              :message="errors.has('reimbursements') ? errors.first('reimbursements') : ''">
              <b-numberinput v-model="newDate.reimbursements" step="0.01" name="reimbursements"
                v-validate="'max_value:1000|min_value:0'" controls-alignment="right">
              </b-numberinput>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Reimbursements Description"
              :type="errors.has('reimbursementsDescription') ? 'is-danger' : ''"
              :message="errors.has('reimbursementsDescription') ? errors.first('reimbursementsDescription') : ''">
              <b-input v-model="newDate.reimbursementsDescription" type="text" name="reimbursementsDescription"
                v-validate="'max:1000'">
              </b-input>
            </b-field>
          </div>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-button type="is-primary" @click="reportWorkerTimSheet">
            {{ newDate.id ? $t("Update") : $t("Save") }}
          </b-button>
        </div>
      </div>
    </transition>
    <transition name="fadeHeight">
      <b-table v-if="!punchCard" :data="rows" :per-page="size" :current-page="currentPage" narrowed hoverable
        :mobile-cards="false" paginated pagination-rounded>
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
        </template>
        <template>
          <b-table-column field="day" label="Day" v-slot="props">
            {{ props.row.day | date }}
          </b-table-column>
          <b-table-column field="timeIn" label="Start Time" v-slot="props">
            {{ props.row.timeIn | time }}
          </b-table-column>
          <b-table-column field="totalHoursApproved" label="Hours Approved" v-slot="props">
            <span class="big-decimal">{{ props.row.totalHoursApproved }}</span>
          </b-table-column>
        </template>
      </b-table>
    </transition>
  </div>
</template>
<script>
import dayjs from "dayjs";

export default {
  props: ["workerId", "workerName", "requestId"],
  data() {
    const emptyTime = dayjs().hour(0).minute(0).second(0).millisecond(0).toDate();
    const maximumMissing = dayjs().hour(12).minute(0).second(0).millisecond(0).toDate();
    return {
      showOptions: false,
      punchCard: true,
      isLoading: false,
      newDate: {
        hours: emptyTime,
        missingHours: emptyTime,
        missingHoursOvertime: emptyTime,
      },
      size: 30,
      currentPage: 1,
      maximumMissing: maximumMissing,
      rows: []
    }
  },
  methods: {
    showTimeSheet() {
      this.isLoading = true;
      this.$store.dispatch("agency/getAgencyWorkerTimeSheet", { requestId: this.requestId, workerId: this.workerId })
        .then(response => {
          this.isLoading = false;
          this.rows = response;
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error)
        });
      this.punchCard = false;
    },
    reportWorkerTimSheet() {
      this.isLoading = true;
      const date = dayjs(this.newDate.date).format('YYYY-MM-DD');
      const hours = dayjs(this.newDate.hours).format('HH:mm:ss');
      const payload = {
        ...this.newDate,
        hours,
        timeIn: dayjs(date + ' ' + hours).format('MM-DD-YYYYY HH:mm:ss'),
        missingHours: dayjs(this.newDate.missingHours).format('HH:mm:ss'),
        missingHoursOvertime: dayjs(this.newDate.missingHoursOvertime).format('HH:mm:ss')
      }
      this.$store.dispatch("agency/postAgencyWorkerTimeSheet", { requestId: this.requestId, workerId: this.workerId, model: payload })
        .then(() => {
          this.isLoading = false;
          this.showAlertSuccess('Created');
          this.$emit('created');
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    },
  }
}
</script>
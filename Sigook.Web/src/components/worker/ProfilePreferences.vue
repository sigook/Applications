<template>
    <div>
        <div class="profile-worker">
            <section id="lift">
                <lift :worker="worker" @updateProfile="() => updateProfile()" />
            </section>
            <section  id="availability" :class="{'missing': worker.availabilities.length === 0}">
                <availability :worker="worker" @updateProfile="() => updateProfile()" />
            </section>
            <section id="availabletime" :class="{'missing': worker.availabilityTimes.length === 0}">
                <availability-times :worker="worker" @updateProfile="() => updateProfile()" />
            </section>

            <section id="availabledays" :class="{'missing': worker.availabilityDays.length === 0}">
                <availability-days :worker="worker" @updateProfile="() => updateProfile()" />
            </section>

            <section id="locationpreferences" :class="{'missing': worker.locationPreferences.length === 0}">
                <location-preferences :worker="worker" @updateProfile="() => updateProfile()" />
            </section>
            <emergency-information
                    id="emergencyinformation"
                    :class="{'missing': !worker.contactEmergencyPhone}"
                    :worker="worker" @updateProfile="() => updateProfile()" />
        </div>
    </div>
</template>

<script>
import toastMixin from "../../mixins/toastMixin";

export default {
    props: ['worker'],
    mixins: [
        toastMixin
    ],
    components: {
        lift: () => import("../../components/worker/WorkLiftDetail"),
        availability: () => import("../../components/worker/WorkAvailabilitiesDetail"),
        availabilityTimes: () => import("../../components/worker/WorkAvailabilityTimesDetail"),
        availabilityDays: () => import("../../components/worker/WorkAvailabilityDaysDetail"),
        locationPreferences: () => import("../../components/worker/WorkLocationPreferencesDetail"),
        emergencyInformation: () => import("./WorkEmergencyInformationDetail")
    },
    methods: {
        updateProfile(){
            this.isLoading = true;
            this.$store.dispatch('worker/getProfile', this.worker.id)
                    .then(() => {
                        this.isLoading = false;
                    })
                    .catch(error => {
                        this.isLoading = false;
                        this.showAlertError(error);
                    })
        }
    }
}
</script>
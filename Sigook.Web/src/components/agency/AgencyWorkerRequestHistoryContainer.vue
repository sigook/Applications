<template>
    <div class="container-requests">
        <!--<router-link :to="'/agency-request/' + data.id">-->
        <div class="container-requests-top">
            <span class="asap" v-if="data.isAsap">{{ 'Asap' }}</span>
            <img :src="data.logo" class="request-logo" />
            <div class="container-title">
                <h3 class="light-title">
                    {{ data.jobTitle }}
                </h3>
                <div class="subtitle">{{ data.companyFullName }}</div>
            </div>
            <div class="text-right container-right">
                <div class="subtitle icon-before-location uppercase icon-before">{{ data.location }}</div>
            </div>
        </div>
        <div class="container-actions">
            <div>
                <b>Id:</b> {{ data.numberId }} |
                <b>Start:</b> {{ date(data.startWorking) }}
                |
                <b>Finish:</b> {{ date(data.finishWorking) }}
            </div>
            <div v-if="data.status" class="container-status uppercase" :class="'status-' + data.status.toLowerCase()"
                v-status="{ status: data.status }"> {{ data.status }}</div>
        </div>
        <!--</router-link>-->
    </div>
</template>

<script lang="ts">
import { date } from '@/utils/filters';

export default {
    props: ['data'],
    methods: {
        date,
    }
}
</script>
<style scoped>
.container-requests {
    cursor: default;
}

.container-requests-top h3 {
    margin: 0;
}
</style>
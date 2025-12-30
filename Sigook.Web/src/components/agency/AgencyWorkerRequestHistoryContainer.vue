<template>
    <div class="container-requests">
        <!--<router-link :to="'/agency-request/' + data.id">-->
        <div class="container-requests-top">
            <span class="asap" v-if="data.isAsap">{{ $t('Asap') }}</span>
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
                <b>Start:</b> {{ data.startWorking | dateFilter }}
                |
                <b>Finish:</b> {{ data.finishWorking | dateFilter }}
            </div>
            <div class="container-status uppercase" :class="'status-' + data.status.toLowerCase()"
                v-status="{ status: data.status }"> {{ $t(data.status) }}</div>
        </div>
        <!--</router-link>-->
    </div>
</template>

<script>

export default {
    props: ['data']
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
<template>
    <div class="container-worker" :class="{'disabled': !data.approvedToWork, 'container-worker-inline': showFull}">
        <div @click="sendToDetail" class="pointer container-worker-inline-company">
            <span class="fz-1 tag_primary_absolute" v-if="data.isCurrentlyWorking" >working</span>
            <img :src="data.profileImage">
            <div class="container-worker-inline-name">
            <h3 class="fz1 fw-200 capitalize">{{lowercase(data.name)}} {{lowercase(data.lastName)}}</h3>
            <span class="fz-1 fw-200 color-gray-light"
                      v-for="(item, index) in data.skills.slice(0, 2)" :key="index"> {{item}}
                <span v-if="index === 0 && data.skills.length > 1 "> - </span>
            </span>
            </div>
        </div>

        <div class="container-worker-inline-bottom">
            <span class="fz-1 fw-700 color-green-dark" :class="{'color-danger': !data.approvedToWork}">
            ID:{{data.numberId}}
            </span>
            <div class="bg-gray">
                <div class="text-center">
                     <span v-for="(item, idx) in data.availabilities" :key="idx">
                    {{item}}
                </span>
                </div>

                <button class="xs-btn background-btn green-button" @click="sendToRequest()" v-if="data.canCreateRequest">
                    {{'Create request to this worker'}}
                </button>
            </div>
        </div>

    </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router';
import { lowercase } from '@/utils/filters';

const props = defineProps<{ data: any; showFull?: boolean }>();
const router = useRouter();

function sendToDetail() {
    router.push({
        path: '/company-workers/worker/' + props.data.id,
        query: {
            agencyId: props.data.agencyId,
            agencyName: props.data.agencyFullName,
            canCreate: props.data.canCreateRequest
        }
    });
}

function sendToRequest() {
    router.push({
        path: '/create-request',
        query: {
            isUserRequest: String(true),
            workerId: props.data.workerId,
            workerFullName: props.data.name + " " + props.data.lastName,
            agencyId: props.data.agencyId,
            agencyName: props.data.agencyFullName,
            canCreate: props.data.canCreateRequest
        }
    });
}
</script>

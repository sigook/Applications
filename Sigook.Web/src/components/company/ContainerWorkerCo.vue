<template>
    <div class="container-worker" :class="{'disabled': !data.approvedToWork, 'container-worker-inline': showFull}">
        <div @click="sendToDetail" class="pointer container-worker-inline-company">
            <span class="fz-1 tag_primary_absolute" v-if="data.isCurrentlyWorking" >working</span>
            <img :src="data.profileImage">
            <div class="container-worker-inline-name">
            <h3 class="fz1 fw-200 capitalize">{{data.name | lowercase}} {{data.lastName | lowercase}}</h3>
            <span class="fz-1 fw-200 color-gray-light"
                      v-for="(item, index) in data.skills" v-if="index < 2"> {{item}}
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
                     <span v-for="item in data.availabilities">
                    {{item}}
                </span>
                </div>

                <button class="xs-btn background-btn green-button" @click="sendToRequest()" v-if="data.canCreateRequest">
                    {{$t('CompanySendRequest')}}
                </button>
            </div>
        </div>

    </div>
</template>

<script>
    export default {
        data() {
            return {}
        },
        props: [
            'data',
            'showFull'
        ],
        methods: {
            sendToDetail() {
                this.$router.push({
                    path: '/company-workers/worker/' + this.data.id,
                    query: {
                        agencyId: this.data.agencyId,
                        agencyName: this.data.agencyFullName,
                        canCreate: this.data.canCreateRequest
                    }
                })
            },
            sendToRequest() {
                this.$router.push({
                    path: '/create-request',
                    query: {
                        isUserRequest: true,
                        workerId: this.data.workerId,
                        workerFullName: this.data.name + " " + this.data.lastName,
                        agencyId: this.data.agencyId,
                        agencyName: this.data.agencyFullName,
                        canCreate: this.data.canCreateRequest
                    }
                })
            }
        }
    }
</script>

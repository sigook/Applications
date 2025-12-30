<template>
    <div class="notifications">
        <b-loading v-model="isLoading"></b-loading>

        <ul v-if="data">
            <li class="header">
                <div class="text-center contain-switch form-100">
                    <span></span>
                    <p>Email</p>
                </div>

            </li>
            <li v-for="item in data" v-bind:key="'userNotification' + item.id">

                <h2 class="title-item">{{item.title}}</h2>

                <div class="text-center contain-switch form-100">
                    <p>{{item.description}}</p>
                    <p class="switch-container-flex">
                        {{$t('No')}}
                        <label class="fz0 fw-400 switch">

                            <input type="checkbox"
                                   v-model="item.emailNotification"
                                   @change="updateUserNotification(item)"/>

                            <span class="slider round"></span>
                        </label>
                        {{$t('Yes')}}
                    </p>
                </div>


                <!--
                <div class="text-center contain-switch form-100">
                    <p>Push Notifications</p>
                    <p class="switch-container-flex">
                        {{$t('No')}}
                        <label class="fz0 fw-400 switch">

                            <input type="checkbox" v-model="item.pushNotification"/>
                            <span class="slider round"></span>
                        </label>
                        {{$t('Yes')}}
                    </p>
                </div>

                <div class="text-center contain-switch form-100">
                    <p>Sms Notifications</p>
                    <p class="switch-container-flex">
                        {{$t('No')}}
                        <label class="fz0 fw-400 switch">

                            <input type="checkbox" v-model="item.smsNotification"/>
                            <span class="slider round"></span>
                        </label>
                        {{$t('Yes')}}
                    </p>
                </div>
                -->

            </li>
        </ul>


    </div>
</template>

<script>

    export default {
        props: ['isDisabled'],
        data(){
            return {
                data: null,
                isLoading: false
            }
        },
        methods: {
            getUserNotification(){
                this.isLoading = true;
                this.$store.dispatch('agency/getUserNotification')
                    .then(response => {
                        this.data = response;
                        this.isLoading = false;
                    })
                    .catch(error => {
                        this.showAlertError(error);
                        this.isLoading = false;
                    })
            },
            updateUserNotification(item){
                this.isLoading = true;
                this.$store.dispatch('agency/updateUserNotification', item)
                    .then(() => {
                        this.isLoading = false;
                    })
                    .catch(error => {
                        this.showAlertError(error);
                        this.isLoading = false;
                    })
            }
        },
        created(){
            this.$emit('hideEditButton', true);
            this.getUserNotification();
        }
    }
</script>

<style lang="scss">

    .notifications {

        .title-item {
            padding: 15px 6px 0;
            font-size: 16px;
            font-weight: 600;
        }

        .contain-switch {
            justify-content: space-between;
        }

        li {
            border-bottom: 1px solid #eaeaea;
        }

        .header {
            p {
                width: 140px;
                margin: 0;
                text-transform: uppercase;
                font-size: 14px;
                font-weight: 700;
            }
        }

    }

</style>
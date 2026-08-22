<template>
    <div class="notifications">
        <b-loading v-model="isLoading"></b-loading>

        <ul v-if="data">
            <li class="header">
                <div class="has-text-centered contain-switch">
                    <span></span>
                    <p>Email</p>
                </div>

            </li>
            <li v-for="item in data" v-bind:key="'userNotification' + item.id">

                <h2 class="title-item">{{item.title}}</h2>

                <div class="has-text-centered contain-switch">
                    <p>{{item.description}}</p>
                    <p class="switch-container-flex">
                        {{'No'}}
                        <label class="fz0 has-text-weight-normal switch">

                            <input type="checkbox"
                                   v-model="item.emailNotification"
                                   @change="saveNotification(item)"/>

                            <span class="slider round"></span>
                        </label>
                        {{'Yes'}}
                    </p>
                </div>


                <!--
                <div class="has-text-centered contain-switch">
                    <p>Push Notifications</p>
                    <p class="switch-container-flex">
                        {{'No'}}
                        <label class="fz0 has-text-weight-normal switch">

                            <input type="checkbox" v-model="item.pushNotification"/>
                            <span class="slider round"></span>
                        </label>
                        {{'Yes'}}
                    </p>
                </div>

                <div class="has-text-centered contain-switch">
                    <p>Sms Notifications</p>
                    <p class="switch-container-flex">
                        {{'No'}}
                        <label class="fz0 has-text-weight-normal switch">

                            <input type="checkbox" v-model="item.smsNotification"/>
                            <span class="slider round"></span>
                        </label>
                        {{'Yes'}}
                    </p>
                </div>
                -->

            </li>
        </ul>


    </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/utils/toast";
import { getUserNotifications, updateUserNotification } from "@/api/userNotificationApi";

defineProps<{ isDisabled?: boolean }>();
const emit = defineEmits<{ (e: 'hideEditButton', v: boolean): void }>();

const data = ref<any[] | null>(null);
const isLoading = ref(false);

function loadNotifications() {
  isLoading.value = true;
  getUserNotifications()
    .then(response => {
      data.value = response;
      isLoading.value = false;
    })
    .catch(error => {
      showAlertError(error);
      isLoading.value = false;
    });
}

function saveNotification(item: any) {
  isLoading.value = true;
  updateUserNotification(item)
    .then(() => {
      isLoading.value = false;
    })
    .catch(error => {
      showAlertError(error);
      isLoading.value = false;
    });
}

emit('hideEditButton', true);
loadNotifications();
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

<style scoped lang="scss">
.contain-switch {
  display: flex;
  & > p {
    margin-right: 15px;
    font-weight: 400;
  }
}

.switch-container-flex {
  display: flex;
  justify-content: center;
  align-items: center;
  text-transform: uppercase;
  font-weight: 400;
  color: #6b6b6b;
  margin-bottom: 40px;
  font-size: 12px;
  label {
    margin: 0 8px;
    display: block;
  }
}
</style>

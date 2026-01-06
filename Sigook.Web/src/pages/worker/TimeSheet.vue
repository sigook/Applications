<template>
    <div class="white-container-mobile">
        <section class="time-sheet scroll scroll-desktop">
            <table class="hide-sm"   v-if="!this.$store.state.isMobile">
                <col width="25%">
                <col width="25%">
                <col width="25%">
                <col width="25%">
                <thead>
                <tr>
                    <th>{{$t("Day")}}</th>
                    <th>Clock In</th>
                    <th>Clock Out</th>
                    <th>{{$t("WorkedHours") }}</th>
                </tr>
                </thead>
                <tbody>
                <tr v-for="item in data.items" v-bind:key="item.id">
                    <td><span v-if="item.day">{{item.day | date}}</span></td>
                    <td><span v-if="item.timeIn">{{item.clockIn | time}}</span></td>
                    <td><span v-if="item.clockOut !== null">
                        {{item.clockOut | time}}
                    </span></td>
                    <td> {{item.totalHours.toFixed(2)}}</td>
                </tr>
                </tbody>
            </table>

            <div v-if="this.$store.state.isMobile" class="worker-time-sheet-mobile">
                <div class="item" v-for="item in data.items" v-bind:key="item.id">

                    <h3>{{item.day | date}}</h3>
                    <div>
                        <p>
                            <b>Clock In:</b><span>{{item.clockIn | time}}</span>
                        </p>
                        <p>
                            <b>Clock out</b>
                            <span v-if="item.clockOut !== null">
                                {{item.clockOut | time}}</span>
                        </p>
                        <p>
                            <b>{{$t("Hours") }}</b><span>{{item.totalHours}}</span>
                        </p>
                    </div>
                </div>
            </div>
        </section>
    </div>
</template>
<script>
import dayjs from "dayjs";
import duration from 'dayjs/plugin/duration';

dayjs.extend(duration);

export default {
    props: ['data'],
    methods: {
        hours(startTime, endTime){
            startTime = dayjs(startTime);
            endTime = dayjs(endTime);

            let duration = dayjs.duration(endTime.diff(startTime));
            let finalHours = duration.asHours();

            if(isNaN(finalHours)){
                finalHours = 0;
            }

            return finalHours.toFixed(1);
        }
    }
}
</script>
<style lang="scss">
    .time-sheet {
        margin-top: 10px;
        table {
            width: 800px;
            margin: 15px auto;
            border-collapse: collapse;
            text-align: left;
            thead > tr th,
            tbody > tr td {
                border-bottom: 1px solid #c5c5c5;
                padding: 8px 10px;
            }

            tbody > tr:last-child td {
                border-bottom: 0px;
            }
        }
    }

    @media(max-width: 767px){
        .time-sheet {
            table {
                width: auto;
            }
        }

        .worker-time-sheet-mobile {

            font-size: 14px;
            padding-top: 15px;
            .item {
                border-bottom: 1px solid #d2d2d2;
                padding:0 0 10px;
                margin-bottom: 0;
                p{
                    margin: 0;
                }
                h3 {
                    margin-bottom: 0;
                    font-size: 14px;
                    padding: 3px 15px;
                    background-color: #f1f1f1
                }
                & > div {
                    display: flex;
                    justify-content: space-between;
                    //padding: 10px 15px 0;
                    p {
                        flex-basis: 33.3%;
                        border: 1px solid #dedede;
                        padding: 4px;
                        b {
                            display: block;
                        }
                    }
                }
                & > div:nth-last-child(1) {
                    p {
                        flex-basis: 50%;
                    }
                }
            }
        }
    }
</style>
<template>
    <div>
        <b-loading v-model="isLoading"></b-loading>
        <ul class="table-shift-detail" v-if="shift">
            <li v-if="shift.sunday">
                Sun {{ shift.sundayStart | hourminutes }} to {{ shift.sundayFinish | hourminutes }}
            </li>
            <li v-if="shift.monday">
                Mon {{ shift.mondayStart | hourminutes }} to {{ shift.mondayFinish | hourminutes }}
            </li>
            <li v-if="shift.tuesday">
                Tue {{ shift.tuesdayStart | hourminutes }} to {{ shift.tuesdayFinish | hourminutes }}
            </li>
            <li v-if="shift.wednesday">
                Wed {{ shift.wednesdayStart | hourminutes }} to {{ shift.wednesdayFinish | hourminutes }}
            </li>
            <li v-if="shift.thursday">
                Thu {{ shift.thursdayStart | hourminutes }} to {{ shift.thursdayFinish | hourminutes }}
            </li>
            <li v-if="shift.friday">
                Fri {{ shift.fridayStart | hourminutes }} to {{ shift.fridayFinish | hourminutes }}
            </li>
            <li v-if="shift.saturday">
                Sat {{ shift.saturdayStart | hourminutes }} to {{ shift.saturdayFinish | hourminutes }}
            </li>
            <li class="fz-1" v-if="shift.comments"><i>*{{ shift.comments }}</i></li>
        </ul>
    </div>
</template>
<script>
export default {
    props: ['shift', 'isLoading']
}
</script>
<style lang="scss">
.table-shift-detail {
    border: 5px solid #f3f3f3;
    border-radius: 5px;
    margin-top: 8px;
    background: #ffffff;
    position: absolute;
    left: -5px;
    top: 12px;
    z-index: 1;
    width: 100%;
    min-width: 200px;

    li {
        padding: 5px;
        border-bottom: 1px solid #f3f3f3;

        &:nth-last-child(1) {
            border-bottom: 0;
        }
    }
}

button.up img {
    transform: rotate(180deg);
}
</style>
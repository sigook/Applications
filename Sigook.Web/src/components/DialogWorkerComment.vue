<template>
    <div class="p-3">
        <span class="fz1 fw-bold">{{ "New comment" }}</span>
        <div class="container-flex mt-3">
            <div class="col-12 col-padding">
                <b-field :label="'Qualification'">
                    <b-rate v-model="rating" :max="5"></b-rate>
                </b-field>
            </div>
            <div class="col-12 col-padding">
                <b-field :type="formErrors.comment ? 'is-danger' : ''" :label="'Comment'"
                    :message="formErrors.comment || ''">
                    <b-input type="textarea" v-model="comment" :name="'comment'" placeholder="Your comment here" />
                </b-field>
            </div>
            <div class="col-12 col-padding">
                <b-button type="is-primary" @click="onComment">{{ "Comment" }}</b-button>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";

const schema = yup.object({
    comment: yup.string().required('Comment is required').min(2, 'Min 2 characters').max(500, 'Max 500 characters'),
});

const emit = defineEmits<{ (e: 'createComment', v: { comment: string; rating: number }): void }>();

const form = useStickyForm<{ comment: string }>({
    schema,
    initialValues: { comment: '' },
});
const { comment } = form.fields;
const formErrors = form.errors;

const rating = ref(1);

function onComment() {
    form.markInteracted();
    form.handleSubmit((values) => {
        emit('createComment', { comment: values.comment, rating: rating.value });
    }, () => {
        showAlertError('Please make sure all required fields are filled out correctly');
    })();
}
</script>

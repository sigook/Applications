<template>
    <div>
        <h2>{{ "New comment" }}</h2>
        <div class="contain-new-comment">
            <p>{{ "Qualification" }}</p>


            <div class="select-rating">

                <input type="radio" id="st5" value="5" name="rating" v-model="rating" />
                <label class="star" for="st5"></label>
                <input type="radio" id="st4" value="4" name="rating" v-model="rating" />
                <label class="star" for="st4"></label>
                <input type="radio" id="st3" value="3" name="rating" v-model="rating" />
                <label class="star" for="st3"></label>
                <input type="radio" id="st2" value="2" name="rating" v-model="rating" />
                <label class="star" for="st2"></label>
                <input type="radio" id="st1" value="1" name="rating" checked v-model="rating" />
                <label class="star" for="st1"></label>

            </div>

            <p>{{ "Comment" }}</p>
            <label>
                <textarea id="textareaComment"
                          v-model="comment"
                          placeholder="Your comment here"
                          name="comment"
                          :class="{ 'is-danger': !!formErrors.comment }"
                ></textarea>
                <span v-show="formErrors.comment" class="help is-danger no-margin">{{ formErrors.comment }}</span>

            </label>

            <button class="background-btn create-btn primary-button btn-radius" @click="onComment">{{ "Comment" }}</button>
        </div>
    </div>
</template>

<script lang="ts">
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";

const schema = yup.object({
    comment: yup.string().required('Comment is required').min(2, 'Min 2 characters').max(500, 'Max 500 characters'),
});

export default {
    setup() {
        const form = useStickyForm({
            schema,
            initialValues: { comment: '' },
        });
        return {
            ...form.fields,
            formErrors: form.errors,
            handleSubmit: form.handleSubmit,
            markInteracted: form.markInteracted,
        };
    },
    data() {
        return {
            rating: 1,
        };
    },
    methods: {
        onComment() {
            this.markInteracted();
            this.handleSubmit((values) => {
                this.$emit('createComment', { comment: values.comment, rating: this.rating });
            }, () => {
                showAlertError('Please make sure all required fields are filled out correctly');
            })();
        }
    }
}
</script>

<style lang="scss" scoped>
.contain-new-comment {
    padding: 10px 6px 10px;
    text-align: left;

    textarea {
        width: 100%;
        height: 100px;
        margin-top: 7px;
        background: #f3f3f3;
        border: 1px solid #afafaf;
        padding: 10px;

        &.is-danger {
            border-color: #da3025;
        }
    }

    .star {
        width: 30px;
        height: 30px;
        background-size: contain;
        display: inline-block;
        margin: 0 10px;
        background: url('../assets/images/star-off.svg') no-repeat;
        cursor: pointer;

        &:hover {
            background-image: url('../assets/images/star-on.svg');
            opacity: .8;
        }
    }

    input:checked~label {
        background-image: url('../assets/images/star-on.svg');
    }


    .star:hover~label {
        background-image: url('../assets/images/star-on.svg');
        opacity: .8;
    }

    .select-rating {
        margin: 8px 0 15px;
        text-align: center;
        direction: rtl;

        input {
            display: none;
        }
    }
}
</style>

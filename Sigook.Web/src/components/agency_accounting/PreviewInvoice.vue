<template>
  <div>
    <div class="p-3">
      <div class="container-flex mb-4">
        <div class="col-12 col-padding text-center">
          <h1 class="fz1 fw-semibold mb-0">PREVIEW INVOICE</h1>
          <p class="color-gray">{{ currentDate }}</p>
        </div>
      </div>

      <div class="container-flex" v-if="preview.items && preview.items.length > 0">
        <div class="col-12 col-padding">
          <h3 class="fz1 fw-semibold mb-3">Items & Services</h3>

          <div class="container-flex fw-semibold mb-2">
            <div class="col-sm-6 col-md-6 col-lg-6 col-padding">Description</div>
            <div class="col-sm-2 col-md-2 col-lg-2 col-padding text-end">Qty</div>
            <div class="col-sm-2 col-md-2 col-lg-2 col-padding text-end">Price</div>
            <div class="col-sm-2 col-md-2 col-lg-2 col-padding text-end">Total</div>
          </div>

          <div class="container-flex mb-2" v-for="(item, index) in preview.items" :key="'item-' + index">
            <div class="col-sm-6 col-md-6 col-lg-6 col-padding">{{ item.description }}</div>
            <div class="col-sm-2 col-md-2 col-lg-2 col-padding text-end">{{ parseFloat(item.quantity).toFixed(2) }}</div>
            <div class="col-sm-2 col-md-2 col-lg-2 col-padding text-end">{{ currency(item.unitPrice) }}</div>
            <div class="col-sm-2 col-md-2 col-lg-2 col-padding text-end">{{ currency(item.total) }}</div>
          </div>
        </div>
      </div>

      <div class="container-flex" v-if="preview.discounts && preview.discounts.length > 0">
        <div class="col-12 col-padding">
          <hr class="my-3">
          <h3 class="fz1 fw-semibold mb-3 color-red">Discounts</h3>

          <div class="container-flex fw-semibold mb-2">
            <div class="col-sm-6 col-md-6 col-lg-6 col-padding">Description</div>
            <div class="col-sm-2 col-md-2 col-lg-2 col-padding text-end">Qty</div>
            <div class="col-sm-2 col-md-2 col-lg-2 col-padding text-end">Price</div>
            <div class="col-sm-2 col-md-2 col-lg-2 col-padding text-end">Total</div>
          </div>

          <div class="container-flex mb-2 color-red" v-for="(discount, index) in preview.discounts"
            :key="'discount-' + index">
            <div class="col-sm-6 col-md-6 col-lg-6 col-padding">{{ discount.description }}</div>
            <div class="col-sm-2 col-md-2 col-lg-2 col-padding text-end">{{ parseFloat(discount.quantity).toFixed(2) }}</div>
            <div class="col-sm-2 col-md-2 col-lg-2 col-padding text-end">{{ currency(discount.unitPrice) }}
            </div>
            <div class="col-sm-2 col-md-2 col-lg-2 col-padding text-end">-{{ currency(discount.total) }}</div>
          </div>
        </div>
      </div>

      <div class="container-flex mt-4">
        <div class="col-sm-12 col-md-6 col-lg-8 col-padding"></div>
        <div class="col-sm-12 col-md-6 col-lg-4 col-padding">
          <div class="container-flex mb-2">
            <div class="col-6 col-padding">Subtotal:</div>
            <div class="col-6 col-padding text-end fw-semibold">{{ currency(preview.subTotal) }}</div>
          </div>
          <div class="container-flex mb-2">
            <div class="col-6 col-padding">TAX/HST:</div>
            <div class="col-6 col-padding text-end fw-semibold">{{ currency(preview.hst) }}</div>
          </div>
          <hr class="my-2">
          <div class="container-flex">
            <div class="col-6 col-padding fz1 fw-semibold">Total:</div>
            <div class="col-6 col-padding text-end fz1 fw-semibold">{{ currency(preview.total) }}</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { currency } from '@/utils/filters';

defineProps<{ preview: any }>();

const currentDate = computed(() =>
  new Date().toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
);
</script>

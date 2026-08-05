<template>
  <div>
    <div class="p-3">
      <div class="columns is-multiline mb-4">
        <div class="column is-12 has-text-centered">
          <h1 class="fz1 has-text-weight-semibold mb-0">PREVIEW INVOICE</h1>
          <p class="color-gray">{{ currentDate }}</p>
        </div>
      </div>

      <div class="columns is-multiline" v-if="preview.items && preview.items.length > 0">
        <div class="column is-12">
          <h3 class="fz1 has-text-weight-semibold mb-3">Items & Services</h3>

          <div class="columns is-multiline has-text-weight-semibold mb-2">
            <div class="column is-6-mobile is-6">Description</div>
            <div class="column is-2-mobile is-2 has-text-right">Qty</div>
            <div class="column is-2-mobile is-2 has-text-right">Price</div>
            <div class="column is-2-mobile is-2 has-text-right">Total</div>
          </div>

          <div class="columns is-multiline mb-2" v-for="(item, index) in preview.items" :key="'item-' + index">
            <div class="column is-6-mobile is-6">{{ item.description }}</div>
            <div class="column is-2-mobile is-2 has-text-right">{{ parseFloat(item.quantity).toFixed(2) }}</div>
            <div class="column is-2-mobile is-2 has-text-right">{{ currency(item.unitPrice) }}</div>
            <div class="column is-2-mobile is-2 has-text-right">{{ currency(item.total) }}</div>
          </div>
        </div>
      </div>

      <div class="columns is-multiline" v-if="preview.discounts && preview.discounts.length > 0">
        <div class="column is-12">
          <hr class="my-3">
          <h3 class="fz1 has-text-weight-semibold mb-3 color-red">Discounts</h3>

          <div class="columns is-multiline has-text-weight-semibold mb-2">
            <div class="column is-6-mobile is-6">Description</div>
            <div class="column is-2-mobile is-2 has-text-right">Qty</div>
            <div class="column is-2-mobile is-2 has-text-right">Price</div>
            <div class="column is-2-mobile is-2 has-text-right">Total</div>
          </div>

          <div class="columns is-multiline mb-2 color-red" v-for="(discount, index) in preview.discounts"
            :key="'discount-' + index">
            <div class="column is-6-mobile is-6">{{ discount.description }}</div>
            <div class="column is-2-mobile is-2 has-text-right">{{ parseFloat(discount.quantity).toFixed(2) }}</div>
            <div class="column is-2-mobile is-2 has-text-right">{{ currency(discount.unitPrice) }}
            </div>
            <div class="column is-2-mobile is-2 has-text-right">-{{ currency(discount.total) }}</div>
          </div>
        </div>
      </div>

      <div class="columns is-multiline mt-4">
        <div class="column is-6 is-8-desktop"></div>
        <div class="column is-6 is-4-desktop">
          <div class="columns is-multiline mb-2">
            <div class="column is-6-mobile is-6">Subtotal:</div>
            <div class="column is-6-mobile is-6 has-text-right has-text-weight-semibold">{{ currency(preview.subTotal) }}</div>
          </div>
          <div class="columns is-multiline mb-2">
            <div class="column is-6-mobile is-6">TAX/HST:</div>
            <div class="column is-6-mobile is-6 has-text-right has-text-weight-semibold">{{ currency(preview.hst) }}</div>
          </div>
          <hr class="my-2">
          <div class="columns is-multiline">
            <div class="column is-6-mobile is-6 fz1 has-text-weight-semibold">Total:</div>
            <div class="column is-6-mobile is-6 has-text-right fz1 has-text-weight-semibold">{{ currency(preview.total) }}</div>
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

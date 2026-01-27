<template>
  <router-view />

  <!-- Global toast notification -->
  <v-snackbar
    v-model="toastState.show"
    :color="toastState.color"
    :timeout="toastState.timeout"
    location="top"
  >
    {{ toastState.message }}
  </v-snackbar>
</template>

<script setup lang="ts">
import { useToast } from '@/composables/useToast'

const { toastState } = useToast()
</script>

<template>
  <footer class="tw-bg-gray-900 tw-text-gray-300">
    <div class="tw-mx-auto tw-max-w-7xl tw-px-6 tw-py-12 lg:tw-px-8">
      <div class="tw-grid tw-grid-cols-1 tw-gap-10 sm:tw-grid-cols-2 lg:tw-grid-cols-4">
        <!-- Brand -->
        <div class="tw-col-span-1 sm:tw-col-span-2 lg:tw-col-span-1">
          <img src="@/assets/images/logo.png" alt="Sigook" class="tw-mb-4 tw-h-8 tw-w-auto tw-brightness-0 tw-invert" />
          <p class="tw-text-sm tw-leading-relaxed tw-text-gray-400">
            Connecting talent with opportunity across Canada.
          </p>
          <div class="tw-mt-4 tw-flex tw-gap-3">
            <a href="https://apps.apple.com/us/app/sigook/id1446736193" target="_blank" rel="noopener" aria-label="App Store">
              <img src="@/assets/images/appstore.png" alt="App Store" class="tw-h-8 tw-w-auto" />
            </a>
            <a href="https://play.google.com/store/apps/details?id=com.sigook.sigook" target="_blank" rel="noopener" aria-label="Google Play">
              <img src="@/assets/images/googleplay.png" alt="Google Play" class="tw-h-8 tw-w-auto" />
            </a>
          </div>
        </div>

        <!-- For Job Seekers -->
        <div>
          <h3 class="tw-mb-4 tw-text-sm tw-font-semibold tw-uppercase tw-tracking-wider tw-text-white">
            For Job Seekers
          </h3>
          <ul class="tw-space-y-2">
            <li><router-link to="/jobSeekers" class="tw-text-sm tw-transition-colors hover:tw-text-white">Find a Job</router-link></li>
            <li><router-link to="/register-worker" class="tw-text-sm tw-transition-colors hover:tw-text-white">Register</router-link></li>
          </ul>
        </div>

        <!-- For Business -->
        <div>
          <h3 class="tw-mb-4 tw-text-sm tw-font-semibold tw-uppercase tw-tracking-wider tw-text-white">
            For Business
          </h3>
          <ul class="tw-space-y-2">
            <li><router-link to="/business" class="tw-text-sm tw-transition-colors hover:tw-text-white">Find Staff</router-link></li>
            <li><router-link to="/direct-hiring" class="tw-text-sm tw-transition-colors hover:tw-text-white">Direct Hiring</router-link></li>
          </ul>
        </div>

        <!-- Company -->
        <div>
          <h3 class="tw-mb-4 tw-text-sm tw-font-semibold tw-uppercase tw-tracking-wider tw-text-white">
            Company
          </h3>
          <ul class="tw-space-y-2">
            <li><router-link to="/about-us" class="tw-text-sm tw-transition-colors hover:tw-text-white">About Us</router-link></li>
            <li><router-link to="/contact" class="tw-text-sm tw-transition-colors hover:tw-text-white">Contact</router-link></li>
            <li><router-link to="/terms-and-conditions" class="tw-text-sm tw-transition-colors hover:tw-text-white">Terms & Conditions</router-link></li>
            <li><router-link to="/privacy-policy" class="tw-text-sm tw-transition-colors hover:tw-text-white">Privacy Policy</router-link></li>
          </ul>
        </div>
      </div>

      <div class="tw-mt-10 tw-border-t tw-border-gray-800 tw-pt-6 tw-text-center">
        <p class="tw-text-xs tw-text-gray-500">
          &copy; {{ new Date().getFullYear() }} Sigook<sup>®</sup>. All rights reserved.
        </p>
      </div>
    </div>
  </footer>
</template>

<script setup lang="ts">
</script>

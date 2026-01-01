<template>
  <div>
    <section class="container-fluid">
      <nav class="navbar navbar-expand-md navbar-dark bg-grey-dark ps-2">
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav"
          aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
          <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse p-2 p-sm-0 ps-md-5" id="navbarNav">
          <ul class="navbar-nav gap-md-5">
            <li class="nav-item dropdown">
              <router-link class="nav-link text-decoration-none" :to="`/${solutionType}/general-labor`"
                active-class="active">
                GENERAL LABOR
              </router-link>
            </li>
            <li class="nav-item">
              <router-link class="nav-link text-decoration-none" :to="`/${solutionType}/clerical`" active-class="active">
                CLERICAL
              </router-link>
            </li>
            <li class="nav-item">
              <router-link class="nav-link text-decoration-none" :to="`/${solutionType}/skilled-job`"
                active-class="active">
                SKILLED JOBS
              </router-link>
            </li>
            <li class="nav-item" title="AUTOMATED ATTENDANCE TRACKING SYSTEM">
              <router-link :to="{ path: '/atas', query: { solutionType: solutionType } }" active-class="active"
                class="nav-link text-decoration-none">
                ATAS
              </router-link>
            </li>
          </ul>
        </div>
      </nav>
    </section>
  </div>
</template>
<script>
export default {
  props: ['solutionType']
}
</script>
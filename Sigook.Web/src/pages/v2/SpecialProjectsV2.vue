<template>
  <main>
    <!-- ── Hero ──────────────────────────────────────────────────────────── -->
    <section class="sp-hero">
      <div class="sp-hero__inner">
        <p class="v2-eyebrow sp-hero__eyebrow">Special Projects</p>
        <h1 class="v2-h1 sp-hero__title">Special Projects</h1>
        <p class="v2-lead sp-hero__lead">
          At SIGOOK, our work naturally expands into initiatives that combine workforce
          development, specialized training, and community engagement to create new
          opportunities across industries and regions.
        </p>
        <a href="#sp-contact" class="btn btn--primary">Get in Touch</a>
      </div>
    </section>

    <!-- ── Focus Areas ───────────────────────────────────────────────────── -->
    <section class="sp-focus">
      <div class="sp-focus__inner">
        <div class="sp-focus__header">
          <p class="v2-eyebrow sp-focus__eyebrow">Our Approach</p>
          <h2 class="v2-h2 sp-focus__title">Our Focus Areas</h2>
          <p class="v2-body sp-focus__desc">
            We develop initiatives that support workforce growth, industry needs, and talent
            development across Canada.
          </p>
        </div>

        <div class="sp-focus__grid">
          <div v-for="area in focusAreas" :key="area.title" class="sp-focus__card">
            <h3 class="v2-h3 sp-focus__card-title">{{ area.title }}</h3>
            <p class="v2-body sp-focus__card-desc">{{ area.desc }}</p>
          </div>
        </div>
      </div>
    </section>

    <!-- ── Contact ───────────────────────────────────────────────────────── -->
    <div id="sp-contact">
      <ContactSectionV2 />
    </div>
  </main>
</template>

<script setup lang="ts">
import ContactSectionV2 from '@/components/v2/ContactSectionV2.vue'

const focusAreas = [
  {
    title: 'Our Focus Areas',
    desc: 'Strategic workforce initiatives driving growth and industry development across Canada.',
  },
  {
    title: 'Featured Initiative',
    desc: 'Programs and projects making a real impact in our communities and creating lasting change.',
  },
  {
    title: 'How We Collaborate',
    desc: 'Working with clients, partners, and communities to deliver meaningful workforce solutions.',
  },
  {
    title: 'Future Initiatives',
    desc: 'Exploring new opportunities and projects that will shape the future of work.',
  },
]
</script>

<style scoped>
/* ── Hero ──────────────────────────────────────────────────────────────────── */
.sp-hero {
  background: var(--c-brand-navy);
  padding: calc(64px + var(--sp-12)) 0 var(--sp-16); /* 64px = mobile header height */
}

.sp-hero__inner {
  max-width: var(--container-max);
  margin: 0 auto;
  padding: 0 var(--gutter-mobile);
}

.sp-hero__eyebrow {
  color: rgba(255, 255, 255, 0.55);
  margin: 0 0 var(--sp-4);
}

.sp-hero__title {
  color: #fff;
  margin: 0 0 var(--sp-6);
}

.sp-hero__lead {
  color: #d7e1e5;
  margin: 0 0 var(--sp-8);
}

/* ── Focus Areas ───────────────────────────────────────────────────────────── */
.sp-focus {
  background: var(--c-surface);
  padding: var(--sp-16) 0;
}

.sp-focus__inner {
  max-width: var(--container-max);
  margin: 0 auto;
  padding: 0 var(--gutter-mobile);
}

.sp-focus__header {
  text-align: center;
  margin-bottom: var(--sp-8);
}

.sp-focus__eyebrow {
  color: var(--c-brand-blue);
  margin: 0 0 var(--sp-2);
}

.sp-focus__title {
  color: var(--c-ink);
  margin: 0 0 var(--sp-4);
}

.sp-focus__desc {
  color: var(--c-ink-2);
  margin: 0;
}

.sp-focus__grid {
  display: flex;
  flex-direction: column;
  gap: var(--sp-4);
}

.sp-focus__card {
  background: var(--c-bg);
  border-radius: var(--r-md);
  padding: var(--sp-6);
  box-shadow: var(--sh-2);
}

.sp-focus__card-title {
  color: var(--c-ink);
  margin: 0 0 var(--sp-3);
}

.sp-focus__card-desc {
  color: var(--c-ink-2);
  margin: 0;
}

/* ── Desktop ───────────────────────────────────────────────────────────────── */
@media (min-width: 1024px) {
  .sp-hero {
    padding: calc(80px + var(--sp-20)) 0 var(--sp-20); /* 80px = desktop header height */
  }

  .sp-hero__inner {
    padding: 0 var(--gutter-desktop);
    max-width: 800px;
    margin: 0 0; /* left-aligned hero */
  }

  .sp-hero__title {
    font-size: var(--text-display-size);
    line-height: var(--text-display-lh);
    letter-spacing: var(--text-display-ls);
  }

  .sp-focus {
    padding: var(--sp-20) 0;
  }

  .sp-focus__inner {
    padding: 0 var(--gutter-desktop);
  }

  .sp-focus__header {
    max-width: 560px;
    margin-left: auto;
    margin-right: auto;
    margin-bottom: var(--sp-12);
  }

  .sp-focus__grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: var(--sp-6);
  }
}
</style>

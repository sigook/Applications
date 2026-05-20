<template>
  <main>
    <!-- ── Hero ──────────────────────────────────────────────────────────── -->
    <section class="sp-hero">
      <div class="sp-hero__inner">
        <h1 class="sp-hero__title">Special Projects</h1>
        <div class="sp-hero__desc">
          <p>At SIGOOK, connecting organizations with talent is just the beginning.</p>
          <p>Over time, our work with employers, communities, and public institutions has naturally expanded into initiatives that combine workforce development, specialized training, and innovative projects aimed at strengthening local economies.</p>
          <p>Through our Special Projects initiatives, we participate in efforts that bring together talent, technology, and community engagement to create new opportunities across industries and regions.</p>
        </div>
        <a href="#sp-contact" class="btn btn--lg btn--cyan">Reach Out ></a>
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
main {
  background: var(--c-brand-teal-dark);
}

/* ── Hero ──────────────────────────────────────────────────────────────────── */
.sp-hero {
  background:
    linear-gradient(to top, rgba(0, 173, 239, 0.7), rgba(235, 28, 36, 0.7)),
    url('/images/v2/sp-hero-bg.png') center / cover no-repeat;
  border-bottom-right-radius: 94px;
  padding: 159px 0 var(--sp-16);
  min-height: 992px;
}

.sp-hero__inner {
  padding: 0 39px;
}

.sp-hero__title {
  font-family: var(--font-family);
  font-size: var(--text-h1-size);
  font-weight: var(--text-display-weight);
  line-height: var(--text-h1-lh);
  color: #fff;
  margin: 0 0 var(--sp-6);
}

.sp-hero__desc {
  color: #fff;
  font-family: var(--font-family);
  font-size: var(--text-body-size);
  font-weight: var(--text-body-weight);
  line-height: var(--text-body-lh);
  text-align: justify;
  max-width: 560px;
  margin: 0;
}

.sp-hero__desc p {
  margin: 0 0 var(--sp-4);
}

.sp-hero__desc p:last-child {
  margin-bottom: 0;
}

.sp-hero .btn {
  margin-top: 43px;
}

/* ── Focus Areas ───────────────────────────────────────────────────────────── */
.sp-focus {
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
  color: #fff;
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
    padding: 218px 0 var(--sp-24);
    min-height: 924px;
  }

  .sp-hero__inner {
    padding: 0 0 0 169px;
    max-width: 800px;
  }

  .sp-hero__title {
    font-size: 55px;
    line-height: var(--text-display-lh);
    letter-spacing: var(--text-display-ls);
  }

  .sp-hero .btn {
    margin-top: var(--sp-16);
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

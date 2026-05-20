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
        <h2 class="sp-focus__title">Our Focus Areas</h2>

        <div class="sp-focus__grid">
          <div v-for="area in focusAreas" :key="area.title" class="sp-focus__card">
            <img class="sp-focus__card-icon" :src="area.icon" :alt="area.title" />
            <div class="sp-focus__card-body">
              <h3 class="sp-focus__card-title">{{ area.title }}</h3>
              <p class="sp-focus__card-desc">{{ area.desc }}</p>
            </div>
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
    desc: 'We develop initiatives to support workforce growth, industry needs, and talent development.',
    icon: '/images/v2/sp-icon-focus-areas.png',
  },
  {
    title: 'Featured Initiative',
    desc: 'Programs and projects that are making an impact.',
    icon: '/images/v2/sp-icon-featured-initiative.png',
  },
  {
    title: 'How we collaborate',
    desc: 'Working together with clients, partners, and communities to deliver meaningful workforce solutions.',
    icon: '/images/v2/sp-icon-collaborate.png',
  },
  {
    title: 'Future Initiatives',
    desc: 'Exploring new opportunities and projects that will shape the future of work.',
    icon: '/images/v2/sp-icon-future-initiatives.png',
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
  padding: var(--sp-12) 0;
}

.sp-focus__inner {
  max-width: var(--container-max);
  margin: 0 auto;
  padding: 0 39px;
}

.sp-focus__title {
  font-size: 30px;
  font-weight: 700;
  color: #fff;
  text-align: center;
  margin: 0 0 36px;
}

.sp-focus__grid {
  display: flex;
  flex-direction: column;
  gap: 21px;
}

.sp-focus__card {
  display: flex;
  align-items: center;
  gap: 18px;
  padding: 13px 20px;
  border-radius: 30px;
  background: var(--c-bg);
  cursor: pointer;
  transition: background 0.25s ease;
}

.sp-focus__card:hover {
  background: var(--c-brand-cyan);
}

/* Mobile: alternate icon side on even cards */
.sp-focus__card:nth-child(even) {
  flex-direction: row-reverse;
}

.sp-focus__card-icon {
  width: 90px;
  height: 90px;
  flex-shrink: 0;
  border-radius: 50%; /* clip square PNG to circle */
  object-fit: cover;
}

.sp-focus__card-body {
  flex: 1;
}

.sp-focus__card-title {
  font-size: 22px;
  font-weight: 700;
  color: var(--c-ink);
  margin: 0 0 6px;
  line-height: 1.2;
}

.sp-focus__card-desc {
  font-size: 12px;
  font-weight: 400;
  color: var(--c-ink);
  margin: 0;
  line-height: 1.5;
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

  .sp-focus__title {
    font-size: 40px;
    margin-bottom: 68px;
  }

  .sp-focus__grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 40px 48px;
    max-width: 1122px;
    margin: 0 auto;
  }

  /* Reset mobile icon alternating — all cards are icon-left on desktop */
  .sp-focus__card:nth-child(even) {
    flex-direction: row;
  }

  .sp-focus__card {
    min-height: 116px;
  }

  .sp-focus__card-body {
    max-width: 368px;
  }
}
</style>

<template>
  <section class="ind-grid" data-aos="fade-up">
    <div class="ind-grid__inner">
      <div
        v-for="industry in industries"
        :key="industry.id"
        class="ind-card"
      >
        <div
          class="ind-card__inner"
          :class="{ 'ind-card__inner--flipped': activeId === industry.id }"
        >
          <!-- CARA FRONTAL -->
          <div class="ind-card__face ind-card__face--front">
            <div class="ind-card__bg">
              <img :src="industry.image" :alt="industry.label" />
              <div class="ind-card__overlay"></div>
            </div>

            <div class="ind-card__content">
              <!-- Icono verde = botón para girar -->
              <button
                class="ind-card__icon"
                @click.stop="toggleCard(industry.id)"
              >
                +
              </button>
              <p class="ind-card__label">{{ industry.label }}</p>
            </div>
          </div>

          <!-- CARA TRASERA -->
          <div class="ind-card__face ind-card__face--back">
            <!-- Borde blanco siguiendo la forma -->
            <div class="ind-card__border"></div>

            <div class="ind-card__back-content">
              <!-- mismo icono, también botón para volver -->
              <button
                class="ind-card__icon ind-card__icon--back"
                @click.stop="toggleCard(industry.id)"
              >
                +
              </button>

              <h3 class="ind-card__back-title">{{ industry.label }}</h3>
              <p class="ind-card__back-text">
                {{ industry.description }}
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { ref } from 'vue'

const industries = [
  {
    id: 1,
    label: 'Automotive',
    image: new URL('@/assets/images/ind-automotive.jpg', import.meta.url).href,
    description:
      'We supply skilled talent for the automotive industry—from technicians to engineers. Our candidates support manufacturing, repair, and sales across all vehicle types.',
  },
  {
    id: 2,
    label: 'Aviation',
    image: new URL('@/assets/images/ind-aviation.jpg', import.meta.url).href,
    description:
      'We recruit aviation experts—pilots, technicians, and support staff. Our candidates help airlines and aviation firms maintain safety, compliance, and operational excellence.',
  },
  {
    id: 3,
    label: 'Construction',
    image: new URL('@/assets/images/ind-construction.jpg', import.meta.url).href,
    description:
      'We connect skilled tradespeople and professionals with top-tier construction companies. We provide talent that drives growth and ensures quality from the ground up.',
  },
  {
    id: 4,
    label: 'Engineering',
    image: new URL('@/assets/images/ind-engineering.jpg', import.meta.url).href,
    description:
      'We recruit engineers across multiple disciplines. Our candidates bring innovation, precision, and technical expertise to every project, fueling infrastructure and development.',
  },
  {
    id: 5,
    label: 'IT / AI',
    image: new URL('@/assets/images/ind-it-ai.jpg', import.meta.url).href,
    description:
      'We source top tech talent in software, data, and AI. Our candidates fuel innovation, boost performance, and deliver smart solutions in fast-evolving digital environments.',
  },
  {
    id: 6,
    label: 'Financial',
    image: new URL('@/assets/images/ind-financial.jpg', import.meta.url).href,
    description:
      'We source skilled finance professionals in banking, accounting, and more. Our candidates bring accuracy, compliance, and insight to fast-paced financial environments.',
  },
  {
    id: 7,
    label: 'Legal / Accounting',
    image: new URL('@/assets/images/ind-legal.jpg', import.meta.url).href,
    description:
      'We recruit legal professionals, including assistants, paralegals, and lawyers. Our candidates bring organization, precision, and expertise to every legal team.',
  },
  {
    id: 8,
    label: 'Logistics, 3PL/4PL',
    image: new URL('@/assets/images/ind-logistics.jpg', import.meta.url).href,
    description:
      'We provide logistics and supply chain talent—from warehouse workers to dispatchers. Our candidates help streamline operations and ensure timely, accurate delivery.',
  },
  {
    id: 9,
    label: 'Manufacturing',
    image: new URL('@/assets/images/ind-manufacturing.jpg', import.meta.url).href,
    description:
      'We support manufacturing with reliable workers—machine operators, assemblers, and supervisors. Our talent helps boost productivity, safety, and product quality.',
  },
  {
    id: 10,
    label: 'Retail',
    image: new URL('@/assets/images/ind-retail.jpg', import.meta.url).href,
    description:
      'We provide retail staff at all levels—from sales associates to managers. Our candidates deliver strong customer service and help drive daily operations and sales success.',
  },
  {
    id: 11,
    label: 'Transportation',
    image: new URL('@/assets/images/ind-transportation.jpg', import.meta.url)
      .href,
    description:
      'We recruit drivers, coordinators, and support staff for transport operations. Our candidates keep people and goods moving safely, efficiently, and on schedule.',
  },
]

// id de la card actualmente abierta
const activeId = ref(null)

const toggleCard = (id) => {
  activeId.value = activeId.value === id ? null : id
}
</script>

<style scoped>
.ind-grid {
  background: linear-gradient(to bottom, #020d1e, #06152c);
  padding: 40px 0 80px;
}

.ind-grid__inner {
  max-width: 1400px;              /* más ancho */
  margin: 0 auto;
  padding: 0 10px;                /* menos margen lateral */
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 11px;
}

/* ========== CARD CON GIRO 3D ========= */

.ind-card {
  position: relative;
  height: 500px;
  perspective: 1200px; /* necesario para el efecto 3D */
  overflow: hidden;

  /* tu forma tipo flecha */
  clip-path: polygon(
    0 15%,
    0 100%,
    50% 85%,
    100% 100%,
    100% 15%,
    50% 0
  );

  border-radius: 15px;
}

/* contenedor interno que rota */
.ind-card__inner {
  position: relative;
  width: 100%;
  height: 100%;
  transform-style: preserve-3d;
  transition: transform 0.6s ease;
}

.ind-card__inner--flipped {
  transform: rotateY(180deg);
}

/* caras */
.ind-card__face {
  position: absolute;
  inset: 0;
  backface-visibility: hidden;
}

/* ======= FRONT ======= */

.ind-card__face--front {
}

.ind-card__bg {
  position: absolute;
  inset: 0;
}

.ind-card__bg img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.ind-card__overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(
    to bottom,
    rgba(0, 0, 0, 0.35),
    rgba(0, 0, 0, 0.85)
  );
}

.ind-card__content {
  position: relative;
  z-index: 1;
  height: 100%;
  padding: 26px 18px 18px;
  margin-top: 30px;

  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: flex-start;

  text-align: center;
}

/* icono verde que sirve de botón (frente y dorso) */
.ind-card__icon {
  width: 76px;
  height: 76px;
  border-radius: 999px;
  border: none;
  outline: none;
  cursor: pointer;

  display: flex;
  align-items: center;
  justify-content: center;

  font-size: 2rem;
  font-weight: 600;
  line-height: 0;

  background: #3ee272;
  color: white;
  box-shadow: 0 16px 30px rgba(0, 0, 0, 0.45);
}

.ind-card__label {
  margin-top: 22px;
  font-size: 2rem;
  color: #ffffff;
  font-weight: 700;
}


/* ======= BACK ======= */

.ind-card__face--back {
  background: #334E60;
  color: #ffffff;
  transform: rotateY(180deg);
  position: relative;
  height: 500px;
}


/* contenido del reverso */
.ind-card__back-content {
  position: relative;
  z-index: 1;
  height: 100%;
  padding: 30px 28px 26px;

  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: flex-start;

  text-align: center;
}

/* icono en el reverso, un poco separado del texto */
.ind-card__icon--back {
  margin-top: 30px;
  margin-bottom: 0;
}

.ind-card__back-title {
  font-size: 2.3rem;
  font-weight: 700;
  margin-bottom: 8px;
}

.ind-card__back-text {
  font-size: 0.9rem;
  line-height: 1.8;
  max-width: 260px;
  opacity: 0.95;
}

/* Responsive */
@media (max-width: 1024px) {
  .ind-grid__inner {
    grid-template-columns: repeat(3, minmax(0, 1fr));
  }
}

@media (max-width: 768px) {
  .ind-grid__inner {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }
}

@media (max-width: 520px) {
  .ind-grid__inner {
    grid-template-columns: 1fr;
  }
}
</style>


<template>
  <section class="ind-grid" data-aos="fade-up">
    <div class="ind-grid__inner">
      <div
        v-for="industry in industries"
        :key="industry.id"
        class="ind-card"
        :class="{ 'ind-card--flipped-active': borderId === industry.id }"
      >
        <div
          class="ind-card__inner"
          :ref="(el) => setCardRef(el, industry.id)"
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
              <p
                class="ind-card__label"
                :class="{ 'ind-card__label--hidden': activeId === industry.id }"
              >
                {{ industry.label }}
              </p>
            </div>
          </div>

      <!-- CARA TRASERA -->
          <div class="ind-card__face ind-card__face--back">
            <!-- Borde blanco siguiendo la forma -->
            <div class="ind-card__border"></div>

            <div class="ind-card__back-content">
              <!-- Icono específico de la industria (también sirve para cerrar al hacer click) -->
              <div
                class="ind-card__icon-wrapper"
                @click.stop="toggleCard(industry.id)"
              >
                <img :src="industry.icon" :alt="industry.label" class="ind-card__icon-img" />
              </div>

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

<script setup lang="ts">
  import { ref } from 'vue'
  import gsap from 'gsap'

  // --- ICONOS DE LAS CARDS ---
  import iconAi from '@/assets/images/industries-cards-icons/ai-it.png'
  import iconAutomotive from '@/assets/images/industries-cards-icons/automotive.png'
  import iconAviation from '@/assets/images/industries-cards-icons/aviation.png'
  import iconConstruction from '@/assets/images/industries-cards-icons/construction.png'
  import iconEngineering from '@/assets/images/industries-cards-icons/engineering.png'
  import iconFinancial from '@/assets/images/industries-cards-icons/financial.png'
  import iconLegal from '@/assets/images/industries-cards-icons/legal.png'
  import iconLogistics from '@/assets/images/industries-cards-icons/logistics.png'
  import iconManufacturing from '@/assets/images/industries-cards-icons/manufacturing.png'
  import iconRetail from '@/assets/images/industries-cards-icons/retail.png'
  import iconTransportation from '@/assets/images/industries-cards-icons/transportation.png'

  type Industry = {
    id: number
    label: string
    image: string
    icon: string
    description: string
  }

  const industries: Industry[] = [
    {
      id: 1,
      label: 'Automotive',
      image: new URL('@/assets/images/ind-automotive.jpg', import.meta.url).href,
      icon: iconAutomotive,
      description:
        'We supply skilled talent for the automotive industry—from technicians to engineers. Our candidates support manufacturing, repair, and sales across all vehicle types.',
    },
    {
      id: 2,
      label: 'Aviation',
      image: new URL('@/assets/images/ind-aviation.jpg', import.meta.url).href,
      icon: iconAviation,
      description:
        'We recruit aviation experts—pilots, technicians, and support staff. Our candidates help airlines and aviation firms maintain safety, compliance, and operational excellence.',
    },
    {
      id: 3,
      label: 'Construction',
      image: new URL('@/assets/images/ind-construction.jpg', import.meta.url).href,
      icon: iconConstruction,
      description:
        'We connect skilled tradespeople and professionals with top-tier construction companies. We provide talent that drives growth and ensures quality from the ground up.',
    },
    {
      id: 4,
      label: 'Engineering',
      image: new URL('@/assets/images/ind-engineering.jpg', import.meta.url).href,
      icon: iconEngineering,
      description:
        'We recruit engineers across multiple disciplines. Our candidates bring innovation, precision, and technical expertise to every project, fueling infrastructure and development.',
    },
    {
      id: 5,
      label: 'IT / AI',
      image: new URL('@/assets/images/ind-it-ai.jpg', import.meta.url).href,
      icon: iconAi,
      description:
        'We source top tech talent in software, data, and AI. Our candidates fuel innovation, boost performance, and deliver smart solutions in fast-evolving digital environments.',
    },
    {
      id: 6,
      label: 'Financial',
      image: new URL('@/assets/images/ind-financial.jpg', import.meta.url).href,
      icon: iconFinancial,
      description:
        'We source skilled finance professionals in banking, accounting, and more. Our candidates bring accuracy, compliance, and insight to fast-paced financial environments.',
    },
    {
      id: 7,
      label: 'Legal / Accounting',
      image: new URL('@/assets/images/ind-legal.jpg', import.meta.url).href,
      icon: iconLegal,
      description:
        'We recruit legal professionals, including assistants, paralegals, and lawyers. Our candidates bring organization, precision, and expertise to every legal team.',
    },
    {
      id: 8,
      label: 'Logistics, 3PL/4PL',
      image: new URL('@/assets/images/ind-logistics.jpg', import.meta.url).href,
      icon: iconLogistics,
      description:
        'We provide logistics and supply chain talent—from warehouse workers to dispatchers. Our candidates help streamline operations and ensure timely, accurate delivery.',
    },
    {
      id: 9,
      label: 'Manufacturing',
      image: new URL('@/assets/images/ind-manufacturing.jpg', import.meta.url).href,
      icon: iconManufacturing,
      description:
        'We support manufacturing with reliable workers—machine operators, assemblers, and supervisors. Our talent helps boost productivity, safety, and product quality.',
    },
    {
      id: 10,
      label: 'Retail',
      image: new URL('@/assets/images/ind-retail.jpg', import.meta.url).href,
      icon: iconRetail,
      description:
        'We provide retail staff at all levels—from sales associates to managers. Our candidates deliver strong customer service and help drive daily operations and sales success.',
    },
    {
      id: 11,
      label: 'Transportation',
      image: new URL('@/assets/images/ind-transportation.jpg', import.meta.url).href,
      icon: iconTransportation,
      description:
        'We recruit drivers, coordinators, and support staff for transport operations. Our candidates keep people and goods moving safely, efficiently, and on schedule.',
    },
  ]

  // id de la card actualmente abierta
  const activeId = ref<number | null>(null)
  // id de la card que debe mostrar el borde (solo cuando está quieta y abierta)
  const borderId = ref<number | null>(null)
  
  // Guardamos refs de los elementos DOM
  const cardRefs = new Map<number, HTMLElement>()
  const setCardRef = (el: any, id: number) => {
    if (el) cardRefs.set(id, el)
  }

  const toggleCard = (id: number): void => {
    // Si cliqueamos la misma que ya está abierta -> cerrar
    if (activeId.value === id) {
      animateCard(id, false)
      activeId.value = null
    } else {
      // Si hay otra abierta, cerrarla primero
      if (activeId.value !== null) {
        animateCard(activeId.value, false)
      }
      // Abrir la nueva
      activeId.value = id
      animateCard(id, true)
    }
  }

  const animateCard = (id: number, open: boolean) => {
    const el = cardRefs.get(id)
    if (!el) return

    gsap.to(el, {
      rotateY: open ? 180 : 0,
      duration: 1.75, // Animación lenta como se pidió
      ease: 'power2.inOut',
      onStart: () => {
        // Al empezar, si cerramos, quitamos el borde inmediatamente
        // Si abrimos, esperamos al final.
        if (!open) {
          if (borderId.value === id) borderId.value = null
        }
        // Aseguramos que el navegador sepa que estamos en 3D
        gsap.set(el, { transformStyle: 'preserve-3d' })
      },
      onComplete: () => {
        // Al terminar, si abrimos, mostramos el borde
        if (open) {
          borderId.value = id
        }
      }
    })
  }
</script>

<style scoped>
.ind-grid {
  background: #0F2F44;
  padding: 40px 0 80px;
}

.ind-grid__inner {
  max-width: 1600px;              /* más ancho */
  margin: 0 auto;
  padding: 0 40px;                /* menos margen lateral */
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 30px;
}

/* ========== CARD CON GIRO 3D ========= */

.ind-card {
  position: relative;
  height: 580px;
  perspective: 1500px; /* Restaurado 3D */
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
  transition: filter 0.3s ease;
}

/* Borde blanco "sólido" usando filtros cuando está girada */
.ind-card--flipped-active {
  /* Truco para borde sólido en formas irregulares (clip-path) */
  filter: 
    drop-shadow(1.5px 0 0 #fff) 
    drop-shadow(-1.5px 0 0 #fff) 
    drop-shadow(0 1.5px 0 #fff) 
    drop-shadow(0 -1.5px 0 #fff);
  z-index: 10; /* Asegurar que se vea por encima de las otras */
}

/* contenedor interno que rota */
.ind-card__inner {
  position: relative;
  width: 100%;
  height: 100%;
  background-color: #0F2F44; /* Fondo oscuro para evitar líneas blancas al girar */
  /* Importante para 3D */
  transform-style: preserve-3d;
  /* GSAP maneja la transición de transform ahora */
}

/* .ind-card__inner--flipped se usa para selectores descendientes */

/* caras */
.ind-card__face {
  position: absolute;
  inset: 0;
  /* Importante: oculta la cara trasera al girar */
  -webkit-backface-visibility: hidden;
  backface-visibility: hidden;
  /* El borde jagged puede arreglarse con esto */
  transform: translateZ(1px); 
}

/* ======= FRONT ======= */

.ind-card__face--front {
  background: #0F2F44; /* Fondo oscuro para evitar bordes blancos */
  z-index: 2; /* Logically on top initially */
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
    rgba(0, 0, 0, 0.05),
    rgba(0, 0, 0, 0.45)
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
  width: 86px;
  height: 86px;
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

/* ===== título frontal oculto al girar ===== */
.ind-card__label--hidden {
  opacity: 0;
  transition: opacity 0.2s ease-out; /* Fade out quick */
}


/* ======= BACK ======= */

/* ======= BACK (Slide Up Animation) ======= */

/* ======= BACK (3D Flip) ======= */

.ind-card__face--back {
  /* Gradiente sólido */
  background: linear-gradient(
    135deg, 
    #334e60 0%, 
    #1b3141 60%, 
    #0f1f2a 100%
  );
  color: #ffffff;
  
  /* Posición inicial para 3D: Rotada 180deg */
  transform: rotateY(180deg);
  position: absolute;
  inset: 0;
  height: 100%;
  z-index: 5;
  
  /* GSAP maneja la animación */
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

/* Wrapper del icono trasero: Reutilizamos estilo de botón verde para el círculo */
.ind-card__icon-wrapper {
  margin-top: 30px;
  margin-bottom: 0;
  width: 86px;
  height: 86px;
  border-radius: 999px;
  
  /* Estilos del círculo verde */
  background: #3ee272;
  box-shadow: 0 16px 30px rgba(0, 0, 0, 0.45);
  
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: transform 0.3s ease;
}

.ind-card__icon-wrapper:hover {
  transform: scale(1.05);
}

.ind-card__icon-img {
  width: 50%; /* Icono más pequeño dentro del círculo */
  height: 50%;
  object-fit: contain;
  filter: brightness(0) invert(1); /* Volver blanco el icono si es negro, o asegurar contraste */
}

/* Animación de textos */
.ind-card__back-title,
.ind-card__back-text {
  opacity: 0;
  transform: translateY(20px);
  transition: opacity 0.5s ease, transform 0.5s ease;
}

.ind-card__back-title {
  font-size: 2rem;
  font-weight: 700;
  margin-top: 20px;     /* Separación del icono */
  margin-bottom: 10px;  /* Separación del párrafo */
  transition-delay: 0.1s; /* Se lanza después de que empieza el giro */
}

.ind-card__back-text {
  font-size: 1.25rem; /* Más grande */
  line-height: 1.5;
  max-width: 85%;    /* Un poco más ancho relativo */
  opacity: 0; /* Base state hidden */
  transition-delay: 0.2s;
}

/* Estado activo (cuando la tarjeta se gira) */
.ind-card__inner--flipped .ind-card__back-title {
  opacity: 1;
  transform: translateY(0);
  transition-delay: 0.6s; /* Espera a que la tarjeta esté casi girada */
}

.ind-card__inner--flipped .ind-card__back-text {
  opacity: 0.95;
  transform: translateY(0);
  transition-delay: 0.7s;
}

/* cuando está girada, mostramos la cara trasera */
/* cuando está activa, deslizamos la cara trasera */
/* cuando está activa, GSAP rota el inner, no necesitamos CSS transform aquí */
/* .ind-card__inner--flipped .ind-card__face--back {} */


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
    padding: 20px 30px; /* Un poco de padding vertical */
    gap: 0;
  }
  
  /* Efecto de encastre vertical (puzzle) */
  .ind-card {
    margin-top: -50px; /* Forzar solapamiento para reducir espacio visual */
  }
  
  .ind-card:first-child {
    margin-top: 0;
  }
}
</style>


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
          :ref="(el) => setCardRef(el, industry.id)"
          :class="{ 'ind-card__inner--flipped': activeId === industry.id }"
        >
          <!-- CARA FRONTAL -->
          <div class="ind-card__face ind-card__face--front">
            <div class="ind-card__bg">
              <img
                :src="industry.image"
                :alt="industry.label"
                class="ind-card__img--gray"
              />
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
    <!-- CLIP PATH SVG DEFINITION (Hidden) -->
    <svg width="0" height="0" style="position: absolute; pointer-events: none;">
      <defs>
        <clipPath id="ind-card-clip" clipPathUnits="objectBoundingBox">
         <path d="M 0 0.2
                  L 0 0.95
                  Q 0 1 0.05 0.985
                  L 0.45 0.865
                  Q 0.5 0.85 0.55 0.865
                  L 0.95 0.985
                  Q 1 1 1 0.95
                  L 1 0.2
                  Q 1 0.15 0.95 0.135
                  L 0.55 0.015
                  Q 0.5 0 0.45 0.015
                  L 0.05 0.135
                  Q 0 0.15 0 0.2
                  Z" />
        </clipPath>
      </defs>
    </svg>
  </section>
</template>

<script setup lang="ts">
  import { ref } from 'vue'
  import gsap from 'gsap'
  import industriesData from '../../assets/json/IndustriesData.json'

  type Industry = {
    id: number
    label: string
    image: string
    icon: string
    description: string
  }

  const imageModules = import.meta.glob('@/assets/images/ind-*.png', { eager: true, import: 'default' }) as Record<string, string>
  const iconModules = import.meta.glob('@/assets/images/industries-cards-icons/*.png', { eager: true, import: 'default' }) as Record<string, string>

  const resolveImage = (filename: string): string => {
    const key = Object.keys(imageModules).find(k => k.endsWith(`/${filename}`))
    return key ? imageModules[key] : ''
  }

  const resolveIcon = (filename: string): string => {
    const key = Object.keys(iconModules).find(k => k.endsWith(`/${filename}`))
    return key ? iconModules[key] : ''
  }

  const industries: Industry[] = industriesData.map(item => ({
    ...item,
    image: resolveImage(item.image),
    icon: resolveIcon(item.icon),
  }))

  // id de la card actualmente abierta
  const activeId = ref<number | null>(null)
  
  // Guardamos refs de los elementos DOM
  const cardRefs = new Map<number, HTMLElement>()
  const setCardRef = (el: any, id: number) => {
    if (el) cardRefs.set(id, el)
  }

  const toggleCard = (id: number): void => {
    // Si cliqueamos la misma que ya está abierta -> cerrar
    if (activeId.value === id) {
      animateFlip(id, false)
      activeId.value = null
    } else {
      // Si hay otra abierta, cerrarla primero
      if (activeId.value !== null) {
        animateFlip(activeId.value, false)
      }
      // Abrir la nueva
      activeId.value = id
      animateFlip(id, true)
    }
  }

  const animateFlip = (id: number, open: boolean) => {
    const el = cardRefs.get(id)
    if (!el) return

    gsap.to(el, {
      rotateY: open ? 180 : 0,
      duration: 0.5, 
      ease: 'sine.inOut', /* Animación más fluida y constante */
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
  perspective: 2000px; 
  overflow: hidden; 

  /* tu forma tipo flecha suave (SVG clip) */
  clip-path: url(#ind-card-clip); /* Referencia al SVG definido abajo */

  border-radius: 15px;
  transition: filter 0.3s ease;
  /* Fix para Safari: Evita parpadeos en bordes */
  -webkit-transform: translate3d(0,0,0);
  transform: translate3d(0,0,0);
}

/* Borde blanco "sólido" usando filtros cuando está girada */
.ind-card--flipped-active {
  /* Truco para borde sólido en formas irregulares (clip-path) */
  filter: 
    drop-shadow(1.5px 0 0 #fff) 
    drop-shadow(-1.5px 0 0 #fff) 
    drop-shadow(0 1.5px 0 #fff) 
    drop-shadow(0 -1.5px 0 #fff);
  z-index: 100; /* Asegurar que se vea por encima */
}

/* contenedor interno que rota */
.ind-card__inner {
  position: relative;
  width: 100%;
  height: 100%;
  background-color: #0F2F44; 
  transition: transform 1.2s cubic-bezier(0.4, 0.0, 0.2, 1); /* Fallback CSS si GSAP falla */
  transform-style: preserve-3d;
  -webkit-transform-style: preserve-3d;
}

/* caras */
.ind-card__face {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  -webkit-backface-visibility: hidden;
  backface-visibility: hidden;
  /* Fix para "white lines": Un pixel de borde transparente o background color sólido */
  background-color: #0F2F44; 
  /* Fix Z-fighting */
  transform: translateZ(1px); 
}

/* ======= FRONT ======= */

.ind-card__face--front {
  background: #0F2F44; 
  z-index: 2;
  transform: rotateY(0deg);
}
/* No ocultamos front con opacidad, el backface-visibility lo hará */
.ind-card__inner--flipped .ind-card__face--front {
    pointer-events: none;
}
.ind-card__inner--flipped .ind-card__face--back {
    pointer-events: auto;
}

.ind-card__bg {
  position: absolute;
  inset: 0;
  border-radius: 15px; /* asegurar border radius */
  overflow: hidden;
}

.ind-card__bg img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.ind-card__img--gray {
  filter: grayscale(100%);
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
  /* Fix para que el contenido no parpadee en 3D */
  transform: translateZ(30px);
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

.ind-card__face--back {
  /* Color sólido único solicitado */
  background: #334E60;
  color: #ffffff;
  
  /* Posición inicial 3D: Rotada 180deg */
  transform: rotateY(180deg);
  position: absolute;
  inset: 0;
  height: 100%;
  z-index: 1;
  /* Restauramos visibilidad normal, oculta por backface */
  opacity: 1; 
  pointer-events: auto; 
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
    grid-template-columns: repeat(2, minmax(0, 1fr));
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


<template>
  <form class="contact-form">
    <header class="contact-form__header">
      <h2 class="contact-form__title">Your Information</h2>
      <p class="contact-form__subtitle">
        We’ll reach out to you for the next steps
      </p>
    </header>

    <div class="contact-form__group">
      <div class="contact-form__field">
        <label class="contact-form__label" for="firstName">Name</label>
        <input
          id="firstName"
          type="text"
          class="contact-form__input"
          placeholder="First Name"
        />
      </div>
    </div>

    <div class="contact-form__group">
      <label class="contact-form__label" for="email">Email</label>
      <input
        id="email"
        type="email"
        class="contact-form__input"
        placeholder="Your Email Address"
      />
    </div>

    <div class="contact-form__group">
      <label class="contact-form__label" for="phone">Phone</label>
      <input
        ref="phoneInput"
        id="phone"
        type="text"
        class="contact-form__input"
        placeholder="300 123-4567"
      />
    </div>

    <div class="contact-form__group">
      <label class="contact-form__label" for="message">Message</label>
      <textarea
        id="message"
        class="contact-form__input contact-form__textarea"
        rows="4"
        placeholder="Tell us more about the role you need to fill"
      ></textarea>
    </div>

    <button type="submit" class="contact-form__submit">
      Save &amp; Send
    </button>

    <button type="button" class="contact-form__reset">
      Reset Information
    </button>
  </form>
</template>

<script setup lang="ts">
  import { onMounted, ref } from 'vue';
  // CAMBIO 2: Importamos Cleave
  import Cleave from 'cleave.js';
  // Si necesitas importar estilos específicos de la región (opcional en este caso manual)
  // import 'cleave.js/dist/addons/cleave-phone.us';

  // Referencia al elemento del DOM
  const phoneInput = ref<HTMLElement | null>(null);

  onMounted(() => {
    // CAMBIO 3: Inicializamos la máscara cuando el componente carga
    if (phoneInput.value) {
      new Cleave(phoneInput.value, {
        blocks: [3, 3, 4],       // Los bloques de tu imagen
        delimiters: [' ', '-'],  // Los delimitadores de tu imagen
        numericOnly: true        // Solo números
      });
    }
  });
  </script>


<style scoped>
* {
  box-sizing: border-box;
}

/* Contenedor del formulario */
.contact-form {
  width: 100%;
}

/* Encabezado */
.contact-form__header {
  text-align: center;
  margin-bottom: 28px;
}

.contact-form__title {
  font-size: 1.6rem;
  font-weight: 700;
  color: #222222;
  margin-bottom: 4px;
}

.contact-form__subtitle {
  font-size: 0.9rem;
  color: #a0a0a0;
}

/* Grupos */
.contact-form__group {
  margin-bottom: 18px;
}

/* Estilo para centrar el Recaptcha si lo deseas */
.contact-form__recaptcha {
  display: flex;
  flex-direction: column;
  align-items: center; /* Centrado horizontalmente */
  justify-content: center;
  margin-top: 10px;
}

.error-text {
  color: #e74c3c;
  font-size: 0.8rem;
  margin-top: 5px;
}

/* Labels */
.contact-form__label {
  display: block;
  font-size: 0.9rem;
  font-weight: 600;
  color: #222222;
  margin-bottom: 6px;
}

/* Inputs / selects / textarea */
.contact-form__input {
  width: 100%;
  padding: 10px 16px;
  border-radius: 999px;
  border: 1px solid #ececec;
  background-color: #fafafa;
  font-size: 0.9rem;
  outline: none;
  transition: border-color 0.18s ease, box-shadow 0.18s ease,
    background-color 0.18s ease;
  color: #333333;
}

.contact-form__input::placeholder {
  color: #c3c3c3;
}

/* textareas */
.contact-form__textarea {
  border-radius: 18px;
  resize: vertical;
  min-height: 120px;
  padding-top: 12px;
}

/* focus */
.contact-form__input:focus,
.contact-form__textarea:focus {
  border-color: #32d26a;
  background-color: #ffffff;
  box-shadow: 0 0 0 2px rgba(50, 210, 106, 0.12);
}

/* Botón principal */
.contact-form__submit {
  width: 100%;
  margin-top: 8px;
  padding: 12px 20px;
  border-radius: 999px;
  border: none;
  outline: none;
  background-color: #32d26a;
  color: #ffffff;
  font-weight: 700;
  font-size: 0.95rem;
  cursor: pointer;
  box-shadow: 0 14px 30px rgba(0, 0, 0, 0.25);
  transition: background-color 0.18s ease, transform 0.18s ease,
    box-shadow 0.18s ease;
}

.contact-form__submit:hover {
  background-color: #27b058;
  transform: translateY(-1px);
  box-shadow: 0 18px 36px rgba(0, 0, 0, 0.3);
}

/* Reset */
.contact-form__reset {
  display: block;
  margin: 14px auto 0;
  border: none;
  background: none;
  color: #333333;
  font-size: 0.85rem;
  cursor: pointer;
  text-decoration: none;
}

.contact-form__reset:hover {
  text-decoration: underline;
}

@media (max-width: 480px) {
  .contact-form__title {
    font-size: 1.4rem;
  }
}
</style>

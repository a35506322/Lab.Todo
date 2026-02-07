import './assets/main.css';
import './assets/layout.scss';

import { createApp } from 'vue';
import { createPinia } from 'pinia';
import PrimeVue from 'primevue/config';
import Material from '@primeuix/themes/material';
import { Form } from '@primevue/forms';
import { definePreset, palette } from '@primeuix/themes';
import ToastService from 'primevue/toastservice';
import ConfirmationService from 'primevue/confirmationservice';
import App from './App.vue';
import router from './router';
import { z } from 'zod';

z.config(z.locales.zhTW());

const app = createApp(App);

const customPrimaryPalette = palette('#8b5cf6');

const CustomPreset = definePreset(Material, {
  semantic: {
    primary: {
      50: customPrimaryPalette[50],
      100: customPrimaryPalette[100],
      200: customPrimaryPalette[200],
      300: customPrimaryPalette[300],
      400: customPrimaryPalette[400],
      500: customPrimaryPalette[500],
      600: customPrimaryPalette[600],
      700: customPrimaryPalette[700],
      800: customPrimaryPalette[800],
      900: customPrimaryPalette[900],
      950: customPrimaryPalette[950],
    },
  },
  components: {
    datatable: {
      header: {
        background: '{primary.500}',
        color: '{primary.contrast.color}',
      },
      headerCell: {
        background: '{primary.500}',
        color: '{primary.contrast.color}',
        hoverBackground: '{primary.600}',
      },
    },
  },
});

app.use(createPinia());
app.use(router);
app.use(PrimeVue, {
  theme: {
    preset: CustomPreset,
    options: {
      darkModeSelector: false, // 停用 dark mode，永遠 light mode
      cssLayer: {
        name: 'primevue',
        order: 'theme, base, primevue',
      },
    },
  },
});
app.use(ToastService);
app.use(ConfirmationService);

app.component('Form', Form);

app.mount('#app');

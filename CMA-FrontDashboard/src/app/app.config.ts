import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideAnimations } from '@angular/platform-browser/animations';
import Lara  from '@primeuix/themes/aura';
import { providePrimeNG } from 'primeng/config';
import Aura from '@primeuix/themes/aura';


export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideAnimations(),
    providePrimeNG(
      {
      theme: {
      preset: Aura ,
      options: {
        darkmode: false,
        darkModeSelector: false
      }

      }
    }
  )
    ,
  ]
};

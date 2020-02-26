import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';

// Habilitar Angular routing
import { RouterModule } from '@angular/router';
import { appRoutes } from './routes';

// Formularios y Cliente Http
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

// Evitar errores con NGX bootstrap
import { HammerGestureConfig, HAMMER_GESTURE_CONFIG } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

export class CustomHammerConfig extends HammerGestureConfig  {
  overrides = {
      pinch: { enable: false },
      rotate: { enable: false }
  };
}

@NgModule({
   declarations: [
      AppComponent,
      NavComponent
   ],
   imports: [
      BrowserModule,
      
      // Evitar errores del bootstrap
      BrowserAnimationsModule,

      // Routing, Http y formularios
      FormsModule,
      RouterModule.forRoot(appRoutes),
      HttpClientModule
   ],
   providers: [
      // Evitar errores del bootstrap
      { provide: HAMMER_GESTURE_CONFIG, useClass: CustomHammerConfig }
   ],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }

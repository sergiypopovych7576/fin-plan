import {
	APP_INITIALIZER,
	ApplicationConfig,
	provideExperimentalZonelessChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';

import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { apiUrlInterceptor, initializeConfig, routes } from './configuration';
import { ConfigurationService } from '@fp-core/services';

export const appConfig: ApplicationConfig = {
	providers: [
		provideExperimentalZonelessChangeDetection(),
		provideRouter(routes),
		provideAnimationsAsync(),
		provideHttpClient(withInterceptors([apiUrlInterceptor])),
		{
			provide: APP_INITIALIZER,
			useFactory: initializeConfig,
			deps: [ConfigurationService],
			multi: true,
		},
	],
};

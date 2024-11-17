import {
	APP_INITIALIZER,
	ApplicationConfig,
	provideExperimentalZonelessChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';

import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { apiUrlInterceptor, appFactoryInitalization, routes } from './configuration';
import { ConfigurationService, OperationsService } from '@fp-core/services';
import { provideMomentDateAdapter } from '@angular/material-moment-adapter';

export const appConfig: ApplicationConfig = {
	providers: [
		provideExperimentalZonelessChangeDetection(),
		provideRouter(routes),
		provideAnimationsAsync(),
		provideMomentDateAdapter(),
		provideHttpClient(withInterceptors([apiUrlInterceptor])),
		{
			provide: APP_INITIALIZER,
			useFactory: appFactoryInitalization,
			deps: [ConfigurationService],
			multi: true,
		},
	],
};

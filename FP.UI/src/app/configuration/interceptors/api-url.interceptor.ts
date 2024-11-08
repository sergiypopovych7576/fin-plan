import { HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { ConfigurationService } from '@fp-core/services';

export function apiUrlInterceptor(
	req: HttpRequest<unknown>,
	next: HttpHandlerFn,
) {
	if (req.url.includes('assets')) {
		return next(req);
	}
	const configurationService = inject(ConfigurationService);
	const apiReq = req.clone({
		url: `${configurationService.config?.apiUrl}${req.url}`,
	});
	return next(apiReq);
}

import { HttpClient } from '@angular/common/http';
import { inject } from '@angular/core';

export class BaseService {
	protected readonly _httpClient = inject(HttpClient);
}

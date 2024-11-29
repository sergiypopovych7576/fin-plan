import { inject, Injectable } from '@angular/core';
import { IAppConfiguration } from '../models';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class ConfigurationService {
	protected readonly _httpClient = inject(HttpClient);
	public config?: IAppConfiguration;

	public loadConfig(): Promise<void> {
		return new Promise((res) => {
			this._httpClient
				.get<IAppConfiguration>('/assets/config.json')
				.subscribe((config: IAppConfiguration) => {
					this.config = config;
					res();
				});
		});
	}
}

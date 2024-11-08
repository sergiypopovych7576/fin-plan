import { Injectable } from '@angular/core';
import { IAppConfiguration } from '../models';
import { BaseService } from './base.service';

@Injectable({ providedIn: 'root' })
export class ConfigurationService extends BaseService {
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

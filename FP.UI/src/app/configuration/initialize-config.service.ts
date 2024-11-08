import { ConfigurationService } from '@fp-core/services';

export function initializeConfig(
	configService: ConfigurationService,
): () => Promise<void> {
	return () => configService.loadConfig();
}

import { ConfigurationService } from '@fp-core/services';

export function configInitialization(
	configService: ConfigurationService,
): () => Promise<void> {
	return () => configService.loadConfig();
};

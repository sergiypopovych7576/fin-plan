import { ConfigurationService } from '@fp-core/services';
import { Chart, registerables } from 'chart.js';
import { configInitialization } from './config-initalization.function';

export function appFactoryInitalization(
    configService: ConfigurationService,
): () => Promise<void> {
    Chart.register(...registerables);
    return configInitialization(configService);
}

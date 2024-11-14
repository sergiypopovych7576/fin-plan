import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
    selector: 'fp-metric',
    templateUrl: './metric.component.html',
    styleUrl: './metric.component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class MetricComponent {
}

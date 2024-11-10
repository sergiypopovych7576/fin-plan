import { ChangeDetectionStrategy, Component, HostBinding, Input } from '@angular/core';

@Component({
    selector: 'fp-indicator',
    templateUrl: './indicator.component.html',
    styleUrls: ['./indicator.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class IndicatorComponent {
    @Input()
    public color?: string;

    @Input()
    public size = 16;

    @HostBinding('style.height.px') get hostHeight() {
        return this.size;
    }

    @HostBinding('style.width.px') get hostWidth() {
        return this.size;
    }
}

import { Component, Input } from '@angular/core';

@Component({
	selector: 'fp-indicator',
	templateUrl: './indicator.component.html',
})
export class IndicatorComponent {
    @Input()
    public color?: string;

    @Input()
    public size = 12;
}

import { Component, Input, ViewEncapsulation } from '@angular/core';
import { ProgressBarMode } from '@angular/material/progress-bar';

@Component({
    selector: 'fp-progress-bar',
    templateUrl: './progress-bar.component.html',
    styleUrl: './progress-bar.component.scss',
    encapsulation: ViewEncapsulation.None
})
export class ProgressBar {
    @Input()
    public color = '#3366FF';

    @Input()
    public mode: ProgressBarMode = 'indeterminate';

    @Input()
    public value: string = '50';
}

import { AfterViewInit, ChangeDetectionStrategy, Component, ElementRef, Input, ViewChild } from '@angular/core';
import { Chart, ChartConfiguration } from 'chart.js';

@Component({
    selector: 'fp-chart',
    styleUrls: ['./chart.component.scss'],
    templateUrl: './chart.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class ChartComponent implements AfterViewInit {
    private _config: unknown;
    private _rendered = false;

    @ViewChild('canvas')
    private _canvas!: ElementRef<HTMLCanvasElement>;

    public chart!: Chart;

    @Input()
    public title = '';

    @Input()
    public set config(config: any) {
        this._config = config;
        this._rendered = false;
        this.renderChart();
    }

    public get config() {
        return this._config;
    }

    public renderChart(): void {
        if (this.config && this._canvas?.nativeElement && !this._rendered) {
            if (!this.chart) {
                this.chart = new Chart(this._canvas.nativeElement, this.config as ChartConfiguration);
            } else {
                this.chart.data = this.config['data'];
                this.chart.update();
            }
            this._rendered = true;
        }
    }

    public ngAfterViewInit(): void {
        this.renderChart();
    }
}

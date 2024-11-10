import { AfterViewInit, Component, ElementRef, Input, ViewChild } from '@angular/core';
import { Chart, ChartConfiguration } from 'chart.js';

@Component({
    selector: 'fp-chart',
    templateUrl: './chart.component.html',
})
export class ChartComponent implements AfterViewInit {
    private _config: unknown;
    private _rendered = false;

    @ViewChild('canvas')
    private _canvas!: ElementRef<HTMLCanvasElement>;

    @Input()
    public title = '';

    @Input()
    public set config(config: unknown) {
        this._config = config;
        this._rendered = false;
        this.renderChart();
    }

    public get config() {
        return this._config;
    }

    public chart!: Chart;

    public renderChart(): void {
        if (this.config && this._canvas?.nativeElement && !this._rendered) {
            this.chart = new Chart(this._canvas.nativeElement, this.config as ChartConfiguration);
            this._rendered = true;
        }
    }

    public ngAfterViewInit(): void {
        this.renderChart();
    }
}

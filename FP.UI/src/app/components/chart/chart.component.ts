import { AfterViewInit, ChangeDetectionStrategy, Component, ElementRef, Input, signal, ViewChild } from '@angular/core';
import { Chart, ChartConfiguration } from 'chart.js';

@Component({
    selector: 'fp-chart',
    styleUrls: ['./chart.component.scss'],
    templateUrl: './chart.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class ChartComponent implements AfterViewInit {
    @ViewChild('canvas')
    private _canvas!: ElementRef<HTMLCanvasElement>;

    private _config: any;
    public chart!: Chart;
    public hasData = signal(false);
    public loaded = signal(false);

    @Input()
    public set config(config: any) {
        this._config = config;
        this.renderChart(config);
    }

    public ngAfterViewInit(): void {
        this.renderChart(this._config);
    }

    public renderChart(config: any): void {
        if (config && this._canvas?.nativeElement) {
            this.hasData.set(config.data.datasets.length && config.data.datasets[0].data.length);
            this.loaded.set(true);
            if (this.hasData()) {
                if (!this.chart) {
                    this.chart = new Chart(this._canvas.nativeElement, config as ChartConfiguration);
                } else {
                    this.chart.data = config['data'];
                    this.chart.update();
                }
            }
        }
    }
}

import { AfterViewInit, ChangeDetectionStrategy, Component, ElementRef, Input, OnDestroy, signal, ViewChild } from '@angular/core';
import { Chart, ChartConfiguration } from 'chart.js';

@Component({
  selector: 'fp-chart',
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ChartComponent implements AfterViewInit, OnDestroy {
  @ViewChild('canvas') private _canvas!: ElementRef<HTMLCanvasElement>;

  private _config: ChartConfiguration | null = null;
  private chartInstance: Chart | null = null;

  public hasData = signal(false);
  public loaded = signal(false);

  @Input()
  public set config(config: ChartConfiguration | null) {
    this._config = config;
    if (this._canvas?.nativeElement) {
      this.renderChart();
    }
  }

  public ngAfterViewInit(): void {
    if (this._config) {
      this.renderChart();
    }
  }

  public ngOnDestroy(): void {
    this.destroyChart();
  }

  private renderChart(): void {
    if (!this._config || !this._canvas?.nativeElement) {
        this.loaded.set(false); // Ensure the spinner is displayed if configuration or canvas is missing
        return;
    }

    // Check if there is valid data in the datasets
    const hasDatasets = this._config.data?.datasets?.length ?? 0;
    const hasDataPoints = hasDatasets > 0 ? (this._config.data.datasets[0].data?.length ?? 0) : 0;

    this.hasData.set(hasDataPoints > 0);    
    this.loaded.set(true); // Mark as loaded after determining hasData

    // Handle chart initialization or updates
    if (this.hasData()) {
        if (!this.chartInstance) {
            // Create a new chart instance
            this.chartInstance = new Chart(this._canvas.nativeElement, this._config);
        } else {
            // Update the existing chart instance
            this.chartInstance.data = this._config.data!;
            this.chartInstance.options = this._config.options ?? this.chartInstance.options;
            this.chartInstance.update();
        }
    } else {
        // If there's no data, destroy any existing chart
        this.destroyChart();
    }
}


  private destroyChart(): void {
    if (this.chartInstance) {
      this.chartInstance.destroy();
      this.chartInstance = null;
    }
  }
}

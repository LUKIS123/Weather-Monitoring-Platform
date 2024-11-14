import { CommonModule } from '@angular/common';
import { Component, computed, inject, input, Signal } from '@angular/core';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ApexOptions, NgApexchartsModule } from 'ng-apexcharts';
import { ThemeService } from '../../../features/menu/services/theme.service';

@Component({
  selector: 'app-daily-two-series-chart',
  standalone: true,
  imports: [CommonModule, TranslateModule, NgApexchartsModule],
  templateUrl: './daily-two-series-chart.component.html',
})
export class DailyTwoSeriesChartComponent {
  private readonly themeService = inject(ThemeService);
  private readonly translateService = inject(TranslateService);

  private readonly textColor: Signal<string> = computed(() =>
    this.themeService.darkTheme() ? 'white' : 'black'
  );

  chartDataFirstSeries = input.required<[number, number | null][]>();
  chartDataSecondSeries = input.required<[number, number | null][]>();
  firstSeriesName = input.required<string>();
  secondSeriesName = input.required<string>();
  dataDisplayName = input.required<string>();
  start = input.required<number>();
  end = input.required<number>();
  unit = input.required<string>();
  chartName = input.required<string>();
  colors = input.required<string[]>();

  dailyChartOptions: Signal<Partial<ApexOptions>> = computed(() => {
    return {
      series: [
        {
          name: `${this.firstSeriesName()} [${this.unit()}]`,
          data: this.chartDataFirstSeries(),
        },
        {
          name: `${this.secondSeriesName()} [${this.unit()}]`,
          data: this.chartDataSecondSeries(),
        },
      ],
      chart: {
        id: `daily-chart-${this.dataDisplayName()}-${this.chartName()}`,
        group: `daily-chart-group-${this.dataDisplayName()}-${this.chartName()}-${+Math.random().toString(
          36
        )}`,
        type: 'area',
        height: 320,
        zoom: {
          enabled: true,
        },
        toolbar: {
          show: true,
          tools: {
            zoom: true,
            zoomin: true,
            zoomout: true,
            pan: true,
            reset: true,
            download: true,
            selection: true,
          },
        },
      },
      colors: this.colors(),
      xaxis: {
        type: 'datetime',
        labels: {
          format: 'dd.MM',
          style: {
            colors: this.textColor(),
          },
          datetimeUTC: false,
        },
        min: this.start(),
        max: this.end(),
      },
      yaxis: {
        labels: {
          style: {
            colors: this.textColor(),
          },
        },
      },
      legend: {
        show: true,
        showForSingleSeries: true,
        labels: {
          colors: this.textColor(),
        },
        onItemClick: {
          toggleDataSeries: false,
        },
      },
    };
  });

  commonOptions: Partial<ApexOptions> = {
    dataLabels: {
      enabled: false,
    },
    stroke: {
      curve: 'straight',
    },
    markers: {
      size: 5,
      hover: {
        size: 8,
      },
    },
    tooltip: {
      followCursor: false,
      theme: 'dark',
      x: {
        show: true,
        formatter: (value) => {
          const date = new Date(value);
          const day = ('0' + date.getDate()).slice(-2);
          const month = ('0' + (date.getMonth() + 1)).slice(-2);
          const hours = ('0' + date.getHours()).slice(-2);
          const minutes = ('0' + date.getMinutes()).slice(-2);
          return `${day}.${month} : ${hours}:${minutes}`;
        },
      },
      y: {
        formatter: (value) => {
          if (value === null) {
            return '';
          }
          return `${value.toFixed(2)} ${this.unit()}`;
        },
      },
      marker: {
        show: false,
      },
    },
    noData: {
      text: this.translateService.instant('DataVisualisation.NoData'),
      align: 'center',
      verticalAlign: 'middle',
    },
  };
}

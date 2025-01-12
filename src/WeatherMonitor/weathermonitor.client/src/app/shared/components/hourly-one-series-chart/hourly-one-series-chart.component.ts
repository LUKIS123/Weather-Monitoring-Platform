import { Component, computed, inject, input, Signal } from '@angular/core';
import { ApexOptions, NgApexchartsModule } from 'ng-apexcharts';
import { ThemeService } from '../../../features/menu/services/theme.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-hourly-one-series-chart',
  standalone: true,
  imports: [CommonModule, TranslateModule, NgApexchartsModule],
  templateUrl: './hourly-one-series-chart.component.html',
})
export class HourlyOneSeriesChartComponent {
  private readonly themeService = inject(ThemeService);
  private readonly translateService = inject(TranslateService);

  private readonly textColor: Signal<string> = computed(() =>
    this.themeService.darkTheme() ? 'white' : 'black'
  );

  chartDisplayData = input.required<[number, number | null][]>();
  start = input.required<number>();
  end = input.required<number>();
  seriesName = input.required<string>();
  unit = input.required<string>();
  chartName = input.required<string>();
  color = input.required<string>();

  dailyChartOptions: Signal<Partial<ApexOptions>> = computed(() => {
    const chartData = this.chartDisplayData();
    const allYValuesNull = chartData.every((point) => point[1] === null);
    return {
      series: [
        {
          name: `${this.seriesName()} [${this.unit()}]`,
          data: allYValuesNull ? [] : chartData,
        },
      ],
      chart: {
        id: `daily-chart-${this.seriesName()}-${this.chartName()}`,
        group: `daily-chart-group-${this.seriesName()}-${this.chartName()}-${+Math.random().toString(
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
        background: this.themeService.darkTheme() ? '#424242' : '#fff',
      },
      colors: [this.color()],
      xaxis: {
        type: 'datetime',
        labels: {
          format: 'dd.MM HH:mm',
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
      style: {
        color: this.textColor(),
        fontSize: '16px',
      },
    },
  };
}

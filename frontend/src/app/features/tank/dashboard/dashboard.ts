import { Component, ViewChild, OnInit, NgZone, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule, MatTable, MatTableDataSource } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatTabsModule } from '@angular/material/tabs';
import { MatToolbarModule } from '@angular/material/toolbar';
import { BaseChartDirective } from 'ng2-charts';
import type { ChartConfiguration, ChartOptions } from 'chart.js';
import { registerables, Chart } from 'chart.js';
import { TelemetrySignalRService } from '../../../services/telemetry-signalr.service';
import type { Telemetry } from '../../../models/telemetry';
import type { TelemetryZone } from '../../../models/telemetryzone';

Chart.register(...registerables);

type Zone = 'green' | 'yellow' | 'red';
function computeZone(value: number, greenMax: number, yellowMax: number): Zone {
  if (value <= greenMax) return 'green';
  if (value <= yellowMax) return 'yellow';
  return 'red';
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatSelectModule,
    MatButtonModule,
    MatTabsModule,
    MatToolbarModule,
    BaseChartDirective,
  ],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.scss'],
})
export class Dashboard implements OnInit {
  displayedColumns = ['tankName', 'timestamp', 'level', 'temperature', 'radiation', 'pressure'];
  dataSource = new MatTableDataSource<TelemetryZone>([]);
  tankNames: string[] = ['Tank Alpha', 'Tank Bravo', 'Tank Charlie']; // Pre-populate to avoid filtering issues
  selectedTank = 'all';

  @ViewChild('table') table!: MatTable<TelemetryZone>;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(BaseChartDirective) chartDirective!: BaseChartDirective;

  public levelChartData: ChartConfiguration<'line'>['data'] = { labels: [], datasets: [{ data: [], label: 'Level' }] };
  public tempChartData: ChartConfiguration<'line'>['data'] = { labels: [], datasets: [{ data: [], label: 'Temp (°C)' }] };
  public radChartData: ChartConfiguration<'line'>['data'] = { labels: [], datasets: [{ data: [], label: 'Radiation (mSv)' }] };
  public presChartData: ChartConfiguration<'line'>['data'] = { labels: [], datasets: [{ data: [], label: 'Pressure (psi)' }] };

  public chartOptions: ChartOptions<'line'> = {
    responsive: true,
    animation: false,
    plugins: { legend: { display: true }, tooltip: { mode: 'index', intersect: false } },
    scales: { x: { title: { display: true, text: 'Time (UTC)' } }, y: { beginAtZero: true } }
  };

  constructor(
    private http: HttpClient,
    private signalRService: TelemetrySignalRService,
    private ngZone: NgZone,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    console.log('🧪 Dashboard Initialized');
    this.fetchData();
    this.signalRService.telemetry$.subscribe({
      next: (data: Telemetry) => {
        console.log('🔔 Received Telemetry:', JSON.stringify(data, null, 2));
        if (!data || !data.tankName) {
          console.warn('⚠️ Invalid Telemetry Data:', data);
          return;
        }
        this.ngZone.run(() => {
          const newRow: TelemetryZone = {
            ...data,
            tankName: data.tankName || 'Unknown',
            levelZone: computeZone(data.level, 60, 80),
            temperatureZone: computeZone(data.temperature, 50, 70),
            radiationZone: computeZone(data.radiation, 1, 3),
            pressureZone: computeZone(data.pressure, 6, 8),
          };
          console.log('🔍 Processed Row:', JSON.stringify(newRow, null, 2));

          if (this.selectedTank !== 'all' && newRow.tankName !== this.selectedTank) {
            console.log(`⏭️ Skipping Telemetry: selectedTank=${this.selectedTank}, tankName=${newRow.tankName}`);
            return;
          }

          const updated = [newRow, ...this.dataSource.data].slice(0, 100);
          this.dataSource.data = updated;
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;

          this.table.renderRows();
          this.updateCharts(updated);
          this.cdr.detectChanges();

          if (!this.tankNames.includes(newRow.tankName)) {
            console.log('➕ Adding Tank Name:', newRow.tankName);
            this.tankNames.push(newRow.tankName);
            this.tankNames.sort();
          }

          console.log('📊 Charts Updated with', updated.length, 'rows');
        });
      },
      error: (err) => console.error('❌ Telemetry Subscription Error:', err),
      complete: () => console.log('🏁 Telemetry Subscription Completed')
    });
  }

  fetchData() {
    console.log('📡 Fetching Initial Data');
    this.http.get<Telemetry[]>('/api/telemetry/latest').subscribe({
      next: raw => {
        console.log('📥 Initial Data Loaded:', raw.length, 'rows');
        const rows = raw.map(r => ({
          ...r,
          tankName: r.tankName || 'Unknown',
          levelZone: computeZone(r.level, 60, 80),
          temperatureZone: computeZone(r.temperature, 50, 70),
          radiationZone: computeZone(r.radiation, 1, 3),
          pressureZone: computeZone(r.pressure, 6, 8),
        }));
        this.tankNames = Array.from(new Set(rows.map(r => r.tankName)));
        this.dataSource.data = this.selectedTank === 'all'
          ? rows
          : rows.filter(r => r.tankName === this.selectedTank);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.updateCharts(this.dataSource.data);
        console.log('🛠️ Table Rows Rendered:', this.dataSource.data.length);
        this.cdr.detectChanges();
      },
      error: err => console.error('❌ Fetch Error:', err)
    });
  }

  updateCharts(rows: TelemetryZone[]) {
    console.log('📈 Updating Charts');
    const filtered = this.selectedTank === 'all'
      ? rows
      : rows.filter(r => r.tankName === this.selectedTank);
    const labels = filtered.map(r => new Date(r.timestamp).toLocaleTimeString());

    this.levelChartData.labels = labels;
    this.levelChartData.datasets[0].data = filtered.map(r => r.level);

    this.tempChartData.labels = labels;
    this.tempChartData.datasets[0].data = filtered.map(r => r.temperature);

    this.radChartData.labels = labels;
    this.radChartData.datasets[0].data = filtered.map(r => r.radiation);

    this.presChartData.labels = labels;
    this.presChartData.datasets[0].data = filtered.map(r => r.pressure);

    this.chartDirective?.update();
    console.log('✅ Charts Updated');
  }

  filterByTank() {
    console.log('🎚️ Filtering by Tank:', this.selectedTank);
    this.fetchData();
  }

  getZoneClass(zone: Zone) {
    return `cell-${zone}`;
  }

  onTabChange(event: any) {
    console.log('🔁 Tab Changed:', event);
  }
}
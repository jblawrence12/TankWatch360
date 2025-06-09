import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./layout/shell/shell').then(m => m.ShellComponent),
    children: [
      {
        path: 'tank',
        loadComponent: () =>
          import('./features/tank/dashboard/dashboard').then(m => m.Dashboard)
      },
      { path: '', redirectTo: 'tank', pathMatch: 'full' },
      { path: '**', redirectTo: 'tank' }
    ]
  }
];

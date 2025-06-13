import { Routes } from '@angular/router';


export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./layout/shell/shell').then(m => m.ShellComponent),
    children: [
      {                                   // EXISTING dashboard
        path: 'tank',
        loadComponent: () =>
          import('./features/tank/dashboard/dashboard').then(m => m.Dashboard)
      },

      {                                   // NEW – full-screen map
        path: 'tank/map',
        loadComponent: () =>
          import('./features/tank/map-page/map-page').then(m => m.MapPage)
      },

      { path: '',  redirectTo: 'tank', pathMatch: 'full' },
      { path: '**', redirectTo: 'tank' }
    ]
  }
];

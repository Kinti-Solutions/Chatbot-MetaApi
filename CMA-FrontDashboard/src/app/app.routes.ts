
import { Routes } from '@angular/router';
import { MainLytComponent } from './shared/layouts/main-lyt/main-lyt.component';
import { DashboardComponent } from './pages/Dashboard/dashboard/dashboard.component';

export const routes: Routes = [
  // {
  //   path: '',
  // },
  // {
  //   path: 'Inicio'
  //   //compoenente de incio par login.
  // },
  {
    path: 'Dashboard',
    component: MainLytComponent,
    children: [
      {
        path: 'Inicio',
        component: DashboardComponent
      }
    ]
  },
  {
    path: '**',
    redirectTo: '',
  }
];

import { Component }            from '@angular/core';
import { CommonModule }         from '@angular/common';
import { RouterModule }         from '@angular/router';

import { MatToolbarModule }     from '@angular/material/toolbar';
import { MatButtonModule }      from '@angular/material/button';

@Component({
  selector:    'app-shell',
  standalone:  true,
  imports: [
    CommonModule,
    RouterModule,
    MatToolbarModule,
    MatButtonModule
  ],
  templateUrl: './shell.html',
  styleUrl:    './shell.scss'
})
export class ShellComponent {}

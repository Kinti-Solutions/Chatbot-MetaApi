
import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { CardModule } from 'primeng/card';
import { MenuItem , PrimeIcons} from 'primeng/api';
import { MenuModule } from 'primeng/menu';
import { BadgeModule } from 'primeng/badge';
import { RippleModule } from 'primeng/ripple';
import { CommonModule } from '@angular/common';
import { AvatarModule } from 'primeng/avatar';
import { ButtonModule } from 'primeng/button';
import { PanelMenu } from 'primeng/panelmenu';


@Component({
  selector: 'app-main-lyt',
  imports: [CardModule, RouterOutlet, MenuModule,
    BadgeModule, RippleModule, CommonModule,
    AvatarModule, ButtonModule, PanelMenu],
  templateUrl: './main-lyt.component.html',
  styleUrl: './main-lyt.component.scss'
})
export class MainLytComponent implements OnInit {
  items: MenuItem[] | undefined;

    ngOnInit() {
        this.items = [
            {
              label: 'Inicio',
              expanded: true,
              items: [
                    {
                        label: 'Dashboard',
                        icon: 'pi pi-home'
                    },
                    {
                      label: 'BI',
                      icon: 'pi pi-chart-line'
                    },
                    {
                        label: 'Leads',
                        icon: 'pi pi-sitemap'
                    },
                    {
                        label: 'Chats',
                        icon: 'pi pi-comments'
                    }
                ]
            },
            {
              label: 'Marketing',
              items: [
                {
                  label: "Calendario",
                  icon: 'pi pi-calendar'
                },
                {
                  label: 'Anuncios',
                  icon: 'pi pi-megaphone'
                  //badge: '2'
                }
              ]
            },
            {
                label: 'Ajustes',
                items: [
                    {
                        label: 'Configuraciones',
                        icon: 'pi pi-cog',
                        items: [
                          {
                            label: 'Empresa',
                            icon: 'pi pi-building'
                          },
                          {
                            label: 'Personal',
                            icon: 'pi pi-user-edit',
                            items: [
                              {
                                label: 'Usuarios',
                                icon: 'pi pi-users'
                              },
                              {
                                label: 'Permisos',
                                icon: 'pi pi-wrench'
                              },
                            ]
                          },
                          {
                            label: 'Conexiones',
                            icon: 'pi pi-share-alt'
                          }
                        ]
                    },
                    {
                      label: 'Comandos ChatBot',
                      icon: 'pi pi-box',
                      items: [
                        {
                          label: 'Catalogo',
                          icon: 'pi pi-tags'
                        },
                        {
                          label: 'Creador de comandos',
                          icon: 'pi pi-hammer'
                        }
                      ]
                    },
                    {
                      label: 'Integraciones',
                      icon: 'pi pi-objects-column'
                    }
                ]
            },
        ];
      }
    userItems = [
      { label: 'Perfil',   icon: 'pi pi-user',   command: () => this.goProfile() },
      // { label: 'Settings',  icon: 'pi pi-cog',    command: () => this.goSettings() },
      { separator: true },
      { label: 'Cerrar Sesion',    icon: 'pi pi-sign-out', command: () => this.logout() }
    ];

goProfile()  { /* navegación */ }
goSettings() { /* … */ }
logout()     { /* … */ }
}

import { AppComponentBase } from '@shared/common/app-component-base';
import { Component, OnInit, ViewEncapsulation, Injector } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Location } from '@angular/common';

@Component({
  templateUrl: './direct-response.component.html',
  styleUrls: ['./direct-response.component.less'],
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()]
})
export class DirectResponseComponent extends AppComponentBase implements OnInit {

  constructor(
    injector: Injector,
    private _location: Location
  ) {
    super(injector);
  }

  ngOnInit() {
  }

  back(): void {
    this._location.back();
  }

}

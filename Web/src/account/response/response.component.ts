import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from 'shared/common/app-component-base';
import { Component, OnInit, ViewEncapsulation, Injector } from '@angular/core';

@Component({
  templateUrl: './response.component.html',
  styleUrls: ['./response.component.less'],
  encapsulation: ViewEncapsulation.None,
  animations: [accountModuleAnimation()]
})
export class ResponseComponent extends AppComponentBase implements OnInit {

  constructor(
    injector: Injector
  ) {
    super(injector);
   }

  ngOnInit() {
  }

}

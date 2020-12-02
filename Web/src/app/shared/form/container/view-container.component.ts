import {Component, Input, ViewChild, ViewContainerRef, ComponentFactoryResolver, AfterViewInit} from '@angular/core';

@Component({
    selector: 'view-container',
    template: '<ng-content #vc></ng-content>'
})
export class ViewContainerComponent implements AfterViewInit {
    @Input()
    title: string;

    @Input()
    template;

    constructor(private resolver: ComponentFactoryResolver) {
    }

  ngAfterViewInit() {
  }
}
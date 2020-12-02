// import { Component, Injector, ViewChild, ViewEncapsulation, AfterViewInit, OnInit, AfterContentChecked, ChangeDetectorRef, Input, ElementRef, Inject } from '@angular/core';
// import { AppComponentBase } from '@shared/common/app-component-base';
// import { DomSanitizer } from '@angular/platform-browser';

// @Component({
//   templateUrl: './countdown.component.html',
//   styleUrls: ['./countdown.component.less'],
//   selector: 'countdownTimer',
//   encapsulation: ViewEncapsulation.None
// })

// export class CountdownComponent extends AppComponentBase implements OnInit, AfterViewInit {
//   @ViewChild('cd', { static: false }) private countdown: CountdownComponent;

//   constructor(
//     injector: Injector,
//     private sanitizer: DomSanitizer
//   ) {
//     super(injector);
//   }

//   ngOnInit(): void {
//   }

//   ngAfterViewInit(): void {
//   }

//   begin(countDownDateInput): void {
//     // Set the date we're counting down to
//     // let countDownDate = new Date("Jan 5, 2021 15:37:25").getTime();
//     let countDownDate = new Date(countDownDateInput).getTime();

//     // Update the count down every 1 second
//     let x = setInterval(function () {

//       // Get today's date and time
//       let now = new Date().getTime();

//       // Find the distance between now and the count down date
//       let distance = countDownDate - now;

//       // Time calculations for days, hours, minutes and seconds
//       let days = Math.floor(distance / (1000 * 60 * 60 * 24));
//       let hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
//       let minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
//       let seconds = Math.floor((distance % (1000 * 60)) / 1000);

//       // Display the result in the element with id="demo"
//       document.getElementById('myDate').innerHTML = days + "d " + hours + "h "
//         + minutes + "m " + seconds + "s ";

//       // If the count down is finished, write some text
//       if (distance < 0) {
//         clearInterval(x);
//         document.getElementById('myDate').innerHTML = "EXPIRED";
//       }
//     }, 1000);
//   }
// }

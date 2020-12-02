import { Component, Injector, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize, tap } from 'rxjs/operators';
import { Observable, combineLatest as _observableCombineLatest, of as _observableOf } from 'rxjs';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { UserServiceProxy, AccountServiceProxy, CreateOrUpdateUserInput } from '@shared/service-proxies/service-proxies';
import { LoginService } from '@account/login/login.service';
import { AppAuthService } from '@app/shared/common/auth/app-auth.service';

@Component({
  templateUrl: './registerEmail.component.html',
  animations: [accountModuleAnimation()]
})
export class RegisterEmailComponent extends AppComponentBase implements OnInit {
  sendEmail = ProxyURL.ActivatedEmail;
  loading = false;
  saving = false;
  emailAddress: any;
  userID: any;
  password: any;
  entity: any = [];

  constructor(
    injector: Injector,
    private _proxy: GenericServiceProxy,
    private readonly _loginService: LoginService,
    private _activatedRoute: ActivatedRoute,
    private _accountService: AccountServiceProxy,
    private _userService: UserServiceProxy,
    private _appAuthService: AppAuthService,
    private _router: Router
  ) {
    super(injector);
  }

  ngOnInit(): void {
    let userModel = new CreateOrUpdateUserInput();
    this._activatedRoute.queryParams
      .subscribe(params => {
        this.emailAddress = params.email;
        this.userID = params.userID;
        this.password = params.password;
      });

    this.sendEmail += 'emailAddress=' + this.emailAddress + '&userID=' + this.userID + '&password=123456&';

    this._proxy.request(this.sendEmail, RequestType.Get)
      .subscribe(result => {
        this.saving = true;
      });

    // console.log('sebelum log in');
    // this._loginService.authenticateModel.userNameOrEmailAddress = 'LGM-REGISTER';
    // this._loginService.authenticateModel.password = '123456';
    // this._loginService.authenticate(() => {

    //   console.log('sesudah log in');
    //   this._userService.getUsers(this.userID, [], 14, false, '', 1, 0)
    //   .pipe(
    //     tap(result => {
    //       this.entity = result;
    //       this.entity.IsActive = 1;
    //       this.entity.IsEmailConfirmed = 1;
    //       console.log('entity = ' + JSON.stringify(this.entity));
    //       this._userService.createOrUpdateUser(this.entity)
    //         .toPromise().then(() => {
    //           console.log('logged in');
    //         }).catch((err) => {
    //           console.error(err);
    //         });
    //     }),
    //     finalize(() => {
    //       this._appAuthService.logout(false);
    //       this.loading = false;
    //       this._loginService.authenticateModel.userNameOrEmailAddress = '';
    //       this._loginService.authenticateModel.password = '';
    //     })
    //   );
    // });

  }
}

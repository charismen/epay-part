import { Injectable, Injector, NgZone } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HubConnection } from '@microsoft/signalr';
import * as _ from 'lodash';

@Injectable()
export class RealTimeService extends AppComponentBase {

    constructor(
        injector: Injector,
        public _zone: NgZone
    ) {
        super(injector);
    }

    realTimeHub: HubConnection;
    isRealTimeConnected = false;

    configureConnection(connection, methods: string[]): void {
        // Set the common hub
        this.realTimeHub = connection;

        // Reconnect loop
        function start() {
            return new Promise(function (resolve, reject) {
                connection.start()
                    .then(resolve)
                    .catch(() => {
                        setTimeout(() => {
                            start().then(resolve);
                        }, 5000);
                    });
            });
        }

        // Reconnect if hub disconnects
        connection.onclose(e => {
            this.isRealTimeConnected = false;

            if (e) {
                abp.log.debug('Real time connection closed with error: ' + e);
            } else {
                abp.log.debug('Real time disconnected');
            }

            start().then(() => {
                this.isRealTimeConnected = true;
            });
        });

        // Register to get notifications
        _.forEach(methods, method => {
            this.registerRealTimeEvents(connection, method);
        });
    }

    registerRealTimeEvents(connection, method): void {
        connection.on(method, message => {
            abp.event.trigger(method, message);
        });
    }

    init(methods: string[]): void {
        this._zone.runOutsideAngular(() => {
            abp.signalr.connect();
            abp.signalr.startConnection(abp.appPath + 'signalr-realtime', connection => {
                this.configureConnection(connection, methods);
            }).then(() => {
                abp.event.trigger('app.chat.connected');
                this.isRealTimeConnected = true;
            });
        });
    }
}

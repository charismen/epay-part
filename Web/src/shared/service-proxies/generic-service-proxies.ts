import { mergeMap as _observableMergeMap, catchError as _observableCatch } from 'rxjs/operators';
import { Observable, throwError as _observableThrow, of as _observableOf } from 'rxjs';
import { Injectable, Inject, Optional, InjectionToken } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse, HttpResponseBase } from '@angular/common/http';

import * as moment from 'moment';
import { API_BASE_URL } from './service-proxies';

@Injectable()
export class GenericServiceProxy {
    private http: HttpClient;
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(@Inject(HttpClient) http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
        this.http = http;
        this.baseUrl = baseUrl ? baseUrl : '';
    }

    /**
     * @param url (mandatory)
     * @param requestType (mandatory)
     * @return Success
     */

    request(url: string | null | undefined, requestType: RequestType | null | undefined, input?: any | null | undefined): Observable<any> {
        let url_ = this.baseUrl;
        if (url !== undefined) {
            url_ += url;

            url_ = url_.replace(/[?&]$/, '');

            let header_ = new HttpHeaders();
            let content_ = '';

            if (requestType === RequestType.Get) {
                header_ = new HttpHeaders({
                    'Accept': 'text/plain'
                });
            } else if (requestType === RequestType.Post) {
                header_ = new HttpHeaders({
                    'Content-Type': 'application/json-patch+json'
                });

                if (input === undefined) {
                    return throwException('Proxy usage violation', 404, 'Input value undefined', header_);
                }
                content_ = JSON.stringify(input);
            }

            let options_: any = {
                body: content_,
                observe: 'response',
                responseType: 'blob',
                headers: header_
            };

            return this.http.request(requestType, url_, options_).pipe(_observableMergeMap((response_: any) => {
                return this.getResponse(response_, requestType);
            })).pipe(_observableCatch((response_: any) => {
                if (response_ instanceof HttpResponseBase) {
                    try {
                        return this.getResponse(<any>response_, requestType);
                    } catch (e) {
                        return <Observable<any>><any>_observableThrow(e);
                    }
                } else {
                    return <Observable<any>><any>_observableThrow(response_);
                }
            }));
        }
    }

    protected getResponse(response: HttpResponseBase, requestType: RequestType | null | undefined): Observable<any> {
        const status = response.status;
        const responseBlob =
            response instanceof HttpResponse ? response.body :
                (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        let _headers: any = {}; if (response.headers) { for (let key of response.headers.keys()) { _headers[key] = response.headers.get(key); }}
        if (status === 200) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
                let result200: any = null;
                let resultData200 = _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
                result200 = resultData200 !== undefined ? resultData200 : <any>null;
                return _observableOf(result200);
            }));
        } else if (status !== 200 && status !== 204) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
                return throwException('An unexpected server error occurred.', status, _responseText, _headers);
            }));
        }
        return _observableOf<number>(<any>null);
    }
}

export function encodeURIArrayComponent(key: string, arr: any[]) {
    arr = arr.map(encodeURIComponent);
    return key + '=' + arr.join('&' + key + '=');
}

export enum RequestType {
    Get = 'get',
    Post = 'post'
}

export class SwaggerException extends Error {
    message: string;
    status: number;
    response: string;
    headers: { [key: string]: any; };
    result: any;

    constructor(message: string, status: number, response: string, headers: { [key: string]: any; }, result: any) {
        super();

        this.message = message;
        this.status = status;
        this.response = response;
        this.headers = headers;
        this.result = result;
    }

    protected isSwaggerException = true;

    static isSwaggerException(obj: any): obj is SwaggerException {
        return obj.isSwaggerException === true;
    }
}

function throwException(message: string, status: number, response: string, headers: { [key: string]: any; }, result?: any): Observable<any> {
    if (result !== null && result !== undefined) {
        return _observableThrow(result);
    } else {
        return _observableThrow(new SwaggerException(message, status, response, headers, null));
    }
}

function blobToText(blob: any): Observable<string> {
    return new Observable<string>((observer: any) => {
        if (!blob) {
            observer.next('');
            observer.complete();
        } else {
            let reader = new FileReader();
            reader.onload = event => {
                observer.next((<any>event.target).result);
                observer.complete();
            };
            reader.readAsText(blob);
        }
    });
}

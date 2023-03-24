import { HttpEvent, HttpHandler, HttpHeaders, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { from, Observable } from "rxjs";
import { AuthService } from "./auth.service";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    constructor(private authService: AuthService) {
    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return from(this.handleAccess(req, next));
    }

    private async handleAccess(req: HttpRequest<any>, next: HttpHandler):
        Promise<HttpEvent<any>> {
        const token = await this.authService.getToken();
        const changedReq = req.clone({
            setHeaders: {
                Authorization: `Bearer ${ token }`
            }
        });
        return next.handle(changedReq).toPromise();
    }
}
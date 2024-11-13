import { Observable, of, throwError } from "rxjs";
import { BackendServerResponse } from "../extensions/backend-server-response";
import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { RegisterModel } from "../models/register.model";
import { LoginModel } from "../models/login.model";
import { JwtHelperService } from "@auth0/angular-jwt";
import { AuthorizationResponse } from "../extensions/authorization-response";
import { UserAuthorizedModel } from "../models/user-authorized.model";
import { ResetPasswordModel } from "../models/reset-password.model";
import { ChangeEmailModel } from "../models/change-email.model";
import { Pagination } from "../extensions/pagination";
import { PageRequest } from "../extensions/page-request";
import { UserInfoModel } from "../models/user-info.model";

@Injectable({
    providedIn: 'root'
})
export class AuthenticationService {
    private baseUrl: string = "http://localhost:1000";
    private jwtHelper: JwtHelperService;

    constructor(private http: HttpClient) {
        this.jwtHelper = new JwtHelperService();
    }

    checkUsernameExistence(username: string): Observable<BackendServerResponse> {
        return this.http.get<BackendServerResponse>(`${this.baseUrl}/check-username-existence/${username}`);
    }

    checkEmailExistence(email: string): Observable<BackendServerResponse> {
        return this.http.get<BackendServerResponse>(`${this.baseUrl}/check-email-existence/${email}`);
    }

    confirmEmail(userId: string, code: string): Observable<BackendServerResponse> {
        return this.http.get<BackendServerResponse>(`${this.baseUrl}/confirm-email?userId=${userId}&code=${code}`, {
            withCredentials: false, headers: {
                'X-Ignore-Cert': 'true',
                'X-Skip-SSL-Check': 'true'
            }
        });
    }

    registerUser(model: RegisterModel): Observable<BackendServerResponse> {
        const formData = new FormData();

        if (model.icon != null)
            formData.append('avatar', model.icon);

        formData.append('username', model.username);
        formData.append('email', model.email);
        formData.append('password', model.password);

        return this.http.post<BackendServerResponse>(`${this.baseUrl}/register`, formData);
    }

    login(model: LoginModel): Observable<BackendServerResponse> {
        const formData = new FormData();

        formData.append('UsernameOrEmailInput', model.usernameOrEmail);
        formData.append('Password', model.password);

        return this.http.post<BackendServerResponse>(`${this.baseUrl}/login`, formData);
    }

    isAuthorized(): AuthorizationResponse {
        if (this.isLocalStorageAvailable()) {
            const token = localStorage.getItem('accessToken');
            const refreshToken = localStorage.getItem('refreshToken');
    
            if (token && refreshToken && !this.jwtHelper.isTokenExpired(token)) {
                return AuthorizationResponse.Authorized;
            } else if (token && refreshToken && this.jwtHelper.isTokenExpired(token)) {
                const tokenPayload = this.jwtHelper.decodeToken(token);
                const tokenIssuedAt = tokenPayload.iat * 1000; // `iat` в секундах, тому множимо на 1000, щоб отримати мілісекунди
                const currentTime = new Date().getTime();
                const twoHoursInMilliseconds = 3 * 60 * 60 * 1000;
    
                // Якщо токен був виданий більше 3 годин тому, видаляємо його
                if (currentTime - tokenIssuedAt > twoHoursInMilliseconds) {
                    localStorage.removeItem('accessToken');
                    localStorage.removeItem('refreshToken');
                    
                    return AuthorizationResponse.TokenNotFound;
                }
    
                return AuthorizationResponse.TokenExpired;
            }
        }
    
        return AuthorizationResponse.TokenNotFound;
    }

    forgotPassword(email: string): Observable<BackendServerResponse> {
        const formData = new FormData();

        formData.append('Email', email);
        return this.http.post<BackendServerResponse>(`${this.baseUrl}/forgot-password`, formData);
    }

    resetPassword(model: ResetPasswordModel): Observable<BackendServerResponse> {
        const formData = new FormData();

        formData.append('Email', model.email);
        formData.append('Password', model.password);
        formData.append('ConfirmPassword', model.confirmPassword);
        formData.append('Code', model.code);

        return this.http.post<BackendServerResponse>(`${this.baseUrl}/reset-password`, formData);
    }

    changeAvatar(id: string, icon: File): Observable<BackendServerResponse> {

        if (this.isLocalStorageAvailable()) {
            let accessToken = localStorage.getItem('accessToken');

            const headers = new HttpHeaders({
                'Authorization': `Bearer ${accessToken}`
            });

            const formData = new FormData();

            formData.append('Id', id);
            formData.append('NewAvatar', icon);

            return this.http.put<BackendServerResponse>(`${this.baseUrl}/change-avatar`, formData, { headers: headers });
        }

        const errorResponse: BackendServerResponse = new BackendServerResponse('Error', false, undefined, undefined, undefined, undefined);
        return of(errorResponse);
    }

    changeUsername(id: string, username: string): Observable<BackendServerResponse> {
        if (this.isLocalStorageAvailable()) {
            let accessToken = localStorage.getItem('accessToken');

            const headers = new HttpHeaders({
                'Authorization': `Bearer ${accessToken}`
            });

            const formData = new FormData();

            formData.append('Id', id);
            formData.append('NewUsername', username);

            return this.http.put<BackendServerResponse>(`${this.baseUrl}/change-username`, formData, { headers: headers });
        }

        const errorResponse: BackendServerResponse = new BackendServerResponse('Error', false, undefined, undefined, undefined, undefined);
        return of(errorResponse);
    }

    generateChangeEmailToken(id: string, email: string) {
        if (this.isLocalStorageAvailable()) {
            let accessToken = localStorage.getItem('accessToken');

            const headers = new HttpHeaders({
                'Authorization': `Bearer ${accessToken}`
            });

            const formData = new FormData();

            formData.append('UserId', id);
            formData.append('NewEmail', email);

            return this.http.post<BackendServerResponse>(`${this.baseUrl}/change-email`, formData, { headers: headers });
        }

        const errorResponse: BackendServerResponse = new BackendServerResponse('Error', false, undefined, undefined, undefined, undefined);
        return of(errorResponse);
    }

    changeEmail(model: ChangeEmailModel): Observable<BackendServerResponse> {
        if (model.userId && model.email && model.code) {

            return this.http.get<BackendServerResponse>(`${this.baseUrl}/change-email?userId=${model.userId}&email=${model.email}&code=${model.code}`);
        }

        const errorResponse: BackendServerResponse = new BackendServerResponse('Error', false, undefined, undefined, undefined, undefined);
        return of(errorResponse);
    }

    refreshToken(): void {
        if (this.isLocalStorageAvailable()) {
            let accessToken = localStorage.getItem('accessToken');
            let refreshToken = localStorage.getItem('refreshToken');

            if (accessToken && refreshToken) {
                const formData = new FormData();
                formData.append('AccessToken', accessToken);
                formData.append('RefreshToken', refreshToken);

                this.http.post<BackendServerResponse>(`${this.baseUrl}/refresh-token`, formData)
                    .subscribe((response: BackendServerResponse) => {
                        if (response.accessToken && response.refreshToken) {
                            localStorage.removeItem('accessToken');
                            localStorage.removeItem('refreshToken');
                            localStorage.setItem('accessToken', response.accessToken);
                            localStorage.setItem('refreshToken', response.refreshToken);
                        }
                    }, (error) => {
                        console.log(error);
                    });
            }
        }
    }

    getAuthorizeHeaderData(): UserAuthorizedModel {
        const model = new UserAuthorizedModel('', '', '', '', '');

        if (this.isLocalStorageAvailable()) {
            const token = localStorage.getItem('accessToken');

            if (token) {
                const data = this.jwtHelper.decodeToken(token);

                console.log(data);

                model.imageDirectory = data['PhotoDirectory'];
                model.username = data['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
                model.id = data['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
                model.email = data['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'];
                model.role = data['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
            }
        }

        return model;
    }

    getAccessToken(): string {
        if (this.isLocalStorageAvailable()) {
            return localStorage.getItem('accessToken') || '';
        }

        return '';
    }

    clearLocalStorage() {
        if (this.isLocalStorageAvailable()) {
            const token = localStorage.getItem('accessToken');

            if (token) {
                const data = this.jwtHelper.decodeToken(token);
                let userId = data['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];

                this.http.post(`${this.baseUrl}/revoke/${userId}`, null)
                    .subscribe();
            }

            localStorage.clear();
        }
    }

    private isLocalStorageAvailable() {
        try {
            localStorage.setItem('test', 'test');
            localStorage.removeItem('test');
            return true;
        } catch (e) {
            return false;
        }
    }

    // PRIVILEGED USER METHODS

    getUsers(searchText: string, pagination: Pagination): Observable<PageRequest<UserInfoModel>> {
        if (this.isLocalStorageAvailable()) {
            let accessToken = localStorage.getItem('accessToken');

            const headers = new HttpHeaders({
                'Authorization': `Bearer ${accessToken}`
            });

            let request = `${this.baseUrl}/get-users?`;

            if (searchText !== null && searchText !== '') {
                request += `SearchText=${searchText}&`;
            }

            request += `Page=${pagination.page}&PageSize=${pagination.pageSize}`;

            return this.http.get<PageRequest<UserInfoModel>>(request,
                { headers: headers });
        }

        return throwError('');
    }

    banUser(userId: string, description: string, bannedBy: string): Observable<BackendServerResponse> {
        if (this.isLocalStorageAvailable()) {
            let accessToken = localStorage.getItem('accessToken');

            const headers = new HttpHeaders({
                'Authorization': `Bearer ${accessToken}`
            });

            const formData = new FormData();

            formData.append('UserId', userId);

            if (description !== '') {
                formData.append('Description', description);
            }

            formData.append('BannedBy', bannedBy);

            return this.http.post<BackendServerResponse>(`${this.baseUrl}/ban-user`, formData, { headers: headers });
        }

        return throwError('');
    }

    upgradeRole(userId: string): Observable<BackendServerResponse> {
        if (this.isLocalStorageAvailable()) {
            let accessToken = localStorage.getItem('accessToken');

            const headers = new HttpHeaders({
                'Authorization': `Bearer ${accessToken}`
            });

            return this.http.post<BackendServerResponse>(`${this.baseUrl}/upgrade-role/${userId}`, null, { headers: headers });
        }

        return throwError('');
    }

    downgradeRole(userId: string, description: string): Observable<BackendServerResponse> {
        if (this.isLocalStorageAvailable()) {
            let accessToken = localStorage.getItem('accessToken');

            const headers = new HttpHeaders({
                'Authorization': `Bearer ${accessToken}`
            });

            const formData = new FormData();

            formData.append('UserId', userId);

            if (description !== '') {
                formData.append('Description', description);
            }

            return this.http.post<BackendServerResponse>(`${this.baseUrl}/downgrade-role`, formData, { headers: headers });
        }

        return throwError('');
    }
}
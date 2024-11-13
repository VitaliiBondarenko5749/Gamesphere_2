import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { CategoryInfoModel } from "../models/category-info.model";
import { BackendServerResponse } from "../extensions/backend-server-response";
import { DeveloperInfoModel } from "../models/developer-info.model";
import { LanguageInfoModel } from "../models/language-info.model";
import { PlatformInfoModel } from "../models/platform-info.model";
import { PublisherInfoModel } from "../models/publisher-info.model";
import { GameListModel } from "../models/game-list.model";
import { GameSorter } from "../extensions/game-sorter.enum";
import { Pagination } from "../extensions/pagination";
import { PageRequest } from "../extensions/page-request";
import { ShortGameModel } from "../models/short-game.model";
import { GameInfoModel } from "../models/game-info.model";
import { publicDecrypt } from "crypto";
import { CommentInfoModel } from "../models/comment-info.model";

@Injectable({
    providedIn: 'root'
})
export class GameService {
    private baseUrl: string = "http://localhost:1000";

    constructor(private http: HttpClient) { }

    public predictGame(file: File) : Observable<{ result: string }> {
        const formData = new FormData();

        formData.append('file', file);

        return this.http.post<{ result: string }>(`${this.baseUrl}/predict-game`, formData);
    }

    // CATEGORIES

    public getCategories(): Observable<CategoryInfoModel[]> {
        return this.http.get<CategoryInfoModel[]>(`${this.baseUrl}/get-categories`);
    }

    public addCategory(name: string, accessToken: string): Observable<BackendServerResponse> {
        const formData = new FormData();

        formData.append('Name', name);

        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        return this.http.post<BackendServerResponse>(`${this.baseUrl}/add-category`, formData, { headers: headers });
    }

    public deleteCategory(id: string, accessToken: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        return this.http.delete<BackendServerResponse>(`${this.baseUrl}/delete-category/${id}`, { headers: headers });
    }

    public getTop10Categories(name: string): Observable<CategoryInfoModel[]> {
        return this.http.get<CategoryInfoModel[]>(`${this.baseUrl}/categories/${name}`);
    }

    // DEVELOPERS

    public addDeveloper(name: string, accessToken: string): Observable<BackendServerResponse> {
        const formData = new FormData();

        formData.append('Name', name);

        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        return this.http.post<BackendServerResponse>(`${this.baseUrl}/add-developer`, formData, { headers: headers });
    }

    public deleteDeveloper(id: string, accessToken: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        return this.http.delete<BackendServerResponse>(`${this.baseUrl}/delete-developer/${id}`, { headers: headers });
    }

    public getTop10Developers(name: string): Observable<DeveloperInfoModel[]> {
        return this.http.get<DeveloperInfoModel[]>(`${this.baseUrl}/developers/${name}`);
    }

    // LANGUAGES

    public addLanguage(name: string, accessToken: string): Observable<BackendServerResponse> {
        const formData = new FormData();

        formData.append('Name', name);

        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        return this.http.post<BackendServerResponse>(`${this.baseUrl}/add-language`, formData, { headers: headers });
    }

    public deleteLanguage(id: string, accessToken: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        return this.http.delete<BackendServerResponse>(`${this.baseUrl}/delete-language/${id}`, { headers: headers });
    }

    public getTop10Languages(name: string): Observable<LanguageInfoModel[]> {
        return this.http.get<LanguageInfoModel[]>(`${this.baseUrl}/languages/${name}`);
    }

    // PLATFORMS

    public addPlatform(name: string, accessToken: string): Observable<BackendServerResponse> {
        const formData = new FormData();

        formData.append('Name', name);

        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        return this.http.post<BackendServerResponse>(`${this.baseUrl}/add-platform`, formData, { headers: headers });
    }

    public deletePlatform(id: string, accessToken: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        return this.http.delete<BackendServerResponse>(`${this.baseUrl}/delete-platform/${id}`, { headers: headers });
    }

    public getTop10Platforms(name: string): Observable<PlatformInfoModel[]> {
        return this.http.get<PlatformInfoModel[]>(`${this.baseUrl}/platforms/${name}`);
    }

    // PUBLISHERS

    public addPublisher(name: string, accessToken: string): Observable<BackendServerResponse> {
        const formData = new FormData();

        formData.append('Name', name);

        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        return this.http.post<BackendServerResponse>(`${this.baseUrl}/add-publisher`, formData, { headers: headers });
    }

    public deletePublisher(id: string, accessToken: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        return this.http.delete<BackendServerResponse>(`${this.baseUrl}/delete-publisher/${id}`, { headers: headers });
    }

    public getTop10Publishers(name: string): Observable<PublisherInfoModel[]> {
        return this.http.get<PublisherInfoModel[]>(`${this.baseUrl}/publishers/${name}`);
    }

    // GAMES

    public checkGameNameExistence(name: string) {
        return this.http.get<BackendServerResponse>(`${this.baseUrl}/check-game-existence/${name}`);
    }

    public addGame(accessToken: string, name: string, description: string, publisherId: string, releaseDate: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        const formData = new FormData();

        formData.append('Name', name);
        formData.append('Description', description);
        formData.append('PublisherId', publisherId);
        formData.append('ReleaseDate', releaseDate);

        return this.http.post<BackendServerResponse>(`${this.baseUrl}/add-game`, formData, { headers: headers });
    }

    public uploadGameImage(accessToken: string, image: File, gameName: string, directory: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        const formData = new FormData();

        formData.append('Image', image);
        formData.append('GameName', gameName);
        formData.append('Directory', directory);

        return this.http.post<BackendServerResponse>(`${this.baseUrl}/upload-game-image`, formData, { headers: headers });
    }

    public deleteGameImage(accessToken: string, imageId: string, directory: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        const params = new HttpParams();

        params.append('Id', imageId);
        params.append('Directory', directory);

        return this.http.delete<BackendServerResponse>(`${this.baseUrl}/delete-game-image`, { headers: headers, params: params });
    }

    public getTop10Games(name: string): Observable<GameListModel[]> {
        return this.http.get<GameListModel[]>(`${this.baseUrl}/games/${name}`);
    }

    public deleteGame(accessToken: string, gameId: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        return this.http.delete<BackendServerResponse>(`${this.baseUrl}/delete-game/${gameId}`, { headers: headers });
    }

    public addVideoLink(accessToken: string, gameName: string, videoLink: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        const formData = new FormData();

        formData.append('GameName', gameName);
        formData.append('VideoLink', videoLink);

        return this.http.post<BackendServerResponse>(`${this.baseUrl}/add-game-video-link`, formData, { headers: headers });
    }

    public deleteVideoLink(accessToken: string, videoId: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        return this.http.delete<BackendServerResponse>(`${this.baseUrl}/delete-game-video-link/${videoId}`, { headers: headers });
    }

    public updatePublisherData(accessToken: string, publisherId: string, gameId: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        const formData = new FormData();

        formData.append('PublisherId', publisherId);
        formData.append('GameId', gameId);

        return this.http.put<BackendServerResponse>(`${this.baseUrl}/add-game-video-link`, formData, { headers: headers });
    }

    public addCategoryToGame(accessToken: string, gameName: string, categoryId: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        const formData = new FormData();

        formData.append('CategoryId', categoryId);
        formData.append('GameName', gameName);

        return this.http.post<BackendServerResponse>(`${this.baseUrl}/add-category-to-game`, formData, { headers: headers });
    }

    public deleteCategoryFromGame(accessToken: string, gameName: string, categoryId: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        const params = new HttpParams();

        params.append('GameName', gameName);
        params.append('CategoryId', categoryId);

        return this.http.delete<BackendServerResponse>(`${this.baseUrl}/delete-category-from-game`, { headers: headers, params: params });
    }

    public addDeveloperToGame(accessToken: string, gameName: string, developerId: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        const formData = new FormData();

        formData.append('DeveloperId', developerId);
        formData.append('GameName', gameName);

        return this.http.post<BackendServerResponse>(`${this.baseUrl}/add-developer-to-game`, formData, { headers: headers });
    }

    public deleteDeveloperFromGame(accessToken: string, gameName: string, developerId: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        const params = new HttpParams();

        params.append('GameName', gameName);
        params.append('DeveloperId', developerId);

        return this.http.delete<BackendServerResponse>(`${this.baseUrl}/delete-developer-from-game`, { headers: headers, params: params });
    }

    public addLanguageToGame(accessToken: string, gameName: string, languageId: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        const formData = new FormData();

        formData.append('LanguageId', languageId);
        formData.append('GameName', gameName);

        return this.http.post<BackendServerResponse>(`${this.baseUrl}/add-language-to-game`, formData, { headers: headers });
    }

    public deleteLanguageFromGame(accessToken: string, gameName: string, languageId: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        const params = new HttpParams();

        params.append('GameName', gameName);
        params.append('LanguageId', languageId);

        return this.http.delete<BackendServerResponse>(`${this.baseUrl}/delete-language-from-game`, { headers: headers, params: params });
    }

    public addPlatformToGame(accessToken: string, gameName: string, platformId: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        const formData = new FormData();

        formData.append('PlatformId', platformId);
        formData.append('GameName', gameName);

        return this.http.post<BackendServerResponse>(`${this.baseUrl}/add-platform-to-game`, formData, { headers: headers });
    }

    public deletePlatformFromGame(accessToken: string, gameName: string, platformId: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        const params = new HttpParams();

        params.append('GameName', gameName);
        params.append('PlatformId', platformId);

        return this.http.delete<BackendServerResponse>(`${this.baseUrl}/delete-platform-from-game`, { headers: headers, params: params });
    }

    public getAllGames(gameSorter: GameSorter, searchText: string, pagination: Pagination): Observable<PageRequest<ShortGameModel>> {

        let request = `${this.baseUrl}/get-all-games?GameSorter=${gameSorter}&`;

        if (searchText !== null && searchText !== '') {
            request += `SearchText=${searchText}&`;
        }

        request += `Page=${pagination.page}&PageSize=${pagination.pageSize}`;

        return this.http.get<PageRequest<ShortGameModel>>(request);
    }

    public getGamesByCategory(gameSorter: GameSorter, searchText: string, categoryName: string, pagination: Pagination): Observable<PageRequest<ShortGameModel>> {

        let request = `${this.baseUrl}/get-games-by-category?GameSorter=${gameSorter}&`;

        if (searchText !== null && searchText !== '') {
            request += `SearchText=${searchText}&`;
        }

        request += `Page=${pagination.page}&PageSize=${pagination.pageSize}&CategoryName=${categoryName}`;

        return this.http.get<PageRequest<ShortGameModel>>(request);
    }

    public getFavoriteGames(accessToken: string, gameSorter: GameSorter, searchText: string, userId: string, pagination: Pagination):
        Observable<PageRequest<ShortGameModel>> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        let request = `${this.baseUrl}/get-favorite-games?GameSorter=${gameSorter}&`;

        if (searchText !== null && searchText !== '') {
            request += `SearchText=${searchText}&`;
        }

        request += `Page=${pagination.page}&PageSize=${pagination.pageSize}&UserId=${userId}`;

        return this.http.get<PageRequest<ShortGameModel>>(request, { headers: headers });
    }

    public getGameById(id: string): Observable<GameInfoModel> {
        return this.http.get<GameInfoModel>(`${this.baseUrl}/game/${id}`);
    }

    public getRecGames(accessToken: string, userId: string): Observable<ShortGameModel[]> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        return this.http.get<ShortGameModel[]>(`${this.baseUrl}/get-rec-games/${userId}`, { headers: headers });
    }

    public addGameToFavoriteList(accessToken: string, gameId: string, userId: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        const formData = new FormData();

        formData.append('GameId', gameId);
        formData.append('UserId', userId);

        return this.http.post<BackendServerResponse>(`${this.baseUrl}/add-game-to-fav-list`, formData, { headers: headers });
    }

    public removeGameFromFavoriteList(accessToken: string, gameId: string, userId: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        let request = `${this.baseUrl}/remove-game-from-fav-list?GameId=${gameId}&UserId=${userId}`;

        return this.http.delete<BackendServerResponse>(request, { headers: headers });
    }

    public checkGameInFavoriteList(gameId: string, userId: string): Observable<BackendServerResponse> {
        return this.http.get<BackendServerResponse>(`${this.baseUrl}/check-game-in-fav-list?GameId=${gameId}&UserId=${userId}`);
    }

    public getComments(gameId: string, pagination: Pagination): Observable<PageRequest<CommentInfoModel>> {
        return this.http.get<PageRequest<CommentInfoModel>>(`${this.baseUrl}/get-comments?GameId=${gameId}&Page=${pagination.page}&PageSize=${pagination.pageSize}`);
    }

    public addComment(accessToken: string, userId: string, gameId: string, content: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        const formData = new FormData();

        formData.append('UserId', userId);
        formData.append('GameId', gameId);
        formData.append('Content', content);

        return this.http.post<BackendServerResponse>(`${this.baseUrl}/add-comment-to-game`, formData, { headers: headers });
    }

    public checkLikeExistence(commentId: string, userId: string): Observable<BackendServerResponse> {
        return this.http.get<BackendServerResponse>(`${this.baseUrl}/check-like-existence?CommentId=${commentId}&UserId=${userId}`);
    }

    public getCountOfLikes(commentId: string): Observable<number> {
        return this.http.get<number>(`${this.baseUrl}/get-likes/${commentId}`);
    }

    public doLikeOperation(accessToken: string, commentId: string, userId: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        const formData = new FormData();

        formData.append('CommentId', commentId);
        formData.append('UserId', userId);

        return this.http.post<BackendServerResponse>(`${this.baseUrl}/do-like-operation`, formData, {headers: headers});
    }

    public deleteComment(accessToken: string, commentId: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        return this.http.delete<BackendServerResponse>(`${this.baseUrl}/delete-comment-from-game/${commentId}`, {headers: headers});
    }

    public deleteUserData(accessToken: string, userId: string): Observable<BackendServerResponse>{
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        return this.http.delete<BackendServerResponse>(`${this.baseUrl}/delete-user-data-game/${userId}`, {headers: headers});
    }
}
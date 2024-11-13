import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { Pagination } from "../extensions/pagination";
import { TopicSorter } from "../extensions/topic-sorter.enum";
import { TopicViewer } from "../extensions/topic-viewer.enum";
import { PageRequest } from "../extensions/page-request";
import { Observable } from "rxjs";
import { ShortPostInfoModel } from "../models/short-post-info.model";
import { BackendServerResponse } from "../extensions/backend-server-response";
import { AddPostModel } from "../models/add-post.model";
import { FullPostInfoModel } from "../models/full-post-info-model";
import { SendReplyModel } from "../models/send-reply.model";
import { ReplyInfoModel } from "../models/reply-info.model";

@Injectable({
    providedIn: 'root'
})
export class ForumService {
    private baseUrl: string = "http://localhost:1000";

    constructor(private http: HttpClient) { }

    getTopics(pagination: Pagination, topicViewer: TopicViewer, topicSorter: TopicSorter, searchText: string, gameIds: string[],
        userId: string | null
    )
        : Observable<PageRequest<ShortPostInfoModel>> {
        console.log(gameIds);
        let request = `${this.baseUrl}/get-topics?Page=${pagination.page}&PageSize=${pagination.pageSize}&TopicViewer=${topicViewer}&TopicSorter=${topicSorter}`;

        if (searchText !== null && searchText !== '') {
            request += `&SearchText=${searchText}`;
        }

        if (userId !== null && topicViewer !== TopicViewer.All) {
            request += `&UserId=${userId}`;
        }

        if (gameIds.length > 0) {
            request += `&GameIds=${gameIds.join(',')}`;
        }

        return this.http.get<PageRequest<ShortPostInfoModel>>(request);
    } // DONE.

    getSimilarTopics(postId: string, currentPostText: string, gameId: string | null, count: number) : Observable<ShortPostInfoModel[]> {
        let query = `${this.baseUrl}/get-similar-posts?PostId=${postId}&CurrentPostText=${currentPostText}`;

        if(gameId){
            query += `&GameId=${gameId}`;
        }

        query += `&Count=${count}`;

        return this.http.get<ShortPostInfoModel[]>(query);
    }

    addPost(post: AddPostModel, accessToken: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        const formData = new FormData();

        formData.append('Subject', post.subject);
        formData.append('Text', post.text);

        if (post.gameId) {
            formData.append('GameId', post.gameId);
        }

        formData.append('UserId', post.userId);

        console.log(formData);

        return this.http.post<BackendServerResponse>(`${this.baseUrl}/add-post`, formData, { headers: headers });
    } // DONE.

    getPostInfo(postId: string): Observable<FullPostInfoModel> {
        return this.http.get<FullPostInfoModel>(`${this.baseUrl}/get-post/${postId}`);
    } // DONE.

    deletePost(accessToken: string, postId: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        return this.http.delete<BackendServerResponse>(`${this.baseUrl}/delete-post/${postId}`, { headers: headers });
    } // DONE.

    addPostToFavorite(accessToken: string, postId: string, userId: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        const formData = new FormData();

        formData.append('PostId', postId);
        formData.append('UserId', userId);

        return this.http.post<BackendServerResponse>(`${this.baseUrl}/add-post-to-favorite`, formData, { headers: headers });
    } // DONE.

    deletePostFromFavorite(accessToken: string, postId: string, userId: string): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        return this.http.delete<BackendServerResponse>(`${this.baseUrl}/delete-post-from-favorite?PostId=${postId}&UserId=${userId}`,
            { headers: headers });
    } // DONE.

    checkPostInFavorite(postId: string, userId: string): Observable<BackendServerResponse> {
        return this.http.get<BackendServerResponse>(`${this.baseUrl}/check-post-in-favorite?PostId=${postId}&UserId=${userId}`);
    } // DONE.

    sendReply(accessToken: string, reply: SendReplyModel): Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        const formData = new FormData();

        formData.append('UserId', reply.userId);
        formData.append('PostId', reply.postId);
        formData.append('Content', reply.content);

        if (reply.replyToId) {
            formData.append('ReplyToId', reply.replyToId);
        }

        return this.http.post<BackendServerResponse>(`${this.baseUrl}/send-reply`, formData, { headers });
    } // DONE.

    getReplies(pagination: Pagination, postId: string) : Observable<PageRequest<ReplyInfoModel>> {
        let query = `${this.baseUrl}/get-replies?Page=${pagination.page}&PageSize=${pagination.pageSize}&PostId=${postId}`;

        return this.http.get<PageRequest<ReplyInfoModel>>(query);
    } // DONE.

    deleteReply(accessToken: string, replyId: string) : Observable<BackendServerResponse> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${accessToken}`
        });

        return this.http.delete<BackendServerResponse>(`${this.baseUrl}/delete-reply/${replyId}`, { headers: headers });
    } // DONE.
}
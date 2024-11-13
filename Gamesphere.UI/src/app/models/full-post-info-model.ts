import { FullForumUserInfoModel } from "./full-forum-user-info-model";
import { PostGameInfoModel } from "./post-game-info.model";

export class FullPostInfoModel {
    constructor(
        public id: string,
        public topic: string,
        public content: string,
        public createdAt: string,
        public views: number,
        public user: FullForumUserInfoModel,
        public game: PostGameInfoModel
    ) { }
}
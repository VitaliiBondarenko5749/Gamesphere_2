import { ForumUserInfoModel } from "./forum-user-info.model";

export class ShortPostInfoModel{
    constructor(
        public id: string,
        public topic: string,
        public createdAt: string,
        public views: number,
        public numberOfReplies: number,
        public userInfo: ForumUserInfoModel
    ){}
}
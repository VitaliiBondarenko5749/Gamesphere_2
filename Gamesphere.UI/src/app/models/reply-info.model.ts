import { ReplyOptions } from "../extensions/reply-options";
import { FullForumUserInfoModel } from "./full-forum-user-info-model";

export class ReplyInfoModel {
    constructor(
        public id: string,
        public postId: string,
        public content: string,
        public createdAt: string,
        public replyToId: string | null,
        public user: FullForumUserInfoModel,
        public options: ReplyOptions
    ) {}
}
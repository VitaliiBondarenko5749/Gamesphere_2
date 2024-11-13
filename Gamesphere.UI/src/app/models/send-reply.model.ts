export class SendReplyModel{
    constructor(
        public userId: string,
        public postId: string, 
        public content: string,
        public replyToId: string | null
    ){}
}
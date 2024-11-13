export class CommentInfoModel{
    constructor(
        public id: string,
        public content: string,
        public createdAt: string,
        public userId: string,
        public email: string,
        public role: string,
        public userName: string,
        public iconDirectory: string,
        public countOfLikes: number = 0,
        public isLiked: boolean = false
    ){}
}
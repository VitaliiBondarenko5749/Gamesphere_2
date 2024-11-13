export class AddPostModel{
    constructor(
        public subject: string,
        public text: string,
        public gameId: string | null,
        public userId: string
    ){}
}
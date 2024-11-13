export class ReplyOptions {
    constructor(
        public isFormVisible: boolean = false,
        public replyToText: string = 'Replied to',
        public replyToContent: string = ''
    ){}
}
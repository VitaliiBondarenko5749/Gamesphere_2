export class SendEmailModel{
    constructor(
        public sendTo: string,
        public subject: string,
        public message: string,
        public isHtml: boolean
    ){}
}
export class ChangeEmailModel{
    constructor(
        public userId: string | undefined,
        public email: string | undefined,
        public code: string | undefined
    ){}
}
export class UserAuthorizedModel{
    constructor(
        public id: string,
        public username: string, 
        public imageDirectory: string,
        public email: string,
        public role: string
    ){}
}
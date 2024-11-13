export class FullForumUserInfoModel {
    constructor(
        public id: string,
        public userName: string,
        public iconDirectory: string,
        public email: string,
        public role: string
    ) { }
}
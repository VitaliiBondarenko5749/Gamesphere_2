export class RegisterModel{
    constructor(
        public icon: File | null,
        public username: string,
        public email: string,
        public password: string
    ){}
}
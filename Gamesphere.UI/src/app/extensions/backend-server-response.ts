import { publicDecrypt } from "node:crypto";

export class BackendServerResponse {
    constructor(
        public message: string,
        public isSuccess: boolean,
        public errors?: string[],
        public expiredDate?: Date,
        public accessToken?: string,
        public refreshToken?: string
    ) {}
}
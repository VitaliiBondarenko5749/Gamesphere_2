export class GameInfoModel{
    constructor(
        public id: string,
        public name: string,
        public releaseDate: string,
        public description: string,
        public mainImageDirectory: string,
        public views: number,
        public publisherName: string,
        public categories: string[],
        public developers: string[],
        public languages: string[],
        public platforms: string[],
        public images: string[],
        public videoLinks: string[]
    ){}
}
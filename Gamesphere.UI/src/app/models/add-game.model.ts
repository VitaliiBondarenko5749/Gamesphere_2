import { AddGameImageModel } from "./add-game-image.model";
import { CategoryInfoModel } from "./category-info.model";
import { DeveloperInfoModel } from "./developer-info.model";
import { LanguageInfoModel } from "./language-info.model";
import { PlatformInfoModel } from "./platform-info.model";
import { PublisherInfoModel } from "./publisher-info.model";

export class AddGameModel{
    constructor(
        public icon: File | null,
        public name: string,
        public description: string,
        public releaseDate: string | null,
        public images: AddGameImageModel[],
        public videoLinks: string[],
        public categories: CategoryInfoModel[],
        public developers: DeveloperInfoModel[],
        public languages: LanguageInfoModel[],
        public platforms: PlatformInfoModel[],
        public publisher: PublisherInfoModel | null
    ){}
}
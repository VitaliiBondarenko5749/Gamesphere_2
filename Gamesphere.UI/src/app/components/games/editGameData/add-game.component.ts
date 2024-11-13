// Angular
import { Component } from "@angular/core";
import { NgStyle, NgIf, NgFor } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { DomSanitizer } from "@angular/platform-browser";

// Models
import { AddGameModel } from "../../../models/add-game.model";
import { AddGameImageModel } from "../../../models/add-game-image.model";
import { CategoryInfoModel } from "../../../models/category-info.model";
import { DeveloperInfoModel } from "../../../models/developer-info.model";
import { LanguageInfoModel } from "../../../models/language-info.model";
import { PlatformInfoModel } from "../../../models/platform-info.model";
import { PublisherInfoModel } from "../../../models/publisher-info.model";

// Services
import { GameService } from "../../../services/game.service";
import { AuthenticationService } from "../../../services/authentication.service";


@Component({
    selector: 'add-game-component',
    standalone: true,
    imports: [NgStyle, NgIf, FormsModule, NgFor],
    templateUrl: './add-game.component.html',
    styleUrl: './add-game.component.css'
})

export class AddGameComponent {
    selectedImage: any;
    game: AddGameModel;
    isGameNameAvailable: boolean;
    videoLinkText: string;

    categorySearchText: string;
    top10Categories: CategoryInfoModel[];

    developerSearchText: string;
    top10Developers: DeveloperInfoModel[];

    languageSearchText: string;
    top10Languages: LanguageInfoModel[];

    platformSearchText: string;
    top10Platforms: PlatformInfoModel[];

    publisherSearchText: string;
    top10Publishers: PublisherInfoModel[];

    constructor(private gameService: GameService, private sanitizer: DomSanitizer,
        private authenticationService: AuthenticationService) {
        this.game = new AddGameModel(null, '', '', null, [], [], [], [], [], [], null);
        this.isGameNameAvailable = true;
        this.videoLinkText = '';
        this.categorySearchText = '';
        this.top10Categories = [];
        this.developerSearchText = '';
        this.top10Developers = [];
        this.languageSearchText = '';
        this.top10Languages = [];
        this.platformSearchText = '';
        this.top10Platforms = [];
        this.publisherSearchText = '';
        this.top10Publishers = [];
    }

    resetForm() {
        this.game = new AddGameModel(null, '', '', null, [], [], [], [], [], [], null);
        this.isGameNameAvailable = true;
        this.videoLinkText = '';
        this.categorySearchText = '';
        this.top10Categories = [];
        this.developerSearchText = '';
        this.top10Developers = [];
        this.languageSearchText = '';
        this.top10Languages = [];
        this.platformSearchText = '';
        this.top10Platforms = [];
        this.publisherSearchText = '';
        this.top10Publishers = [];
        this.selectedImage = undefined;
    }

    onFileSelected(event: Event): void {
        const fileInput = event.target as HTMLInputElement;
        if (fileInput.files && fileInput.files.length > 0) {
            const file = fileInput.files[0];
            this.game.icon = fileInput.files[0];
            const reader = new FileReader();
            reader.onload = () => {
                this.selectedImage = reader.result;
            };
            reader.readAsDataURL(file);
        }
    }

    CheckGameNameExistence() {
        this.isGameNameAvailable = true;

        if (this.game.name.length > 0) {
            this.gameService.checkGameNameExistence(this.game.name)
                .subscribe(response => {
                    this.isGameNameAvailable = response.isSuccess;
                }, error => { });
        }
    }

    onImageSelected(event: Event): void {
        const fileInput = event.target as HTMLInputElement;

        if (fileInput.files && fileInput.files.length > 0) {
            let newImage = new AddGameImageModel(fileInput.files[0], '');
            const reader = new FileReader();
            reader.onload = () => {
                newImage.imageUrl = reader.result;
            };
            reader.readAsDataURL(newImage.file);
            this.game.images.push(newImage);
        }
    }

    deleteFromImages(image: AddGameImageModel) {
        let index = this.game.images.indexOf(image);

        if (index >= 0) {
            this.game.images.splice(index, 1);
        }
    }

    isYouTubeLink(text: string): boolean {
        const youtubeLinkRegex = /^(http(s)?:\/\/)?(www\.)?(youtube\.com\/watch\?v=|youtu\.be\/)([a-zA-Z0-9_-]{11})$/;

        return youtubeLinkRegex.test(text);
    }

    private getYoutubeVideoId(link: string): string | null {
        let startIndex = link.lastIndexOf('=');

        if (startIndex === -1) {
            startIndex = link.lastIndexOf('/');
        }

        if (startIndex === -1) {
            return null;
        }

        return link.substring(startIndex + 1);
    }

    addVideo() {
        if (this.isYouTubeLink(this.videoLinkText) === true) {
            let videoId = this.getYoutubeVideoId(this.videoLinkText);

            console.log(videoId);

            if (videoId) {
                this.game.videoLinks.push(videoId);
                this.videoLinkText = '';
            }
        }
    }

    getYoutubeFrameLink(videoId: string) {
        let link = `https://www.youtube.com/embed/${videoId}`;
        return this.sanitizer.bypassSecurityTrustResourceUrl(link);
    }

    deleteFromVideos(link: string) {
        let index = this.game.videoLinks.indexOf(link);

        if (index >= 0) {
            this.game.videoLinks.splice(index, 1);
        }
    }

    getTop10Categories() {
        if (this.categorySearchText !== '') {
            this.gameService.getTop10Categories(this.categorySearchText)
                .subscribe(response => {
                    this.top10Categories = response;
                }, error => { })
        } else {
            this.top10Categories = [];
        }
    }

    existsInCategoryStack(category: CategoryInfoModel): boolean {
        return this.game.categories.some(c => c.name === category.name);
    }

    getTop10Developers() {
        if (this.developerSearchText !== '') {
            this.gameService.getTop10Developers(this.developerSearchText)
                .subscribe(response => {
                    this.top10Developers = response;
                }, error => { })
        } else {
            this.top10Developers = [];
        }
    }

    existsInDeveloperStack(developer: DeveloperInfoModel): boolean {
        return this.game.developers.some(d => d.name === developer.name);
    }

    getTop10Languages() {
        if (this.languageSearchText !== '') {
            this.gameService.getTop10Languages(this.languageSearchText)
                .subscribe(response => {
                    this.top10Languages = response;
                }, error => { })
        } else {
            this.top10Languages = [];
        }
    }

    existsInLanguageStack(language: LanguageInfoModel): boolean {
        return this.game.languages.some(l => l.name === language.name);
    }

    getTop10Platforms() {
        if (this.platformSearchText !== '') {
            this.gameService.getTop10Platforms(this.platformSearchText)
                .subscribe(response => {
                    this.top10Platforms = response;
                }, error => { })
        } else {
            this.top10Platforms = [];
        }
    }

    existsInPlatformStack(platform: PlatformInfoModel): boolean {
        return this.game.platforms.some(p => p.name === platform.name);
    }

    getTop10Publishers() {
        if (this.publisherSearchText !== '') {
            this.gameService.getTop10Publishers(this.publisherSearchText)
                .subscribe(response => {
                    this.top10Publishers = response;
                }, error => { })
        } else {
            this.top10Publishers = [];
        }
    }

    addGame() {
        if (this.game.icon !== null && this.isGameNameAvailable === true && this.game.description.length > 70
            && this.game.publisher !== null && this.game.images.length > 0 && this.game.releaseDate !== null) {
            let accessToken = this.authenticationService.getAccessToken();

            if (accessToken) {
                this.gameService.addGame(accessToken, this.game.name, this.game.description, this.game.publisher.id, this.game.releaseDate)
                    .subscribe(error => {
                        console.log(error);
                    });

                setTimeout(() => { }, 2000);

                this.game.videoLinks.forEach(videoLink => {
                    setTimeout(() => {
                        this.gameService.addVideoLink(accessToken, this.game.name, videoLink)
                            .subscribe(error => { });
                    }, 2000);
                });

                this.game.categories.forEach(category => {
                    setTimeout(() => {
                        this.gameService.addCategoryToGame(accessToken, this.game.name, category.id)
                            .subscribe(error => { });
                    }, 2000);
                });

                this.game.developers.forEach(developer => {
                    setTimeout(() => {
                        this.gameService.addDeveloperToGame(accessToken, this.game.name, developer.id)
                            .subscribe(error => { });
                    }, 2000);
                });

                this.game.languages.forEach(language => {
                    setTimeout(() => {
                        this.gameService.addLanguageToGame(accessToken, this.game.name, language.id)
                            .subscribe(error => { });
                    }, 2000);
                });

                this.game.platforms.forEach(platform => {
                    setTimeout(() => {
                        this.gameService.addPlatformToGame(accessToken, this.game.name, platform.id)
                        .subscribe(error => { });
                    }, 2000);
                });

                setTimeout(() => {
                    if (this.game.icon) {
                        this.gameService.uploadGameImage(accessToken, this.game.icon, this.game.name, "Games/Icons")
                            .subscribe(error => { });
                    }
                }, 2000);

                let completedUploads = 0;
                const totalUploads = this.game.images.length;

                this.game.images.forEach((image, index) => {
                    setTimeout(() => {
                        this.gameService.uploadGameImage(accessToken, image.file, this.game.name, "Games/Images")
                            .subscribe({
                                next: () => {
                                    completedUploads++;
                                    if (completedUploads === totalUploads) {
                                        alert(`Game ${this.game.name} has been added!`);
                                        setTimeout(() => {
                                            this.resetForm();
                                        }, 3000)
                                    }
                                },
                                error: () => {
                                    console.error(`Failed to upload image at index ${index}`);
                                    completedUploads++;
                                    if (completedUploads === totalUploads) {
                                        alert(`Game ${this.game.name} has been added!`);
                                        this.resetForm();
                                    }
                                }
                            });
                    }, 3000);
                });
            }
        }
    }
}
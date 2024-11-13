import { NgModule } from "@angular/core";
import { QuillModule } from "ngx-quill";
import QuillResizeModule from "@botom/quill-resize-module";

@NgModule({
    imports: [QuillModule.forRoot({
        modules: {
            resize: QuillResizeModule
        }
    })],
    exports: [QuillModule]
})

export class QuillEditorModule{}
import { Injectable } from "@angular/core";
import { SendEmailModel } from "../models/send-email.model";
import { HttpClient, HttpHeaders } from "@angular/common/http";

@Injectable({
    providedIn: 'root'
})
export class EmailService{
    private baseUrl: string = "http://localhost:1000";

    constructor(private http: HttpClient){}

    public sendEmail(model: SendEmailModel){
        const headers = new HttpHeaders({
            'Content-Type': 'application/json'
          });

          return this.http.post(`${this.baseUrl}/send-email`, model, {headers: headers})
    }
}
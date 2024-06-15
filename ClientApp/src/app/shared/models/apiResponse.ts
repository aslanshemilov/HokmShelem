export class ApiResponse {
    statusCode: number;
    title: string;
    message: string;
    details: string;
    errors: string[];
    isHtmlEnabled: boolean;
    displayByDefault: boolean;
    showWithToastr: boolean;

    constructor(statusCode: number, title: string, message: string, details: string, errors: string[], 
        isHtmlEnabled: boolean = false, displayByDefault: boolean = false, showWithToastr: boolean = false) {
        this.statusCode = statusCode;
        this.title = title;
        this.message = message;
        this.details = details;
        this.errors = errors;
        this.isHtmlEnabled = isHtmlEnabled;
        this.displayByDefault = displayByDefault;
        this.showWithToastr = showWithToastr;
    }
}
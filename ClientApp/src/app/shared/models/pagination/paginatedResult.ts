export class PaginatedResult<T> {
    totalItemsCount: number;
    pageNumber: number;
    pageSize: number;
    totalPages: number;
    items: T;

    constructor(totalItemsCount: number, pageNumber: number, pageSize: number, totalPages: number, items: T) {
        this.totalItemsCount = totalItemsCount;
        this.pageNumber = pageNumber;
        this.pageSize = pageSize;
        this.totalPages = totalPages;
        this.items = items;
    }
}

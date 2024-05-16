export class A_MemberParams {
    pageNumber = 1;
    pageSize: number;
    sortBy: string = '';
    search: string = '';
    role: string = 'all';
    provider: string = 'all';
    status: string = 'all';

    constructor(pageSize: number) {
        this.pageSize = pageSize;
    }
}
export class PageRequest<T>{
   constructor(
    public count: number,
    public pageIndex: number,
    public pageSize: number,
    public items?: T[]
   ){}
}
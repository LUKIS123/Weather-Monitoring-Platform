export interface PageResult<T> {
  items: T[];
  totalPages: number;
  itemsFrom: number;
  itemsTo: number;
  totalItemsCount: number;
  pageSize: number;
}

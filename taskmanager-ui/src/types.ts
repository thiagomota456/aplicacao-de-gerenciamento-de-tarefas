// Tipos alinhados ao backend .NET
export interface AuthResponse { userId: string; username: string; accessToken: string; expiresAt: string; }
export interface PagedResponse<T> { items: T[]; page: number; pageSize: number; totalItems: number; }

export interface TaskDto {
  id: string; userId: string; title: string; description: string;
  categoryId: number | null; isCompleted: boolean; created: string; updatedAt: string;
}
export interface TaskCreate { userId: string; title: string; description: string; isCompleted?: boolean; categoryId?: number; }
export interface TaskUpdate { title: string; description: string; isCompleted: boolean; categoryId?: number; }
export interface TaskQuery {
  CategoryId?: number; IsCompleted?: '' | boolean; Search?: string;
  SortBy?: 'title'|'created'|'updatedAt'; SortDir?: 'asc'|'desc'; Page?: number; PageSize?: number;
}

export interface CategoryDto { id: number; description?: string | null; }
export interface CategoryCreate { description?: string | null; }
export interface CategoryUpdate { description?: string | null; }
export interface CategoryQuery { Search?: string; SortBy?: 'id'|'description'; SortDir?: 'asc'|'desc'; Page?: number; PageSize?: number; }

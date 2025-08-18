import { api } from './client';
import type { CategoryDto, CategoryCreate, CategoryUpdate, PagedResponse, CategoryQuery } from '../types';

export async function listCategories(q: CategoryQuery): Promise<PagedResponse<CategoryDto>> {
  const { data } = await api.get<PagedResponse<CategoryDto>>('/api/categories', { params: q });
  return data;
}
export async function getCategory(id: number): Promise<CategoryDto> {
  const { data } = await api.get<CategoryDto>(`/api/categories/${id}`);
  return data;
}
export async function createCategory(body: CategoryCreate): Promise<CategoryDto> {
  const { data } = await api.post<CategoryDto>('/api/categories', body);
  return data;
}
export async function updateCategory(id: number, body: CategoryUpdate): Promise<CategoryDto> {
  const { data } = await api.put<CategoryDto>(`/api/categories/${id}`, body);
  return data;
}
export async function deleteCategory(id: number): Promise<void> {
  await api.delete(`/api/categories/${id}`);
}
import { api } from './client';
import type { TaskDto, TaskCreate, TaskUpdate, PagedResponse, TaskQuery } from '../types';

export async function listTasks(q: TaskQuery): Promise<PagedResponse<TaskDto>> {
  const { data } = await api.get<PagedResponse<TaskDto>>('/api/tasks', { params: q });
  return data;
}
export async function getTask(id: string): Promise<TaskDto> {
  const { data } = await api.get<TaskDto>(`/api/tasks/${id}`);
  return data;
}
export async function createTask(body: TaskCreate): Promise<TaskDto> {
  const { data } = await api.post<TaskDto>('/api/tasks', body);
  return data;
}
export async function updateTask(id: string, body: TaskUpdate): Promise<TaskDto> {
  const { data } = await api.put<TaskDto>(`/api/tasks/${id}`, body);
  return data;
}
export async function deleteTask(id: string): Promise<void> {
  await api.delete(`/api/tasks/${id}`);
}

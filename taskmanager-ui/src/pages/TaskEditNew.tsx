import * as React from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { getTask, createTask, updateTask } from '../api/tasks';
import { listCategories } from '../api/categories';
import TaskForm from './TaskForm';
import type { TaskDto, CategoryDto } from '../types';
import { Container, Alert } from '@mui/material';

const TaskEditNewPage: React.FC = () => {
  const { id } = useParams();
  const nav = useNavigate();
  const [task, setTask] = React.useState<TaskDto | null>(null);
  const [cats, setCats] = React.useState<CategoryDto[]>([]);
  const [error, setError] = React.useState<string | null>(null);
  const isEdit = Boolean(id);

  React.useEffect(() => {
    const run = async () => {
      try {
        const [catsRes, taskRes] = await Promise.all([
          listCategories({ Page: 1, PageSize: 200, SortBy: 'id', SortDir: 'asc' }),
          isEdit ? getTask(id!) : Promise.resolve(null as Any)
        ]);
        setCats(catsRes.items);
        if (isEdit) setTask(taskRes);
      } catch (e: Any) {
        setError(e?.response?.data ?? 'Falha ao carregar dados.');
      }
    };
    run();
  }, [id, isEdit]);

  const handleSubmit = async (payload: Any) => {
    if (isEdit) await updateTask(id!, payload);
    else await createTask(payload);
    nav('/tasks');
  };

  return (
    <Container maxWidth="md">
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      <TaskForm initial={isEdit ? task : null} categories={cats} onSubmit={handleSubmit} requireCategory={true} />
    </Container>
  );
};

export default TaskEditNewPage;
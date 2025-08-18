import * as React from 'react';
import { TextField, Button, Grid, Paper, FormControlLabel, Checkbox, MenuItem, Typography, Alert } from '@mui/material';
import type { CategoryDto, TaskCreate, TaskDto, TaskUpdate } from '../types';
import { useAuth } from '../auth/AuthContext';

type Props = {
  initial?: TaskDto | null;
  categories: CategoryDto[];
  onSubmit: (payload: TaskCreate | TaskUpdate) => Promise<void>;
  requireCategory?: boolean;
};

const TaskForm: React.FC<Props> = ({ initial, categories, onSubmit, requireCategory = true }) => {
  const { auth } = useAuth();
  const [title, setTitle] = React.useState(initial?.title ?? '');
  const [description, setDescription] = React.useState(initial?.description ?? '');
  const [isCompleted, setIsCompleted] = React.useState<boolean>(initial?.isCompleted ?? false);
  const [categoryId, setCategoryId] = React.useState<number | ''>(initial?.categoryId ?? '');
  const [error, setError] = React.useState<string | null>(null);
  const [loading, setLoading] = React.useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!title || title.length < 1 || title.length > 160) return setError('Título deve ter entre 1 e 160 caracteres.');
    if (!description || description.length < 1) return setError('Descrição é obrigatória.');
    if (requireCategory && (categoryId === '' || categoryId === null)) return setError('Selecione uma categoria.');

    try {
      setLoading(true);
      const payload: Any = initial
        ? { title, description, isCompleted, categoryId: categoryId === '' ? undefined : Number(categoryId) } as TaskUpdate
        : { userId: auth.userId!, title, description, isCompleted, categoryId: categoryId === '' ? undefined : Number(categoryId) } as TaskCreate;
      await onSubmit(payload);
    } catch (err) {
      setError(err?.response?.data ?? 'Erro ao salvar tarefa.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Paper sx={{ p: 3 }}>
      <Typography variant="h6" mb={2}>{initial ? 'Editar tarefa' : 'Nova tarefa'}</Typography>
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      <form onSubmit={handleSubmit}>
        <Grid container spacing={2}>
          <Grid item xs={12}>
            <TextField label="Título" value={title} onChange={e => setTitle(e.target.value)} fullWidth required inputProps={{ maxLength: 160 }} />
          </Grid>
          <Grid item xs={12}>
            <TextField label="Descrição" value={description} onChange={e => setDescription(e.target.value)} fullWidth required multiline minRows={3} />
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField select label="Categoria" value={categoryId} onChange={e => setCategoryId(e.target.value === '' ? '' : Number(e.target.value))} fullWidth required={requireCategory} helperText={requireCategory ? 'Selecione uma categoria' : 'Opcional'}>
              <MenuItem value="">(Sem categoria)</MenuItem>
              {categories.map(c => <MenuItem key={c.id} value={c.id}>{c.description ?? `Categoria ${c.id}`}</MenuItem>)}
            </TextField>
          </Grid>
          <Grid item xs={12} sm={6} display="flex" alignItems="center">
            <FormControlLabel control={<Checkbox checked={isCompleted} onChange={e => setIsCompleted(e.target.checked)} />} label="Concluída" />
          </Grid>
          <Grid item xs={12} display="flex" gap={2}>
            <Button type="submit" variant="contained" disabled={loading}>{loading ? 'Salvando...' : 'Salvar'}</Button>
          </Grid>
        </Grid>
      </form>
    </Paper>
  );
};

export default TaskForm;

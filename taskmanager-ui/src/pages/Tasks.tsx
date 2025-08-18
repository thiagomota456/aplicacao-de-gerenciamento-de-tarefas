import * as React from 'react';
import { useNavigate } from 'react-router-dom';
import { listTasks, deleteTask } from '../api/tasks';
import { listCategories } from '../api/categories';
import type { TaskDto, CategoryDto, TaskQuery } from '../types';
import { Box, Button, Container, MenuItem, Paper, Stack, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, TextField, Typography, Chip, Alert, Pagination, CircularProgress } from '@mui/material';
import ConfirmDialog from '../components/ConfirmDialog';

const TasksPage: React.FC = () => {
  const nav = useNavigate();
  const [filters, setFilters] = React.useState<TaskQuery>({ Search: '', SortBy: 'updatedAt', SortDir: 'desc', Page: 1, PageSize: 10 });
  const [data, setData] = React.useState<{ items: TaskDto[]; total: number }>({ items: [], total: 0 });
  const [cats, setCats] = React.useState<CategoryDto[]>([]);
  const [loading, setLoading] = React.useState(true);
  const [error, setError] = React.useState<string | null>(null);
  const [delId, setDelId] = React.useState<string | null>(null);

  const fetchAll = async () => {
    try {
      setLoading(true);
      const [tasksRes, catsRes] = await Promise.all([ listTasks(filters), listCategories({ Page:1, PageSize:200, SortBy:'id', SortDir:'asc' }) ]);
      setData({ items: tasksRes.items, total: tasksRes.totalItems });
      setCats(catsRes.items);
    } catch (e:Any) { setError(e?.response?.data ?? 'Falha ao carregar'); }
    finally { setLoading(false); }
  };
  React.useEffect(()=>
    { fetchAll();
    /* eslint-disable-next-line */}, 
    [filters.Page, filters.PageSize, filters.SortBy, filters.SortDir]);

  const handleSearch   = (e: React.ChangeEvent<HTMLInputElement>) => setFilters(f=>({ ...f, Search: e.target.value, Page:1 }));
  const handleCategory = (e: React.ChangeEvent<HTMLInputElement>) => setFilters(f=>({ ...f, CategoryId: e.target.value ? Number(e.target.value) : undefined, Page:1 }));
  const handleStatus   = (e: React.ChangeEvent<HTMLInputElement>) => setFilters(f=>({ ...f, IsCompleted: e.target.value === '' ? '' : e.target.value === 'true', Page:1 }));
  const handleSortBy   = (e: React.ChangeEvent<HTMLInputElement>) => setFilters(f=>({ ...f, SortBy: e.target.value as Any, Page:1 }));
  const handleSortDir  = (e: React.ChangeEvent<HTMLInputElement>) => setFilters(f=>({ ...f, SortDir: e.target.value as Any, Page:1 }));
  const handlePage     = (_:Any, p:number)=> setFilters(f=>({ ...f, Page:p }));

  const deleteSelected = async () => { if (!delId) return; try { setLoading(true); await deleteTask(delId); setDelId(null); await fetchAll(); } catch(e:Any){ setError(e?.response?.data ?? 'Erro ao excluir'); } finally { setLoading(false); } };

  return (<Container maxWidth="lg">
    <Stack direction="row" justifyContent="space-between" alignItems="center" mb={2}><Typography variant="h5">Tarefas</Typography><Button variant="contained" onClick={()=>nav('/tasks/new')}>Nova tarefa</Button></Stack>
    <Paper sx={{ p:2, mb:2 }}>
      <Stack direction={{ xs:'column', sm:'row' }} spacing={2}>
        <TextField label="Buscar" value={filters.Search ?? ''} onChange={handleSearch} fullWidth />
        <TextField select label="Categoria" value={filters.CategoryId ?? ''} onChange={handleCategory} sx={{ minWidth: 180 }}>
          <MenuItem value="">Todas</MenuItem>
          {cats.map(c => <MenuItem key={c.id} value={c.id}>{c.description ?? `Categoria ${c.id}`}</MenuItem>)}
        </TextField>
        <TextField select label="Status" value={filters.IsCompleted ?? ''} onChange={handleStatus} sx={{ minWidth: 160 }}>
          <MenuItem value="">Todos</MenuItem><MenuItem value="false">Pendentes</MenuItem><MenuItem value="true">Concluídas</MenuItem>
        </TextField>
        <TextField select label="Ordenar por" value={filters.SortBy} onChange={handleSortBy} sx={{ minWidth: 160 }}>
          <MenuItem value="updatedAt">Atualizado</MenuItem><MenuItem value="created">Criado</MenuItem><MenuItem value="title">Título</MenuItem>
        </TextField>
        <TextField select label="Direção" value={filters.SortDir} onChange={handleSortDir} sx={{ minWidth: 120 }}>
          <MenuItem value="desc">Desc</MenuItem><MenuItem value="asc">Asc</MenuItem>
        </TextField>
      </Stack>
    </Paper>
    {error && <Alert severity="error" sx={{ mb:2 }}>{error}</Alert>}
    {loading ? <Box display="flex" justifyContent="center" my={4}><CircularProgress/></Box> : (
      <TableContainer component={Paper}>
        <Table size="small">
          <TableHead><TableRow><TableCell>Título</TableCell><TableCell>Descrição</TableCell><TableCell>Categoria</TableCell><TableCell>Status</TableCell><TableCell>Atualizado</TableCell><TableCell align="right">Ações</TableCell></TableRow></TableHead>
          <TableBody>{data.items.map(t => (
            <TableRow key={t.id}>
              <TableCell>{t.title}</TableCell>
              <TableCell>{t.description.slice(0,80)}{t.description.length>80?'…':''}</TableCell>
              <TableCell>{cats.find(c => c.id === (t.categoryId ?? -1))?.description ?? '-'}</TableCell>
              <TableCell>{t.isCompleted ? <Chip label="Concluída" color="success" size="small"/> : <Chip label="Pendente" color="warning" size="small"/>}</TableCell>
              <TableCell>{new Date(t.updatedAt).toLocaleString()}</TableCell>
              <TableCell align="right"><Button size="small" onClick={()=>nav(`/tasks/${t.id}/edit`)}>Editar</Button><Button size="small" color="error" onClick={()=>setDelId(t.id)}>Excluir</Button></TableCell>
            </TableRow>
          ))}</TableBody>
        </Table>
        <Box display="flex" justifyContent="center" my={2}><Pagination count={Math.max(1, Math.ceil(data.total / (filters.PageSize || 10)))} page={filters.Page || 1} onChange={handlePage} /></Box>
      </TableContainer>
    )}
    <ConfirmDialog open={!!delId} title="Excluir tarefa" message="Tem certeza que deseja excluir esta tarefa?" onCancel={()=>setDelId(null)} onConfirm={deleteSelected} confirmText="Excluir"/>
  </Container>);
};
export default TasksPage;
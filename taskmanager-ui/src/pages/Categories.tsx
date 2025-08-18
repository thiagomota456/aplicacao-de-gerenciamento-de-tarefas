import * as React from 'react';
import { listCategories, createCategory, updateCategory, deleteCategory } from '../api/categories';
import type { CategoryDto, CategoryQuery } from '../types';
import { Box, Button, Container, Dialog, DialogActions, DialogContent, DialogTitle, IconButton, Paper, Stack, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, TextField, Typography, Alert, Pagination, MenuItem } from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';

const CategoriesPage: React.FC = () => {
  const [filters, setFilters] = React.useState<CategoryQuery>({ Search:'', SortBy:'id', SortDir:'asc', Page:1, PageSize:10 });
  const [data, setData] = React.useState<{ items: CategoryDto[]; total: number }>({ items: [], total: 0 });
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [loading, setLoading] = React.useState(true);
  const [error, setError] = React.useState<string | null>(null);
  const [open, setOpen] = React.useState(false);
  const [edit, setEdit] = React.useState<CategoryDto | null>(null);
  const [desc, setDesc] = React.useState('');

  const fetchAll = async () => {
    try {
      setLoading(true);
      const res = await listCategories(filters);
      setData({ items: res.items, total: res.totalItems });
    } catch (e:Any) { setError(e?.response?.data ?? 'Falha ao carregar categorias'); }
    finally { setLoading(false); }
  };

  React.useEffect(() => {
    fetchAll();
    /* eslint-disable-next-line */
  }, [filters.Page, filters.PageSize, filters.SortBy, filters.SortDir]);

  const openCreate = () => { setEdit(null); setDesc(''); setOpen(true); };
  const openEdit = (c: CategoryDto) => { setEdit(c); setDesc(c.description ?? ''); setOpen(true); };
  const handleSave = async () => {
    try {
      if (edit) await updateCategory(edit.id, { description: desc });
      else await createCategory({ description: desc });
      setOpen(false);
      await fetchAll();
    } catch (e:Any) { setError(e?.response?.data ?? 'Erro ao salvar categoria'); }
  };
  const handleDelete = async (id: number) => {
    try { await deleteCategory(id); await fetchAll(); }
    catch (e:Any) { setError(e?.response?.status===409 ? 'Não é possível excluir: existem tarefas vinculadas.' : (e?.response?.data ?? 'Erro ao excluir')); }
  };

  return (
    <Container maxWidth="md">
      <Stack direction="row" justifyContent="space-between" alignItems="center" mb={2}>
        <Typography variant="h5">Categorias</Typography>
        <Button variant="contained" onClick={openCreate}>Nova categoria</Button>
      </Stack>

      <Paper sx={{ p: 2, mb: 2 }}>
  <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
    <TextField
      label="Buscar"
      value={filters.Search ?? ''}
      onChange={(e) => {
        const v = e.target.value;
        setFilters(f => ({ ...f, Search: v, Page: 1 }));
        // garante filtros atualizados antes de buscar
        setTimeout(() => fetchAll(), 0);
      }}
      onKeyUp={() => {
        // dispara filtragem ao soltar a tecla
        fetchAll();
      }}
      fullWidth
    />

    <TextField
      select
      fullWidth
      label="Direção"
      value={filters.SortDir}
      onChange={(e) => {
        const v = e.target.value as 'asc' | 'desc';
        setFilters(f => ({ ...f, SortDir: v, Page: 1 }));
        setTimeout(() => fetchAll(), 0);
      }}
      sx={{ minWidth: 120 }}
    >
      <MenuItem value="asc">Asc</MenuItem>
      <MenuItem value="desc">Desc</MenuItem>
    </TextField>
  </Stack>
</Paper>


      {error && <Alert severity="error" sx={{ mb:2 }}>{error}</Alert>}

      <TableContainer component={Paper}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Id</TableCell>
              <TableCell>Descrição</TableCell>
              <TableCell align="right">Ações</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {data.items.map(c => (
              <TableRow key={c.id}>
                <TableCell>{c.id}</TableCell>
                <TableCell>{c.description ?? '-'}</TableCell>
                <TableCell align="right">
                  <IconButton size="small" onClick={()=>openEdit(c)}><EditIcon/></IconButton>
                  <IconButton size="small" color="error" onClick={()=>handleDelete(c.id)}><DeleteIcon/></IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
        <Box display="flex" justifyContent="center" my={2}>
          <Pagination count={Math.max(1, Math.ceil(data.total / (filters.PageSize || 10)))} page={filters.Page || 1} onChange={(_,p)=>setFilters(f=>({...f, Page:p}))}/>
        </Box>
      </TableContainer>

      <Dialog open={open} onClose={()=>setOpen(false)}>
        <DialogTitle>{edit ? 'Editar categoria' : 'Nova categoria'}</DialogTitle>
        <DialogContent>
          <TextField autoFocus margin="dense" label="Descrição" fullWidth value={desc} onChange={e=>setDesc(e.target.value)} />
        </DialogContent>
        <DialogActions>
          <Button onClick={()=>setOpen(false)}>Cancelar</Button>
          <Button variant="contained" onClick={handleSave}>Salvar</Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};

export default CategoriesPage;
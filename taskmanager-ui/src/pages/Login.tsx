import * as React from 'react';
import { useNavigate, Link as RouterLink } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { Box, Button, Container, Paper, TextField, Typography, Alert } from '@mui/material';

const LoginPage: React.FC = () => {
  const { login } = useAuth();
  const nav = useNavigate();
  const [username, setUsername] = React.useState('admin');
  const [password, setPassword] = React.useState('admin12345');
  const [error, setError] = React.useState<string | null>(null);
  const [loading, setLoading] = React.useState(false);

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      await login(username, password);
      nav('/tasks');
    } catch (err) {
      setError(err?.response?.data ?? 'Falha no login');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container maxWidth="sm">
      <Paper sx={{ p: 4, mt: 6 }} elevation={3}>
        <Typography variant="h5" mb={2}>Entrar</Typography>
        {error && <Alert severity="error" sx={{ mb: 2 }}>{String(error)}</Alert>}
        <Box component="form" onSubmit={onSubmit}>
          <TextField label="Usuário" fullWidth margin="normal" value={username} onChange={e => setUsername(e.target.value)} />
          <TextField label="Senha" type="password" fullWidth margin="normal" value={password} onChange={e => setPassword(e.target.value)} />
          <Button type="submit" variant="contained" fullWidth disabled={loading} sx={{ mt: 2 }}>
            {loading ? 'Entrando...' : 'Entrar'}
          </Button>
          <Button component={RouterLink} to="/auth/register" fullWidth sx={{ mt: 1 }}>
            Não tem conta? Registre-se
          </Button>
        </Box>
      </Paper>
    </Container>
  );
};

export default LoginPage;
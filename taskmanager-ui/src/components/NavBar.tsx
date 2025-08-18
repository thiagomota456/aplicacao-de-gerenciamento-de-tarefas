import * as React from 'react';
import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import { Link as RouterLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';

const NavBar: React.FC = () => {
  const { auth, logout } = useAuth();
  const navigate = useNavigate();

  return (
    <Box sx={{ flexGrow: 1, mb: 2 }}>
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6" sx={{ flexGrow: 1 }}>TaskManager</Typography>
          {auth.accessToken ? (
            <>
              <Button color="inherit" component={RouterLink} to="/tasks">Tarefas</Button>
              <Button color="inherit" component={RouterLink} to="/categories">Categorias</Button>
              <Button color="inherit" onClick={() => { logout(); navigate('/auth/login'); }}>Sair</Button>
            </>
          ) : (
            <>
              <Button color="inherit" component={RouterLink} to="/auth/login">Entrar</Button>
              <Button color="inherit" component={RouterLink} to="/auth/register">Registrar</Button>
            </>
          )}
        </Toolbar>
      </AppBar>
    </Box>
  );
};

export default NavBar;

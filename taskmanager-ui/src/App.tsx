import * as React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import NavBar from './components/NavBar';
import LoginPage from './pages/Login';
import RegisterPage from './pages/Register';
import TasksPage from './pages/Tasks';
import TaskEditNewPage from './pages/TaskEditNew';
import CategoriesPage from './pages/Categories';
import PrivateRoute from './auth/PrivateRoute';
import { Container } from '@mui/material';

const App: React.FC = () => {
  return (
    <>
      <NavBar />
      <Container sx={{ pb: 4 }}>
        <Routes>
          <Route path="/" element={<Navigate to="/tasks" />} />
          <Route path="/auth/login" element={<LoginPage />} />
          <Route path="/auth/register" element={<RegisterPage />} />

          <Route element={<PrivateRoute />}>
            <Route path="/tasks" element={<TasksPage />} />
            <Route path="/tasks/new" element={<TaskEditNewPage />} />
            <Route path="/tasks/:id/edit" element={<TaskEditNewPage />} />
            <Route path="/categories" element={<CategoriesPage />} />
          </Route>

          <Route path="*" element={<Navigate to="/" />} />
        </Routes>
      </Container>
    </>
  );
};

export default App;
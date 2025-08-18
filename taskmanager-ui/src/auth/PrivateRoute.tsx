import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from './AuthContext';

const PrivateRoute: React.FC = () => {
  const { auth } = useAuth();
  return auth.accessToken ? <Outlet /> : <Navigate to="/auth/login" replace />;
};

export default PrivateRoute;
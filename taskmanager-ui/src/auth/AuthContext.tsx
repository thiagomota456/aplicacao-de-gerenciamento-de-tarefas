import React, { createContext, useContext, useMemo, useState } from 'react';
import { login as apiLogin, register as apiRegister } from '../api/auth';
import type { AuthResponse } from '../types';

type AuthState = { accessToken: string | null; userId: string | null; username: string | null; };
type AuthContextType = { auth: AuthState; login: (u:string,p:string)=>Promise<void>; register: (u:string,p:string)=>Promise<void>; logout:()=>void; };

const AuthContext = createContext<AuthContextType | null>(null);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [auth, setAuth] = useState<AuthState>(() => ({
    accessToken: localStorage.getItem('accessToken'),
    userId: localStorage.getItem('userId'),
    username: localStorage.getItem('username'),
  }));

  const apply = (res: AuthResponse) => {
    localStorage.setItem('accessToken', res.accessToken);
    localStorage.setItem('userId', res.userId);
    localStorage.setItem('username', res.username);
    setAuth({ accessToken: res.accessToken, userId: res.userId, username: res.username });
  };

  const login = async (u: string, p: string) => apply(await apiLogin(u, p));
  const register = async (u: string, p: string) => apply(await apiRegister(u, p));

  const logout = () => {
    localStorage.removeItem('accessToken'); localStorage.removeItem('userId'); localStorage.removeItem('username');
    setAuth({ accessToken: null, userId: null, username: null });
  };

  const value = useMemo(() => ({ auth, login, register, logout }), [auth]);
  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

// eslint-disable-next-line react-refresh/only-export-components
export const useAuth = () => { const ctx = useContext(AuthContext); if (!ctx) throw new Error('useAuth must be used within AuthProvider'); return ctx; };
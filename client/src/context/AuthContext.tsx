import React,{createContext,useContext,useEffect,useState} from 'react';
import type { AuthContextType } from '../types';
import { authService } from '../services/authService';

const AuthContext=createContext<AuthContextType | undefined>(undefined);

export const AuthProvider:React.FC<{children:React.ReactNode}>=({children})=>{
 const [isAuthenticated,setIsAuthenticated]=useState(false);
 const [username,setUsername]=useState<string|null>(null);
 const [token,setToken]=useState<string|null>(null);
 const [loading,setLoading]=useState(true);

 useEffect(()=>{
   const t=localStorage.getItem('token');
   const u=localStorage.getItem('username');

   if(t&&u){
      setToken(t);
      setUsername(u);
      setIsAuthenticated(true);
   }

   setLoading(false);
 },[]);

 const login=async(username:string,password:string)=>{
   const r=await authService.login(username,password);
   setToken(r.token);
   setUsername(r.username);
   setIsAuthenticated(true);
 };

 const register=async(username:string,email:string,password:string)=>{
   const r=await authService.register(username,email,password);
   setToken(r.token);
   setUsername(r.username);
   setIsAuthenticated(true);
 };

 const logout=()=>{
   authService.logout();
   setToken(null);
   setUsername(null);
   setIsAuthenticated(false);
 };

 return (
 <AuthContext.Provider value={{
   isAuthenticated,
   username,
   token,
   login,
   register,
   logout,
   loading
 } as any}>
 {children}
 </AuthContext.Provider>
 );
};

export const useAuth=()=>{
 const ctx=useContext(AuthContext);
 if(!ctx) throw new Error('useAuth must be used within AuthProvider');
 return ctx;
}
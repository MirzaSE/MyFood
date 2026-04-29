import apiClient from './api';
import type { LoginRequest, RegisterRequest, AuthResponse } from '../types';

export const authService = {
  async login(username:string,password:string):Promise<AuthResponse>{
    try{
      const response=await apiClient.post<AuthResponse>('/authenticate/login',{
        username,
        password,
      } as LoginRequest);

      if(response.data.token){
        localStorage.setItem('token',response.data.token);
        localStorage.setItem('username',response.data.username);
      }

      return response.data;
    }catch(error:any){
      throw new Error(
        error?.response?.data?.message || 'Login failed'
      );
    }
  },

  async register(username:string,email:string,password:string):Promise<AuthResponse>{
    try{
      const response=await apiClient.post<AuthResponse>('/authenticate/register',{
        username,
        email,
        password,
      } as RegisterRequest);

      if(response.data.token){
        localStorage.setItem('token',response.data.token);
        localStorage.setItem('username',response.data.username);
      }

      return response.data;
    }catch(error:any){
      throw new Error(
        error?.response?.data?.message || 'Registration failed'
      );
    }
  },

  logout(){
    localStorage.removeItem('token');
    localStorage.removeItem('username');
  }
};
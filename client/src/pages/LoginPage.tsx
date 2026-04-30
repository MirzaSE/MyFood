import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { useAuth } from '../context/AuthContext';
import { AlertCircle } from 'lucide-react';

type LoginFormData = {
  username: string;
  password: string;
};

type RegisterFormData = {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
};

export const LoginPage: React.FC = () => {
  const [activeTab, setActiveTab] = useState<'login' | 'register'>('login');
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();
  const { login, register: registerUser } = useAuth();

  const loginForm = useForm<LoginFormData>();
  const registerForm = useForm<RegisterFormData>();

  const getErrorMessage = (err: any, fallback: string) => {
    const data = err.response?.data;

    if (err.response?.status === 401) {
      return 'Invalid username or password';
    }

    if (typeof data === 'string') return data;
    if (data?.message) return data.message;
    if (data?.title) return data.title;

    if (data?.errors) {
      const firstError = Object.values(data.errors).flat()[0];
      return String(firstError);
    }

    return fallback;
  };

  const handleLogin = async (data: LoginFormData) => {
    try {
      setError(null);
      await login(data.username, data.password);
      navigate('/foods');
    } catch (err: any) {
      setError(getErrorMessage(err, 'Invalid username or password.'));
    }
  };

  const handleRegister = async (data: RegisterFormData) => {
    try {
      setError(null);

      if (data.password !== data.confirmPassword) {
        setError('Passwords do not match.');
        return;
      }

      await registerUser(data.username, data.email, data.password);
      navigate('/foods');
    } catch (err: any) {
      setError(getErrorMessage(err, 'Registration failed. Please check user details.'));
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900 flex items-center justify-center p-4">
      <div className="absolute top-0 left-0 w-96 h-96 bg-purple-500 rounded-full mix-blend-multiply filter blur-3xl opacity-20 animate-pulse"></div>
      <div className="absolute bottom-0 right-0 w-96 h-96 bg-blue-500 rounded-full mix-blend-multiply filter blur-3xl opacity-20 animate-pulse delay-700"></div>

      <div className="relative z-10 w-full max-w-md">
        <div className="backdrop-blur-xl bg-white/10 border border-white/20 rounded-2xl shadow-2xl overflow-hidden">
          <div className="bg-gradient-to-r from-purple-600 to-blue-600 px-8 py-14 text-center">
            <h1 className="text-4xl font-bold text-white mb-2">MyFood</h1>
            <p className="text-white/80 text-sm">Manage your meals with ease</p>
          </div>

          {error && (
            <div className="mx-6 mt-6 p-4 bg-red-500/20 border border-red-500/50 rounded-lg flex items-start space-x-3">
              <AlertCircle size={20} className="text-red-400 flex-shrink-0 mt-0.5" />
              <p className="text-red-200 text-sm">{error}</p>
            </div>
          )}

          <div className="flex border-b border-white/10 px-6 pt-6">
            <button
              onClick={() => {
                setActiveTab('login');
                setError(null);
              }}
              className={`px-4 py-3 font-semibold text-sm transition-all relative ${
                activeTab === 'login'
                  ? 'text-transparent bg-clip-text bg-gradient-to-r from-purple-400 to-blue-400'
                  : 'text-gray-400 hover:text-gray-300'
              }`}
            >
              Login
              {activeTab === 'login' && (
                <div className="absolute bottom-0 left-0 right-0 h-0.5 bg-gradient-to-r from-purple-400 to-blue-400"></div>
              )}
            </button>

            <button
              onClick={() => {
                setActiveTab('register');
                setError(null);
              }}
              className={`px-4 py-3 font-semibold text-sm transition-all relative ${
                activeTab === 'register'
                  ? 'text-transparent bg-clip-text bg-gradient-to-r from-purple-400 to-blue-400'
                  : 'text-gray-400 hover:text-gray-300'
              }`}
            >
              Register
              {activeTab === 'register' && (
                <div className="absolute bottom-0 left-0 right-0 h-0.5 bg-gradient-to-r from-purple-400 to-blue-400"></div>
              )}
            </button>
          </div>

          <div className="px-8 py-8">
            {activeTab === 'login' && (
              <form onSubmit={loginForm.handleSubmit(handleLogin)} className="food-form">
                <div>
                  <label className="block text-sm font-semibold text-gray-200 mb-2">
                    Username
                  </label>
                  <input
                    {...loginForm.register('username', {
                      required: 'Username is required',
                      minLength: {
                        value: 3,
                        message: 'Username must be at least 3 characters',
                      },
                    })}
                    type="text"
                    className="w-full pl-10 pr-4 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all text-base"
                    placeholder="Enter your username"
                  />
                  {loginForm.formState.errors.username && (
                    <span className="text-red-400 text-xs mt-1 block">
                      {loginForm.formState.errors.username.message}
                    </span>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-semibold text-gray-200 mb-2">
                    Password
                  </label>
                  <input
                    {...loginForm.register('password', {
                      required: 'Password is required',
                      minLength: {
                        value: 6,
                        message: 'Password must be at least 6 characters',
                      },
                    })}
                    type="password"
                    className="w-full pl-10 pr-4 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all text-base"
                    placeholder="Enter your password"
                  />
                  {loginForm.formState.errors.password && (
                    <span className="text-red-400 text-xs mt-1 block">
                      {loginForm.formState.errors.password.message}
                    </span>
                  )}
                </div>

                <button
                  type="submit"
                  style={{ marginTop: '2rem' }}
                  disabled={loginForm.formState.isSubmitting}
                  className="w-full mt-8 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white py-3 rounded-lg font-semibold transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed shadow-lg hover:shadow-purple-500/50"
                >
                  {loginForm.formState.isSubmitting ? 'Logging in...' : 'Sign In'}
                </button>
              </form>
            )}

            {activeTab === 'register' && (
              <form onSubmit={registerForm.handleSubmit(handleRegister)} className="food-form">
                <div>
                  <label className="block text-sm font-semibold text-gray-200 mb-2">
                    Username
                  </label>
                  <input
                    {...registerForm.register('username', {
                      required: 'Username is required',
                      minLength: {
                        value: 3,
                        message: 'Username must be at least 3 characters',
                      },
                    })}
                    type="text"
                    className="w-full pl-10 pr-4 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all text-base"
                    placeholder="Choose a username"
                  />
                  {registerForm.formState.errors.username && (
                    <span className="text-red-400 text-xs mt-1 block">
                      {registerForm.formState.errors.username.message}
                    </span>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-semibold text-gray-200 mb-2">
                    Email
                  </label>
                  <input
                    {...registerForm.register('email', {
                      required: 'Email is required',
                      pattern: {
                        value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                        message: 'Please enter a valid email address',
                      },
                    })}
                    type="email"
                    className="w-full pl-10 pr-4 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all text-base"
                    placeholder="Enter your email"
                  />
                  {registerForm.formState.errors.email && (
                    <span className="text-red-400 text-xs mt-1 block">
                      {registerForm.formState.errors.email.message}
                    </span>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-semibold text-gray-200 mb-2">
                    Password
                  </label>
                  <input
                    {...registerForm.register('password', {
                      required: 'Password is required',
                      minLength: {
                        value: 6,
                        message: 'Password must be at least 6 characters',
                      },
                    })}
                    type="password"
                    className="w-full pl-10 pr-4 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all text-base"
                    placeholder="Enter a password"
                  />
                  {registerForm.formState.errors.password && (
                    <span className="text-red-400 text-xs mt-1 block">
                      {registerForm.formState.errors.password.message}
                    </span>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-semibold text-gray-200 mb-2">
                    Confirm Password
                  </label>
                  <input
                    {...registerForm.register('confirmPassword', {
                      required: 'Please confirm your password',
                      validate: (value) =>
                        value === registerForm.getValues('password') || 'Passwords do not match',
                    })}
                    type="password"
                    className="w-full pl-10 pr-4 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all text-base"
                    placeholder="Confirm your password"
                  />
                  {registerForm.formState.errors.confirmPassword && (
                    <span className="text-red-400 text-xs mt-1 block">
                      {registerForm.formState.errors.confirmPassword.message}
                    </span>
                  )}
                </div>

                <button
                  type="submit"
                  style={{ marginTop: '2rem' }}
                  disabled={registerForm.formState.isSubmitting}
                  className="w-full mt-8 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white py-3 rounded-lg font-semibold transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed shadow-lg hover:shadow-purple-500/50"
                >
                  {registerForm.formState.isSubmitting ? 'Creating account...' : 'Create Account'}
                </button>
              </form>
            )}
          </div>
        </div>

        <p className="text-center text-gray-400 text-xs mt-8">
          Secure authentication with JWT tokens
        </p>
      </div>
    </div>
  );
};
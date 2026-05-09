import { render, screen, fireEvent } from '@testing-library/react';
import { LoginPage } from './LoginPage';
import * as AuthContext from '../context/useAuth';

// E2E: Simulate a full user login and registration flow
const mockLogin = jest.fn(async () => Promise.resolve());
const mockRegister = jest.fn(async () => Promise.resolve());

jest.spyOn(AuthContext, 'useAuth').mockReturnValue({
  login: mockLogin,
  register: mockRegister,
});

describe('LoginPage E2E', () => {
  beforeEach(() => {
    mockLogin.mockClear();
    mockRegister.mockClear();
  });

  it('user can register and then login', async () => {
    render(<LoginPage />);
    // Register
    fireEvent.click(screen.getByText(/register/i));
    fireEvent.change(screen.getByPlaceholderText(/choose a username/i), { target: { value: 'newuser' } });
    fireEvent.change(screen.getByPlaceholderText(/enter your email/i), { target: { value: 'mail@mail.com' } });
    fireEvent.change(screen.getByPlaceholderText(/enter a password/i), { target: { value: 'pass123' } });
    fireEvent.change(screen.getByPlaceholderText(/confirm your password/i), { target: { value: 'pass123' } });
    fireEvent.click(screen.getByRole('button', { name: /create account/i }));
    expect(mockRegister).toHaveBeenCalledWith('newuser', 'mail@mail.com', 'pass123');

    // Login
    fireEvent.click(screen.getByText(/login/i));
    fireEvent.change(screen.getByPlaceholderText(/enter your username/i), { target: { value: 'newuser' } });
    fireEvent.change(screen.getByPlaceholderText(/enter your password/i), { target: { value: 'pass123' } });
    fireEvent.click(screen.getByRole('button', { name: /sign in/i }));
    expect(mockLogin).toHaveBeenCalledWith('newuser', 'pass123');
  });
});

import { render, screen, fireEvent } from '@testing-library/react';
import { LoginPage } from './LoginPage';
import * as AuthContext from '../context/AuthContext';

const mockLogin = jest.fn();
const mockRegister = jest.fn();

jest.spyOn(AuthContext, 'useAuth').mockReturnValue({
  login: mockLogin,
  register: mockRegister,
});

describe('LoginPage', () => {
  beforeEach(() => {
    mockLogin.mockReset();
    mockRegister.mockReset();
  });

  it('shows validation errors on empty submit', async () => {
    render(<LoginPage />);
    fireEvent.click(screen.getByText(/login/i));
    fireEvent.click(screen.getByRole('button', { name: /sign in/i }));
    expect(await screen.findAllByText(/required/i)).toHaveLength(2);
  });

  it('calls login with correct data', async () => {
    render(<LoginPage />);
    fireEvent.click(screen.getByText(/login/i));
    fireEvent.change(screen.getByPlaceholderText(/enter your username/i), { target: { value: 'user' } });
    fireEvent.change(screen.getByPlaceholderText(/enter your password/i), { target: { value: 'pass' } });
    fireEvent.click(screen.getByRole('button', { name: /sign in/i }));
    expect(mockLogin).toHaveBeenCalledWith('user', 'pass');
  });

  it('shows validation errors on register empty submit', async () => {
    render(<LoginPage />);
    fireEvent.click(screen.getByText(/register/i));
    fireEvent.click(screen.getByRole('button', { name: /create account/i }));
    expect(await screen.findAllByText(/required/i)).toHaveLength(3);
  });
});

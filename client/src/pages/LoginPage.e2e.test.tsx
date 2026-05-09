import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter } from 'react-router-dom';
import { LoginPage } from './LoginPage';

const mockAuth = {
  login: jest.fn().mockResolvedValue(undefined),
  register: jest.fn().mockResolvedValue(undefined),
  logout: jest.fn(),
  isAuthenticated: false,
  username: null,
  token: null,
};

jest.mock('../context/AuthContext', () => ({
  useAuth: () => mockAuth,
}));

const renderLogin = () =>
  render(
    <MemoryRouter>
      <LoginPage />
    </MemoryRouter>
  );

describe('LoginPage E2E', () => {
  beforeEach(() => {
    mockAuth.login.mockReset();
    mockAuth.register.mockReset();
    mockAuth.logout.mockReset();
  });

  it('user can register and then login', async () => {
    renderLogin();

    await userEvent.click(screen.getByText(/register/i));
    await userEvent.type(screen.getByPlaceholderText(/choose a username/i), 'newuser');
    await userEvent.type(screen.getByPlaceholderText(/enter your email/i), 'mail@mail.com');
    await userEvent.type(screen.getByPlaceholderText(/enter a password/i), 'pass123');
    await userEvent.type(screen.getByPlaceholderText(/confirm your password/i), 'pass123');
    await userEvent.click(screen.getByRole('button', { name: /create account/i }));

    expect(mockAuth.register).toHaveBeenCalledWith('newuser', 'mail@mail.com', 'pass123');

    await userEvent.click(screen.getByText(/login/i));
    await userEvent.type(screen.getByPlaceholderText(/enter your username/i), 'newuser');
    await userEvent.type(screen.getByPlaceholderText(/enter your password/i), 'pass123');
    await userEvent.click(screen.getByRole('button', { name: /sign in/i }));

    expect(mockAuth.login).toHaveBeenCalledWith('newuser', 'pass123');
  });
});
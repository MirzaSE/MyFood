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

describe('LoginPage', () => {
  beforeEach(() => {
    mockAuth.login.mockReset();
    mockAuth.register.mockReset();
    mockAuth.logout.mockReset();
  });

  it('shows validation errors on empty submit', async () => {
    renderLogin();
    await userEvent.click(screen.getByRole('button', { name: /sign in/i }));
    expect(await screen.findAllByText(/required/i)).toHaveLength(2);
  });

  it('calls login with correct data', async () => {
    renderLogin();
    await userEvent.type(screen.getByPlaceholderText(/enter your username/i), 'user');
    await userEvent.type(screen.getByPlaceholderText(/enter your password/i), 'pass');
    await userEvent.click(screen.getByRole('button', { name: /sign in/i }));
    expect(mockAuth.login).toHaveBeenCalledWith('user', 'pass');
  });

  it('shows validation errors on register empty submit', async () => {
    renderLogin();
    await userEvent.click(screen.getByText(/register/i));
    await userEvent.click(screen.getByRole('button', { name: /create account/i }));
    expect(await screen.findAllByText(/required/i)).toHaveLength(3);
  });
});
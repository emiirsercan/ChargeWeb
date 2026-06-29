import { useState, useEffect } from 'react';
import type { FormEvent } from 'react';
import Lottie from 'lottie-react';
import logo from './assets/logo.svg';
import MapPage from './MapPage';
import ProfilePage from './ProfilePage';
import chargingAnimation from './assets/charging.json';
import './App.css';

const LottiePlayer = (Lottie as any).default || Lottie;

interface User {
  fullName: string;
  email: string;
  role: string;
}

function App() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [remember, setRemember] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  // Routing and session states
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [currentPage, setCurrentPage] = useState<'map' | 'profile'>('map');
  const [user, setUser] = useState<User | null>(null);

  // Check if user session is persisted
  useEffect(() => {
    const token = localStorage.getItem('accessToken');
    const userData = localStorage.getItem('user');
    if (token && userData) {
      setIsLoggedIn(true);
      try {
        setUser(JSON.parse(userData));
      } catch (e) {
        console.error('Error parsing user data:', e);
      }
    }
  }, []);

  const handleLogout = () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
    setUser(null);
    setIsLoggedIn(false);
    setSuccess(null);
    setEmail('');
    setPassword('');
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setSuccess(null);

    try {
      const response = await fetch('/api/auth/login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ email, password }),
      });

      if (!response.ok) {
        if (response.status === 401) {
          throw new Error('E-posta adresi veya şifre hatalı.');
        }
        const errorData = await response.json().catch(() => null);
        throw new Error(errorData?.message || 'Giriş yapılamadı. Lütfen bilgilerinizi kontrol edin.');
      }

      const data = await response.json();

      // Store token and user data in local storage
      localStorage.setItem('accessToken', data.accessToken);
      localStorage.setItem('refreshToken', data.refreshToken);
      localStorage.setItem('user', JSON.stringify(data.user));

      setUser(data.user);
      setSuccess(`Hoş geldiniz, ${data.user.fullName}! Giriş başarılı.`);

      // Redirect after a brief moment
      setTimeout(() => {
        setIsLoggedIn(true);
      }, 1000);
    } catch (err: any) {
      setError(err.message || 'Giriş yapılırken bir hata oluştu.');
    } finally {
      setLoading(false);
    }
  };

  if (isLoggedIn) {
    return (
      <div className="bg-background text-on-background h-screen w-full flex flex-col overflow-hidden">
        {/* Unified Top Navbar */}
        <header className="fixed top-0 w-full z-[1000] bg-surface/60 backdrop-blur-xl border-b border-outline-variant/30 shadow-sm">
          <div className="flex justify-between items-center px-margin-desktop py-4 max-w-max-width mx-auto w-full">
            <div className="flex items-center gap-8">
              <div className="flex items-center gap-2 cursor-pointer" onClick={() => setCurrentPage('map')}>
                <img alt="VoltSpot Logo" className="h-10 w-10 object-contain animate-pulse" src={logo} />
                <span className="font-display-lg text-headline-md font-bold text-primary tracking-wider">VoltSpot</span>
              </div>
              <nav className="flex items-center gap-6 font-label-md text-label-md">
                <button
                  onClick={() => setCurrentPage('map')}
                  className={`px-2 py-1 font-bold transition-all ${currentPage === 'map' ? 'text-primary border-b-2 border-primary' : 'text-on-surface-variant hover:text-primary'}`}
                >
                  Harita
                </button>
                <button
                  onClick={() => setCurrentPage('profile')}
                  className={`px-2 py-1 font-bold transition-all ${currentPage === 'profile' ? 'text-primary border-b-2 border-primary' : 'text-on-surface-variant hover:text-primary'}`}
                >
                  Profil
                </button>
              </nav>
            </div>

            <div className="flex items-center gap-4">
              <button className="p-2 rounded-full hover:bg-surface-variant/50 transition-colors">
                <span className="material-symbols-outlined text-on-surface-variant">notifications</span>
              </button>
              <button
                onClick={() => setCurrentPage('profile')}
                className="p-2 rounded-full hover:bg-surface-variant/50 transition-colors"
              >
                <span className="material-symbols-outlined text-on-surface-variant">account_balance_wallet</span>
              </button>
              <div className="flex items-center gap-3 border-l border-outline-variant/30 pl-4">
                <div className="text-right cursor-pointer" onClick={() => setCurrentPage('profile')}>
                  <p className="font-label-md text-label-md text-on-surface font-semibold">{user?.fullName || 'User'}</p>
                  <p className="text-[11px] text-on-surface-variant">{user?.email || 'user@voltspot.com'}</p>
                </div>
                <button
                  onClick={handleLogout}
                  className="p-2 rounded-full hover:bg-error-container hover:text-error transition-colors"
                  title="Sign Out"
                >
                  <span className="material-symbols-outlined">logout</span>
                </button>
              </div>
            </div>
          </div>
        </header>

        {/* Dynamic Page Content */}
        <div className="flex-grow overflow-auto pt-16 h-full w-full relative flex flex-col pb-16 md:pb-0">
          {currentPage === 'map' ? (
            <MapPage />
          ) : (
            <ProfilePage user={user} />
          )}
        </div>

        {/* Mobile Navigation bar */}
        <nav className="md:hidden fixed bottom-0 left-0 w-full z-[1000] bg-surface/85 backdrop-blur-md border-t border-outline-variant/20 flex justify-around items-center px-4 py-3 pb-safe shadow-lg rounded-t-xl">
          <button
            onClick={() => setCurrentPage('map')}
            className={`flex flex-col items-center justify-center px-4 py-1.5 transition-all ${currentPage === 'map' ? 'bg-secondary-container text-on-secondary-container rounded-full px-4 py-1' : 'text-on-surface-variant'
              }`}
          >
            <span className="material-symbols-outlined text-[20px]" style={{ fontVariationSettings: currentPage === 'map' ? "'FILL' 1" : "'FILL' 0" }}>map</span>
            <span className="font-label-sm text-[10px] mt-0.5">Harita</span>
          </button>
          <button
            onClick={() => setCurrentPage('profile')}
            className={`flex flex-col items-center justify-center px-4 py-1.5 transition-all ${currentPage === 'profile' ? 'bg-secondary-container text-on-secondary-container rounded-full px-4 py-1' : 'text-on-surface-variant'
              }`}
          >
            <span className="material-symbols-outlined text-[20px]" style={{ fontVariationSettings: currentPage === 'profile' ? "'FILL' 1" : "'FILL' 0" }}>person</span>
            <span className="font-label-sm text-[10px] mt-0.5">Profil</span>
          </button>
        </nav>
      </div>
    );
  }

  return (
    <main className="min-h-screen flex flex-col md:flex-row bg-background text-on-background overflow-x-hidden">
      {/* Left Section: Login Form */}
      <section className="w-full md:w-1/2 lg:w-[45%] flex flex-col px-6 md:px-12 lg:px-24 py-12 bg-surface-bright relative z-10">

        {/* Header/Logo */}
        <div className="mb-12 flex items-center gap-3">
          <img alt="VoltSpot Logo" className="h-10 w-auto" src={logo} />
          <span className="font-display-lg text-headline-md font-bold text-primary tracking-wider">VoltSpot</span>
        </div>

        {/* Content Container */}
        <div className="max-w-md w-full mx-auto md:mx-0 flex-grow flex flex-col justify-center">
          <div className="mb-8">
            <h1 className="font-headline-lg text-headline-lg text-on-surface mb-2">Welcome Back</h1>
            <p className="font-body-md text-body-md text-on-surface-variant">Log in to manage your vehicle's charging and find the nearest stations.</p>
          </div>

          {/* Feedback Alerts */}
          {error && (
            <div className="mb-6 p-4 bg-error-container text-error rounded-xl font-body-md flex items-center gap-2 border border-error/20">
              <span className="material-symbols-outlined text-[20px]">error</span>
              <span>{error}</span>
            </div>
          )}

          {success && (
            <div className="mb-6 p-4 bg-on-secondary-container/10 text-on-secondary-container rounded-xl font-body-md flex items-center gap-2 border border-on-secondary-container/20">
              <span className="material-symbols-outlined text-[20px]">check_circle</span>
              <span>{success}</span>
            </div>
          )}

          {/* Login Form */}
          <form className="space-y-6" onSubmit={handleSubmit}>
            <div className="space-y-2">
              <label className="font-label-md text-label-md text-on-surface-variant ml-1" htmlFor="email">Email Address</label>
              <div className="relative">
                <span className="material-symbols-outlined absolute left-4 top-1/2 -translate-y-1/2 text-outline">mail</span>
                <input
                  className="w-full pl-12 pr-4 py-3 bg-surface-container-low border border-outline-variant rounded-xl focus:ring-2 focus:ring-primary focus:border-primary transition-all font-body-md text-body-md outline-none"
                  id="email"
                  placeholder="name@company.com"
                  type="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                />
              </div>
            </div>

            <div className="space-y-2">
              <div className="flex justify-between items-center px-1">
                <label className="font-label-md text-label-md text-on-surface-variant" htmlFor="password">Password</label>
                <a className="font-label-md text-label-md text-primary hover:underline decoration-2 underline-offset-4" href="#">Forgot Password?</a>
              </div>
              <div className="relative">
                <span className="material-symbols-outlined absolute left-4 top-1/2 -translate-y-1/2 text-outline">lock</span>
                <input
                  className="w-full pl-12 pr-4 py-3 bg-surface-container-low border border-outline-variant rounded-xl focus:ring-2 focus:ring-primary focus:border-primary transition-all font-body-md text-body-md outline-none"
                  id="password"
                  placeholder="••••••••"
                  type="password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                />
              </div>
            </div>

            <div className="flex items-center gap-2 px-1">
              <input
                className="w-4 h-4 text-primary rounded border-outline-variant focus:ring-primary"
                id="remember"
                type="checkbox"
                checked={remember}
                onChange={(e) => setRemember(e.target.checked)}
              />
              <label className="font-label-md text-label-md text-on-surface-variant cursor-pointer" htmlFor="remember">Keep me signed in</label>
            </div>

            <button
              className="w-full bg-primary hover:bg-surface-tint text-on-primary font-label-md text-label-md py-4 rounded-xl shadow-lg shadow-primary/20 transition-all active:scale-[0.98] flex items-center justify-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed"
              type="submit"
              disabled={loading}
            >
              <span>{loading ? 'Signing In...' : 'Sign In'}</span>
              <span className="material-symbols-outlined text-[20px]">arrow_forward</span>
            </button>
          </form>

          {/* Divider */}
          <div className="relative my-10">
            <div aria-hidden="true" className="absolute inset-0 flex items-center">
              <div className="w-full border-t border-outline-variant/30"></div>
            </div>
            <div className="relative flex justify-center text-label-sm uppercase">
              <span className="bg-surface-bright px-4 text-on-surface-variant font-label-sm">Or continue with</span>
            </div>
          </div>

          {/* Social Logins */}
          <div className="grid grid-cols-2 gap-4">
            <button className="flex items-center justify-center gap-3 py-3 border border-outline-variant rounded-xl hover:bg-surface-container-low transition-colors group">
              <svg className="w-5 h-5" viewBox="0 0 24 24">
                <path d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z" fill="#4285F4"></path>
                <path d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z" fill="#34A853"></path>
                <path d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z" fill="#FBBC05"></path>
                <path d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 12-4.53z" fill="#EA4335"></path>
              </svg>
              <span className="font-label-md text-label-md text-on-surface">Google</span>
            </button>
            <button className="flex items-center justify-center gap-3 py-3 border border-outline-variant rounded-xl hover:bg-surface-container-low transition-colors">
              <svg className="w-5 h-5 fill-current text-on-surface" viewBox="0 0 24 24">
                <path d="M17.05 20.28c-.96 0-2.04-.6-3.23-.6-1.2 0-2.11.58-3.04.58-1.57 0-3.95-3.17-3.95-6.27 0-3.08 2.05-4.71 4.02-4.71 1.03 0 1.9.7 2.66.7.72 0 1.78-.73 2.92-.73 1.18 0 2.22.56 2.94 1.58-2.43 1.43-2.04 4.58.5 5.62-.64 1.57-1.48 3.12-2.82 4.83zm-2.8-15.65c.53-.64.88-1.53.88-2.41 0-.13-.02-.27-.04-.4-.8.03-1.78.54-2.35 1.22-.48.55-.9 1.46-.9 2.36 0 .14.03.28.05.37.89.07 1.83-.5 2.36-1.14z"></path>
              </svg>
              <span className="font-label-md text-label-md text-on-surface">Apple</span>
            </button>
          </div>

          {/* Footer Link */}
          <p className="mt-12 text-center font-body-md text-body-md text-on-surface-variant">
            Don't have an account?{' '}
            <a className="text-secondary font-bold hover:underline decoration-2 underline-offset-4" href="#">Sign up for free</a>
          </p>
        </div>

        {/* Legal Footer */}
        <footer className="mt-auto pt-8 border-t border-outline-variant/20 flex flex-wrap gap-4 justify-center md:justify-start">
          <a className="font-label-sm text-label-sm text-outline hover:text-on-surface" href="#">Privacy Policy</a>
          <a className="font-label-sm text-label-sm text-outline hover:text-on-surface" href="#">Terms of Service</a>
          <a className="font-label-sm text-label-sm text-outline hover:text-on-surface" href="#">Help Center</a>
        </footer>
      </section>

      {/* Right Section: Visual Illustration */}
      <section className="hidden md:flex md:w-1/2 lg:w-[55%] relative overflow-hidden bg-gradient-to-br from-primary via-on-background to-primary-container items-center justify-center">
        {/* Lottie Animation Player */}
        <div className="w-4/5 h-auto max-w-lg relative z-10 mb-24">
          <LottiePlayer 
            animationData={chargingAnimation} 
            loop={true} 
            className="w-full h-full"
          />
        </div>
        <div className="absolute inset-0 bg-gradient-to-t from-primary/30 via-transparent to-transparent"></div>

        {/* Glassmorphism Stats Overlay */}
        <div className="absolute bottom-8 left-1/2 -translate-x-1/2 w-[calc(100%-48px)] max-w-md z-20">
          <div className="glass-card p-5 rounded-2xl flex flex-col gap-4 border border-white/20">
            <div className="flex items-start justify-between">
              <div>
                <div className="flex items-center gap-2 mb-1.5">
                  <span className="material-symbols-outlined text-secondary-fixed-dim charging-pulse text-[16px]" style={{ fontVariationSettings: "'FILL' 1" }}>bolt</span>
                  <span className="font-label-sm text-[11px] text-white/80 tracking-widest uppercase">Live Status</span>
                </div>
                <h3 className="font-headline-md text-body-md font-bold text-white">Hyper-Charge Station #042</h3>
              </div>
              <div className="bg-secondary-container text-on-secondary-container px-2.5 py-0.5 rounded-full font-label-sm text-[11px] flex items-center gap-1">
                <span className="w-1.5 h-1.5 rounded-full bg-secondary"></span> Available
              </div>
            </div>

            <div className="grid grid-cols-3 gap-3">
              <div className="bg-white/10 p-3 rounded-xl border border-white/5">
                <p className="font-label-sm text-[10px] text-white/60 mb-1">Charge Rate</p>
                <p className="font-headline-md text-body-md font-bold text-white">350 <span className="text-[11px] font-normal opacity-70">kW</span></p>
              </div>
              <div className="bg-white/10 p-3 rounded-xl border border-white/5">
                <p className="font-label-sm text-[10px] text-white/60 mb-1">CO2 Saved</p>
                <p className="font-headline-md text-body-md font-bold text-secondary-fixed-dim">1.2 <span className="text-[11px] font-normal opacity-70">t</span></p>
              </div>
              <div className="bg-white/10 p-3 rounded-xl border border-white/5">
                <p className="font-label-sm text-[10px] text-white/60 mb-1">Efficiency</p>
                <p className="font-headline-md text-body-md font-bold text-white">98 <span className="text-[11px] font-normal opacity-70">%</span></p>
              </div>
            </div>
            <div className="h-px bg-white/10 my-0.5"></div>
            <p className="text-[11px] text-white/80 italic leading-relaxed text-center">
              "VoltSpot ile şarj etmek artık son derece hızlı ve zahmetsiz."
              <span className="block mt-0.5 font-bold not-italic text-white/90 text-[10px]">— Sarah J.</span>
            </p>
          </div>
        </div>

        {/* Floating Decorative Elements */}
        <div className="absolute top-20 right-20 w-32 h-32 bg-secondary/30 rounded-full blur-3xl animate-pulse"></div>
        <div className="absolute top-40 left-10 w-24 h-24 bg-primary-fixed-dim/20 rounded-full blur-2xl animate-bounce" style={{ animationDuration: '5s' }}></div>
      </section>
    </main>
  );
}

export default App;

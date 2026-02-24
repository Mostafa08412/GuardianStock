import { useState, useEffect } from 'react';
import { Mail, Lock, User, ArrowRight, Eye, EyeOff, Loader2, ArrowLeft, KeyRound } from 'lucide-react';
import { useNavigate, useLocation } from 'react-router-dom';
import { toast } from 'sonner';
import { GoogleLogin } from '@react-oauth/google';
import { useAuth } from '@/contexts/AuthContext';

type AuthView = 'login' | 'signup' | 'forgot-password' | 'verify-otp' | 'reset-password';

export default function Auth() {
  const [view, setView] = useState<AuthView>('login');
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();
  const { login, register, googleLogin, isAuthenticated, forgetPassword, verifyOtp, resetPassword } = useAuth();

  const from = location.state?.from?.pathname || '/';

  const [formData, setFormData] = useState({
    email: '',
    password: '',
    confirmPassword: '',
    firstName: '',
    lastName: '',
    otp: '',
    newPassword: '',
  });

  const [otpToken, setOtpToken] = useState<string | null>(null);
  const [resendTimer, setResendTimer] = useState(0);
  const [otpValidationError, setOtpValidationError] = useState<string | null>(null);

  // Redirect if already authenticated
  useEffect(() => {
    if (isAuthenticated) {
      navigate(from, { replace: true });
    }
  }, [isAuthenticated, navigate, from]);

  // Countdown timer logic
  useEffect(() => {
    if (resendTimer > 0) {
      const timer = setTimeout(() => setResendTimer(resendTimer - 1), 1000);
      return () => clearTimeout(timer);
    }
  }, [resendTimer]);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
    if (e.target.name === 'otp') setOtpValidationError(null);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);

    try {
      if (view === 'login') {
        if (!formData.email || !formData.password) {
          toast.error('Please fill in all required fields');
          return;
        }
        const result = await login({ email: formData.email, password: formData.password });
        if (result.success) {
          toast.success('Welcome back!');
          navigate(from, { replace: true });
        } else {
          toast.error(result.error || 'Login failed');
        }
      } else if (view === 'signup') {
        if (!formData.email || !formData.password || !formData.firstName || !formData.lastName) {
          toast.error('Please fill in all required fields');
          return;
        }
        if (formData.password !== formData.confirmPassword) {
          toast.error('Passwords do not match');
          return;
        }
        const result = await register({
          email: formData.email,
          password: formData.password,
          firstName: formData.firstName,
          lastName: formData.lastName,
        });
        if (result.success) {
          toast.success('Account created successfully! Please sign in.');
          setView('login');
          setFormData(prev => ({ ...prev, password: '', confirmPassword: '' }));
        } else {
          toast.error(result.error || 'Registration failed');
        }
      } else if (view === 'forgot-password') {
        if (!formData.email) {
          toast.error('Please enter your email');
          return;
        }
        const result = await forgetPassword(formData.email);
        if (result.success) {
          toast.success('OTP sent to your email');
          setView('verify-otp');
          setResendTimer(30);
        } else {
          toast.error(result.error || 'Failed to send OTP');
        }
      } else if (view === 'verify-otp') {
        if (!formData.otp) {
          toast.error('Please enter the OTP');
          return;
        }
        const result = await verifyOtp(formData.email, formData.otp);
        if (result.success && result.token) {
          setOtpToken(result.token);
          setView('reset-password');
        } else {
          if (result.errorCode === 'VALIDATION_ERROR' && result.validationErrors?.otp) {
            setOtpValidationError(result.validationErrors.otp);
          } else {
            toast.error(result.error || 'Verification failed');
          }
        }
      } else if (view === 'reset-password') {
        if (!formData.newPassword) {
          toast.error('Please enter your new password');
          return;
        }
        if (!otpToken) {
          toast.error('Invalid session. Please start over.');
          setView('forgot-password');
          return;
        }
        const result = await resetPassword({
          email: formData.email,
          token: otpToken,
          newPassword: formData.newPassword
        });
        if (result.success) {
          toast.success('Password reset successfully! Please sign in.');
          setView('login');
          setFormData(prev => ({ ...prev, password: '', otp: '', newPassword: '' }));
        } else {
          toast.error(result.error || 'Reset failed');
        }
      }
    } catch (error) {
      toast.error('An unexpected error occurred');
    } finally {
      setIsLoading(false);
    }
  };

  const handleResendOtp = async () => {
    if (resendTimer > 0) return;
    setIsLoading(true);
    const result = await forgetPassword(formData.email);
    if (result.success) {
      toast.success('New OTP sent');
      setResendTimer(30);
    } else {
      toast.error(result.error || 'Failed to resend OTP');
    }
    setIsLoading(false);
  };

  return (
    <div className="min-h-screen bg-background flex">
      {/* Left side - Branding */}
      <div className="hidden lg:flex lg:w-1/2 bg-gradient-to-br from-primary via-primary/90 to-primary/70 p-12 flex-col justify-between relative overflow-hidden">
        <div className="absolute inset-0 bg-[url('data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNjAiIGhlaWdodD0iNjAiIHZpZXdCb3g9IjAgMCA2MCA2MCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48ZyBmaWxsPSJub25lIiBmaWxsLXJ1bGU9ImV2ZW5vZGQiPjxnIGZpbGw9IiNmZmYiIGZpbGwtb3BhY2l0eT0iMC4xIj48cGF0aCBkPSJNMzYgMzRoLTJ2LTRoMnY0em0wLTZoLTJ2LTRoMnY0em0wLTZoLTJ2LTRoMnY0em0wLTZoLTJWMTJoMnY0em0wLTZoLTJWNmgydjR6bTAgMzZoLTJ2LTRoMnY0em0wIDZoLTJ2LTRoMnY0eiIvPjwvZz48L2c+PC9zdmc+')] opacity-30"></div>

        <div className="relative z-10">
          <div className="flex items-center gap-3">
            <img src="https://github.com/Mostafa08412/GuardianStock/blob/main/web-ui/public/logo.png?raw=true" alt="GuardianStock Logo" className="w-12 h-12 rounded-xl" />
            <span className="text-2xl font-bold text-white">GuardianStock</span>
          </div>
        </div>

        <div className="relative z-10 space-y-6">
          <h1 className="text-4xl font-bold text-white leading-tight">
            Manage Your Inventory<br />
            <span className="text-white/80">With Confidence</span>
          </h1>
          <p className="text-white/70 text-lg max-w-md">
            Streamline your inventory operations with our powerful management system. Track products, monitor stock levels, and make data-driven decisions.
          </p>
        </div>

        <div className="relative z-10">
          <p className="text-white/50 text-sm">© 2025 GuardianStock. All rights reserved.</p>
        </div>
      </div>

      {/* Right side - Form */}
      <div className="flex-1 flex items-center justify-center p-8">
        <div className="w-full max-w-md space-y-8">
          {/* Back Button for non-main views */}
          {(view !== 'login' && view !== 'signup') && (
            <button
              onClick={() => setView(view === 'reset-password' ? 'forgot-password' : 'login')}
              className="flex items-center gap-2 text-sm text-muted-foreground hover:text-foreground transition-colors"
            >
              <ArrowLeft className="w-4 h-4" />
              Back to Sign In
            </button>
          )}

          {/* Mobile logo */}
          <div className="lg:hidden flex items-center justify-center gap-3 mb-8">
            <img src="https://github.com/Mostafa08412/insight-dash/blob/main/public/logo.png?raw=true" alt="GuardianStock Logo" className="w-10 h-10 rounded-xl" />
            <span className="text-xl font-bold text-foreground">GuardianStock</span>
          </div>

          <div className="text-center">
            <h2 className="text-3xl font-bold text-foreground">
              {view === 'login' && 'Welcome back'}
              {view === 'signup' && 'Create account'}
              {view === 'forgot-password' && 'Forgot password?'}
              {view === 'verify-otp' && 'Verify OTP'}
              {view === 'reset-password' && 'Reset your password'}
            </h2>
            <p className="mt-2 text-muted-foreground">
              {view === 'login' && 'Sign in to your account to continue'}
              {view === 'signup' && 'Get started with your free account'}
              {view === 'forgot-password' && 'Enter your email to receive a verification code'}
              {view === 'verify-otp' && `Enter the OTP sent to ${formData.email}`}
              {view === 'reset-password' && 'Choose a strong new password for your account'}
            </p>
          </div>

          {/* Tab switcher - Only for login/signup */}
          {(view === 'login' || view === 'signup') && (
            <div className="flex p-1 bg-secondary rounded-xl">
              <button
                onClick={() => setView('login')}
                className={`flex-1 py-2.5 text-sm font-medium rounded-lg transition-all ${view === 'login'
                  ? 'bg-card text-foreground shadow-sm'
                  : 'text-muted-foreground hover:text-foreground'
                  }`}
              >
                Sign In
              </button>
              <button
                onClick={() => setView('signup')}
                className={`flex-1 py-2.5 text-sm font-medium rounded-lg transition-all ${view === 'signup'
                  ? 'bg-card text-foreground shadow-sm'
                  : 'text-muted-foreground hover:text-foreground'
                  }`}
              >
                Sign Up
              </button>
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-5">
            {view === 'signup' && (
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <label className="text-sm font-medium text-foreground">First Name</label>
                  <div className="relative">
                    <User className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
                    <input
                      type="text"
                      name="firstName"
                      value={formData.firstName}
                      onChange={handleInputChange}
                      placeholder="John"
                      className="w-full pl-11 pr-4 py-3 bg-secondary border border-border rounded-xl text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                    />
                  </div>
                </div>
                <div className="space-y-2">
                  <label className="text-sm font-medium text-foreground">Last Name</label>
                  <div className="relative">
                    <User className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
                    <input
                      type="text"
                      name="lastName"
                      value={formData.lastName}
                      onChange={handleInputChange}
                      placeholder="Doe"
                      className="w-full pl-11 pr-4 py-3 bg-secondary border border-border rounded-xl text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                    />
                  </div>
                </div>
              </div>
            )}

            {(view === 'login' || view === 'signup' || view === 'forgot-password') && (
              <div className="space-y-2">
                <label className="text-sm font-medium text-foreground">Email</label>
                <div className="relative">
                  <Mail className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
                  <input
                    type="email"
                    name="email"
                    value={formData.email}
                    onChange={handleInputChange}
                    placeholder="you@example.com"
                    autoComplete="email"
                    className="w-full pl-11 pr-4 py-3 bg-secondary border border-border rounded-xl text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                  />
                </div>
              </div>
            )}

            {(view === 'login' || view === 'signup') && (
              <div className="space-y-2">
                <label className="text-sm font-medium text-foreground">Password</label>
                <div className="relative">
                  <Lock className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
                  <input
                    type={showPassword ? 'text' : 'password'}
                    name="password"
                    value={formData.password}
                    onChange={handleInputChange}
                    placeholder="••••••••"
                    autoComplete={view === 'login' ? 'current-password' : 'new-password'}
                    className="w-full pl-11 pr-12 py-3 bg-secondary border border-border rounded-xl text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                  />
                  <button
                    type="button"
                    onClick={() => setShowPassword(!showPassword)}
                    className="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground transition-colors"
                  >
                    {showPassword ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
                  </button>
                </div>
              </div>
            )}

            {view === 'signup' && (
              <div className="space-y-2">
                <label className="text-sm font-medium text-foreground">Confirm Password</label>
                <div className="relative">
                  <Lock className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
                  <input
                    type={showPassword ? 'text' : 'password'}
                    name="confirmPassword"
                    value={formData.confirmPassword}
                    onChange={handleInputChange}
                    placeholder="••••••••"
                    autoComplete="new-password"
                    className="w-full pl-11 pr-4 py-3 bg-secondary border border-border rounded-xl text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                  />
                </div>
              </div>
            )}

            {view === 'verify-otp' && (
              <div className="space-y-2">
                <label className="text-sm font-medium text-foreground">OTP Code</label>
                <div className="relative">
                  <KeyRound className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
                  <input
                    type="text"
                    name="otp"
                    value={formData.otp}
                    onChange={handleInputChange}
                    placeholder="Enter 6-digit code"
                    className={`w-full pl-11 pr-4 py-3 bg-secondary border ${otpValidationError ? 'border-destructive' : 'border-border'} rounded-xl text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all`}
                  />
                </div>
                {otpValidationError && (
                  <p className="text-xs text-destructive mt-1 font-medium">{otpValidationError}</p>
                )}
                <div className="flex justify-between items-center pt-2">
                  <span className="text-xs text-muted-foreground">
                    {resendTimer > 0 ? `Resend code in ${resendTimer}s` : "Didn't receive a code?"}
                  </span>
                  {resendTimer === 0 && (
                    <button
                      type="button"
                      onClick={handleResendOtp}
                      className="text-xs text-primary hover:text-primary/80 font-medium"
                    >
                      Resend OTP
                    </button>
                  )}
                </div>
              </div>
            )}

            {view === 'reset-password' && (
              <div className="space-y-2">
                <label className="text-sm font-medium text-foreground">New Password</label>
                <div className="relative">
                  <Lock className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
                  <input
                    type={showPassword ? 'text' : 'password'}
                    name="newPassword"
                    value={formData.newPassword}
                    onChange={handleInputChange}
                    placeholder="••••••••"
                    autoComplete="new-password"
                    className="w-full pl-11 pr-12 py-3 bg-secondary border border-border rounded-xl text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                  />
                  <button
                    type="button"
                    onClick={() => setShowPassword(!showPassword)}
                    className="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground transition-colors"
                  >
                    {showPassword ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
                  </button>
                </div>
              </div>
            )}

            {view === 'login' && (
              <div className="flex items-center justify-between">
                <label className="flex items-center gap-2 cursor-pointer">
                  <input type="checkbox" className="w-4 h-4 rounded border-border bg-secondary text-primary focus:ring-primary/50" />
                  <span className="text-sm text-muted-foreground">Remember me</span>
                </label>
                <button
                  type="button"
                  onClick={() => setView('forgot-password')}
                  className="text-sm text-primary hover:text-primary/80 font-medium"
                >
                  Forgot password?
                </button>
              </div>
            )}

            <button
              type="submit"
              disabled={isLoading}
              className="w-full flex items-center justify-center gap-2 py-3 bg-primary text-primary-foreground rounded-xl font-medium hover:bg-primary/90 transition-all disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {isLoading ? (
                <Loader2 className="w-5 h-5 animate-spin" />
              ) : (
                <>
                  {view === 'login' && 'Sign In'}
                  {view === 'signup' && 'Create Account'}
                  {view === 'forgot-password' && 'Send OTP'}
                  {view === 'verify-otp' && 'Verify Code'}
                  {view === 'reset-password' && 'Reset Password'}
                  <ArrowRight className="w-5 h-5" />
                </>
              )}
            </button>

            {view === 'login' && (
              <div className="space-y-4">
                <div className="relative">
                  <div className="absolute inset-0 flex items-center">
                    <span className="w-full border-t border-border"></span>
                  </div>
                  <div className="relative flex justify-center text-xs uppercase">
                    <span className="bg-background px-2 text-muted-foreground">Or continue with</span>
                  </div>
                </div>

                <div className="flex justify-center">
                  <GoogleLogin
                    onSuccess={async (credentialResponse) => {
                      if (credentialResponse.credential) {
                        setIsLoading(true);
                        const result = await googleLogin(credentialResponse.credential);
                        if (result.success) {
                          toast.success('Welcome back!');
                          navigate(from, { replace: true });
                        } else {
                          toast.error(result.error || 'Google login failed');
                        }
                        setIsLoading(false);
                      }
                    }}
                    onError={() => {
                      toast.error('Google login failed');
                    }}
                    useOneTap
                    theme="filled_black"
                    shape="pill"
                    width="100%"
                  />
                </div>
              </div>
            )}
          </form>

          {/* Demo Credentials - Only on Login */}
          {view === 'login' && (
            <div className="bg-secondary/50 border border-border rounded-xl p-4 space-y-3">
              <p className="text-sm font-medium text-foreground">Demo Accounts:</p>
              <div className="space-y-2 text-sm">
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <span className="px-2 py-0.5 bg-destructive/20 text-destructive text-xs font-medium rounded">Admin</span>
                    <span className="text-muted-foreground">admin@localhost</span>
                  </div>
                  <span className="text-muted-foreground font-mono text-xs">Admin@123</span>
                </div>
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <span className="px-2 py-0.5 bg-primary/20 text-primary text-xs font-medium rounded">Manager</span>
                    <span className="text-muted-foreground">Manager@localhost</span>
                  </div>
                  <span className="text-muted-foreground font-mono text-xs">Manager@123</span>
                </div>
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <span className="px-2 py-0.5 bg-warning/20 text-warning text-xs font-medium rounded">Staff</span>
                    <span className="text-muted-foreground">Staff@localhost</span>
                  </div>
                  <span className="text-muted-foreground font-mono text-xs">Staff@123</span>
                </div>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

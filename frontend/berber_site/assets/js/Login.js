document.addEventListener('DOMContentLoaded', function() {
    // Initialize AOS animations
    AOS.init({
        duration: 800,
        easing: 'ease-in-out',
        once: true,
        offset: 100
    });

    // Form elements
    const loginForm = document.getElementById('loginForm');
    const emailInput = document.getElementById('loginEmail');
    const passwordInput = document.getElementById('loginPassword');
    const submitBtn = loginForm.querySelector('.auth-button');
    const pwToggle = loginForm.querySelector('.password-toggle');

    // Password toggle functionality
    if (pwToggle) {
        pwToggle.addEventListener('click', function() {
            const isPassword = passwordInput.type === 'password';
            passwordInput.type = isPassword ? 'text' : 'password';
            pwToggle.classList.toggle('show', !isPassword);
        });
    }

    // Form submission
    loginForm.addEventListener('submit', async function(e) {
        e.preventDefault();
        
        // Reset previous errors
        loginForm.querySelectorAll('.input-group').forEach(group => {
            group.classList.remove('error');
        });
        
        const errorMessages = loginForm.querySelectorAll('.error-message');
        errorMessages.forEach(msg => msg.remove());

        // Validate inputs
        let isValid = true;
        
        if (!emailInput.value.trim()) {
            showError(emailInput, 'E-posta adresi gereklidir');
            isValid = false;
        } else if (!/^\S+@\S+\.\S+$/.test(emailInput.value.trim())) {
            showError(emailInput, 'Geçerli bir e-posta adresi girin');
            isValid = false;
        }
        
        if (!passwordInput.value.trim()) {
            showError(passwordInput, 'Şifre gereklidir');
            isValid = false;
        } else if (passwordInput.value.trim().length < 6) {
            showError(passwordInput, 'Şifre en az 6 karakter olmalıdır');
            isValid = false;
        }
        
        if (!isValid) return;

        // Show loading state
        submitBtn.classList.add('loading');

        try {
            const response = await fetch('http://localhost:5009/api/web/webauth/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ 
                    Email: emailInput.value.trim(), 
                    Password: passwordInput.value.trim() 
                })
            });

            // Reset loading state
            submitBtn.classList.remove('loading');

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || 'Giriş başarısız');
            }

            const data = await response.json();
            
            // Save user info
            localStorage.setItem('token', data.token || data.Token);
            localStorage.setItem('username', data.userName || data.UserName || emailInput.value.trim().split('@')[0]);
            
            // Show success state
            submitBtn.classList.add('success');
            submitBtn.querySelector('.button-text').textContent = 'Giriş Başarılı';
            
            // Redirect after delay
            setTimeout(() => {
                window.location.href = 'index.html';
            }, 1000);
            
        } catch (error) {
            submitBtn.classList.remove('loading');
            showError(passwordInput, error.message || 'Sunucu hatası. Lütfen tekrar deneyin.');
            console.error('Login error:', error);
        }
    });

    // Show error message
    function showError(input, message) {
        const inputGroup = input.closest('.input-group');
        inputGroup.classList.add('error');
        
        const errorElement = document.createElement('span');
        errorElement.className = 'error-message';
        errorElement.textContent = message;
        
        const formGroup = input.closest('.form-group');
        formGroup.appendChild(errorElement);
        
        // Add shake animation
        inputGroup.style.animation = 'shake 0.5s ease-in-out';
        setTimeout(() => {
            inputGroup.style.animation = '';
        }, 500);
    }

    // Add shake animation
    const style = document.createElement('style');
    style.textContent = `
        @keyframes shake {
            0%, 100% { transform: translateX(0); }
            20%, 60% { transform: translateX(-5px); }
            40%, 80% { transform: translateX(5px); }
        }
    `;
    document.head.appendChild(style);
});
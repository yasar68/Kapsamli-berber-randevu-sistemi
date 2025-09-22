document.addEventListener('DOMContentLoaded', function() {
    // Initialize AOS animations
    AOS.init({
        duration: 800,
        easing: 'ease-in-out',
        once: true,
        offset: 100
    });

    // Form elements
    const registerForm = document.getElementById('registerForm');
    const firstNameInput = document.getElementById('registerFirstName');
    const lastNameInput = document.getElementById('registerLastName');
    const phoneInput = document.getElementById('registerPhoneNumber');
    const emailInput = document.getElementById('registerEmail');
    const passwordInput = document.getElementById('registerPassword');
    const passwordConfirmInput = document.getElementById('registerPasswordConfirm');
    const submitBtn = registerForm.querySelector('.auth-button');
    const pwToggles = registerForm.querySelectorAll('.password-toggle');

    // Password toggle functionality
    pwToggles.forEach(toggle => {
        toggle.addEventListener('click', function() {
            const input = this.closest('.input-group').querySelector('input');
            const isPassword = input.type === 'password';
            input.type = isPassword ? 'text' : 'password';
            this.innerHTML = isPassword ? '<i class="fas fa-eye-slash"></i>' : '<i class="fas fa-eye"></i>';
        });
    });

    // Phone number formatting
    phoneInput.addEventListener('input', function(e) {
        let value = this.value.replace(/\D/g, '');
        if (value.length > 0) {
            value = value.match(/.{1,3}/g).join(' ');
        }
        this.value = value;
    });

    // Password strength indicator
    passwordInput.addEventListener('input', function() {
        updatePasswordStrength(this.value);
    });

    function updatePasswordStrength(password) {
        const strengthBar = document.querySelector('.password-strength-bar');
        const strengthText = document.querySelector('.password-strength-text');
        
        if (!strengthBar || !strengthText) return;
        
        const strength = calculatePasswordStrength(password);
        
        strengthBar.style.width = strength.percentage + '%';
        strengthBar.style.backgroundColor = strength.color;
        strengthText.textContent = strength.text;
        strengthText.style.color = strength.color;
    }

    function calculatePasswordStrength(password) {
        let strength = 0;
        if (password.length >= 8) strength += 1;
        if (/[A-Z]/.test(password)) strength += 1;
        if (/[0-9]/.test(password)) strength += 1;
        if (/[^A-Za-z0-9]/.test(password)) strength += 1;
        
        const strengthMap = [
            { percentage: 25, color: '#e74c3c', text: 'Zayıf' },
            { percentage: 50, color: '#f39c12', text: 'Orta' },
            { percentage: 75, color: '#3498db', text: 'Güçlü' },
            { percentage: 100, color: '#2ecc71', text: 'Çok Güçlü' }
        ];
        
        return strengthMap[Math.min(strength, strengthMap.length - 1)];
    }

    // Form submission
    registerForm.addEventListener('submit', async function(e) {
        e.preventDefault();
        
        // Reset previous errors
        registerForm.querySelectorAll('.input-group').forEach(group => {
            group.classList.remove('error');
        });
        
        const errorMessages = registerForm.querySelectorAll('.error-message');
        errorMessages.forEach(msg => msg.remove());

        // Validate inputs
        let isValid = true;
        
        if (!firstNameInput.value.trim()) {
            showError(firstNameInput, 'Adınız gereklidir');
            isValid = false;
        }
        
        if (!lastNameInput.value.trim()) {
            showError(lastNameInput, 'Soyadınız gereklidir');
            isValid = false;
        }
        
        const phoneValue = phoneInput.value.replace(/\s/g, '');
        if (!phoneValue) {
            showError(phoneInput, 'Telefon numarası gereklidir');
            isValid = false;
        } else if (phoneValue.length < 10) {
            showError(phoneInput, 'Geçerli bir telefon numarası girin');
            isValid = false;
        }
        
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
        
        if (passwordInput.value !== passwordConfirmInput.value) {
            showError(passwordConfirmInput, 'Şifreler uyuşmuyor');
            isValid = false;
        }
        
        if (!isValid) return;

        // Show loading state
        submitBtn.classList.add('loading');

        try {
            const response = await fetch('http://localhost:5009/api/web/webauth/register', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ 
                    FirstName: firstNameInput.value.trim(),
                    LastName: lastNameInput.value.trim(),
                    PhoneNumber: phoneValue,
                    Email: emailInput.value.trim(),
                    Password: passwordInput.value.trim()
                })
            });

            // Reset loading state
            submitBtn.classList.remove('loading');

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || 'Kayıt başarısız');
            }

            const data = await response.json();
            
            // Show success state
            submitBtn.classList.add('success');
            submitBtn.querySelector('.button-text').textContent = 'Kayıt Başarılı';
            
            // Reset form
            registerForm.reset();
            
            // Show success message
            showSuccessMessage('Kayıt işlemi başarıyla tamamlandı. Giriş yapabilirsiniz.');
            
        } catch (error) {
            submitBtn.classList.remove('loading');
            showError(emailInput, error.message || 'Sunucu hatası. Lütfen tekrar deneyin.');
            console.error('Register error:', error);
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

    // Show success message
    function showSuccessMessage(message) {
        const existingAlert = document.querySelector('.alert-success');
        if (existingAlert) existingAlert.remove();
        
        const alertDiv = document.createElement('div');
        alertDiv.className = 'alert alert-success animate__animated animate__fadeIn';
        alertDiv.innerHTML = `
            <i class="fas fa-check-circle"></i>
            <span>${message}</span>
        `;
        
        registerForm.prepend(alertDiv);
        
        // Auto hide after 5 seconds
        setTimeout(() => {
            alertDiv.classList.add('animate__fadeOut');
            setTimeout(() => {
                alertDiv.remove();
            }, 500);
        }, 5000);
    }

    // Add shake animation
    const style = document.createElement('style');
    style.textContent = `
        @keyframes shake {
            0%, 100% { transform: translateX(0); }
            20%, 60% { transform: translateX(-5px); }
            40%, 80% { transform: translateX(5px); }
        }
        
        .alert-success {
            padding: 15px;
            background-color: rgba(46, 204, 113, 0.1);
            border-left: 4px solid #2ecc71;
            border-radius: 4px;
            margin-bottom: 20px;
            display: flex;
            align-items: center;
            color: #2ecc71;
        }
        
        .alert-success i {
            margin-right: 10px;
            font-size: 1.2rem;
        }
    `;
    document.head.appendChild(style);
});
document.addEventListener('DOMContentLoaded', function() {
  // AOS Animasyon Başlatma
  AOS.init({
    duration: 800,
    easing: 'ease-in-out',
    once: true,
    mirror: false,
    offset: 120
  });

  // Header ve Footer Yükleme Fonksiyonu
  async function loadComponent(id, url) {
    try {
      const response = await fetch(url);
      if (!response.ok) throw new Error(`${url} yüklenemedi (${response.status})`);
      
      const html = await response.text();
      const placeholder = document.getElementById(id);
      
      if (placeholder) {
        placeholder.innerHTML = html;
        
        // Header yüklendikten sonra mobile menüyü aktif et
        if (id === 'header-placeholder') {
          initMobileMenu();
        }
        
        // Footer yüklendikten sonra back-to-top butonunu aktif et
        if (id === 'footer-placeholder') {
          initBackToTop();
        }
      }
    } catch (error) {
      console.error('Component yükleme hatası:', error);
      // Fallback olarak basit bir header/footer göster
      if (id === 'header-placeholder' || id === 'footer-placeholder') {
        document.getElementById(id).innerHTML = `
          <div class="component-error" style="padding: 1rem; background: #f8d7da; color: #721c24; border-radius: 4px;">
            ${id.replace('-placeholder', '')} yüklenemedi
          </div>
        `;
      }
    }
  }

  // Mobile Menü İşlevselliği
  function initMobileMenu() {
    const mobileMenuBtn = document.querySelector('.mobile-menu-btn');
    const mainNav = document.querySelector('.main-nav');
    
    if (mobileMenuBtn && mainNav) {
      mobileMenuBtn.addEventListener('click', function() {
        mainNav.classList.toggle('active');
        const icon = this.querySelector('i');
        if (icon) {
          icon.classList.toggle('fa-bars');
          icon.classList.toggle('fa-times');
        }
        
        // Body overflow'u kontrol et
        if (mainNav.classList.contains('active')) {
          document.body.style.overflow = 'hidden';
        } else {
          document.body.style.overflow = '';
        }
      });
      
      // Menü dışına tıklandığında kapat
      document.addEventListener('click', function(e) {
        if (!mainNav.contains(e.target) && !mobileMenuBtn.contains(e.target)) {
          mainNav.classList.remove('active');
          document.body.style.overflow = '';
        }
      });
    }
  }

  // Back to Top Butonu
  function initBackToTop() {
    const backToTop = document.getElementById('backToTop');
    
    if (backToTop) {
      window.addEventListener('scroll', function() {
        if (window.scrollY > 300) {
          backToTop.classList.add('active');
        } else {
          backToTop.classList.remove('active');
        }
      });
      
      backToTop.addEventListener('click', function(e) {
        e.preventDefault();
        window.scrollTo({
          top: 0,
          behavior: 'smooth'
        });
      });
    }
  }

  // Form Validasyon ve Gönderim
  function initContactForm() {
    const contactForm = document.getElementById('contactForm');
    
    if (contactForm) {
      // Inputlar için real-time validasyon
      const inputs = contactForm.querySelectorAll('input, textarea');
      inputs.forEach(input => {
        input.addEventListener('input', function() {
          validateField(this);
        });
        
        input.addEventListener('blur', function() {
          validateField(this);
        });
      });
      
      // Form submit işlemi
      contactForm.addEventListener('submit', async function(e) {
        e.preventDefault();
        
        // Tüm alanları validate et
        let isValid = true;
        inputs.forEach(input => {
          if (!validateField(input)) {
            isValid = false;
          }
        });
        
        if (isValid) {
          const submitBtn = contactForm.querySelector('button[type="submit"]');
          const originalBtnContent = submitBtn.innerHTML;
          
          // Butonu yükleme durumuna getir
          submitBtn.innerHTML = '<i class="bi bi-arrow-repeat animate-spin"></i> Gönderiliyor...';
          submitBtn.disabled = true;
          
          try {
            // Burada gerçek bir fetch işlemi yapılabilir
            // Örnek: 
            // const formData = new FormData(contactForm);
            // const response = await fetch('/api/contact', {
            //   method: 'POST',
            //   body: formData
            // });
            
            // Simüle edilmiş gecikme
            await new Promise(resolve => setTimeout(resolve, 1500));
            
            // Başarılı gönderim
            showAlert('Mesajınız başarıyla gönderildi!', 'success');
            contactForm.reset();
            
            // Animasyonlu teşekkür mesajı
            const thankYouMessage = document.createElement('div');
            thankYouMessage.className = 'thank-you-message';
            thankYouMessage.innerHTML = `
              <i class="bi bi-check-circle-fill"></i>
              <h3>Teşekkürler!</h3>
              <p>En kısa sürede sizinle iletişime geçeceğiz.</p>
            `;
            contactForm.parentNode.insertBefore(thankYouMessage, contactForm);
            contactForm.style.display = 'none';
            
            // Animasyon ekle
            setTimeout(() => {
              thankYouMessage.classList.add('show');
            }, 50);
            
          } catch (error) {
            console.error('Form gönderim hatası:', error);
            showAlert('Bir hata oluştu, lütfen daha sonra tekrar deneyin.', 'error');
          } finally {
            submitBtn.innerHTML = originalBtnContent;
            submitBtn.disabled = false;
          }
        } else {
          showAlert('Lütfen tüm alanları doğru şekilde doldurun.', 'error');
        }
      });
    }
  }

  // Alan validasyon fonksiyonu
  function validateField(field) {
    const formGroup = field.closest('.form-group');
    if (!formGroup) return true;
    
    let isValid = true;
    let errorMessage = '';
    
    // Validasyon kuralları
    if (field.required && !field.value.trim()) {
      isValid = false;
      errorMessage = 'Bu alan zorunludur';
    } else if (field.type === 'email' && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(field.value.trim())) {
      isValid = false;
      errorMessage = 'Geçerli bir email adresi girin';
    } else if (field.type === 'tel' && !/^[0-9]{10}$/.test(field.value.trim())) {
      isValid = false;
      errorMessage = 'Geçerli bir telefon numarası girin';
    }
    
    // Hata durumunu işle
    if (isValid) {
      formGroup.classList.remove('error');
      const existingError = formGroup.querySelector('.error-message');
      if (existingError) existingError.remove();
    } else {
      formGroup.classList.add('error');
      let errorElement = formGroup.querySelector('.error-message');
      
      if (!errorElement) {
        errorElement = document.createElement('small');
        errorElement.className = 'error-message';
        formGroup.appendChild(errorElement);
      }
      
      errorElement.textContent = errorMessage;
    }
    
    return isValid;
  }

  // Alert gösterme fonksiyonu
  function showAlert(message, type) {
    // Önceki alert'i temizle
    const oldAlert = document.querySelector('.form-alert');
    if (oldAlert) oldAlert.remove();
    
    const alertDiv = document.createElement('div');
    alertDiv.className = `form-alert alert-${type}`;
    alertDiv.innerHTML = `
      <i class="bi ${type === 'success' ? 'bi-check-circle-fill' : 'bi-exclamation-circle-fill'}"></i>
      <span>${message}</span>
    `;
    
    const contactForm = document.getElementById('contactForm');
    if (contactForm) {
      contactForm.prepend(alertDiv);
      
      // Alert'i otomatik kapat
      setTimeout(() => {
        alertDiv.classList.add('fade-out');
        setTimeout(() => alertDiv.remove(), 300);
      }, 5000);
    }
  }

  // CSS ekleme (dinamik stiller)
  function addDynamicStyles() {
    const style = document.createElement('style');
    style.textContent = `
      /* Form Alert Stilleri */
      .form-alert {
        padding: 1rem;
        margin-bottom: 1.5rem;
        border-radius: 8px;
        display: flex;
        align-items: center;
        gap: 0.75rem;
        animation: fadeIn 0.3s ease-out;
        font-size: 0.95rem;
      }
      .alert-success {
        background-color: rgba(39, 174, 96, 0.1);
        color: #27ae60;
        border: 1px solid rgba(39, 174, 96, 0.2);
      }
      .alert-error {
        background-color: rgba(231, 76, 60, 0.1);
        color: #e74c3c;
        border: 1px solid rgba(231, 76, 60, 0.2);
      }
      .form-alert.fade-out {
        opacity: 0;
        transition: opacity 0.3s ease;
      }
      
      /* Thank You Message */
      .thank-you-message {
        text-align: center;
        padding: 2rem;
        opacity: 0;
        transform: translateY(20px);
        transition: all 0.5s ease;
      }
      .thank-you-message.show {
        opacity: 1;
        transform: translateY(0);
      }
      .thank-you-message i {
        font-size: 3rem;
        color: var(--primary-color);
        margin-bottom: 1rem;
      }
      .thank-you-message h3 {
        color: var(--secondary-color);
        margin-bottom: 0.5rem;
      }
      .thank-you-message p {
        color: var(--text-light);
      }
      
      /* Loading Spinner */
      .animate-spin {
        animation: spin 1s linear infinite;
      }
      @keyframes spin {
        from { transform: rotate(0deg); }
        to { transform: rotate(360deg); }
      }
      
      /* Error State */
      .form-group.error input,
      .form-group.error textarea {
        border-color: var(--accent-color) !important;
      }
      .form-group.error i {
        color: var(--accent-color) !important;
      }
      .error-message {
        color: var(--accent-color);
        font-size: 0.85rem;
        margin-top: 0.5rem;
        display: block;
      }
    `;
    document.head.appendChild(style);
  }
  initContactForm();
  addDynamicStyles();
});
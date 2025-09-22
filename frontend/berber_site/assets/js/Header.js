// Header.js - Optimize Edilmiş Versiyon
document.addEventListener('DOMContentLoaded', function() {
    initializeHeader();
});

function initializeHeader() {
    // Header tam yüklendikten sonra çalışacak
    setTimeout(() => {
        renderUserArea();
        setActivePage();
        setupEventListeners();
    }, 100); // 100ms gecikme header'ın tam yüklenmesini sağlar
}

function renderUserArea() {
    const userArea = document.getElementById('user-area');
    if (!userArea) {
        console.warn('user-area elementi bulunamadı!');
        setTimeout(renderUserArea, 200); // Element henüz yüklenmediyse tekrar dene
        return;
    }

    const username = localStorage.getItem('username');
    const token = localStorage.getItem('token');

    userArea.innerHTML = '';

    if (username && token) {
        const userInitial = username.charAt(0).toUpperCase();

        userArea.innerHTML = `
            <div class="user-info">
                <div class="user-avatar">${userInitial}</div>
                <span class="username" title="Profil Bilgileri">${username}</span>
            </div>
            <button class="btn btn-nav btn-logout">
                <span class="logout-text">Çıkış Yap</span>
            </button>
        `;
    } else {
        userArea.innerHTML = `
            <a href="login.html" class="btn btn-nav btn-login">Giriş Yap</a>
            <a href="register.html" class="btn btn-nav btn-register">Kayıt Ol</a>
        `;
    }
}

function setupEventListeners() {
    const logoutBtn = document.querySelector('.btn-logout');
    if (logoutBtn) {
        logoutBtn.addEventListener('click', handleLogout);
    }
}

function handleLogout() {
    if (confirm('Çıkış yapmak istediğinize emin misiniz?')) {
        localStorage.removeItem('username');
        localStorage.removeItem('token');
        window.location.href = 'index.html';
    }
}

function setActivePage() {
    const currentPage = window.location.pathname.split('/').pop() || 'index.html';
    const activeLinks = {
        'index.html': 'home-link',
        'services.html': 'services-link',
        'appointment.html': 'appointment-link',
        'about.html': 'about-link',
        'contact.html': 'contact-link',
        '': 'home-link'
    };

    document.querySelectorAll('.nav-link').forEach(link => {
        link.classList.remove('active');
    });

    const activeLinkId = activeLinks[currentPage];
    if (activeLinkId) {
        const activeElement = document.getElementById(activeLinkId);
        if (activeElement) {
            activeElement.classList.add('active');
        }
    }
}

// Diğer sekmede giriş/çıkış yapıldığında güncelle
window.addEventListener('storage', function() {
    renderUserArea();
    setupEventListeners();
});

// Sayfa geçişlerinde de çalışması için
window.addEventListener('pageshow', function() {
    renderUserArea();
    setActivePage();
});
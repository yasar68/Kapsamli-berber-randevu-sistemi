// API base URL - projenize göre ayarlayın
const apiBaseUrl = 'http://localhost:5009/api/web';

document.addEventListener('DOMContentLoaded', function() {
    // Back to Top Button
    const backToTopButton = document.getElementById('backToTop');
    
    window.addEventListener('scroll', function() {
        if (window.pageYOffset > 300) {
            backToTopButton.classList.add('active');
        } else {
            backToTopButton.classList.remove('active');
        }
    });
    
    backToTopButton.addEventListener('click', function(e) {
        e.preventDefault();
        window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
    });
    
    // Initialize AOS animations
    AOS.init({
        duration: 800,
        easing: 'ease-in-out',
        once: true
    });
    
    // Check authentication and load barbers
    checkAuthAndLoadBarbers();
});

function getAuthHeaders() {
    const token = localStorage.getItem('token') || '';
    return {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
    };
}

function checkAuthAndLoadBarbers() {
    const token = localStorage.getItem('token');
    
    if (!token) {
        showError('Lütfen giriş yapınız. Yönlendiriliyorsunuz...');
        setTimeout(() => {
            window.location.href = 'login.html';
        }, 2000);
        return;
    }
    
    loadBarbers();
}

async function loadBarbers() {
    const barbersContainer = document.getElementById('barbers-container');
    
    // Show loading state
    barbersContainer.innerHTML = `
        <div class="loading-barbers">
            <div class="loading-spinner"></div>
        </div>
    `;
    
    try {
        const response = await fetch(`${apiBaseUrl}/webbarber/all`, {
            headers: getAuthHeaders()
        });
        
        if (response.status === 401) {
            // Token expired or invalid
            localStorage.removeItem('token');
            showError('Oturum süreniz doldu. Lütfen tekrar giriş yapınız. Yönlendiriliyorsunuz...');
            setTimeout(() => {
                window.location.href = 'login.html';
            }, 2000);
            return;
        }
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const barbers = await response.json();
        renderBarbers(barbers);
    } catch (error) {
        console.error('Error loading barbers:', error);
        showError('Berber bilgileri yüklenirken bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.');
    }
}

function renderBarbers(barbers) {
    const barbersContainer = document.getElementById('barbers-container');
    
    if (!barbers || barbers.length === 0) {
        showError('Henüz eklenmiş bir berber bulunmamaktadır.');
        return;
    }
    
    barbersContainer.innerHTML = barbers.map(barber => `
        <div class="team-card" data-aos="zoom-in">
            <div class="member-image">
                <img src="../pages/images/team-${barber.id}.jpg" alt="${barber.fullName}" onerror="this.src='../pages/images/barber-default.png'">
                <div class="social-links">
                    ${barber.phone ? `<a href="tel:${barber.phone}" title="Telefon"><i class="fas fa-phone-alt"></i></a>` : ''}
                    ${barber.email ? `<a href="mailto:${barber.email}" title="E-posta"><i class="fas fa-envelope"></i></a>` : ''}
                    ${barber.phone ? `<a href="https://wa.me/${barber.phone}" title="WhatsApp" target="_blank"><i class="fab fa-whatsapp"></i></a>` : ''}
                </div>
            </div>
            <div class="member-info">
                <h3>${barber.fullName}</h3>
                <span class="position">${barber.specialties.join(', ')}</span>
                <p>${getRandomExperience()} yıllık deneyimle ${barber.specialties[0] || 'berberlik'} konusunda uzman.</p>
            </div>
        </div>
    `).join('');
}

function showError(message) {
    const barbersContainer = document.getElementById('barbers-container');
    barbersContainer.innerHTML = `
        <div class="error-message">
            <i class="fas fa-exclamation-triangle"></i>
            <p>${message}</p>
        </div>
    `;
}

function getRandomExperience() {
    return Math.floor(Math.random() * 10) + 5; // 5-15 yıl arası rastgele deneyim
}
// workingHours.js

document.addEventListener('DOMContentLoaded', function() {
    updateWorkingHoursUI();
});

function updateWorkingHoursUI() {
    const workingHoursContainer = document.querySelector('.working-hours');
    if (!workingHoursContainer) return;

    // Bugünün gün bilgisini al
    const today = new Date();
    const dayNames = ['Pazar', 'Pazartesi', 'Salı', 'Çarşamba', 'Perşembe', 'Cuma', 'Cumartesi'];
    const todayName = dayNames[today.getDay()];
    
    // Çalışma saatleri bilgisi
    const workingHours = {
        weekdays: { start: '09:00', end: '20:00' },
        saturday: { start: '09:00', end: '20:00' },
        sunday: { start: '10:00', end: '16:00' }
    };

    // Bugünün çalışma saatlerini belirle
    let todayHours = '';
    if (today.getDay() === 0) { // Pazar
        todayHours = workingHours.sunday;
    } else if (today.getDay() === 6) { // Cumartesi
        todayHours = workingHours.saturday;
    } else { // Hafta içi
        todayHours = workingHours.weekdays;
    }

    // HTML içeriğini oluştur
    workingHoursContainer.innerHTML = `
        <h4><i class="fas fa-clock"></i> Çalışma Saatleri</h4>
        <div class="today-info">
            <span class="today-label">Bugün (${todayName})</span>
            <span class="today-hours">${todayHours.start} - ${todayHours.end}</span>
        </div>
        <div class="hours-row ${today.getDay() >= 1 && today.getDay() <= 5 ? 'active' : ''}">
            <span>Pazartesi - Cuma</span>
            <span>${workingHours.weekdays.start} - ${workingHours.weekdays.end}</span>
        </div>
        <div class="hours-row special ${today.getDay() === 6 ? 'active' : ''}">
            <span>Cumartesi</span>
            <span>${workingHours.saturday.start} - ${workingHours.saturday.end}</span>
        </div>

    `;
}
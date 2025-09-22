document.addEventListener('DOMContentLoaded', function () {
    // API Base URLs - Production'a hazır
    const API_BASE_URL = 'http://localhost:5009/api/admin';
    const API_WEB_BASE_URL = 'http://localhost:5009/api/web';
    
    // Authentication kontrolü
    let authToken = localStorage.getItem('token');
    if (!authToken) {
        window.location.href = '/pages/login.html';
        return;
    }

    // DOM Elements
    const sidebar = document.querySelector('.sidebar');
    const sidebarToggle = document.querySelector('.sidebar-toggle');
    const navItems = document.querySelectorAll('.sidebar-nav li');
    const contentSections = document.querySelectorAll('.content-section');
    const pageTitle = document.getElementById('page-title');
    const logoutBtn = document.getElementById('logout-btn');

    // Modal Elements
    const modals = document.querySelectorAll('.modal');
    const closeModalButtons = document.querySelectorAll('.close-modal');
    const addBarberBtn = document.getElementById('add-barber-btn');
    const addBarberModal = document.getElementById('add-barber-modal');
    const addWorkingHourBtn = document.getElementById('add-working-hour-btn');
    const addWorkingHourModal = document.getElementById('add-working-hour-modal');
    const addServiceBtn = document.getElementById('add-service-btn');
    const addServiceModal = document.getElementById('add-service-modal');
    const confirmationModal = document.getElementById('confirmation-modal');
    const cancelConfirmationBtn = document.getElementById('cancel-confirmation');
    const confirmActionBtn = document.getElementById('confirm-action');

    // Form Elements
    const addBarberForm = document.getElementById('add-barber-form');
    const addWorkingHourForm = document.getElementById('add-working-hour-form');
    const addServiceForm = document.getElementById('add-service-form');

    // Chart Elements
    const recentAppointmentsChartCtx = document.getElementById('recent-appointments-chart')?.getContext('2d');
    const servicesChartCtx = document.getElementById('services-chart')?.getContext('2d');
    const appointmentsChartCtx = document.getElementById('appointments-chart')?.getContext('2d');
    const revenueChartCtx = document.getElementById('revenue-chart')?.getContext('2d');

    // Data Variables
    let recentAppointmentsChart, servicesChart, appointmentsChart, revenueChart;
    let currentDeletionId = null;
    let currentDeletionType = null;

    // Initialize the dashboard
    initDashboard();

    // Core Functions
    function initDashboard() {
        setupEventListeners();
        loadInitialData();
        showSection('dashboard');
    }

    function setupEventListeners() {
        // Sidebar toggle
        sidebarToggle?.addEventListener('click', toggleSidebar);

        // Navigation items
        navItems.forEach(item => {
            item.addEventListener('click', function () {
                const section = this.getAttribute('data-section');
                showSection(section);
                updateActiveNavItem(this);
            });
        });

        // Modal close buttons
        closeModalButtons.forEach(button => {
            button.addEventListener('click', closeAllModals);
        });

        // Modal backdrop clicks
        modals.forEach(modal => {
            modal.addEventListener('click', function (e) {
                if (e.target === this) {
                    closeAllModals();
                }
            });
        });

        // Add buttons
        addBarberBtn?.addEventListener('click', () => addBarberModal?.classList.add('active'));
        addWorkingHourBtn?.addEventListener('click', async () => {
            await loadBarbersForWorkingHours();
            addWorkingHourModal?.classList.add('active');
        });
        addServiceBtn?.addEventListener('click', () => addServiceModal?.classList.add('active'));

        // Confirmation modal
        cancelConfirmationBtn?.addEventListener('click', closeAllModals);
        confirmActionBtn?.addEventListener('click', confirmDeletion);

        // Forms
        addBarberForm?.addEventListener('submit', handleAddBarber);
        addWorkingHourForm?.addEventListener('submit', handleAddWorkingHour);
        addServiceForm?.addEventListener('submit', handleAddService);

        // Logout
        logoutBtn?.addEventListener('click', handleLogout);

        // Filters
        document.getElementById('appointment-period')?.addEventListener('change', loadAppointments);
        document.getElementById('appointment-status')?.addEventListener('change', loadAppointments);
        document.getElementById('generate-report-btn')?.addEventListener('click', loadReports);
    }

    function loadInitialData() {
        loadDashboardStats();
        loadRecentActivity();
        initCharts();
    }

    // API Helper Functions
    async function fetchWithAuth(url, options = {}) {
        try {
            showLoading(true);

            const headers = {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authToken}`
            };

            const response = await fetch(url, {
                ...options,
                headers: {
                    ...headers,
                    ...(options.headers || {})
                }
            });

            if (!response.ok) {
                let errorMsg = 'API request failed';
                let errorDetails = null;

                try {
                    const errorData = await response.json();
                    errorMsg = errorData.message || errorMsg;
                    errorDetails = errorData.errors || null;
                } catch (e) {
                    errorMsg = response.statusText;
                }

                throw new EnhancedError(
                    `HTTP ${response.status}: ${errorMsg}`,
                    response.status,
                    errorDetails
                );
            }

            const contentLength = response.headers.get('content-length');
            if (contentLength === '0' || response.status === 204) return null;

            return await response.json();
        } catch (error) {
            console.error('API Request Error:', {
                url,
                error: error.toString(),
                stack: error.stack
            });

            if (error.status === 401) {
                window.location.href = '/pages/login.html';
                return;
            }

            throw error;
        } finally {
            showLoading(false);
        }
    }

    class EnhancedError extends Error {
        constructor(message, status, details = null) {
            super(message);
            this.status = status;
            this.details = details;
            this.name = 'EnhancedError';
        }
    }

    function showLoading(show) {
        const loader = document.getElementById('global-loader') || createLoader();
        loader.style.display = show ? 'flex' : 'none';
    }

    function createLoader() {
        const loader = document.createElement('div');
        loader.id = 'global-loader';
        loader.style.position = 'fixed';
        loader.style.top = '0';
        loader.style.left = '0';
        loader.style.width = '100%';
        loader.style.height = '100%';
        loader.style.backgroundColor = 'rgba(0,0,0,0.5)';
        loader.style.justifyContent = 'center';
        loader.style.alignItems = 'center';
        loader.style.zIndex = '1000';
        loader.innerHTML = '<div class="spinner"></div>';
        document.body.appendChild(loader);
        return loader;
    }

    // UI Functions
    function toggleSidebar() {
        sidebar?.classList.toggle('active');
    }

    function showSection(section) {
        contentSections.forEach(sec => {
            if (sec) sec.classList.remove('active');
        });

        const sectionToShow = document.getElementById(`${section}-section`);
        if (sectionToShow) {
            sectionToShow.classList.add('active');
            
            const sectionTitle = sectionToShow.querySelector('h2');
            if (sectionTitle && pageTitle) {
                pageTitle.textContent = sectionTitle.textContent;
            }

            switch (section) {
                case 'barbers':
                    loadBarbers();
                    break;
                case 'users':
                    loadUsers();
                    break;
                case 'appointments':
                    loadAppointments();
                    break;
                case 'working-hours':
                    loadWorkingHours();
                    break;
                case 'services':
                    loadServices();
                    break;
                case 'reports':
                    loadReports();
                    break;
            }
        }
    }

    function updateActiveNavItem(activeItem) {
        navItems.forEach(item => item.classList.remove('active'));
        activeItem.classList.add('active');
    }

    function closeAllModals() {
        modals.forEach(modal => modal.classList.remove('active'));
    }

    function showConfirmationModal(title, message, id, type) {
        const confirmationTitle = document.getElementById('confirmation-title');
        const confirmationMessage = document.getElementById('confirmation-message');

        if (confirmationTitle && confirmationMessage) {
            confirmationTitle.textContent = title;
            confirmationMessage.textContent = message;
            currentDeletionId = id;
            currentDeletionType = type;
            confirmationModal.classList.add('active');
        }
    }

    async function confirmDeletion() {
        if (!currentDeletionId || !currentDeletionType) return;

        try {
            let endpoint = '';
            switch (currentDeletionType) {
                case 'barber':
                    endpoint = `${API_BASE_URL}/barbers/${currentDeletionId}`;
                    break;
                case 'working-hour':
                    endpoint = `${API_BASE_URL}/working-hours/${currentDeletionId}`;
                    break;
                case 'service':
                    endpoint = `${API_WEB_BASE_URL}/webservices/${currentDeletionId}`;
                    break;
                case 'user':
                    endpoint = `${API_BASE_URL}/users/${currentDeletionId}`;
                    break;
                case 'appointment':
                    endpoint = `${API_WEB_BASE_URL}/appointments/${currentDeletionId}`;
                    break;
            }

            await fetchWithAuth(endpoint, { method: 'DELETE' });
            showToast(`${currentDeletionType} başarıyla silindi`, 'success');
            closeAllModals();

            switch (currentDeletionType) {
                case 'barber':
                    loadBarbers();
                    break;
                case 'working-hour':
                    loadWorkingHours();
                    break;
                case 'service':
                    loadServices();
                    break;
                case 'user':
                    loadUsers();
                    break;
                case 'appointment':
                    loadAppointments();
                    break;
            }

            currentDeletionId = null;
            currentDeletionType = null;
        } catch (error) {
            showToast(error.message, 'error');
        }
    }

    // Data Loading Functions
    async function loadDashboardStats() {
        try {
            // Barber count
            const barbers = await fetchWithAuth(`${API_WEB_BASE_URL}/webbarber/all`);
            const barberCount = document.getElementById('barber-count');
            if (barberCount) barberCount.textContent = barbers.length;

            // User count
            const users = await fetchWithAuth(`${API_BASE_URL}/users`);
            const userCount = document.getElementById('user-count');
            if (userCount) userCount.textContent = users.length;

            // Today's appointments
            const todayAppointments = await fetchWithAuth(`${API_BASE_URL}/appointments?period=today`);
            const appointmentCount = document.getElementById('appointment-count');
            if (appointmentCount) appointmentCount.textContent = todayAppointments.length;

            // Monthly revenue
            const startDate = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
            const endDate = new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0);
            const report = await fetchWithAuth(`${API_BASE_URL}/report?startDate=${startDate.toISOString()}&endDate=${endDate.toISOString()}`);
            const revenueCount = document.getElementById('revenue-count');
            if (revenueCount) revenueCount.textContent = `${report.totalRevenue} ₺`;
        } catch (error) {
            console.error('Error loading dashboard stats:', error);
            showToast('Dashboard istatistikleri yüklenirken hata oluştu', 'error');
        }
    }

    async function loadRecentActivity() {
        const activityList = document.querySelector('.activity-list');
        if (!activityList) return;

        try {
            const appointments = await fetchWithAuth(`${API_WEB_BASE_URL}/appointments`);

            const activities = appointments.slice(0, 5).map(appointment => ({
                icon: 'fas fa-calendar-check',
                message: `Yeni randevu oluşturuldu: ${appointment.barberName}`,
                time: formatTimeAgo(new Date(appointment.startTime))
            }));

            activityList.innerHTML = '';
            activities.forEach(activity => {
                const activityItem = document.createElement('div');
                activityItem.className = 'activity-item';
                activityItem.innerHTML = `
                    <div class="activity-icon">
                        <i class="${activity.icon}"></i>
                    </div>
                    <div class="activity-info">
                        <p>${activity.message}</p>
                        <small>${activity.time}</small>
                    </div>
                `;
                activityList.appendChild(activityItem);
            });
        } catch (error) {
            console.error('Error loading recent activity:', error);
        }
    }

    function formatTimeAgo(date) {
        const seconds = Math.floor((new Date() - date) / 1000);

        let interval = Math.floor(seconds / 31536000);
        if (interval >= 1) return `${interval} yıl önce`;

        interval = Math.floor(seconds / 2592000);
        if (interval >= 1) return `${interval} ay önce`;

        interval = Math.floor(seconds / 86400);
        if (interval >= 1) return `${interval} gün önce`;

        interval = Math.floor(seconds / 3600);
        if (interval >= 1) return `${interval} saat önce`;

        interval = Math.floor(seconds / 60);
        if (interval >= 1) return `${interval} dakika önce`;

        return `${Math.floor(seconds)} saniye önce`;
    }

    function initCharts() {
        // Recent Appointments Chart (Last 7 days)
        if (recentAppointmentsChartCtx) {
            fetchWithAuth(`${API_BASE_URL}/appointments?period=week`)
                .then(appointments => {
                    const days = ['Pzt', 'Sal', 'Çar', 'Per', 'Cum', 'Cmt', 'Paz'];
                    const counts = days.map(day => 0);

                    appointments.forEach(appointment => {
                        const day = new Date(appointment.startTime).getDay();
                        counts[day === 0 ? 6 : day - 1]++;
                    });

                    recentAppointmentsChart = new Chart(recentAppointmentsChartCtx, {
                        type: 'bar',
                        data: {
                            labels: days,
                            datasets: [{
                                label: 'Randevu Sayısı',
                                data: counts,
                                backgroundColor: 'rgba(67, 97, 238, 0.7)',
                                borderColor: 'rgba(67, 97, 238, 1)',
                                borderWidth: 1
                            }]
                        },
                        options: {
                            responsive: true,
                            maintainAspectRatio: false,
                            scales: {
                                y: {
                                    beginAtZero: true
                                }
                            }
                        }
                    });
                })
                .catch(error => {
                    console.error('Error loading appointments chart data:', error);
                });
        }

        // Services Chart
        if (servicesChartCtx) {
            fetchWithAuth(`${API_WEB_BASE_URL}/webservices/all`)
                .then(services => {
                    servicesChart = new Chart(servicesChartCtx, {
                        type: 'doughnut',
                        data: {
                            labels: services.map(s => s.name),
                            datasets: [{
                                data: services.map(s => s.price),
                                backgroundColor: [
                                    'rgba(67, 97, 238, 0.7)',
                                    'rgba(76, 201, 240, 0.7)',
                                    'rgba(239, 35, 60, 0.7)',
                                    'rgba(248, 150, 30, 0.7)',
                                    'rgba(243, 104, 224, 0.7)'
                                ],
                                borderWidth: 1
                            }]
                        },
                        options: {
                            responsive: true,
                            maintainAspectRatio: false
                        }
                    });
                })
                .catch(error => {
                    console.error('Error loading services chart data:', error);
                });
        }
    }

    async function loadBarbers() {
        const barbersTable = document.getElementById('barbers-table');
        if (!barbersTable) return;

        try {
            const barbers = await fetchWithAuth(`${API_WEB_BASE_URL}/webbarber/all`);
            const tbody = barbersTable.querySelector('tbody');
            if (!tbody) return;

            tbody.innerHTML = '';

            barbers.forEach(barber => {
                const tr = document.createElement('tr');

                tr.innerHTML = `
                    <td>${barber.id}</td>
                    <td><img src="${barber.photoUrl || 'https://via.placeholder.com/40'}" alt="${barber.fullName}" class="user-avatar"></td>
                    <td>${barber.fullName}</td>
                    <td>${barber.email}</td>
                    <td>${barber.phone}</td>
                    <td><span class="status-badge status-active">Aktif</span></td>
                    <td>
                        <button class="btn btn-sm btn-secondary"><i class="fas fa-edit"></i></button>
                        <button class="btn btn-sm btn-danger" onclick="showConfirmationModal('Berber Sil', 'Bu berberi silmek istediğinize emin misiniz?', ${barber.id}, 'barber')">
                            <i class="fas fa-trash"></i>
                        </button>
                    </td>
                `;

                tbody.appendChild(tr);
            });
        } catch (error) {
            console.error('Error loading barbers:', error);
            showToast('Berberler yüklenirken hata oluştu', 'error');
        }
    }

    async function loadUsers() {
        const usersTable = document.getElementById('users-table');
        if (!usersTable) return;

        try {
            const users = await fetchWithAuth(`${API_BASE_URL}/users`);
            const tbody = usersTable.querySelector('tbody');
            if (!tbody) return;

            tbody.innerHTML = '';

            users.forEach(user => {
                const tr = document.createElement('tr');

                tr.innerHTML = `
                    <td>${user.id}</td>
                    <td><img src="${user.photoUrl || 'https://via.placeholder.com/40'}" alt="${user.fullName}" class="user-avatar"></td>
                    <td>${user.fullName}</td>
                    <td>${user.email}</td>
                    <td>${user.phoneNumber}</td>
                    <td>${new Date(user.createdAt).toLocaleDateString()}</td>
                    <td>
                        <button class="btn btn-sm btn-secondary"><i class="fas fa-edit"></i></button>
                        <button class="btn btn-sm btn-danger" onclick="showConfirmationModal('Kullanıcı Sil', 'Bu kullanıcıyı silmek istediğinize emin misiniz?', ${user.id}, 'user')">
                            <i class="fas fa-trash"></i>
                        </button>
                    </td>
                `;

                tbody.appendChild(tr);
            });
        } catch (error) {
            console.error('Error loading users:', error);
            showToast('Kullanıcılar yüklenirken hata oluştu', 'error');
        }
    }

    async function loadAppointments() {
        const appointmentsTable = document.getElementById('appointments-table');
        if (!appointmentsTable) return;

        try {
            const period = document.getElementById('appointment-period')?.value || 'today';
            const status = document.getElementById('appointment-status')?.value || 'all';

            let appointments = [];
            if (period === 'all') {
                appointments = await fetchWithAuth(`${API_WEB_BASE_URL}/appointments`);
            } else {
                appointments = await fetchWithAuth(`${API_BASE_URL}/appointments?period=${period}`);
            }

            if (status !== 'all') {
                appointments = appointments.filter(a => a.status.toLowerCase() === status);
            }

            const tbody = appointmentsTable.querySelector('tbody');
            if (!tbody) return;

            tbody.innerHTML = '';

            appointments.forEach(appointment => {
                const tr = document.createElement('tr');

                let statusClass = '';
                let statusText = '';
                switch (appointment.status.toLowerCase()) {
                    case 'confirmed':
                        statusClass = 'status-active';
                        statusText = 'Onaylandı';
                        break;
                    case 'completed':
                        statusClass = 'status-success';
                        statusText = 'Tamamlandı';
                        break;
                    case 'cancelled':
                        statusClass = 'status-inactive';
                        statusText = 'İptal Edildi';
                        break;
                    case 'pending':
                        statusClass = 'status-pending';
                        statusText = 'Beklemede';
                        break;
                }

                const startTime = new Date(appointment.startTime);
                const endTime = new Date(startTime.getTime() + appointment.durationMinutes * 60000);

                tr.innerHTML = `
                    <td>${appointment.id}</td>
                    <td>
                        <div style="display: flex; align-items: center; gap: 8px;">
                            <img src="${appointment.customerPhoto || 'https://via.placeholder.com/40'}" alt="${appointment.customerName}" class="user-avatar">
                            ${appointment.customerName}
                        </div>
                    </td>
                    <td>
                        <div style="display: flex; align-items: center; gap: 8px;">
                            <img src="${appointment.barberPhoto || 'https://via.placeholder.com/40'}" alt="${appointment.barberName}" class="user-avatar">
                            ${appointment.barberName}
                        </div>
                    </td>
                    <td>${startTime.toLocaleString()}</td>
                    <td>${appointment.serviceNames?.join(', ') || ''}</td>
                    <td>${appointment.price} ₺</td>
                    <td><span class="status-badge ${statusClass}">${statusText}</span></td>
                    <td>
                        <button class="btn btn-sm btn-secondary"><i class="fas fa-edit"></i></button>
                        <button class="btn btn-sm btn-danger" onclick="showConfirmationModal('Randevu Sil', 'Bu randevuyu silmek istediğinize emin misiniz?', ${appointment.id}, 'appointment')">
                            <i class="fas fa-trash"></i>
                        </button>
                    </td>
                `;

                tbody.appendChild(tr);
            });
        } catch (error) {
            console.error('Error loading appointments:', error);
            showToast('Randevular yüklenirken hata oluştu', 'error');
        }
    }

    async function loadWorkingHours() {
        const workingHoursTable = document.getElementById('working-hours-table');
        if (!workingHoursTable) return;

        try {
            const barbers = await fetchWithAuth(`${API_WEB_BASE_URL}/webbarber/all`);
            const tbody = workingHoursTable.querySelector('tbody');
            if (!tbody) return;

            tbody.innerHTML = '';

            for (const barber of barbers) {
                try {
                    const workingHours = await fetchWithAuth(`${API_BASE_URL}/working-hours/${barber.id}`);

                    workingHours.forEach(wh => {
                        const tr = document.createElement('tr');

                        tr.innerHTML = `
                            <td>${wh.id}</td>
                            <td>${barber.fullName}</td>
                            <td>${getDayName(wh.dayOfWeek)}</td>
                            <td>${wh.startTime}</td>
                            <td>${wh.endTime}</td>
                            <td><i class="fas fa-check" style="color: var(--success);"></i></td>
                            <td>
                                <button class="btn btn-sm btn-secondary"><i class="fas fa-edit"></i></button>
                                <button class="btn btn-sm btn-danger" onclick="showConfirmationModal('Çalışma Saati Sil', 'Bu çalışma saatini silmek istediğinize emin misiniz?', ${wh.id}, 'working-hour')">
                                    <i class="fas fa-trash"></i>
                                </button>
                            </td>
                        `;

                        tbody.appendChild(tr);
                    });
                } catch (error) {
                    console.error(`Error loading working hours for barber ${barber.id}:`, error);
                }
            }
        } catch (error) {
            console.error('Error loading working hours:', error);
            showToast('Çalışma saatleri yüklenirken hata oluştu', 'error');
        }
    }

    async function loadBarbersForWorkingHours() {
        try {
            const barberSelect = document.getElementById('working-hour-barber');
            if (!barberSelect) return;

            const barbers = await fetchWithAuth(`${API_WEB_BASE_URL}/webbarber/all`);
            barberSelect.innerHTML = '';

            barbers.forEach(barber => {
                const option = document.createElement('option');
                option.value = barber.id;
                option.textContent = barber.fullName;
                barberSelect.appendChild(option);
            });
        } catch (error) {
            console.error('Error loading barbers for working hours:', error);
            showToast('Berberler yüklenirken hata oluştu', 'error');
        }
    }

    async function loadServices() {
        const servicesTable = document.getElementById('services-table');
        if (!servicesTable) return;

        try {
            const services = await fetchWithAuth(`${API_WEB_BASE_URL}/webservices/all`);
            const tbody = servicesTable.querySelector('tbody');
            if (!tbody) return;

            tbody.innerHTML = '';

            services.forEach(service => {
                const tr = document.createElement('tr');

                tr.innerHTML = `
                    <td>${service.id}</td>
                    <td>${service.name}</td>
                    <td>${service.description || '-'}</td>
                    <td>${service.durationMinutes}</td>
                    <td>${service.price} ₺</td>
                    <td>${service.isActive ? '<i class="fas fa-check" style="color: var(--success);"></i>' : '<i class="fas fa-times" style="color: var(--danger);"></i>'}</td>
                    <td>
                        <button class="btn btn-sm btn-secondary"><i class="fas fa-edit"></i></button>
                        <button class="btn btn-sm btn-danger" onclick="showConfirmationModal('Hizmet Sil', 'Bu hizmeti silmek istediğinize emin misiniz?', ${service.id}, 'service')">
                            <i class="fas fa-trash"></i>
                        </button>
                    </td>
                `;

                tbody.appendChild(tr);
            });
        } catch (error) {
            console.error('Error loading services:', error);
            showToast('Hizmetler yüklenirken hata oluştu', 'error');
        }
    }

    async function loadReports() {
        try {
            const startDate = document.getElementById('report-start-date')?.value || 
                             new Date(new Date().getFullYear(), new Date().getMonth(), 1).toISOString().split('T')[0];
            const endDate = document.getElementById('report-end-date')?.value || 
                            new Date().toISOString().split('T')[0];

            const report = await fetchWithAuth(`${API_BASE_URL}/report?startDate=${startDate}&endDate=${endDate}`);
            
            document.getElementById('total-appointments').textContent = report.totalAppointments;
            document.getElementById('total-revenue').textContent = `${report.totalRevenue} ₺`;
            
            updateCharts(report);
        } catch (error) {
            console.error('Error loading reports:', error);
            showToast('Raporlar yüklenirken hata oluştu', 'error');
        }
    }

    function updateCharts(report) {
        if (appointmentsChart) appointmentsChart.destroy();
        if (revenueChart) revenueChart.destroy();

        if (appointmentsChartCtx) {
            appointmentsChart = new Chart(appointmentsChartCtx, {
                type: 'line',
                data: {
                    labels: ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran'],
                    datasets: [{
                        label: 'Randevu Sayısı',
                        data: [120, 150, 180, 200, 170, 220],
                        borderColor: 'rgba(67, 97, 238, 1)',
                        backgroundColor: 'rgba(67, 97, 238, 0.1)',
                        borderWidth: 2,
                        fill: true
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {
                        y: {
                            beginAtZero: true
                        }
                    }
                }
            });
        }

        if (revenueChartCtx) {
            revenueChart = new Chart(revenueChartCtx, {
                type: 'bar',
                data: {
                    labels: ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran'],
                    datasets: [{
                        label: 'Gelir (₺)',
                        data: [8500, 9200, 10500, 12450, 11000, 13500],
                        backgroundColor: 'rgba(76, 201, 240, 0.7)',
                        borderColor: 'rgba(76, 201, 240, 1)',
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {
                        y: {
                            beginAtZero: true
                        }
                    }
                }
            });
        }
    }

    // Form Handlers
    async function handleAddBarber(e) {
        e.preventDefault();

        try {
            const formData = {
                fullName: document.getElementById('barber-name').value,
                email: document.getElementById('barber-email').value,
                phone: document.getElementById('barber-phone').value,
                bio: document.getElementById('barber-bio').value,
                specialties: ["Saç Kesimi", "Sakal Tıraşı"]
            };

            if (!formData.fullName || !formData.email || !formData.phone) {
                throw new Error("Lütfen zorunlu alanları doldurun");
            }

            const result = await fetchWithAuth(`${API_BASE_URL}/barbers`, {
                method: 'POST',
                body: JSON.stringify(formData)
            });

            showToast('Berber başarıyla eklendi', 'success');
            closeAllModals();
            loadBarbers();
            e.target.reset();
        } catch (error) {
            console.error('Add barber error:', error);
            showToast(error.message, 'error');
        }
    }

    async function handleAddWorkingHour(e) {
        e.preventDefault();

        try {
            const formData = {
                barberId: parseInt(document.getElementById('working-hour-barber').value),
                dayOfWeek: document.getElementById('working-hour-day').value,
                startTime: document.getElementById('working-hour-start').value,
                endTime: document.getElementById('working-hour-end').value,
                isActive: document.getElementById('working-hour-active').checked
            };

            if (!formData.barberId || !formData.dayOfWeek || !formData.startTime || !formData.endTime) {
                throw new Error("Lütfen tüm zorunlu alanları doldurun");
            }

            await fetchWithAuth(`${API_BASE_URL}/working-hours`, {
                method: 'POST',
                body: JSON.stringify(formData)
            });

            showToast('Çalışma saati başarıyla eklendi', 'success');
            closeAllModals();
            loadWorkingHours();
            e.target.reset();
        } catch (error) {
            console.error('Add working hour error:', error);
            showToast(error.message, 'error');
        }
    }

    async function handleAddService(e) {
        e.preventDefault();

        try {
            const formData = {
                name: document.getElementById('service-name').value,
                description: document.getElementById('service-description').value,
                durationMinutes: parseInt(document.getElementById('service-duration').value),
                price: parseFloat(document.getElementById('service-price').value),
                isActive: document.getElementById('service-active').checked
            };

            await fetchWithAuth(`${API_WEB_BASE_URL}/webservices`, {
                method: 'POST',
                body: JSON.stringify(formData)
            });

            showToast('Hizmet başarıyla eklendi', 'success');
            closeAllModals();
            loadServices();
            e.target.reset();
        } catch (error) {
            showToast(error.message, 'error');
        }
    }

    async function handleLogout() {
        try {
            localStorage.removeItem('token');
            window.location.href = '/pages/login.html';
        } catch (error) {
            console.error('Error during logout:', error);
            showToast('Çıkış yapılırken hata oluştu', 'error');
        }
    }

    // Helper Functions
    function getDayName(day) {
        const days = {
            'Monday': 'Pazartesi',
            'Tuesday': 'Salı',
            'Wednesday': 'Çarşamba',
            'Thursday': 'Perşembe',
            'Friday': 'Cuma',
            'Saturday': 'Cumartesi',
            'Sunday': 'Pazar'
        };
        return days[day] || day;
    }

    function showToast(message, type) {
        console.log(`${type.toUpperCase()}: ${message}`);

        const toast = document.createElement('div');
        toast.className = `toast toast-${type}`;
        toast.textContent = message;
        document.body.appendChild(toast);

        setTimeout(() => {
            toast.remove();
        }, 3000);
    }

    // Make functions available globally for inline event handlers
    window.showConfirmationModal = showConfirmationModal;
});

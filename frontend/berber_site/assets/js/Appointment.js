const tarihInput = document.getElementById('tarih');
const berberSelect = document.getElementById('berber');
const saatSelect = document.getElementById('saat');
const hizmetSelect = document.getElementById('hizmet');
const notInput = document.getElementById('not');
const smsHatirlatma = document.getElementById('smsHatirlatma');
const randevuForm = document.getElementById('randevuForm');
const randevuListesiDiv = document.getElementById('randevuListesi');
const filterSelect = document.getElementById('filterRandevu');
const filterBerber = document.getElementById('filterBerber');

const today = new Date().toISOString().split('T')[0];
tarihInput.setAttribute('min', today);

const apiBaseUrl = 'http://localhost:5009/api/web';

function getAuthHeaders() {
  const token = localStorage.getItem('token') || '';
  return {
    'Authorization': `Bearer ${token}`
  };
}

async function loadBarbers() {
  try {
    const res = await fetch(`${apiBaseUrl}/webbarber/all`, {
      headers: getAuthHeaders()
    });
    if (!res.ok) throw new Error('Berberler yüklenemedi');
    const data = await res.json();

    berberSelect.innerHTML = '<option value="" disabled selected>Seçiniz</option>';
    data.forEach(b => {
      const option = document.createElement('option');
      option.value = b.id;
      option.textContent = b.fullName;
      berberSelect.appendChild(option);
    });
  } catch (err) {
    berberSelect.innerHTML = `<option disabled>${err.message}</option>`;
  }
}

async function loadServices() {
  try {
    const res = await fetch(`${apiBaseUrl}/webservices/all`, {
      headers: getAuthHeaders()
    });
    if (!res.ok) throw new Error('Hizmetler yüklenemedi');
    const data = await res.json();

    hizmetSelect.innerHTML = '<option value="" disabled selected>Seçiniz</option>';
    data.forEach(h => {
      const option = document.createElement('option');
      option.value = h.id;
      option.textContent = h.name;
      hizmetSelect.appendChild(option);
    });
  } catch (err) {
    hizmetSelect.innerHTML = `<option disabled>${err.message}</option>`;
  }
}

let randevular = [];

async function fetchRandevular() {
  try {
    const res = await fetch(`${apiBaseUrl}/appointments/by-customer`, {
      headers: getAuthHeaders()
    });
    if (!res.ok) throw new Error('Randevular alınamadı.');
    const data = await res.json();

    randevular = data.map(r => ({
      tarih: r.startTime.split('T')[0],
      saat: new Date(r.startTime).toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' }),
      hizmet: r.serviceNames ? r.serviceNames.join(', ') : '',
      berber: r.barberName || 'Berber Bilgisi Yok',
      durum: (r.status || '').toLowerCase(),
      not: r.notes || ''
    }));

    renderRandevular(filterSelect.value);
    updateBerberFilterDropdown();
  } catch (err) {
    randevuListesiDiv.innerHTML = `<p class="text-danger">Randevular yüklenemedi: ${err.message}</p>`;
  }
}

function updateBerberFilterDropdown() {
  const uniqueBarbers = [...new Set(randevular.map(r => r.berber))];
  filterBerber.innerHTML = '<option value="all">Tümü</option>';
  uniqueBarbers.forEach(name => {
    const opt = document.createElement('option');
    opt.value = name;
    opt.textContent = name;
    filterBerber.appendChild(opt);
  });
}

function renderRandevular(filter = 'all') {
  const selectedBerber = filterBerber.value;
  randevuListesiDiv.innerHTML = '';

  let filtered = randevular;

  if (filter !== 'all') {
    if (filter === 'gelecek') {
      filtered = filtered.filter(r => new Date(`${r.tarih}T${r.saat}`) >= new Date());
    } else if (filter === 'gecmis') {
      filtered = filtered.filter(r => new Date(`${r.tarih}T${r.saat}`) < new Date());
    } else {
      filtered = filtered.filter(r => r.durum === filter);
    }
  }

  if (selectedBerber !== 'all') {
    filtered = filtered.filter(r => r.berber === selectedBerber);
  }

  if (filtered.length === 0) {
    randevuListesiDiv.innerHTML = '<p>Gösterilecek randevu bulunamadı.</p>';
    return;
  }

  filtered.forEach(r => {
    const durumYazisi = {
      confirmed: { icon: '✅', text: 'Onaylandı', className: 'onaylandi' },
      cancelled: { icon: '❌', text: 'İptal', className: 'iptal' },
      gecmis: { icon: '🕒', text: 'Geçmiş', className: 'gecmis' },
      gelecek: { icon: '⏳', text: 'Gelecek', className: 'onaylandi' }
    };

    const durum = durumYazisi[r.durum] || { icon: '❓', text: r.durum, className: '' };

    const card = document.createElement('div');
    card.classList.add('appointment-card');
    card.innerHTML = `
      <div class="appointment-header">🧾 ${new Date(r.tarih).toLocaleDateString('tr-TR', { day: '2-digit', month: 'long', year: 'numeric' })} - ${r.saat}</div>
      <div class="appointment-info">Hizmet: ${r.hizmet}</div>
      <div class="appointment-info">Berber: ${r.berber}</div>
      <div class="appointment-info">Not: ${r.not || '-'}</div>
      <div class="appointment-info status ${durum.className}">Durum: ${durum.icon} ${durum.text}</div>
    `;
    randevuListesiDiv.appendChild(card);
  });
}

function generateHalfHourSlots(timeRanges) {
  const slots = [];

  const timeToMinutes = timeStr => {
    const [hh, mm] = timeStr.split(':').map(Number);
    return hh * 60 + mm;
  };

  const minutesToTime = mins => {
    const hh = Math.floor(mins / 60).toString().padStart(2, '0');
    const mm = (mins % 60).toString().padStart(2, '0');
    return `${hh}:${mm}`;
  };

  timeRanges.forEach(range => {
    let startMins = timeToMinutes(range.start);
    const endMins = timeToMinutes(range.end);

    while (startMins < endMins) {
      slots.push(minutesToTime(startMins));
      startMins += 30;
    }
  });

  return slots;
}

async function loadAvailableSlots() {
  saatSelect.innerHTML = '<option value="" disabled selected>Yükleniyor...</option>';

  const tarih = tarihInput.value;
  const berberId = berberSelect.value;

  if (!tarih || !berberId) {
    saatSelect.innerHTML = '<option value="" disabled>Önce tarih ve berber seçin</option>';
    return;
  }

  try {
    const res = await fetch(`${apiBaseUrl}/appointments/available-timeslots?barberId=${berberId}&date=${tarih}`, {
      headers: getAuthHeaders()
    });
    if (!res.ok) throw new Error('Boş saatler alınamadı');

    const data = await res.json();
    const slots = generateHalfHourSlots(data);

    if (slots.length === 0) {
      saatSelect.innerHTML = '<option value="" disabled>Uygun saat bulunamadı</option>';
      return;
    }

    saatSelect.innerHTML = '<option value="" disabled selected>Seçiniz</option>';
    slots.forEach(saat => {
      const option = document.createElement('option');
      option.value = saat;
      option.textContent = saat;
      saatSelect.appendChild(option);
    });
  } catch (error) {
    saatSelect.innerHTML = `<option disabled>${error.message}</option>`;
  }
}

document.getElementById('randevuForm').addEventListener('submit', async e => {
  e.preventDefault();

  const tarih = document.getElementById('tarih').value;
  const saat = document.getElementById('saat').value;
  const hizmetId = document.getElementById('hizmet').value;
  const berberId = document.getElementById('berber').value;
  const not = document.getElementById('not').value.trim();
  const emailBildirimiGonder = document.getElementById('smsHatirlatma').checked;

  if (!tarih || !saat || !hizmetId || !berberId) {
    alert('Lütfen tüm zorunlu alanları doldurun.');
    return;
  }

  // Seçilen tarih ve saat
  const secilenTarihSaat = new Date(`${tarih}T${saat}:00`);
  const simdi = new Date();

  if (secilenTarihSaat < simdi) {
    alert('Geçmiş bir tarihe veya saate randevu alamazsınız. Lütfen geçerli bir tarih ve saat seçin.');
    return;
  }

  const startTime = `${tarih}T${saat}:00`;

  const dto = {
    serviceIds: [parseInt(hizmetId)],

    barberId: parseInt(berberId),
    startTime: startTime,
    note: not,
    emailBildirimiGonder: emailBildirimiGonder
  };

  try {
    const res = await fetch(`${apiBaseUrl}/appointments`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...getAuthHeaders()
      },
      body: JSON.stringify(dto)
    });

    const response = await res.json();

    console.log('API cevabı:', response);

    if (!res.ok) {
      // API’den validation hataları olabilir, varsa detayları gösterelim
      if (response.errors) {
        let mesajlar = '';
        for (const key in response.errors) {
          if (Array.isArray(response.errors[key])) {
            mesajlar += `${response.errors[key].join(', ')}\n`;
          } else {
            mesajlar += `${response.errors[key]}\n`;
          }
        }
        alert(`Hata oluştu:\n${mesajlar}`);
      } else {
        alert(`Hata: ${response.Message || 'Randevu oluşturulamadı'}`);
      }
      return;
    }

    alert('Randevunuz başarıyla kaydedildi!');
    e.target.reset();
    document.getElementById('tarih').setAttribute('min', today);
    document.getElementById('saat').innerHTML = '<option value="" disabled selected>Önce tarih ve berber seçin</option>';
    await fetchRandevular();

  } catch (error) {
    alert('Beklenmedik bir hata oluştu: ' + error.message);
  }
});

tarihInput.addEventListener('change', loadAvailableSlots);
berberSelect.addEventListener('change', loadAvailableSlots);
filterSelect.addEventListener('change', () => renderRandevular(filterSelect.value));
filterBerber.addEventListener('change', () => renderRandevular(filterSelect.value));

async function init() {
  await Promise.all([loadBarbers(), loadServices()]);
  await fetchRandevular();
}

init();

# Kapsamlı Berber Randevu Sistemi

**Kapsamlı Berber Randevu Sistemi**, berberlerden randevu almayı kolaylaştırmak ve berberlerin dijital vitrinlerini oluşturmak için geliştirilmiş kapsamlı bir platformdur. Hem kullanıcı hem de berber için hızlı, güvenilir ve görsel olarak anlaşılır bir deneyim sunar.

---

## 📌 Genel Bakış

Proje iki ana bileşenden oluşur:

1. **Backend (ASP.NET API)**  
   - Sağlam ve güvenilir bir altyapı sağlar.  
   - SQLite veritabanı kullanır; proje yayına alınmadığı için taşınabilir ve basittir.  
   - Berberler, hizmetler, müşteriler ve raporlar backend üzerinden yönetilir.

2. **Frontend (HTML, CSS, JavaScript)**  
   - Kullanıcı ve berber etkileşimini görsel olarak sağlar.  
   - Berberin vitrinini oluşturur: açık adres, konum, hizmetler, çalışanlar ve yapılan işler resimlerle gösterilir.  
   - Randevu alma, görüntüleme ve not bırakma işlemleri burada gerçekleşir.

---

## ✨ Özellikler

### Berberler için:
- Berber ekleme, çıkarma ve güncelleme işlemleri.  
- Hizmet ekleme, fiyatlandırma ve düzenleme.  
- Günlük, haftalık ve aylık raporlar.  
- Admin panelinde müşteri verileri ve grafiklerle analiz.  
- Çalışan ve çalışma saatleri yönetimi.

### Müşteriler için:
- İleri tarih ve saat seçimi ile randevu alma.  
- O saatte müsait berberlerin listelenmesi ve seçim.  
- İlgili hizmetin süresine göre randevu çakışmasının engellenmesi.  
- İsteğe bağlı not bırakma.  
- Randevu alındığında hem müşteriye hem berbere e-posta bilgilendirmesi.  
- Randevuya 30 dakika kala müşteriye hatırlatma e-postası.

### Admin Paneli:
- Gelen müşteriler ve gelir grafikleri.  
- Hizmet ve berber yüzdeleri analizi.  
- Berber, hizmet ve çalışma saatlerinin güncellenmesi.  
- Yeni berber veya hizmet ekleme/çıkarma.  
- Tarihler arasında rapor talep etme.

---

## ⚙️ Teknolojik Altyapı

- **Backend:** ASP.NET Web API  
- **Veritabanı:** SQLite  
- **Frontend:** HTML, CSS, JavaScript  
- **Mail Bildirimleri:** Randevu alındığında ve yaklaşan randevularda otomatik bilgilendirme  
- **Depo Yapısı:** backend/BerberApp.API/ # ASP.NET API
frontend/ # HTML, CSS, JS

---

## ⚠️ Bilinen Hatalar / Sınırlamalar

- Frontend tarafında bazı alanlar hatalı veya çalışmıyor.  
- Admin panelinde bazı özellikler tam işlevsel değil.  
- Sistem yayına alınmadığı için SQLite kullanılmıştır, büyük ölçekli veri için sınırlamalar vardır.

---

## 📝 Kullanım Mantığı

- Müşteri ileri tarih ve saat seçer.  
- Sistem, seçilen saate uygun berberleri listeler.  
- Müşteri berber ve hizmet seçimi yapar, randevu oluşturulur.  
- Randevu süresince aynı berberin başka randevu alamaması sağlanır.  
- E-posta bildirimleri hem müşteriye hem berbere gider.  
- 30 dakika kala müşteriye hatırlatma gönderilir.  
- Admin panelinden raporlar alınabilir, güncellemeler yapılabilir.

---

## 🚀 Kurulum

1. Repo’yu klonlayın:  

    ```bash
        git clone https://github.com/yasar68/      Kapsamli-berber-randevu-sistemi.git

2. Backend’e gidin ve bağımlılıkları yükleyin:

    ```bash
        cd backend/BerberApp.API
        dotnet restore
        dotnet run

3. Frontend’i tarayıcıda çalıştırmak için kök dizinde:

    ```bash
        python -m http.server 8000

Bu sistem, berberler ve müşteriler için hızlı, güvenilir ve görsel olarak anlaşılır bir randevu deneyimi sunmayı amaçlamaktadır.

---


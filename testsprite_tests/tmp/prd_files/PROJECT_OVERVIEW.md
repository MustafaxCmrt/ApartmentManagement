# ApartmentManagement — Proje Dokümantasyonu (TestSprite Hazırlık)

> Bu doküman TestSprite'a (veya başka bir test otomasyon aracına) projeyi tanıtmak için hazırlandı. Tüm endpoint'leri, request/response şemaları, auth akışı, multi-tenancy, roller ve test credential'ları içerir.

---

## 1. Proje Hakkında

**Ad:** ApartmentManagement (Apartman Yönetim Sistemi)
**Stack:** .NET 8, ASP.NET Core, EF Core 8, MySQL, JWT, FluentValidation, MediatR, Serilog, Swagger
**Mimari:** Clean Architecture (Domain → Application → Infrastructure / Persistence → API)
**Çoklu Kiracılık:** Evet, `TenantId` ile global query filter (her tenant kendi verisini görür; SuperAdmin tümünü görür)
**Dil:** Backend API + Türkçe veri/mesajlar (UI yok, frontend henüz yazılmadı)
**Veritabanı:** MySQL — connection: `Server=localhost;Port=3306;Database=apartmanyonetim;User=root;Password=12345678;`

### Proje yapısı

```
src/
├── ApartmentManagement.Domain/         # Entity'ler, Enum'lar, Domain exceptions
├── ApartmentManagement.Application/    # Commands, Queries, Validators, DTOs, Common interfaces
├── ApartmentManagement.Infrastructure/ # JWT, PasswordHasher (BCrypt), EmailService, DateTime
├── ApartmentManagement.Persistence/    # DbContext, Configurations, Migrations, Seeder
└── ApartmentManagement.API/            # Controllers, Middlewares, Extensions (DI, CORS, Auth, Swagger)
```

---

## 2. Uygulamayı Çalıştırma

```bash
cd /Users/comert/Desktop/ApartmentManagement
dotnet build
dotnet run --project src/ApartmentManagement.API
```

- **API base URL:** `http://localhost:5000` (default Kestrel) veya `https://localhost:5001`
- **Swagger UI:** `http://localhost:5000/swagger` (sadece Development)
- **Healthcheck:** `GET /health/live`, `GET /health/ready`
- **API versioning:** URL segment ile — `api/v{version}/...`. Şu an tek versiyon: `v1`.

---

## 3. Auth Akışı (Telefon-Tabanlı Login)

### 3.1 Önemli notlar
- **Login email değil, TELEFON ile** (`telefon` field'ı).
- Telefon **TR formatlarını otomatik normalize eder** (`+90`, `0...`, `5...` hepsi `+905XXXXXXXXX`'e dönüşür).
- Email login'de kullanılmaz ama register/invite akışlarında lazım.
- **JWT access token:** 15 dakika, **refresh token:** 7 gün (rotation'lı).
- Refresh token kullanıldığında eski revoke edilir, yenisi döner.
- `Authorization: Bearer <accessToken>` header'ı tüm korumalı endpoint'lerde zorunlu.

### 3.2 Rate limit
- **`auth-strict`**: 5 istek / dakika — `/auth/login`, `/auth/register-yonetici`, `/auth/davet-kabul`, `/auth/refresh`'e uygulanır.
- **`api-general`**: 100 istek / dakika — diğer endpoint'lere.

### 3.3 Şifre kuralları
- En az **8 karakter**, **en az 1 harf + 1 rakam**
- Maksimum 100 karakter
- ChangePassword'de yeni şifre eskisi ile aynı olamaz

---

## 4. Roller ve Yetki

| Role | Açıklama | TenantId |
|---|---|---|
| `SuperAdmin` | Tüm tenant'lara erişebilir, sistem yönetir | null |
| `TenantAdmin` | Bir tenant'ı yönetir (apartman yöneticisi) | doludur |
| `Resident` | Bir tenant içindeki sakin (dairesi olan kişi) | doludur |

**Endpoint authorize kuralları kısaca:**
- `/tenants/**` → sadece `SuperAdmin`
- `/buildings/**`, `/apartments/**`, `/residents/**`, `/decisions/**`, `/meetings/**`, `/dashboard/**` → sadece `TenantAdmin`
- `/announcements`, `/maintenance-tickets`, `/dues/{id}` → `TenantAdmin` + `Resident` (resident sadece okuyabilir; bazıları create de yapabilir)
- `/auth/login`, `/auth/register-yonetici`, `/auth/davet-kabul`, `/auth/refresh` → `AllowAnonymous` (token yok)

---

## 5. Test Credential'ları (Seeder Üretiyor)

Seeder Development ortamında otomatik çalışır. Üç hazır kullanıcı:

| Email | Telefon | Şifre | Rol | Tenant |
|---|---|---|---|---|
| `admin@admin.com` | `+905550000001` | `Admin44.` | SuperAdmin | — |
| `yonetici@demo.com` | `+905551112233` | `Demo44.` | TenantAdmin | demo |
| `sakin@demo.com` | `+905554445566` | `Demo44.` | Resident | demo |

**Login örneği:**
```http
POST /api/v1/auth/login
Content-Type: application/json

{ "telefon": "+905551112233", "sifre": "Demo44." }
```

Yanıt:
```json
{
  "userId": "...",
  "tenantId": "...",
  "accessToken": "eyJhbGc...",
  "refreshToken": "...",
  "expiresAt": "2026-05-24T19:30:00Z",
  "expiresIn": 900,
  "user": { "id": "...", "tenantId": "...", "email": "...", "fullName": "...", "role": "TenantAdmin" }
}
```

**Demo tenant'a ek hazır data:**
- 1 building ("A Block"), 10 apartment (1-1, 1-2, 2-1, ..., 5-2), 1 resident kaydı (sakin@demo.com → apartment 1-1)

---

## 6. Tüm Endpoint'ler

> Tüm korumalı endpoint'ler `Authorization: Bearer <token>` header ister. Tüm response body'leri `Result<T>` pattern'inde (`success`, `data`, `error`) — başarılıysa data dolu, hata varsa error doludur.

### 6.1 Auth (`/api/v1/auth`) — Anonymous + rate-limit'li
| Method | Path | Body | Açıklama |
|---|---|---|---|
| POST | `/login` | `{ telefon, sifre }` | Telefon ile giriş, JWT döner |
| POST | `/register-yonetici` | RegisterAdmin (aşağıda) | Yeni tenant + admin user oluşturur |
| POST | `/davet-kabul` | `{ inviteToken, sifre, sifreTekrar }` | Davet kabulü ile sakin user oluşturur |
| POST | `/refresh` | `{ refreshToken }` | Token yenileme (rotation) |
| POST | `/logout` | `{ refreshToken }` | Refresh token'ı revoke eder |
| POST | `/change-password` | `{ eskiSifre, yeniSifre, yeniSifreTekrar }` | Şifre değiştirir, tüm aktif token'ları revoke eder |
| GET | `/me` | — | Mevcut kullanıcının bilgilerini döner |

### 6.2 Tenants (`/api/v1/tenants`) — SuperAdmin only
| Method | Path | Açıklama |
|---|---|---|
| GET | `/` | Tüm tenant'lar (paged query) |
| GET | `/{id}` | ID ile tenant detay |
| POST | `/` | Yeni tenant oluştur |
| PUT | `/{id}` | Tenant güncelle |
| DELETE | `/{id}` | Tenant sil (soft) |
| PATCH | `/{id}/active-status` | Tenant'ı aktif/pasif yap |
| GET | `/{id}/statistics` | Tenant istatistikleri |

### 6.3 Buildings (`/api/v1/buildings`) — TenantAdmin only
| Method | Path |
|---|---|
| GET | `/` (list) |
| GET | `/{id}` |
| POST | `/` (create) |
| PUT | `/{id}` (update) |
| DELETE | `/{id}` |
| GET | `/{id}/apartments` (buildingin daireleri) |

### 6.4 Apartments (`/api/v1/apartments`) — TenantAdmin only
| Method | Path |
|---|---|
| GET | `/` (list) |
| GET | `/{id}` |
| POST | `/` (single create) |
| POST | `/batch` (bulk create) |
| PUT | `/{id}` |
| DELETE | `/{id}` |
| GET | `/{id}/residents` |
| GET | `/{id}/dues` |

### 6.5 Residents (`/api/v1/residents`) — TenantAdmin only
| Method | Path |
|---|---|
| GET | `/` (list, query: ApartmentId, ResidentType filterları) |
| GET | `/{id}` |
| POST | `/` |
| PUT | `/{id}` |
| DELETE | `/{id}` |
| POST | `/{id}/user-invite` (sakine davet linki üretir) |

### 6.6 Announcements (`/api/v1/announcements`) — mixed
| Method | Path | Role |
|---|---|---|
| GET | `/` | TenantAdmin, Resident |
| GET | `/{id}` | TenantAdmin, Resident |
| POST | `/` | TenantAdmin |
| PUT | `/{id}` | TenantAdmin |
| DELETE | `/{id}` | TenantAdmin |
| POST | `/{id}/read` | TenantAdmin, Resident (okundu işaretle) |
| GET | `/{id}/read-statistics` | TenantAdmin |

### 6.7 Decisions (`/api/v1/decisions`) — TenantAdmin only
| Method | Path |
|---|---|
| GET | `/` |
| GET | `/{id}` |
| POST | `/` |
| PUT | `/{id}` |
| DELETE | `/{id}` |
| GET | `/decision-number/{decisionNumber:int}` (numarayla bul) |

### 6.8 Meetings (`/api/v1/meetings`) — TenantAdmin only
| Method | Path |
|---|---|
| GET | `/` |
| GET | `/{id}` |
| POST | `/` |
| PUT | `/{id}` |
| DELETE | `/{id}` |
| GET | `/{id}/participants` |
| PUT | `/{id}/participants/{participantId}` (katılım durumu güncelle) |
| PUT | `/{id}/minutes` (toplantı tutanak özetini kaydet) |

### 6.9 Dues (`/api/v1/dues`) — mixed
| Method | Path | Role |
|---|---|---|
| GET | `/` | TenantAdmin |
| GET | `/{id}` | TenantAdmin, Resident |
| POST | `/` | TenantAdmin |
| POST | `/batch` | TenantAdmin (toplu aidat oluştur) |
| PUT | `/{id}` | TenantAdmin |
| DELETE | `/{id}` | TenantAdmin |
| POST | `/{id}/payments` | TenantAdmin (ödeme kaydı oluştur) |
| GET | `/{id}/payments` | TenantAdmin, Resident |
| DELETE | `/payments/{paymentId}` | TenantAdmin (ödemeyi geri al) |
| GET | `/overdue` | TenantAdmin |
| GET | `/report` | TenantAdmin (aylık rapor) |
| GET | `/resident/me` | Resident, TenantAdmin (kullanıcının kendi aidatları) |

### 6.10 MaintenanceTickets (`/api/v1/maintenance-tickets`) — mixed
| Method | Path | Role |
|---|---|---|
| GET | `/` | TenantAdmin, Resident |
| GET | `/{id}` | TenantAdmin, Resident |
| POST | `/` | TenantAdmin, Resident |
| PUT | `/{id}` | TenantAdmin |
| DELETE | `/{id}` | TenantAdmin |
| PATCH | `/{id}/status` | TenantAdmin |
| PATCH | `/{id}/priority` | TenantAdmin |
| POST | `/{id}/comment` | TenantAdmin, Resident |
| GET | `/{id}/comments` | TenantAdmin, Resident |

### 6.11 Dashboard (`/api/v1/dashboard`) — TenantAdmin only
| Method | Path |
|---|---|
| GET | `/summary` (özet kart verileri) |
| GET | `/membership-fee-trend` (aidat trend grafiği) |
| GET | `/recent-activities` (son aktiviteler) |

---

## 7. Request Body Örnekleri (JSON Şemaları)

### 7.1 Auth

**POST /api/v1/auth/login**
```json
{
  "telefon": "+905551112233",
  "sifre": "Demo44."
}
```

**POST /api/v1/auth/register-yonetici**
```json
{
  "apartmanAdi": "Park Sitesi",
  "apartmanKisaAd": "parksitesi",
  "contactEmail": "iletisim@park.com",
  "contactPhone": "+902121234567",
  "adminAdSoyad": "Ahmet Yılmaz",
  "adminEmail": "ahmet@park.com",
  "adminTelefon": "+905321112233",
  "sifre": "Yonetici123",
  "sifreTekrar": "Yonetici123"
}
```

**POST /api/v1/auth/davet-kabul**
```json
{
  "inviteToken": "ham-token-davetten-gelen",
  "sifre": "Sakin123abc",
  "sifreTekrar": "Sakin123abc"
}
```

**POST /api/v1/auth/refresh**
```json
{ "refreshToken": "<refresh-token-stringi>" }
```

**POST /api/v1/auth/logout**
```json
{ "refreshToken": "<refresh-token-stringi>" }
```

**POST /api/v1/auth/change-password**
```json
{
  "eskiSifre": "Demo44.",
  "yeniSifre": "YeniSifre123",
  "yeniSifreTekrar": "YeniSifre123"
}
```

### 7.2 Tenants

**POST /api/v1/tenants** (SuperAdmin)
```json
{
  "name": "Yeşil Sitesi",
  "shortName": "yesil",
  "contactEmail": "iletisim@yesil.com",
  "contactPhone": "+902121234567",
  "address": "İstanbul, Kadıköy",
  "maxApartmentCount": 100,
  "subscriptionEnd": "2027-05-24T00:00:00Z",
  "logoUrl": "https://example.com/logo.png"
}
```

**PUT /api/v1/tenants/{id}**
```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "name": "Yeşil Sitesi",
  "contactEmail": "iletisim@yesil.com",
  "contactPhone": "+902121234567",
  "address": "İstanbul, Kadıköy",
  "maxApartmentCount": 100,
  "subscriptionEnd": "2027-05-24T00:00:00Z",
  "logoUrl": "https://example.com/logo.png"
}
```

**PATCH /api/v1/tenants/{id}/active-status**
```json
{ "id": "00000000-0000-0000-0000-000000000000", "isActive": true }
```

### 7.3 Buildings

**POST /api/v1/buildings**
```json
{
  "name": "A Blok",
  "address": "Site içi, A Blok",
  "floorCount": 5,
  "apartmentCount": 10,
  "constructionYear": 2015
}
```

**PUT /api/v1/buildings/{id}** — aynı alanlar + `id`

### 7.4 Apartments

**POST /api/v1/apartments**
```json
{
  "buildingId": "00000000-0000-0000-0000-000000000000",
  "apartmentNumber": "13",
  "floor": 1,
  "grossSquareMeters": 110.0,
  "netSquareMeters": 95.0,
  "occupancyStatus": "Occupied",
  "dueMultiplier": 1.0
}
```

**POST /api/v1/apartments/batch**
```json
{
  "buildingId": "00000000-0000-0000-0000-000000000000",
  "apartments": [
    { "apartmentNumber": "11", "floor": 1, "grossSquareMeters": 110, "netSquareMeters": 95, "occupancyStatus": "Occupied", "dueMultiplier": 1.0 },
    { "apartmentNumber": "12", "floor": 1, "grossSquareMeters": 110, "netSquareMeters": 95, "occupancyStatus": "Vacant", "dueMultiplier": 1.0 }
  ]
}
```

### 7.5 Residents

**POST /api/v1/residents**
```json
{
  "apartmentId": "00000000-0000-0000-0000-000000000000",
  "fullName": "Mehmet Demir",
  "phone": "+905321234567",
  "email": "mehmet@ornek.com",
  "residentType": "Owner",
  "moveInDate": "2025-01-15T00:00:00Z",
  "isPrimaryContact": true
}
```

**POST /api/v1/residents/{id}/user-invite** — body boş, ResidentId URL'den.

### 7.6 Announcements

**POST /api/v1/announcements**
```json
{
  "title": "Su Kesintisi",
  "content": "Yarın 09:00-12:00 arası su kesilecek.",
  "severity": "Warning",
  "publishedAt": "2026-05-24T08:00:00Z",
  "expiresAt": "2026-05-25T12:00:00Z",
  "audience": "All",
  "buildingId": null
}
```

### 7.7 Decisions

**POST /api/v1/decisions**
```json
{
  "meetingId": "00000000-0000-0000-0000-000000000000",
  "decisionDate": "2026-05-15T18:00:00Z",
  "decisionTitle": "Asansör Bakımı",
  "decisionText": "Asansörler için yıllık bakım sözleşmesi imzalandı.",
  "votersCount": 10,
  "approvalVotes": 8,
  "rejectionVotes": 1,
  "abstentionVotes": 1
}
```

### 7.8 Meetings

**POST /api/v1/meetings**
```json
{
  "title": "Yıllık Genel Kurul",
  "meetingDate": "2026-06-15T18:00:00Z",
  "venue": "Site Sosyal Tesisi",
  "agenda": "1. Bütçe sunumu\n2. Aidat artışı\n3. Tadilat planı"
}
```

**PUT /api/v1/meetings/{id}/minutes**
```json
{ "id": "00000000-0000-0000-0000-000000000000", "minutesSummary": "Toplantı tutanak özeti..." }
```

**PUT /api/v1/meetings/{id}/participants/{participantId}**
```json
{
  "participantId": "00000000-0000-0000-0000-000000000000",
  "attendanceStatus": "Attended",
  "proxyApartmentId": null
}
```

### 7.9 Dues

**POST /api/v1/dues**
```json
{
  "apartmentId": "00000000-0000-0000-0000-000000000000",
  "period": "2026-05-01T00:00:00Z",
  "amount": 500.00,
  "dueDate": "2026-06-01T00:00:00Z",
  "description": "Mayıs aidatı"
}
```

**POST /api/v1/dues/batch**
```json
{
  "period": "2026-05-01T00:00:00Z",
  "baseAmount": 500.00,
  "dueDate": "2026-06-01T00:00:00Z",
  "description": "Mayıs toplu aidat",
  "buildingId": "00000000-0000-0000-0000-000000000000"
}
```

**POST /api/v1/dues/{id}/payments**
```json
{
  "dueId": "00000000-0000-0000-0000-000000000000",
  "paidAmount": 500.00,
  "paymentDate": "2026-05-28T10:00:00Z",
  "paymentMethod": "BankTransfer",
  "description": "Banka havalesi",
  "receiptNumber": "MAK-2026-001"
}
```

### 7.10 MaintenanceTickets

**POST /api/v1/maintenance-tickets**
```json
{
  "apartmentId": "00000000-0000-0000-0000-000000000000",
  "title": "Musluk damlatıyor",
  "description": "Mutfak musluğu sürekli damlatıyor.",
  "location": "Mutfak",
  "priority": "Medium"
}
```

**PATCH /api/v1/maintenance-tickets/{id}/status**
```json
{ "id": "00000000-0000-0000-0000-000000000000", "status": "InProgress" }
```

**PATCH /api/v1/maintenance-tickets/{id}/priority**
```json
{ "id": "00000000-0000-0000-0000-000000000000", "priority": "High" }
```

**POST /api/v1/maintenance-tickets/{id}/comment**
```json
{ "maintenanceTicketId": "00000000-0000-0000-0000-000000000000", "comment": "Yarın gelinecek." }
```

---

## 8. Enum Değerleri

API'da tüm enum'lar **string** olarak gönderilir/alınır:

| Enum | Geçerli değerler |
|---|---|
| `UserRole` | `SuperAdmin`, `TenantAdmin`, `Resident` |
| `ResidentType` | `Owner`, `Tenant` |
| `OccupancyStatus` | `Occupied`, `Vacant`, `Rented` |
| `AnnouncementSeverity` | `Info`, `Warning`, `Urgent` |
| `AnnouncementAudience` | `All`, `OwnersOnly`, `TenantsOnly` |
| `DueStatus` | `Pending`, `Paid`, `Overdue`, `PartiallyPaid` |
| `DueCreationType` | `Manual`, `Bulk` |
| `PaymentMethod` | `Cash`, `BankTransfer`, `CreditCard`, `Other` |
| `MaintenancePriority` | `Low`, `Medium`, `High`, `Urgent` |
| `MaintenanceStatus` | `Open`, `InProgress`, `Completed`, `Cancelled` |
| `MeetingStatus` | `Scheduled`, `Held`, `Cancelled` |
| `AttendanceStatus` | `Invited`, `Attended`, `Absent`, `ProxyGiven` |

---

## 9. Hata Yanıtları

### Validation hatası (400)
FluentValidation Türkçe mesajlar üretir:
```json
{
  "title": "One or more validation errors occurred.",
  "errors": {
    "Telefon": ["Telefon numarası geçersiz. Örnek: +905551112233 veya 05551112233."],
    "Sifre": ["Sifre alanı zorunludur."]
  }
}
```

### Auth hatası (401)
```json
{ "success": false, "error": { "code": "Unauthorized", "message": "Telefon numarası veya şifre hatalı." } }
```

### Yetkisiz (403)
JWT geçerli ama role uymuyorsa boş 403.

### Bulunamadı (404)
```json
{ "success": false, "error": { "code": "NotFound", "message": "Apartment bulunamadı." } }
```

### Çakışma (409)
```json
{ "success": false, "error": { "code": "Conflict", "message": "Bu email zaten kayıtlı." } }
```

### Rate limit (429)
Boş 429 response (`auth-strict` 5/dk, `api-general` 100/dk aşılınca).

### Sunucu hatası (500)
Production'da generic mesaj: `"Bir hata oluştu."`. Dev'de stack trace.

---

## 10. TestSprite İçin Önemli Notlar

### 10.1 Test sıralaması (önerilen)
1. **Auth happy path** — login (3 farklı telefon formatı ile), refresh, logout, change-password
2. **Auth negatif** — yanlış şifre, geçersiz telefon, eksik field, weak şifre
3. **SuperAdmin akışı** — login(admin@admin.com), tenant list/create/update/delete
4. **TenantAdmin akışı** — login(yonetici@demo.com), building/apartment/resident CRUD
5. **Resident akışı** — login(sakin@demo.com), kendi aidatlarını gör, ticket aç, duyuru okundu işaretle
6. **Cross-tenant izolasyon testi** — TenantAdmin başka tenant'ın verisine erişmeye çalışsın → 404 dönmeli (global filter)
7. **Rate limit testi** — `/auth/login`'e 6. istek → 429

### 10.2 Dikkat edilmesi gereken yerler
- **Telefon formatı**: `+905551112233`, `05551112233`, `5551112233`, `+90 555 111 22 33` → hepsi aynı user'a düşer.
- **Email normalize**: `ADMIN@DEMO.COM` ile login denemeyin (login email kullanmaz), ama RegisterAdmin'de uppercase email aynı kişiye düşürür.
- **Tenant isolation**: TenantAdmin login token'ı başka tenant'ın `tenantId`'sini path/query'de verirse → bulamaz (filter).
- **Refresh token rotation**: Bir refresh token sadece bir kez kullanılabilir; ikinci kullanımda 401.
- **ChangePassword sonrası**: Eski access token'lar HÂLÂ geçerli (15 dk içinde expire olana kadar), ama tüm refresh token'lar revoke edilmiş olur.
- **Soft delete**: DELETE çağrıları entity'i fiziksel silmez, `IsDeleted=true` yapar. Sonraki listelerde görünmez.
- **Update commands**: URL'deki `{id}` body'deki `Id`'yi override eder — body'de placeholder Guid bırakmak güvenli.

### 10.3 JWT token nasıl elde edilir
```bash
curl -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"telefon":"+905551112233","sifre":"Demo44."}'
```
Yanıttaki `accessToken`'ı sonraki isteklerde `Authorization: Bearer <token>` header'ı olarak kullan.

### 10.4 Swagger
Development modda `/swagger` aç → tüm endpoint'leri görsel olarak inceleyebilir, "Authorize" butonu ile JWT geçirip deneyebilirsin. TestSprite içe aktarma için OpenAPI JSON: `http://localhost:5000/swagger/v1/swagger.json`.

---

## 11. Bilinen Sınırlamalar / Yapılacaklar

- JWT secret hâlâ `appsettings.json`'da (prod öncesi `user-secrets` veya env var'a taşınacak)
- Refresh token "breach detection" yok (revoke edilmiş token tekrar kullanılsa sadece reject, alarm yok)
- Frontend UI yok — sadece backend API
- Email gönderimi `EmailService` üzerinden ama prod SMTP konfigürasyonu yok (Logger'a basıyor)
- IP audit trail'i `"unknown"` fallback ile sessizce kaydedebilir (improvement bekliyor)
- File upload (avatar, döküman) endpoint'i yok

---

## 12. Quick Reference

```
Base URL:    http://localhost:5000
Auth:        Bearer JWT (15 min) + refresh (7 days, rotated)
DB:          MySQL @ localhost:3306 / apartmanyonetim
Swagger:     /swagger
Healthcheck: /health/live, /health/ready

Roles:       SuperAdmin > TenantAdmin > Resident
Tenancy:     Multi-tenant, global query filter on TenantId

Demo Login:  +905551112233 / Demo44. (TenantAdmin)
             +905554445566 / Demo44. (Resident)
             +905550000001 / Admin44. (SuperAdmin)
```
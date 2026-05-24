# TestSprite AI Testing Report (MCP)

---

## 1️⃣ Document Metadata

- **Project Name:** ApartmentManagement
- **Date:** 2026-05-24
- **Prepared by:** TestSprite AI Team + supplementary endpoint smoke tests
- **Base URL:** http://localhost:5050
- **API Version:** `/api/v1` (also `/api/v1.0`)
- **Seed Users Used:**
  - SuperAdmin — telefon: `+905550000001`, sifre: `Admin44.`
  - TenantAdmin — telefon: `+905551112233`, sifre: `Demo44.`
  - Resident — telefon: `+905554445566`, sifre: `Demo44.`

---

## 2️⃣ Requirement Validation Summary

### Requirement: Authentication API

#### Test TC001 — POST /api/v1.0/auth/login with valid credentials
- **Test Code:** [TC001_post_apiv10authlogin_with_valid_credentials.py](./TC001_post_apiv10authlogin_with_valid_credentials.py)
- **Test Visualization:** [TestSprite Dashboard](https://www.testsprite.com/dashboard/mcp/tests/d2fe4a76-6d92-4db7-a3ba-6cdb69482162/2810ca7c-8e79-4ab8-8b87-01b78766685a)
- **Status:** ✅ Passed
- **Severity:** LOW
- **Analysis / Findings:** Login with SuperAdmin seed credentials returns 200, valid `accessToken`, `refreshToken`, and `user` object. JWT includes `tid` claim for tenant-scoped users.

#### Supplementary — GET /auth/me (all roles)
- **Status:** ✅ Passed (SuperAdmin, TenantAdmin, Resident)
- **Analysis:** Current user profile endpoint works for all three seed roles.

#### Supplementary — POST /auth/refresh
- **Status:** ✅ Passed
- **Analysis:** Refresh token rotation works with valid refresh token.

---

### Requirement: Health Checks

#### Supplementary — GET /health/live & /health/ready
- **Status:** ✅ Passed
- **Analysis:** Both probes return 200 without authentication.

---

### Requirement: Tenants API (SuperAdmin)

#### Supplementary — GET /tenants, GET /tenants/{id}, GET /tenants/{id}/statistics
- **Status:** ✅ Passed
- **Analysis:** SuperAdmin can list tenants, fetch demo tenant details, and view statistics.

---

### Requirement: Tenant-Scoped APIs (Buildings, Apartments, Residents, Dues, Decisions, Announcements, Meetings, Maintenance, Dashboard)

#### Supplementary — All TenantAdmin & Resident list/read endpoints
- **Status:** ❌ Failed (17 endpoints)
- **Severity:** CRITICAL
- **Error:** `500 — {"code": "Failure", "message": "Nullable object must have a value."}`
- **Affected endpoints:**
  - GET `/buildings`, `/apartments`, `/residents`
  - GET `/dues`, `/dues/overdue`, `/dues/report`, `/dues/resident/me`
  - GET `/decisions`, `/announcements`, `/meetings`
  - GET `/maintenance-tickets`, `/dashboard/summary`, `/dashboard/membership-fee-trend`, `/dashboard/recent-activities`
- **Analysis / Findings:** All tenant-scoped EF Core queries fail at global query filter evaluation. Root cause is in `ApplicationDbContext`:

```96:97:src/ApartmentManagement.Persistence/Contexts/ApplicationDbContext.cs
            (_currentTenant.IsSuperAdmin || e.TenantId == _currentTenant.TenantId!.Value)
            && !e.IsDeleted);
```

When `IsSuperAdmin` is false and `ICurrentTenantService.TenantId` resolves to null during query execution, `_currentTenant.TenantId!.Value` throws `InvalidOperationException`. JWT tokens for TenantAdmin **do** contain the `tid` claim, so the issue is likely that `CurrentTenantService` cannot resolve `HttpContext` (or the claim) at EF query-filter evaluation time — a known pitfall with `IHttpContextAccessor` inside EF global filters.

**Impact:** ~40+ API endpoints requiring tenant data are broken for TenantAdmin and Resident roles despite valid authentication.

---

### Requirement: Untested Endpoints (blocked by above bug or not reached)

The following endpoint groups were **not fully tested** due to the tenant filter failure or TestSprite generating only 1 test case:

| Group | Endpoints | Reason |
|-------|-----------|--------|
| Auth | register-yonetici, davet-kabul, logout, change-password | Not in TestSprite plan; skipped to avoid rate limit |
| Buildings | POST, PUT, DELETE | Blocked — cannot list/get IDs |
| Apartments | All CRUD + batch + sub-resources | Blocked |
| Residents | All CRUD + user-invite | Blocked |
| Dues | POST, PUT, DELETE, payments | Blocked |
| Decisions | All CRUD + by-number | Blocked |
| Announcements | POST, PUT, DELETE, read, stats | Blocked |
| Meetings | All CRUD + participants + minutes | Blocked |
| Maintenance | All CRUD + status/priority/comment | Blocked |

**Total Swagger paths:** 51 (+ 2 health)

---

## 3️⃣ Coverage & Matching Metrics

| Source | Total Tests | ✅ Passed | ❌ Failed | ⚠️ Blocked |
|--------|-------------|-----------|-----------|------------|
| TestSprite (TC001) | 1 | 1 | 0 | 0 |
| Supplementary smoke tests | 26 | 9 | 17 | 0 |
| **Combined** | **27** | **10** | **17** | **~34 endpoints untested** |

| Requirement | Total Tests | ✅ Passed | ❌ Failed |
|-------------|-------------|-----------|------------|
| Authentication | 4 | 4 | 0 |
| Health Checks | 2 | 2 | 0 |
| Tenants (SuperAdmin) | 3 | 3 | 0 |
| Tenant-Scoped APIs | 17 | 0 | 17 |
| CRUD / Write Operations | 0 | 0 | 0 (blocked) |

**Pass rate (executed tests):** 37% (10/27)

---

## 4️⃣ Key Gaps / Risks

1. **CRITICAL — Tenant query filter crash:** All tenant-scoped read endpoints return 500 for TenantAdmin/Resident. This blocks the majority of the API from functioning. Fix `ApplicationDbContext` global filters to handle null `TenantId` safely (e.g. return no rows instead of throwing), and ensure `ICurrentTenantService` resolves correctly during EF query execution.

2. **TestSprite coverage gap:** Backend test plan generated only **1 test case** (login) out of 51+ endpoints. Full endpoint coverage requires either expanding the TestSprite plan or maintaining the supplementary script at `testsprite_tests/run_all_endpoint_tests.py`.

3. **Auth rate limiting:** `auth-strict` policy limits login to **5 requests/minute per IP**. Bulk test runs hit 429 errors. Tests should cache tokens per role and add ≥15s delay between logins.

4. **Untested write operations:** POST/PUT/DELETE/PATCH endpoints were not validated. Once the tenant filter bug is fixed, re-run full CRUD tests for all 12 controllers.

5. **Role-based access control:** No negative tests (e.g. Resident accessing `/tenants`, unauthenticated access) were executed in this run.

---

## Artifacts

- TestSprite raw report: `testsprite_tests/tmp/raw_report.md`
- Endpoint smoke test script: `testsprite_tests/run_all_endpoint_tests.py`
- Endpoint test output: `testsprite_tests/tmp/endpoint_test_output.txt`
- TestSprite dashboard: https://www.testsprite.com/dashboard/mcp/tests/d2fe4a76-6d92-4db7-a3ba-6cdb69482162/2810ca7c-8e79-4ab8-8b87-01b78766685a

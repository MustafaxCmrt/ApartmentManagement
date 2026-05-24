#!/usr/bin/env python3
"""Comprehensive API endpoint tests for ApartmentManagement."""
import json
import sys
import time
import uuid
from dataclasses import dataclass
from datetime import datetime, timezone

import requests

BASE = "http://localhost:5050"
API = f"{BASE}/api/v1"
TIMEOUT = 30
UID = uuid.uuid4().hex[:8]

USERS = {
    "superadmin": {"telefon": "+905550000001", "sifre": "Admin44."},
    "tenantadmin": {"telefon": "+905551112233", "sifre": "Demo44."},
    "resident": {"telefon": "+905554445566", "sifre": "Demo44."},
}


@dataclass
class Result:
    name: str
    method: str
    path: str
    status: int | None
    ok: bool
    note: str = ""


results: list[Result] = []
tokens: dict[str, str] = {}
refresh_tokens: dict[str, str] = {}
ids: dict[str, str] = {}


def login(role: str, force: bool = False) -> str:
    if not force and role in tokens:
        return tokens[role]
    time.sleep(12)
    r = requests.post(f"{API}/auth/login", json=USERS[role], timeout=TIMEOUT)
    if r.status_code == 429:
        time.sleep(30)
        r = requests.post(f"{API}/auth/login", json=USERS[role], timeout=TIMEOUT)
    if r.status_code != 200:
        raise RuntimeError(f"Login failed for {role}: {r.status_code} {r.text[:200]}")
    data = r.json()
    tokens[role] = data["accessToken"]
    refresh_tokens[role] = data.get("refreshToken", "")
    return tokens[role]


def login_all():
    for role in USERS:
        login(role)


def req(name: str, method: str, path: str, role: str | None = None, expect_ok: bool = True, **kwargs) -> requests.Response:
    headers = kwargs.pop("headers", {})
    if role:
        headers["Authorization"] = f"Bearer {login(role)}"
    headers.setdefault("Content-Type", "application/json")
    url = path if path.startswith("http") else (f"{BASE}{path}" if path.startswith("/health") else f"{API}{path}")
    r = requests.request(method, url, headers=headers, timeout=TIMEOUT, **kwargs)
    ok = (200 <= r.status_code < 300) if expect_ok else True
    note = ""
    if not ok:
        try:
            note = json.dumps(r.json())[:250]
        except Exception:
            note = r.text[:250]
    results.append(Result(name, method, path, r.status_code, ok, note))
    return r


def body(r: requests.Response):
    try:
        return r.json()
    except Exception:
        return {}


def pick_id(data) -> str | None:
    if isinstance(data, dict):
        if data.get("items"):
            return str(data["items"][0].get("id"))
        if isinstance(data.get("data"), list) and data["data"]:
            return str(data["data"][0].get("id"))
        if data.get("id"):
            return str(data["id"])
    if isinstance(data, list) and data:
        return str(data[0].get("id"))
    return None


def now_iso():
    return datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")


def main():
    print(f"Running full endpoint tests (run id: {UID})")

    # ── Health ──
    req("health live", "GET", "/health/live")
    req("health ready", "GET", "/health/ready")

    # ── Auth ──
    login_all()
    req("auth login superadmin", "POST", "/auth/login", json=USERS["superadmin"], expect_ok=False)
    req("auth me superadmin", "GET", "/auth/me", role="superadmin")
    req("auth me tenantadmin", "GET", "/auth/me", role="tenantadmin")
    req("auth me resident", "GET", "/auth/me", role="resident")

    if refresh_tokens.get("superadmin"):
        req("auth refresh", "POST", "/auth/refresh", json={"refreshToken": refresh_tokens["superadmin"]})

    # register-yonetici skipped here — auth-strict rate limit (5/min); tested separately if needed

    # ── Tenants (SuperAdmin) ──
    r = req("tenants list", "GET", "/tenants", role="superadmin")
    ids["tenant"] = pick_id(body(r))
    if ids.get("tenant"):
        tid = ids["tenant"]
        req("tenants get", "GET", f"/tenants/{tid}", role="superadmin")
        req("tenants statistics", "GET", f"/tenants/{tid}/statistics", role="superadmin")

    # create + update + toggle temp tenant
    create_tenant = {
        "name": f"Temp Tenant {UID}",
        "shortName": f"tmp{UID}",
        "contactEmail": f"tmp{UID}@example.com",
        "contactPhone": "+905559990003",
        "address": "Test Address",
        "maxApartmentCount": 20,
        "subscriptionEnd": None,
        "logoUrl": None,
    }
    r = req("tenants create", "POST", "/tenants", role="superadmin", json=create_tenant)
    temp_tenant = body(r).get("id")
    if temp_tenant:
        req("tenants update", "PUT", f"/tenants/{temp_tenant}", role="superadmin", json={
            **create_tenant, "name": f"Temp Tenant Updated {UID}"
        })
        req("tenants toggle active", "PATCH", f"/tenants/{temp_tenant}/active-status", role="superadmin",
            json={"isActive": False})

    # ── Buildings ──
    r = req("buildings list", "GET", "/buildings", role="tenantadmin")
    ids["building"] = pick_id(body(r))

    r = req("buildings create", "POST", "/buildings", role="tenantadmin", json={
        "name": f"Test Block {UID}",
        "address": "Test Street 1",
        "floorCount": 3,
        "apartmentCount": 6,
        "constructionYear": 2020,
    })
    temp_building = body(r).get("id")
    if temp_building:
        ids["building"] = temp_building
        req("buildings get", "GET", f"/buildings/{temp_building}", role="tenantadmin")
        req("buildings update", "PUT", f"/buildings/{temp_building}", role="tenantadmin", json={
            "name": f"Test Block Updated {UID}",
            "address": "Test Street 2",
            "floorCount": 3,
            "apartmentCount": 6,
            "constructionYear": 2021,
        })
        req("buildings apartments", "GET", f"/buildings/{temp_building}/apartments", role="tenantadmin")

    bid = ids.get("building")
    temp_apt = None
    batch_apt = None

    # ── Apartments ──
    r = req("apartments list", "GET", "/apartments", role="tenantadmin")
    ids["apartment"] = pick_id(body(r))

    if bid:
        r = req("apartments create", "POST", "/apartments", role="tenantadmin", json={
            "buildingId": bid,
            "apartmentNumber": f"T{UID}",
            "floor": 1,
            "grossSquareMeters": 100,
            "netSquareMeters": 85,
            "occupancyStatus": "Vacant",
            "dueMultiplier": 1.0,
        })
        temp_apt = body(r).get("id")
        if temp_apt:
            ids["apartment"] = temp_apt
            req("apartments get", "GET", f"/apartments/{temp_apt}", role="tenantadmin")
            req("apartments update", "PUT", f"/apartments/{temp_apt}", role="tenantadmin", json={
                "buildingId": bid,
                "apartmentNumber": f"T{UID}",
                "floor": 1,
                "grossSquareMeters": 105,
                "netSquareMeters": 90,
                "occupancyStatus": "Occupied",
                "dueMultiplier": 1.0,
            })
            req("apartments residents", "GET", f"/apartments/{temp_apt}/residents", role="tenantadmin")
            req("apartments dues", "GET", f"/apartments/{temp_apt}/dues", role="tenantadmin")

        rb = req("apartments batch", "POST", "/apartments/batch", role="tenantadmin", json={
            "buildingId": bid,
            "apartments": [{
                "apartmentNumber": f"B{UID}",
                "floor": 2,
                "grossSquareMeters": 90,
                "netSquareMeters": 75,
                "occupancyStatus": "Vacant",
                "dueMultiplier": 1.0,
            }],
        })
        batch_items = body(rb)
        if isinstance(batch_items, list) and batch_items:
            batch_apt = batch_items[0].get("id")
        elif isinstance(batch_items, dict):
            batch_apt = pick_id(batch_items)

    aid = ids.get("apartment")

    # ── Residents ──
    r = req("residents list", "GET", "/residents", role="tenantadmin")
    ids["resident"] = pick_id(body(r))

    temp_resident = None
    if aid:
        valid_phone = f"+9055599{int(UID[:4], 16) % 10000:04d}"
        r = req("residents create", "POST", "/residents", role="tenantadmin", json={
            "apartmentId": aid,
            "fullName": f"Test Resident {UID}",
            "phone": valid_phone,
            "email": f"resident{UID}@example.com",
            "residentType": "Tenant",
            "moveInDate": now_iso(),
            "isPrimaryContact": False,
        })
        temp_resident = body(r).get("id")
        if temp_resident:
            ids["resident"] = temp_resident
            req("residents get", "GET", f"/residents/{temp_resident}", role="tenantadmin")
            req("residents update", "PUT", f"/residents/{temp_resident}", role="tenantadmin", json={
                "fullName": f"Test Resident Updated {UID}",
                "phone": valid_phone,
                "email": f"resident{UID}@example.com",
                "residentType": "Tenant",
                "moveInDate": now_iso(),
                "moveOutDate": None,
                "isPrimaryContact": False,
            })
            req("residents user-invite", "POST", f"/residents/{temp_resident}/user-invite", role="tenantadmin", json={})

    # ── Dues ──
    req("dues list", "GET", "/dues", role="tenantadmin")
    req("dues overdue", "GET", "/dues/overdue", role="tenantadmin")
    req("dues report", "GET", "/dues/report?year=2025&month=1", role="tenantadmin")
    req("dues resident me", "GET", "/dues/resident/me", role="resident")

    temp_due = None
    if aid:
        period = "2025-06-01T00:00:00Z"
        r = req("dues create", "POST", "/dues", role="tenantadmin", json={
            "apartmentId": aid,
            "period": period,
            "amount": 500,
            "dueDate": "2025-06-15T00:00:00Z",
            "description": f"Test due {UID}",
        })
        temp_due = body(r).get("id")

        req("dues batch", "POST", "/dues/batch", role="tenantadmin", json={
            "period": "2025-07-01T00:00:00Z",
            "baseAmount": 400,
            "dueDate": "2025-07-15T00:00:00Z",
            "description": f"Bulk due {UID}",
            "buildingId": bid,
        })

    if temp_due:
        req("dues get", "GET", f"/dues/{temp_due}", role="tenantadmin")
        req("dues get resident", "GET", f"/dues/{temp_due}", role="resident")
        r = req("dues payment", "POST", f"/dues/{temp_due}/payments", role="tenantadmin", json={
            "paidAmount": 500,
            "paymentDate": now_iso(),
            "paymentMethod": "Cash",
            "description": "Test payment",
            "receiptNumber": f"RCP-{UID}",
        })
        payment_id = body(r).get("id")
        req("dues payments list", "GET", f"/dues/{temp_due}/payments", role="tenantadmin")
        if payment_id:
            req("dues payment reverse", "DELETE", f"/dues/payments/{payment_id}", role="tenantadmin")
        req("dues update", "PUT", f"/dues/{temp_due}", role="tenantadmin", json={
            "amount": 550,
            "dueDate": "2025-06-20T00:00:00Z",
            "description": f"Updated due {UID}",
        })

    # ── Decisions ──
    r = req("decisions create", "POST", "/decisions", role="tenantadmin", json={
        "meetingId": None,
        "decisionDate": now_iso(),
        "decisionTitle": f"Test Decision {UID}",
        "decisionText": "Test decision text",
        "votersCount": 10,
        "approvalVotes": 8,
        "rejectionVotes": 1,
        "abstentionVotes": 1,
    })
    temp_decision = body(r).get("id")
    decision_number = body(r).get("decisionNumber")

    r = req("decisions list", "GET", "/decisions", role="tenantadmin")
    if temp_decision:
        req("decisions get", "GET", f"/decisions/{temp_decision}", role="tenantadmin")
        req("decisions update", "PUT", f"/decisions/{temp_decision}", role="tenantadmin", json={
            "decisionDate": now_iso(),
            "decisionTitle": f"Updated Decision {UID}",
            "decisionText": "Updated text",
            "votersCount": 10,
            "approvalVotes": 9,
            "rejectionVotes": 1,
            "abstentionVotes": 0,
        })
    if decision_number:
        req("decisions by number", "GET", f"/decisions/decision-number/{decision_number}", role="tenantadmin")

    # ── Announcements ──
    r = req("announcements create", "POST", "/announcements", role="tenantadmin", json={
        "title": f"Test Announcement {UID}",
        "content": "Test content",
        "severity": "Info",
        "publishedAt": now_iso(),
        "expiresAt": None,
        "audience": "All",
        "buildingId": None,
    })
    temp_ann = body(r).get("id")
    req("announcements list admin", "GET", "/announcements", role="tenantadmin")
    req("announcements list resident", "GET", "/announcements", role="resident")
    if temp_ann:
        req("announcements get", "GET", f"/announcements/{temp_ann}", role="tenantadmin")
        req("announcements update", "PUT", f"/announcements/{temp_ann}", role="tenantadmin", json={
            "title": f"Updated Announcement {UID}",
            "content": "Updated content",
            "severity": "Warning",
            "publishedAt": now_iso(),
            "expiresAt": None,
            "audience": "All",
            "buildingId": None,
        })
        req("announcements read", "POST", f"/announcements/{temp_ann}/read", role="resident")
        req("announcements read stats", "GET", f"/announcements/{temp_ann}/read-statistics", role="tenantadmin")

    # ── Meetings ──
    r = req("meetings create", "POST", "/meetings", role="tenantadmin", json={
        "title": f"Test Meeting {UID}",
        "meetingDate": "2025-08-01T10:00:00Z",
        "venue": "Meeting Room",
        "agenda": "Test agenda",
    })
    temp_meeting = body(r).get("id")
    req("meetings list", "GET", "/meetings", role="tenantadmin")
    if temp_meeting:
        req("meetings get", "GET", f"/meetings/{temp_meeting}", role="tenantadmin")
        req("meetings participants", "GET", f"/meetings/{temp_meeting}/participants", role="tenantadmin")
        req("meetings minutes", "PUT", f"/meetings/{temp_meeting}/minutes", role="tenantadmin",
            json={"minutesSummary": f"Minutes for {UID}"})
        req("meetings update", "PUT", f"/meetings/{temp_meeting}", role="tenantadmin", json={
            "title": f"Updated Meeting {UID}",
            "meetingDate": "2025-08-01T11:00:00Z",
            "venue": "Updated Room",
            "agenda": "Updated agenda",
            "status": "Scheduled",
        })
        mr = requests.get(
            f"{API}/meetings/{temp_meeting}/participants",
            headers={"Authorization": f"Bearer {login('tenantadmin')}"},
            timeout=TIMEOUT,
        )
        participants = body(mr).get("participants") or []
        if participants:
            pid = participants[0]["id"]
            req("meetings update participant", "PUT",
                f"/meetings/{temp_meeting}/participants/{pid}", role="tenantadmin",
                json={"attendanceStatus": "Attended", "proxyApartmentId": None})

    # ── Maintenance ──
    r = req("maintenance create resident", "POST", "/maintenance-tickets", role="resident", json={
        "apartmentId": aid,
        "title": f"Test Ticket {UID}",
        "description": "Something broken",
        "location": "Kitchen",
        "priority": "Medium",
    })
    temp_ticket = body(r).get("id")
    req("maintenance list admin", "GET", "/maintenance-tickets", role="tenantadmin")
    req("maintenance list resident", "GET", "/maintenance-tickets", role="resident")
    if temp_ticket:
        req("maintenance get", "GET", f"/maintenance-tickets/{temp_ticket}", role="tenantadmin")
        req("maintenance update", "PUT", f"/maintenance-tickets/{temp_ticket}", role="tenantadmin", json={
            "title": f"Updated Ticket {UID}",
            "description": "Updated description",
            "location": "Bathroom",
        })
        req("maintenance status", "PATCH", f"/maintenance-tickets/{temp_ticket}/status", role="tenantadmin",
            json={"status": "InProgress"})
        req("maintenance priority", "PATCH", f"/maintenance-tickets/{temp_ticket}/priority", role="tenantadmin",
            json={"priority": "High"})
        req("maintenance comment", "POST", f"/maintenance-tickets/{temp_ticket}/comment", role="resident",
            json={"comment": f"Comment {UID}"})
        req("maintenance comments", "GET", f"/maintenance-tickets/{temp_ticket}/comments", role="tenantadmin")

    # ── Dashboard ──
    req("dashboard summary", "GET", "/dashboard/summary", role="tenantadmin")
    req("dashboard trend", "GET", "/dashboard/membership-fee-trend?months=6", role="tenantadmin")
    req("dashboard activities", "GET", "/dashboard/recent-activities?limit=10", role="tenantadmin")

    # ── Cleanup (DELETE) ──
    if temp_ticket:
        req("maintenance delete", "DELETE", f"/maintenance-tickets/{temp_ticket}", role="tenantadmin")
    if temp_meeting:
        req("meetings delete", "DELETE", f"/meetings/{temp_meeting}", role="tenantadmin")
    if temp_ann:
        req("announcements delete", "DELETE", f"/announcements/{temp_ann}", role="tenantadmin")
    if temp_decision:
        req("decisions delete", "DELETE", f"/decisions/{temp_decision}", role="tenantadmin")
    if temp_due:
        req("dues delete", "DELETE", f"/dues/{temp_due}", role="tenantadmin")
    if temp_resident:
        req("residents delete", "DELETE", f"/residents/{temp_resident}", role="tenantadmin")
    if batch_apt:
        req("apartments delete batch", "DELETE", f"/apartments/{batch_apt}", role="tenantadmin")
    if temp_apt:
        req("apartments delete", "DELETE", f"/apartments/{temp_apt}", role="tenantadmin")
    if temp_building:
        req("buildings delete", "DELETE", f"/buildings/{temp_building}", role="tenantadmin")
    if temp_tenant:
        req("tenants delete", "DELETE", f"/tenants/{temp_tenant}", role="superadmin")

    # auth logout (fresh login for refresh token)
    login("resident", force=True)
    if refresh_tokens.get("resident"):
        req("auth logout", "POST", "/auth/logout", role="resident",
            json={"refreshToken": refresh_tokens["resident"]})

    # ── Summary ──
    passed = sum(1 for x in results if x.ok)
    failed = [x for x in results if not x.ok]
    print(f"\n{'='*70}")
    print(f"TOTAL: {len(results)} | PASSED: {passed} | FAILED: {len(failed)}")
    print(f"{'='*70}")
    for x in results:
        if not x.name:
            continue
        mark = "PASS" if x.ok else "FAIL"
        print(f"[{mark}] {x.method:6} {x.path:55} -> {x.status} {x.note}")

    if failed:
        print(f"\n--- FAILED ({len(failed)}) ---")
        for x in failed:
            print(f"  {x.name}: {x.method} {x.path} -> {x.status} {x.note}")

    report_path = "/Users/comert/Desktop/ApartmentManagement/testsprite_tests/tmp/endpoint_test_output.txt"
    with open(report_path, "w") as f:
        f.write(f"TOTAL: {len(results)} | PASSED: {passed} | FAILED: {len(failed)}\n\n")
        for x in results:
            if x.name:
                mark = "PASS" if x.ok else "FAIL"
                f.write(f"[{mark}] {x.method} {x.path} -> {x.status} {x.note}\n")

    sys.exit(1 if failed else 0)


if __name__ == "__main__":
    main()

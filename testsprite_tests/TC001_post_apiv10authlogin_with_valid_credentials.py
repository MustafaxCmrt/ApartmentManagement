import requests

BASE_URL = "http://localhost:5050"
LOGIN_PATH = "/api/v1.0/auth/login"
TIMEOUT = 30

def test_post_apiv10authlogin_with_valid_credentials():
    url = BASE_URL + LOGIN_PATH
    payload = {
        "telefon": "+905550000001",
        "sifre": "Admin44."
    }
    headers = {
        "Content-Type": "application/json"
    }

    try:
        response = requests.post(url, json=payload, headers=headers, timeout=TIMEOUT)
    except requests.RequestException as e:
        assert False, f"Request failed: {e}"

    assert response.status_code == 200, f"Expected status code 200, got {response.status_code}"
    try:
        data = response.json()
    except ValueError:
        assert False, "Response is not valid JSON"

    assert "accessToken" in data and isinstance(data["accessToken"], str) and data["accessToken"], "Missing or invalid accessToken"
    assert "refreshToken" in data and isinstance(data["refreshToken"], str) and data["refreshToken"], "Missing or invalid refreshToken"
    assert "user" in data and isinstance(data["user"], dict), "Missing or invalid user profile"

test_post_apiv10authlogin_with_valid_credentials()
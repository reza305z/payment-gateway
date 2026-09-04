# Payment Gateway

A small payment gateway demo: a **.NET 8 API** and a **Next.js gateway page**, run together with Docker.

| Service    | URL                                  |
| ---------- | ------------------------------------ |
| API        | http://localhost:8080                |
| Swagger UI | http://localhost:8080/swagger        |
| Gateway UI | http://localhost:3000                |

---

## 1. Start everything

```bash
docker compose up -d
```

The first run builds the images, so give it some minutes. Verify both services are up:

```bash
docker compose ps
```

---

## 2. Create a payment — `get-token`

**Option A — Swagger:** open http://localhost:8080/swagger, expand `POST /api/payment/get-token`, click *Try it out*, and send the example body.

**Option B — curl:**

```bash
curl -X POST http://localhost:8080/api/payment/get-token \
  -H "Content-Type: application/json" \
  -d '{
    "terminalNo": "123456",
    "amount": 500000,
    "redirectUrl": "http://shop.localhost:3080/payment/result",
    "reservationNumber": "RES-123",
    "phoneNumber": "09121234567"
  }'
```

The response returns a `token` and the `gatewayUrl` to open:

```json
{
  "isSuccess": true,
  "token": "267894dc-5797-449c-8557-764b6ca0741b",
  "gatewayUrl": "http://localhost:3000/gateway/267894dc-5797-449c-8557-764b6ca0741b"
}
```

---

## 3. Pay on the gateway page

Open `gatewayUrl` in a browser. You'll see the amount and two buttons:

- **«پرداخت موفق»** — simulate a successful payment
- **«انصراف»** — simulate a cancellation

Click either one. The page finalizes the payment and redirects to the merchant's `redirectUrl` with the token appended, e.g.:

```
http://shop.localhost:3080/payment/result?token=267894dc-5797-449c-8557-764b6ca0741b
```

> The merchant address is a placeholder — nothing runs there, so the browser may show a "site can't be reached" page.

---

## 4. Verify the payment

Call `POST /api/payment/verify` (Swagger or curl) with the same token:

```bash
curl -X POST http://localhost:8080/api/payment/verify \
  -H "Content-Type: application/json" \
  -d '{
    "token": "267894dc-5797-449c-8557-764b6ca0741b",
    "appCode": "DEMO"
  }'
```

The response shows the final status — `Success` if you clicked pay, `Failed` if you clicked cancel:

```json
{
  "status": "Success",
  "amount": 500000,
  "reservationNumber": "RES-123",
  "rrn": "912345678901"
}
```

---

## 5. See a payment expire

Create another payment with `get-token` (step 2) and wait **2 minutes**, then call `verify` with that token — the status will be `Expired`.

---

## Notes

- `get-token` is rate-limited to **3 requests per minute per IP**. If you get a `429`, wait a minute and retry.
- `redirectUrl` must start with an allowed origin — `http://shop.localhost:3080` in the default config. To use your own address, add its origin to `Payment__AllowedRedirectOrigins` in `backend/.env`.
- `phoneNumber` must match the Iranian mobile format (`09xxxxxxxxx`).
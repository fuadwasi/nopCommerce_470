# SteadFast Courier Limited API Documentation (v1)

Base URL: `https://portal.packzy.com/api/v1`

## Authentication
All requests require the following headers:
- `Api-Key`: API Key provided by Steadfast Courier Ltd.
- `Secret-Key`: Secret Key provided by Steadfast Courier Ltd.
- `Content-Type`: `application/json`

---

## Endpoints

### 1. Placing an Order
- **Path:** `/create_order`
- **Method:** `POST`
- **Body Parameters:**
  - `invoice` (string, required): Unique, alpha-numeric, hyphens/underscores allowed
  - `recipient_name` (string, required): Max 100 chars
  - `recipient_phone` (string, required): 11 digits
  - `alternative_phone` (string, optional): 11 digits
  - `recipient_email` (string, optional)
  - `recipient_address` (string, required): Max 250 chars
  - `cod_amount` (numeric, required): >= 0
  - `note` (string, optional)
  - `item_description` (string, optional)
  - `total_lot` (numeric, optional)
  - `delivery_type` (numeric, optional): 0 = home delivery, 1 = hub pickup

- **Response Example:**
```json
{
  "status": 200,
  "message": "Consignment has been created successfully.",
  "consignment": {
    "consignment_id": 1424107,
    "invoice": "Aa12-das4",
    "tracking_code": "15BAEB8A",
    "recipient_name": "John Smith",
    "recipient_phone": "01234567890",
    "recipient_address": "Fla# A1,House# 17/1, Road# 3/A, Dhanmondi,Dhaka-1209",
    "cod_amount": 1060,
    "status": "in_review",
    "note": "Deliver within 3PM",
    "created_at": "2021-03-21T07:05:31.000000Z",
    "updated_at": "2021-03-21T07:05:31.000000Z"
  }
}
```

---

### 2. Bulk Order Create
- **Path:** `/create_order/bulk-order`
- **Method:** `POST`
- **Body:**
  - `data`: JSON array of order objects (max 500)
- **Response Example:**
```json
[
  {
    "invoice": "230822-1",
    "recipient_name": "John Doe",
    "recipient_address": "House 44, Road 2/A, Dhanmondi, Dhaka 1209",
    "recipient_phone": "0171111111",
    "cod_amount": "0.00",
    "note": null,
    "consignment_id": 11543968,
    "tracking_code": "B025A038",
    "status": "success"
  }
]
```
- **Error Example:**
```json
{
  "data": [
    {
      "invoice": "230822-1",
      "status": "error"
    }
  ]
}
```

---

### 3. Checking Delivery Status
- **By Consignment ID:**
  - **Path:** `/status_by_cid/{id}`
  - **Method:** `GET`
- **By Invoice:**
  - **Path:** `/status_by_invoice/{invoice}`
  - **Method:** `GET`
- **By Tracking Code:**
  - **Path:** `/status_by_trackingcode/{trackingCode}`
  - **Method:** `GET`
- **Response Example:**
```json
{
  "status": 200,
  "delivery_status": "in_review"
}
```
- **Delivery Statuses:**
  - `pending`, `delivered_approval_pending`, `partial_delivered_approval_pending`, `cancelled_approval_pending`, `unknown_approval_pending`, `delivered`, `partial_delivered`, `cancelled`, `hold`, `in_review`, `unknown`

---

### 4. Checking Current Balance
- **Path:** `/get_balance`
- **Method:** `GET`
- **Response Example:**
```json
{
  "status": 200,
  "current_balance": 0
}
```

---

### 5. Creating Return Requests
- **Path:** `/create_return_request`
- **Method:** `POST`
- **Body Parameters:**
  - `consignment_id` or `invoice` or `tracking_code` (required)
  - `reason` (optional)
- **Response Example:**
```json
{
  "user_id": 1,
  "consignment_id": 10000042,
  "reason": null,
  "status": "pending",
  "created_at": "2025-07-30T23:11:45.000000Z",
  "updated_at": "2025-07-30T23:11:45.000000Z"
}
```

---

### 6. Single Return Request View
- **Path:** `/get_return_request/{id}`
- **Method:** `GET`

---

### 7. Get Return Requests
- **Path:** `/get_return_requests`
- **Method:** `GET`

---

### 8. Get Payments
- **Path:** `/payments`
- **Method:** `GET`

---

### 9. Get Single Payment with Consignments
- **Path:** `/payments/{payment_id}`
- **Method:** `GET`

---

### 10. Get Policestations
- **Path:** `/police_stations`
- **Method:** `GET`

---

## Notes
- All endpoints require authentication headers.
- All requests and responses are in JSON format.
- For more details, refer to the official documentation or contact Steadfast Courier Ltd.

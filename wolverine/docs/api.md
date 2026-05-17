# API

## POST /orders

Request:

```json
{
  "customerName": "Kannan",
  "productCode": "PRD-1001",
  "quantity": 2,
  "unitPrice": 1500
}
```

Response:

```json
{
  "orderId": "00000000-0000-0000-0000-000000000000",
  "status": "Pending"
}
```

Validation:

- `customerName` is required.
- `productCode` is required.
- `quantity` must be greater than zero.
- `unitPrice` must be greater than zero.

## GET /orders/{id}

Response:

```json
{
  "orderId": "00000000-0000-0000-0000-000000000000",
  "customerName": "Kannan",
  "productCode": "PRD-1001",
  "quantity": 2,
  "unitPrice": 1500,
  "totalAmount": 3000,
  "status": "Confirmed",
  "createdUtc": "2026-04-27T10:30:00Z"
}
```

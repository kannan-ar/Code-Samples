MATCH (c:Customer), (w:Warehouse)
WITH c, w, point.distance(c.location, w.location) AS dist
ORDER BY c.name, dist
WITH c, collect({warehouse: w.name, distance_km: dist/1000.0})[0] AS nearest
RETURN c.name AS customer, nearest.warehouse AS closestWarehouse, round(nearest.distance_km, 2) AS distance_km;


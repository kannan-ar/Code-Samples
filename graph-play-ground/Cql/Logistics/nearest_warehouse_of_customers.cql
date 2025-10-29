MATCH (c:Customer), (w:Warehouse)
WITH c, w, point.distance(c.location, w.location) AS dist
ORDER BY c.name, dist
WITH c, collect({warehouse: w.name, distance_km: round(dist / 1000)})[0] AS nearest
RETURN c.name AS Customer, nearest.warehouse AS NearestWarehouse, nearest.distance_km AS DistanceKm

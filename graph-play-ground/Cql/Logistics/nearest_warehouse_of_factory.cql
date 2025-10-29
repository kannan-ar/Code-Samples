MATCH (f:Factory), (w:Warehouse)
WITH f, w, round(point.distance(f.location, w.location) / 1000) AS distance_km
ORDER BY f.name, distance_km
WITH f, collect({w:w, dist: distance_km})[0] AS nearest
RETURN f.name AS Factory, nearest.w.name AS Warehouse, nearest.dist AS Distance
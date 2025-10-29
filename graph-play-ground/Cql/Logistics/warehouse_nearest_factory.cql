MATCH (w:Warehouse), (f:Factory)
WITH w, f, point.distance(w.location, f.location) AS dist
ORDER BY w.name, dist
WITH w, collect({factory: f.name, distance_km: dist/1000.0})[0] AS nearest
RETURN w.name AS warehouse, nearest.factory AS closestFactory, round(nearest.distance_km, 2) AS distance_km;

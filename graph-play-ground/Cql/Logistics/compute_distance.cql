MATCH (f:Factory), (w:Warehouse)
RETURN f.name, w.name, distance(f.location, w.location) / 1000 AS distance_km
ORDER BY distance_km;

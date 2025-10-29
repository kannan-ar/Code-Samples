MATCH (f:Factory), (w:Warehouse)
WITH f, w, point.distance(f.location, w.location) AS dist
ORDER BY f.name, dist
WITH f, collect({w: w, dist: dist})[0] AS nearest
WITH f, nearest.w AS nearestWarehouse, nearest.dist AS dist
MERGE (f)-[r:CLOSEST_TO]->(nearestWarehouse)
  SET r.distance_km = round(dist / 1000.0, 2)
RETURN f.name AS Factory, nearestWarehouse.name AS Warehouse, r.distance_km;
